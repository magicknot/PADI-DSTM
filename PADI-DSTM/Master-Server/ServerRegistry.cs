using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer {
    /// <summary>
    /// This class represents the registry of a PadInt server on master server
    /// </summary>
    class ServerRegistry {

        /// <summary>
        /// Number of hits on server
        /// </summary>
        private int numberOfHits;
        /// <summary>
        /// Server address
        /// </summary>
        private string serverAddress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverAddress">Server address</param>
        public ServerRegistry(string serverAddress) {
            this.serverAddress=serverAddress;
            numberOfHits=0;
        }

        internal int Hits {
            set { this.numberOfHits = value; }
            get { return numberOfHits; }
        }

        internal string Address {
            set { this.serverAddress = value; }
            get { return serverAddress; }
        }
    }
}
