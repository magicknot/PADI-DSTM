using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CommonTypes;

namespace PadIntServer {
    abstract class ServerState : IDisposable {

        /// <summary>
        /// Server that has this state
        /// </summary>
        private Server server;
        /// <summary>
        /// Timer used in I'm Alive mechanism
        /// </summary>
        internal PadIntTimer imAliveTimer;


        /// <summary>
        /// Primary/backup server
        ///  (backup if this server is the primary server, primary otherwise)
        /// </summary>
        protected IServer pairServerReference;

        /// <summary>
        /// Primary/backup server's address
        ///  (backup server's address if this server is the primary server,
        ///   primary server's address otherwise)
        /// </summary>
        protected string pairServerAddress;

        internal ServerState(Server server) {
            this.server = server;
        }

        internal Server Server {
            set { this.server = value; }
            get { return this.server; }
        }

        internal abstract void ImAlive();
        internal abstract bool CreatePadInt(int uid);
        internal abstract int ReadPadInt(int tid, int uid);
        internal abstract bool WritePadInt(int tid, int uid, int value);
        internal abstract bool Commit(int tid, List<int> usedPadInts);
        internal abstract bool Abort(int tid, List<int> usedPadInts);
        internal virtual bool Recover() { return false; }


        protected virtual void VerifyPadInts(List<int> padInts) {
            try {
                foreach(int uid in padInts) {
                    GetPadInt(uid);
                }
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        public virtual void Dispose() {
            imAliveTimer.Dispose(true);
        }

        protected virtual PadInt GetPadInt(int uid) {
            if(Server.PdInts.ContainsKey(uid)) {
                return (PadInt) Server.PdInts[uid];
            } else {
                throw new PadIntNotFoundException(uid, Server.ID);
            }
        }

        internal virtual bool ConfirmPadInt(int uid) {
            try {
                GetPadInt(uid);
                return true;
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        public void MovePadInts(List<int> padInts, string receiverAddress) {
            Logger.Log(new String[] { "ServerState", "MovePadInts", "to Server", receiverAddress });
            Dictionary<int, IPadInt> removedPadInt = new Dictionary<int, IPadInt>();

            foreach(int padIntId in padInts) {
                removedPadInt.Add(padIntId, server.PdInts[padIntId]);
                server.PdInts.Remove(padIntId);
            }

            IServer receiverServer = (IServer) Activator.GetObject(typeof(IServer), receiverAddress);
            receiverServer.ReceivePadInts(removedPadInt);
        }

        public void ReceivePadInts(Dictionary<int, IPadInt> receivedPadInts) {
            Logger.Log(new String[] { "ServerState", "ReceivePadInts" });
            foreach(KeyValuePair<int, IPadInt> pair in receivedPadInts) {
                server.padIntDictionary.Add(pair.Key, pair.Value);
            }
        }
    }
}
