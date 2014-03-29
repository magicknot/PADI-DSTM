using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary {
    public class PadIntStub {
        private int uid;
        private Library library;

        public PadIntStub(int uid, Library library) {
            this.uid = uid;
            this.library = library;
        }

        public int read() {
            return 0;
        }
        public void write() { }

    }
}
