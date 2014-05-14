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
        /// <summary>
        /// Master Server reference
        /// </summary>
        private static IMaster masterServer;
        /// <summary>
        /// Identifier of the current transaction
        /// </summary>
        private static int actualTID;
        /// <summary>
        /// Cache used to store PadInt's values
        /// </summary>
        private static ClientCache cache;
        /// <summary>
        /// Tcp Channel in use
        /// </summary>
        private static TcpChannel channel;

        /// <summary>
        /// Creates Tcp channel, and gets a reference to master server
        /// </summary>
        /// <returns> a predicate confirming the sucess of the operations</returns>
        public static bool Init() {
            Logger.Log(new String[] { "Library", "init", "\r\n" });
            channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
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
            cache = new ClientCache();
            return true;
        }

        /// <summary>
        /// Commits a transaction on every involved server
        /// </summary>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        public static bool TxCommit() {
            Logger.Log(new String[] { "Library", "txCommit" });
            bool result = true;
            IServer server;

            if(cache.ServersWPadInts.Count == 0) {
                Logger.Log(new String[] { "Library", "txCommit", "nothing to commit" });
                return result;
            }

            cache.FlushCache(actualTID);

            List<int> commitList = new List<int>();

            foreach(ServerRegistry srvr in cache.ServersWPadInts.Values) {
                foreach(PadIntRegistry pd in srvr.PdInts) {
                    commitList.Add(pd.UID);
                }
                server = (IServer) Activator.GetObject(typeof(IServer), srvr.Address);
                result = server.Commit(actualTID, commitList) && result;

                commitList = new List<int>();
            }

            cache.ServersWPadInts.Clear();
            return result;
        }

        /// <summary>
        /// Aborts a transaction on every involved server
        /// </summary>
        /// <returns>a predicate confirming the sucess of the operations</returns>
        public static bool TxAbort() {
            Logger.Log(new String[] { "Library", "txAbort" });
            bool result = true;
            IServer server;

            if(cache.ServersWPadInts.Count == 0) {
                Logger.Log(new String[] { "Library", "txAbort", "nothing to abort" });
                return result;
            }

            List<int> abortList = new List<int>();

            foreach(ServerRegistry srvr in cache.ServersWPadInts.Values) {
                foreach(PadIntRegistry pd in srvr.PdInts) {
                    abortList.Add(pd.UID);
                }

                server = (IServer) Activator.GetObject(typeof(IServer), srvr.Address);
                result = server.Abort(actualTID, abortList) && result;

                abortList = new List<int>();
            }

            cache.ServersWPadInts.Clear();
            return result;
        }


        /// <summary>
        /// Requests the creation of a PadInt on a remote server
        /// </summary>
        /// <param name="uid"> PadInt identifier</param>
        /// <returns>A stub of created padInt</returns>
        public static PadInt CreatePadInt(int uid) {
            Logger.Log(new String[] { "Library", "createPadInt", uid.ToString() });

            try {
                Tuple<int, string> serverInfo = masterServer.RegisterPadInt(uid);
                string serverAddr = serverInfo.Item2;
                int serverID = serverInfo.Item1;
                IServer server = (IServer) Activator.GetObject(typeof(IServer), serverAddr);
                server.CreatePadInt(uid);

                if(!cache.HasServer(serverID)) {
                    cache.AddServer(serverID, serverAddr);
                }
                cache.AddPadInt(serverID, actualTID, new PadIntRegistry(uid));
                return new PadInt(uid, actualTID, serverID, serverAddr, cache);
            } catch(PadIntAlreadyExistsException) {
                throw;
            } catch(WrongPadIntRequestException) {
                throw;
            }
        }

        /// <summary>
        /// Requests a PadInt on a remote server
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns>A stub of request padInt</returns>
        public static PadInt AccessPadInt(int uid) {
            Logger.Log(new String[] { "Library", "accessPadInt", "uid", uid.ToString() });

            try {
                Tuple<int, string> serverInfo = masterServer.GetPadIntServer(uid);
                string serverAddr = serverInfo.Item2;
                int serverID = serverInfo.Item1;
                IServer server = (IServer) Activator.GetObject(typeof(IServer), serverAddr);
                server.ConfirmPadInt(uid);

                if(!cache.HasServer(serverID)) {
                    cache.AddServer(serverID, serverAddr);
                }
                cache.AddPadInt(serverID, actualTID, new PadIntRegistry(uid));
                return new PadInt(uid, actualTID, serverID, serverAddr, cache);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(NoServersFoundException) {
                throw;
            } catch(WrongPadIntRequestException) {
                throw;
            }
        }

        /// <summary>
        /// Sends freeze request to a server
        /// </summary>
        /// <param name="address">server's address</param>
        /// <returns>true if successful</returns>
        public static bool Freeze(String address) {
            Logger.Log(new String[] { "Library", "Freeze", "address", address });
            IServer server = (IServer) Activator.GetObject(typeof(IServer), address);

            return server.Freeze();
        }

        /// <summary>
        /// Sends fail request to a server
        /// </summary>
        /// <param name="address">server's address</param>
        /// <returns>true if successful</returns>
        public static bool Fail(String address) {
            Logger.Log(new String[] { "Library", "Fail", "address", address });

            IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
            return server.Fail();
        }

        /// <summary>
        /// Sends recover request to a server
        /// </summary>
        /// <param name="address">server's address</param>
        /// <returns>true if successful</returns>
        public static bool Recover(String address) {
            Logger.Log(new String[] { "Library", "Recover", "address", address });

            IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
            return server.Recover();
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
