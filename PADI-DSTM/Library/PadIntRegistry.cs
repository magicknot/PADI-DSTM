using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary {
    class PadIntRegistry {

        private bool wasWrite;
        private int uid;
        private int numericValue;

        public PadIntRegistry(int uid, int value, bool wasWrite) {
            this.uid = uid;
            this.Value = value;
            this.wasWrite = wasWrite;
        }

        public Boolean WasWrite {
            get { return wasWrite; }
            set { wasWrite = value; }
        }

        public int UID {
            get { return uid; }
            set { this.uid = value; }
        }

        public int Value {
            get { return numericValue; }
            set {
                WasWrite = true;
                numericValue = value;
            }
        }
    }
}
