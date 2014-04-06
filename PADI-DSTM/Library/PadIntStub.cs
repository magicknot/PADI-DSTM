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

        /* Structure that stores PadInt's uid of the transaction read locks */
        private List<int> readLocks;

        /* Structure that stores PadInt's uid of the transaction writes locks */
        private List<int> writeLocks;

        /* Structure that maps UID to PadInt's actual value */
        private Dictionary<int, int> padIntDict;

        public PadIntStub(int uid, int tid, string address, Library library) {
            this.uid = uid;
            this.tid = tid;
            this.address = address;
            this.library = library;
            this.readLocks = new List<int>();
            this.writeLocks = new List<int>();
            this.padIntDict = new Dictionary<int, int>();
        }

        public int read() {
            Logger.log(new String[] { "PadIntStub", "read" });
            IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
            return server.readPadInt(tid, uid);
        }
        public bool write(int value) {
            Logger.log(new String[] { "PadIntStub", "write" + "value" + value.ToString() });
            IServer server = (IServer) Activator.GetObject(typeof(IServer), address);

            if(server.writePadInt(tid, uid, value)) {
                library.registerWrite(uid);
                return true;
            } else {
                return false;
            }
        }

    }
}
