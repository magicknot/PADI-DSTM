using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PadIntServer {
    abstract class ServerState {

        /// <summary>
        /// Server that has this state
        /// </summary>
        private Server server;

        internal ServerState(Server server) {
            this.server = server;
        }

        internal Server Server {
            set { this.server = value; }
            get { return this.server; }
        }

        internal abstract void ImAlive();
        internal abstract bool createPadInt(int uid);
        internal abstract bool confirmPadInt(int uid);
        internal abstract int readPadInt(int tid, int uid);
        internal abstract bool writePadInt(int tid, int uid, int value);
        internal abstract bool commit(int tid, List<int> usedPadInts);
        internal abstract bool abort(int tid, List<int> usedPadInts);
    }
}
