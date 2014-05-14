using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary {
    class PadIntRegistry {

        private bool wasRead;
        private bool wasWrite;
        private int uid;
        private int numericValue;

        public PadIntRegistry(int uid) {
            this.uid = uid;
            this.Value = 0;
            this.wasRead = false;
            this.wasWrite = false;
        }

        public Boolean WasRead {
            get { return wasRead; }
            set { wasRead = value; }
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
            set { numericValue = value; }
        }
    }
}
