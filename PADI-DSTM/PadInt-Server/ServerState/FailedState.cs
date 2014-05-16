using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CommonTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PadIntServer {
    class FailedState : ServerState {

        internal FailedState(Server server)
            : base(server, new Dictionary<int, IPadInt>()) {
            StateMsg = "FAILED STATE";
        }

        /// <summary>
        /// Frailed servers do nothing when this method is called
        /// </summary>
        internal override void ImAlive() {
            Logger.Log(new String[] { "FailedServer", "ImAlive" });
        }

        internal override bool CreatePadInt(int uid) {
            Logger.Log(new String[] { "FailedServer", Server.ID.ToString(), "createPadInt", "uid ", uid.ToString() });
            throw new ServerDoesNotReplyException(Server.ID);
        }

        internal override bool ConfirmPadInt(int uid) {
            Logger.Log(new String[] { "FailedServer", Server.ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
            throw new ServerDoesNotReplyException(Server.ID);
        }

        internal override int ReadPadInt(int tid, int uid) {
            Logger.Log(new String[] { "FailedServer", Server.ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });
            throw new ServerDoesNotReplyException(Server.ID);
        }

        internal override bool WritePadInt(int tid, int uid, int value) {
            Logger.Log(new String[] { "FailedServer", Server.ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });
            throw new ServerDoesNotReplyException(Server.ID);
        }

        /// <summary>
        /// Commits a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool Commit(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "FailedServer", Server.ID.ToString(), "commit", "tid", tid.ToString() });
            throw new ServerDoesNotReplyException(Server.ID);
        }

        /// <summary>
        /// Aborts a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool Abort(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "FailedServer", Server.ID.ToString(), "abort", "tid", tid.ToString() });
            throw new ServerDoesNotReplyException(Server.ID);
        }

        internal override bool Recover() {
            RemotingServices.Marshal(Server, "PadIntServer", typeof(IServer));
            return true;
        }
    }
}
