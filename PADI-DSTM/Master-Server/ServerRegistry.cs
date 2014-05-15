using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MasterServer {
    /// <summary>
    /// This class represents the registry of a PadInt server on master server
    /// </summary>
    class ServerRegistry {

        /// <summary>
        /// Server address
        /// </summary>
        private string serverAddress;
        /// <summary>
        /// Server identifier
        /// </summary>
        private int serverID;

        private List<int> padInts;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverAddress">Server address</param>
        public ServerRegistry(int serverID, string serverAddress) {
            this.serverAddress = serverAddress;
            this.serverID = serverID;
            this.padInts = new List<int>();
        }

        internal int Hits {
            get { return padInts.Count; }
        }

        internal string Address {
            set { this.serverAddress = value; }
            get { return serverAddress; }
        }

        internal int ID {
            set { this.serverID = value; }
            get { return serverID; }
        }

        public bool HasPadInt(int uid) {
            return padInts.Contains(uid);
        }

        public void AddPadInt(int uid) {
            padInts.Add(uid);
        }

        public int RemovePadInt() {
            int pd = padInts.First<int>();
            padInts.RemoveAt(0);
            return pd;
        }
    }

    class ServerComparer : IComparer<ServerRegistry> {
        public int Compare(ServerRegistry x, ServerRegistry y) {
            return x.Hits - y.Hits;
        }
    }

    class ServerReverseComparer : IComparer<ServerRegistry> {
        public int Compare(ServerRegistry x, ServerRegistry y) {
            return y.Hits - x.Hits;
        }
    }
}
