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
        /*Master Server reference*/
        private static IMaster masterServer;
        /*Identifier of the current transaction*/
        private static int actualTID;
        /*List of PadInts stored on each server*/
        private static List<PadIntRegistry> padIntsList;
        /*Tcp Channel in use*/
        private static TcpChannel channel;

        public static TcpChannel Channel {
            get { return channel; }
        }

        /// <summary>
        /// Creates Tcp channel, and gets a reference to master server
        /// </summary>
        /// <returns> a predicate confirming the sucess of the operations</returns>
        public static bool Init() {
            padIntsList = new List<PadIntRegistry>();
            channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            Logger.Log(new String[] { "Library", "init", "\r\n" });
            masterServer = (IMaster) Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
            return true;
        }

        /// <summary>
        /// Starts a new transactions by requesting master server of a new TID
        /// </summary>
        /// <returns>a predicate confirming the sucess of the operations</returns>
        public static bool TxBegin() {
            Logger.Log(new String[] { "Library", "txBegin" });
            actualTID = masterServer.GetNextTID();
            Logger.Log(new String[] { " " });
            return true;
        }

        /// <summary>
        /// Commits a transaction on every involved server
        /// </summary>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        public static bool TxCommit() {
            Logger.Log(new String[] { "Library", "txCommit" });

            if(padIntsList.Count == 0) {
                Logger.Log(new String[] { "Library", "txCommit", "nothing to commit" });
            }

            bool result = true;
            IServer server;

            foreach(PadIntRegistry pd in padIntsList) {
                server = (IServer) Activator.GetObject(typeof(IServer), pd.Address);
                result = server.Commit(actualTID, pd.PdInts) && result;
            }

            padIntsList.Clear();
            return result;
        }

        /// <summary>
        /// Aborts a transaction on every involved server
        /// </summary>
        /// <returns>a predicate confirming the sucess of the operations</returns>
        public static bool TxAbort() {
            Logger.Log(new String[] { "Library", "txAbort" });

            if(padIntsList.Count == 0) {
                Logger.Log(new String[] { "Library", "txAbort", "nothing to abort" });
            }

            bool result = true;
            IServer server;

            foreach(PadIntRegistry pd in padIntsList) {
                server = (IServer) Activator.GetObject(typeof(IServer), pd.Address);
                result = server.Abort(actualTID, pd.PdInts) && result;
            }

            padIntsList.Clear();
            return result;
        }


        /// <summary>
        /// Requests the creation of a PadInt on a remote server
        /// </summary>
        /// <param name="uid"> PadInt identifier</param>
        /// <returns>A stub of created padInt</returns>
        public static PadIntStub CreatePadInt(int uid) {
            Logger.Log(new String[] { "Library", "createPadInt", uid.ToString() });

            try {
                Tuple<int, string> serverInfo = masterServer.RegisterPadInt(uid);
                string serverAddr = serverInfo.Item2;
                int serverID = serverInfo.Item1;
                IServer server = (IServer) Activator.GetObject(typeof(IServer), serverAddr);
                server.CreatePadInt(uid);
                padIntsList.Insert(serverID, new PadIntRegistry(serverAddr));
                return new PadIntStub(uid, actualTID, serverID, serverAddr);
            } catch(PadIntAlreadyExistsException) {
                throw;
            }
        }

        /// <summary>
        /// Requests a PadInt on a remote server
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns>A stub of request padInt</returns>
        public static PadIntStub AccessPadInt(int uid) {
            Logger.Log(new String[] { "Library", "accessPadInt", "uid", uid.ToString() });

            try {
                Tuple<int, string> serverInfo = masterServer.GetPadIntServer(uid);
                string serverAddr = serverInfo.Item2;
                int serverID = serverInfo.Item1;
                IServer server = (IServer) Activator.GetObject(typeof(IServer), serverAddr);
                server.ConfirmPadInt(uid);
                padIntsList.Insert(serverID, new PadIntRegistry(serverAddr));
                return new PadIntStub(uid, actualTID, serverID, serverAddr);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(NoServersFoundException) {
                throw;
            }
        }

        /// <summary>
        /// Associates an uid, to a server and a transaction, so it is later involved in commit or abort
        /// </summary>
        /// <param name="serverID">Server identifier</param>
        /// <param name="uid">PadInt identifier</</param>
        public static void RegisterUID(int serverID, int uid) {
            Logger.Log(new String[] { "Library", "registerWrite", "uid", uid.ToString() });
            PadIntRegistry registry = padIntsList.ElementAtOrDefault(serverID);
            if(registry != null) {
                registry.PdInts.Add(uid);
            } else {
                throw new WrongPadIntRequestException(uid, actualTID);
            }
        }

        /// <summary>
        /// A request is send to Master server, asking for all nodes to be dumped.
        /// </summary>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        public static bool Status() {
            masterServer.Status();
            return true;
        }
    }
}
