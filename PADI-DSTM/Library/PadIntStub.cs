using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace ClientLibrary {
    public class PadIntStub {
        private int uid;
        private string address;
        private int tid;
        private Library library;

        public PadIntStub(int uid, int tid, string address, Library library) {
            this.uid = uid;
            this.address = address;
            this.library = library;
        }

        public int read() {
            IServer server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:"+ address + "/PadIntServer");
            return server.readPadInt(tid, uid);
        }
        public bool write(int value) {
            IServer server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:"+ address + "/PadIntServer");

            if(server.writePadInt(tid, uid, value)) {
                library.registerWrite(uid);
                return true;
            } else {
                return false;
            }
        }

    }
}
