using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace ClientLibrary {
    public class PadIntStub {

        /// <summary>
        /// Identifier of remote PadInt
        /// </summary>
        private int uid;
        /// <summary>
        /// Address of server where remote PadInt is stored
        /// </summary>
        private string address;
        /// <summary>
        /// Identifier of transaction
        /// </summary>
        private int tid;
        /// <summary>
        /// Identifer of server where remote PadInt is stored
        /// </summary>
        private int serverID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <param name="tid">Transaction identifier</param>
        /// <param name="serverID">Server identifier</param>
        /// <param name="address">Server address</param>
        public PadIntStub(int uid, int tid, int serverID, string address) {
            this.uid = uid;
            this.tid = tid;
            this.address = address;
            this.serverID=serverID;
        }

        /// <summary>
        /// Reads the value of remote PadInt
        /// </summary>
        /// <returns>The value of the remote PadInt</returns>
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

        /// <summary>
        /// Writes the value on remote PadInt
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
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
