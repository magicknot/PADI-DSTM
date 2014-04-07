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
        private int serverID;

        public PadIntStub(int uid, int tid, int serverID, string address) {
            this.uid = uid;
            this.tid = tid;
            this.address = address;
            this.serverID=serverID;
        }

        public int read() {
            Logger.log(new String[] { "PadIntStub", "read" });
            try {
                IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
                int result = server.readPadInt(tid, uid);
                Library.registerUID(serverID, uid);
                return result;
            } catch(PadIntNotFoundException) {
                throw;
            }
        }
        public bool write(int value) {
            Logger.log(new String[] { "PadIntStub", "write" + "value" + value.ToString() });
            try {
                IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
                server.writePadInt(tid, uid, value);
                Library.registerUID(serverID, uid);
                return true;
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

    }
}
