using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary {
    class ServerRegistry {

        /// <summary>
        /// List of PadInts indexed by uid
        /// </summary>
        private List<PadIntRegistry> padIntList;
        /// <summary>
        /// Server address
        /// </summary>
        private string serverAddress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverAddress">Server Address</param>
        public ServerRegistry(string serverAddress) {
            padIntList = new List<PadIntRegistry>();
            this.serverAddress = serverAddress;
        }

        /// <summary>
        /// PadInt list accessor
        /// </summary>
        internal List<PadIntRegistry> PdInts {
            set { this.padIntList = value; }
            get { return padIntList; }
        }

        /// <summary>
        /// Server Address accessor
        /// </summary>
        internal string Address {
            set { this.serverAddress = value; }
            get { return serverAddress; }
        }

        public PadIntRegistry getPadInt(int uid) {
            foreach(PadIntRegistry pd in padIntList) {
                if(pd.UID == uid) {
                    return pd;
                }
            }
            return null;
        }

        public void addPadInt(PadIntRegistry pd) {
            padIntList.Add(pd);
        }
    }
}
