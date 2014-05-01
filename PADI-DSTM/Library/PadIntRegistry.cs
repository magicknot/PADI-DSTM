using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary {
    class PadIntRegistry {

        /// <summary>
        /// List of PadInts indexed by uid
        /// </summary>
        private List<int> padIntsList;
        /// <summary>
        /// Server address
        /// </summary>
        private string serverAddress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverAddress">Server Address</param>
        public PadIntRegistry(string serverAddress) {
            padIntsList = new List<int>();
            this.serverAddress = serverAddress;
        }

        /// <summary>
        /// PadInt list accessor
        /// </summary>
        internal List<int> PdInts {
            set { this.padIntsList = value; }
            get { return padIntsList; }
        }

        /// <summary>
        /// Server Address accessor
        /// </summary>
        internal string Address {
            set { this.serverAddress = value; }
            get { return serverAddress; }
        }
    }
}
