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
        private static List<int> writtenList;
        private static Dictionary<int, string> padIntServers;
        private static TcpChannel channel;

        //porque boolean?
        public static bool init() {
            writtenList = new List<int>();
            padIntServers = new Dictionary<int, string>();
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

            if(writtenList.Count == 0) {
                Logger.log(new String[] { "Library", "txCommit", "nothing to commit" });
            }

            writtenList.Sort();
            string serverID = padIntServers[writtenList.First()];
            string tempServerID;
            List<int> toCommitList = new List<int>();
            bool result = false;

            foreach(int i in writtenList) {
                tempServerID = padIntServers[i];

                if(!tempServerID.Equals(serverID)) {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), serverID);
                    result = result && server.commit(actualTID, toCommitList);
                    serverID = tempServerID;
                    toCommitList = new List<int>();
                }
                toCommitList.Add(i);
            }

            Logger.log(new String[] { " " });
            return result;
        }

        public static bool txAbort() {
            Logger.log(new String[] { "Library", "txAbort" });

            if(writtenList.Count == 0) {
                Logger.log(new String[] { "Library", "txAbort", "nothing to abort" });
            }

            writtenList.Sort();
            string serverID = padIntServers[writtenList.First()];
            string tempServerID;
            List<int> toAbortList = new List<int>();
            bool result = false;

            foreach(int i in writtenList) {
                tempServerID = padIntServers[i];

                if(!tempServerID.Equals(serverID)) {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), serverID);
                    result = result && server.abort(actualTID, toAbortList);
                    serverID = tempServerID;
                    toAbortList = new List<int>();
                }
                toAbortList.Add(i);
            }

            Logger.log(new String[] { " " });
            return result;
        }



        public static PadIntStub createPadInt(int uid) {
            Logger.log(new String[] { "Library", "createPadInt", uid.ToString() });

            try {
                string serverAddr = masterServer.registerPadInt(uid);
                IServer server = (IServer) Activator.GetObject(typeof(IServer), serverAddr);
                server.createPadInt(uid);
                return new PadIntStub(uid, actualTID, serverAddr);
            } catch(PadIntAlreadyExistsException) {
                throw;
            }
        }

        public static PadIntStub accessPadInt(int uid) {
            Logger.log(new String[] { "Library", "accessPadInt", "uid", uid.ToString() });

            try {
                string serverAddr = masterServer.getPadIntServer(uid);
                IServer server = (IServer) Activator.GetObject(typeof(IServer), serverAddr);
                server.confirmPadInt(uid);
                return new PadIntStub(uid, actualTID, serverAddr);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(NoServersFoundException) {
                throw;
            }
        }

        public static void registerUID(int uid) {
            Logger.log(new String[] { "Library", "registerWrite", "uid", uid.ToString() });
            writtenList.Add(uid);
        }
    }
}
