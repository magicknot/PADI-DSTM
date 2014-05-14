using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace ClientLibrary {
    class ClientCache {

        /// <summary>
        /// List of PadInts stored on each server
        /// </summary>
        private static Dictionary<int, ServerRegistry> serverList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverAddress">Server Address</param>
        internal ClientCache() {
            serverList = new Dictionary<int, ServerRegistry>();
        }

        internal Dictionary<int, ServerRegistry> ServersWPadInts {
            get { return serverList; }
        }

        internal bool HasServer(int serverID) {
            Logger.Log(new String[] { "Cache", "HasServer", "serverID", serverID.ToString() });
            return serverList.ContainsKey(serverID);
        }

        internal ServerRegistry GetServer(int serverID) {
            Logger.Log(new String[] { "Cache", "GetServer", "serverID", serverID.ToString() });
            return serverList.ElementAtOrDefault(serverID).Value;
        }

        internal void AddServer(int serverID, string serverAddr) {
            Logger.Log(new String[] { "Cache", "AddServer", "serverID", serverID.ToString(), "serverAddr ", serverAddr.ToString() });
            ServersWPadInts.Add(serverID, new ServerRegistry(serverAddr));
        }

        private PadIntRegistry GetPadInt(int serverID, int uid) {
            Logger.Log(new String[] { "Cache", "GetPadInt", "serverID", serverID.ToString(), "uid", uid.ToString() });
            return GetServer(serverID).GetPadInt(uid);
        }

        /// <summary>
        /// Associates an uid, to a server and a transaction, so it is later involved in commit or abort
        /// </summary>
        /// <param name="serverID">Server identifier</param>
        /// <param name="uid">PadInt identifier</</param>
        internal void AddPadInt(int serverID, PadIntRegistry pd) {
            Logger.Log(new String[] { "Cache", "AddPadInt", "serverID", serverID.ToString() });

            ServerRegistry serverRegistry = GetServer(serverID);

            if(serverRegistry != null) {
                serverRegistry.PdInts.Add(pd);
            } else {
                throw new WrongPadIntRequestException(pd.UID, serverID);
            }
        }

        /// <summary>
        /// Updates a PadInt's server stored in cache
        /// </summary>
        /// <param name="serverID">Server identifier</param>
        /// <param name="uid">PadInt identifier</param>
        internal void UpdatePadIntServer(int serverID, int uid) {
            Tuple<int, string> serverInfo = Library.MasterServer.GetPadIntServer(uid);
            string serverAddr = serverInfo.Item2;
            int newServerID = serverInfo.Item1;

            //obtains and removes the PadIntRegistry from the old server
            PadIntRegistry pd = GetServer(serverID).RemovePadInt(uid);

            if(!HasServer(serverID)) {
                AddServer(serverID, serverAddr);
            }
            //adds the PadIntRegistry to the new server
            AddPadInt(newServerID, pd);
        }

        /// <summary>
        /// See if a given uid is in cache
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <returns>Returns true if the uid is in cache</returns>
        internal bool HasPadInt(int serverID, int uid) {
            Logger.Log(new String[] { "Cache", "HasPadInt", "serverID", serverID.ToString(), "uid", uid.ToString() });
            return GetPadInt(serverID, uid) != null;
        }

        internal bool HasWritePadInt(int serverID, int uid) {
            return GetPadInt(serverID, uid).WasWrite;
        }

        internal void setPadIntAsWrite(int serverID, int uid) {
            GetPadInt(serverID, uid).WasWrite = true;
        }

        internal bool HasReadPadInt(int serverID, int uid) {
            return GetPadInt(serverID, uid).WasRead;
        }

        internal void setPadIntAsRead(int serverID, int uid) {
            GetPadInt(serverID, uid).WasRead = true;
        }

        /// <summary>
        /// Obtains PadInt's value stored in cache
        /// </summary>
        /// <param name="uid">PadInt's value</param>
        /// <returns>PadInt's value</returns>
        internal int GetPadIntValue(int serverID, int uid) {
            Logger.Log(new String[] { "Cache", "GetPadIntValue", "serverID", serverID.ToString(), "uid", uid.ToString() });
            return GetPadInt(serverID, uid).Value;
        }

        /// <summary>
        /// Updates PadInt's write value store in cache
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <param name="value">Value to assign</param>
        internal void UpdatePadIntValue(int serverID, int uid, int value) {
            Logger.Log(new String[] { "Cache", "UpdatePadIntValue", "serverID", serverID.ToString(), "uid", uid.ToString(), "value", value.ToString() });
            GetPadInt(serverID, uid).Value = value;
        }

        /// <summary>
        /// Before commit do writes of the values in cache
        /// </summary>
        /// <param name="tid">Transaction identifier</param>
        internal void FlushCache(int tid) {
            Logger.Log(new String[] { "Cache", "FlushCache", "tid", tid.ToString() });

            foreach(ServerRegistry server in serverList.Values) {
                foreach(PadIntRegistry padInt in server.PdInts) {
                    if(padInt.WasWrite) {
                        IServer serverWrite = (IServer) Activator.GetObject(typeof(IServer), server.Address);
                        serverWrite.WritePadInt(tid, padInt.UID, padInt.Value);
                    }
                }
            }
        }
    }
}
