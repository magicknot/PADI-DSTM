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
        internal abstract bool CreatePadInt(int uid);
        internal abstract bool ConfirmPadInt(int uid);
        internal abstract int ReadPadInt(int tid, int uid);
        internal abstract bool WritePadInt(int tid, int uid, int value);
        internal abstract bool Commit(int tid, List<int> usedPadInts);
        internal abstract bool Abort(int tid, List<int> usedPadInts);
    }
}
