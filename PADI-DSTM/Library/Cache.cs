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
        private static List<ServerRegistry> serverList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverAddress">Server Address</param>
        internal ClientCache() {
            serverList = new List<ServerRegistry>();
        }

        internal List<ServerRegistry> ServersWPadInts {
            get { return serverList; }
        }

        public ServerRegistry GetServer(int serverID) {
            return serverList.ElementAtOrDefault(serverID);
        }

        public void AddServer(int serverID, string serverAddr) {
            ServersWPadInts.Insert(serverID, new ServerRegistry(serverAddr));
        }

        private PadIntRegistry GetPadInt(int serverID, int uid) {
            return GetServer(serverID).getPadInt(uid);
        }

        /// <summary>
        /// Associates an uid, to a server and a transaction, so it is later involved in commit or abort
        /// </summary>
        /// <param name="serverID">Server identifier</param>
        /// <param name="uid">PadInt identifier</</param>
        internal void AddPadInt(int serverID, int tid, PadIntRegistry pd) {
            ServerRegistry serverRegistry = GetServer(serverID);
            if(serverRegistry != null) {
                serverRegistry.PdInts.Add(pd);
            } else {
                throw new WrongPadIntRequestException(pd.UID, tid);
            }
        }


        /// <summary>
        /// See if a previous write has occurred
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <returns>True if a previous write has occurred</returns>
        /*internal bool padIntWasWrite(int serverID, int uid) {
            //isto vai rebentar aqui!
            return getPadInt(serverID, uid).WasWrite;
        }*/

        /// <summary>
        /// See if a given uid is in cache
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <returns>Returns true if the uid is in cache</returns>
        internal bool HasPadInt(int serverID, int uid) {
            return GetPadInt(serverID, uid) != null;
        }

        /// <summary>
        /// Obtains PadInt's value stored in cache
        /// </summary>
        /// <param name="uid">PadInt's value</param>
        /// <returns>PadInt's value</returns>
        internal int GetPadIntValue(int serverID, int uid) {
            return GetPadInt(serverID, uid).Value;
        }

        /// <summary>
        /// Updates PadInt's write value store in cache
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <param name="value">Value to assign</param>
        internal void UpdatePadIntValue(int serverID, int uid, int value) {
            GetPadInt(serverID, uid).Value = value;
        }



        /* Before commit do writes of the values in cache */
        internal void FlushCache(int tid) {
            foreach(ServerRegistry server in serverList) {
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
