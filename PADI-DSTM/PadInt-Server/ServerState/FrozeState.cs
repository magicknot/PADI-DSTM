using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace PadIntServer {
    class FrozenState : ServerState {

        /// <summary>
        /// Server's old state
        /// </summary>
        private ServerState oldState;

        internal FrozenState(Server server)
            : base(server) {
            oldState = server.State;
        }

        /// <summary>
        /// Freezed servers do nothing when this method is called
        /// </summary>
        internal override void ImAlive() {
            //Nothing to do here
        }

        internal override bool CreatePadInt(int uid) {
            Logger.Log(new String[] { "FreezedServer", Server.ID.ToString(), "createPadInt", "uid ", uid.ToString() });

            //TODO cria o pedido e guarda-o
            throw new ServerDoesNotReplyException(Server.ID);
        }

        internal override bool ConfirmPadInt(int uid) {
            Logger.Log(new String[] { "FreezedServer", Server.ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });

            //TODO cria o pedido e guarda-o
            throw new ServerDoesNotReplyException(Server.ID);
        }

        /* Returns the value of the PadInt when the transaction
         *  has the read/write lock.
         * Throw an exception if PadInt not found. 
         */
        internal override int ReadPadInt(int tid, int uid) {
            Logger.Log(new String[] { "FreezedServer", Server.ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            //TODO cria o pedido e guarda-o
            throw new ServerDoesNotReplyException(Server.ID);
        }

        internal override bool WritePadInt(int tid, int uid, int value) {
            Logger.Log(new String[] { "FreezedServer", Server.ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            //TODO cria o pedido e guarda-o
            throw new ServerDoesNotReplyException(Server.ID);
        }

        /// <summary>
        /// Commits a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool Commit(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "FreezedServer", Server.ID.ToString(), "commit", "tid", tid.ToString() });

            //TODO cria o pedido e guarda-o
            throw new ServerDoesNotReplyException(Server.ID);
        }

        /// <summary>
        /// Aborts a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool Abort(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "FreezedServer", Server.ID.ToString(), "abort", "tid", tid.ToString() });

            //TODO cria o pedido e guarda-o
            throw new ServerDoesNotReplyException(Server.ID);
        }

        internal override bool Recover() {
            return false;
        }
    }
}
