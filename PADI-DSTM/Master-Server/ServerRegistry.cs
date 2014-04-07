using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer {
    class ServerRegistry {

        private int numberOfHits;
        private string serverAddress;

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
