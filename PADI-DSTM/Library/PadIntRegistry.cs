using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary {
    class PadIntRegistry {

        private List<int> padIntsList;
        private string serverAddress;

        public PadIntRegistry(string serverAddress) {
            padIntsList = new List<int>();
            this.serverAddress=serverAddress;
        }

        internal List<int> PdInts {
            set { this.padIntsList = value; }
            get { return padIntsList; }
        }

        internal string Address {
            set { this.serverAddress = value; }
            get { return serverAddress; }
        }

    }
}
