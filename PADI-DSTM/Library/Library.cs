using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using CommonTypes;
using System.Collections;
using System.Runtime.Serialization;


namespace ClientLibrary {

    public class Library {

        private static IMaster masterServer;
        private static int actualTID;
        private static List<PadIntRegistry> padIntsList;
        private static TcpChannel channel;

        public static TcpChannel Channel {
            get { return channel; }
        }

        public static bool init() {
            padIntsList = new List<PadIntRegistry>();
            channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            Logger.log(new String[] { "Library", "init", "\r\n" });
            masterServer = (IMaster) Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
            return true;
        }

        public static bool txBegin() {
            Logger.log(new String[] { "Library", "txBegin" });
            actualTID = masterServer.getNextTID();
            Logger.log(new String[] { " " });
            return true;
        }

        public static bool txCommit() {
            Logger.log(new String[] { "Library", "txCommit" });

            if(padIntsList.Count == 0) {
                Logger.log(new String[] { "Library", "txCommit", "nothing to commit" });
            }

            bool result = false;
            IServer server;

            foreach(PadIntRegistry pd in padIntsList) {
                server = (IServer) Activator.GetObject(typeof(IServer), pd.Address);
                result = result && server.commit(actualTID, pd.PdInts);
            }

            padIntsList.Clear();
            return result;
        }

        public static bool txAbort() {
            Logger.log(new String[] { "Library", "txAbort" });

            if(padIntsList.Count == 0) {
                Logger.log(new String[] { "Library", "txAbort", "nothing to abort" });
            }

            bool result = false;
            IServer server;

            foreach(PadIntRegistry pd in padIntsList) {
                server = (IServer) Activator.GetObject(typeof(IServer), pd.Address);
                result = result && server.abort(actualTID, pd.PdInts);
            }

            padIntsList.Clear();
            return result;
        }



        public static PadIntStub createPadInt(int uid) {
            Logger.log(new String[] { "Library", "createPadInt", uid.ToString() });

            try {
                Tuple<int, string> serverInfo = masterServer.registerPadInt(uid);
                string serverAddr = serverInfo.Item2;
                int serverID = serverInfo.Item1;
                IServer server = (IServer) Activator.GetObject(typeof(IServer), serverAddr);
                server.createPadInt(uid);
                padIntsList.Insert(serverID, new PadIntRegistry(serverAddr));
                return new PadIntStub(uid, actualTID, serverID, serverAddr);
            } catch(PadIntAlreadyExistsException) {
                throw;
            }
        }

        public static PadIntStub accessPadInt(int uid) {
            Logger.log(new String[] { "Library", "accessPadInt", "uid", uid.ToString() });

            try {
                Tuple<int, string> serverInfo = masterServer.getPadIntServer(uid);
                string serverAddr = serverInfo.Item2;
                int serverID = serverInfo.Item1;
                IServer server = (IServer) Activator.GetObject(typeof(IServer), serverAddr);
                server.confirmPadInt(uid);
                padIntsList.Insert(serverID, new PadIntRegistry(serverAddr));
                return new PadIntStub(uid, actualTID, serverID, serverAddr);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(NoServersFoundException) {
                throw;
            }
        }

        public static void registerUID(int serverID, int uid) {
            Logger.log(new String[] { "Library", "registerWrite", "uid", uid.ToString() });
            PadIntRegistry registry = padIntsList.ElementAtOrDefault(serverID);
            if(registry!=null) {
                registry.PdInts.Add(uid);
            } else {
                throw new WrongPadIntRequestException(uid, actualTID);
            }
        }
    }
}
