using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    /// <summary>
    /// This exception is thrown when a server has state fail or freeze
    /// </summary>
    [Serializable]
    public class ServerDoesNotReplyException : IPadiException {

        /// <summary>
        /// Server identifier
        /// </summary>
        int serverID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID">Server identifier</param>
        public ServerDoesNotReplyException(int serverID) {
            this.serverID = serverID;
        }

        internal int ServerID {
            set { this.serverID = value; }
            get { return this.serverID; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ServerDoesNotReplyException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Returns exception message
        /// </summary>
        /// <returns>message</returns>
        public override string getMessage() {
            return "The server with id " + serverID + " does not reply";
        }

    }
}
