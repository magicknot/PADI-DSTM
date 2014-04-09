using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    /// <summary>
    /// This exception is thrown when a server tries to register itself with an 
    /// identifier which was already registered on master server
    /// </summary>
    [Serializable]
    public class ServerAlreadyExistsException : IPadiException {

        /// <summary>
        /// Server identifier
        /// </summary>
        int serverID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverID">Server identifier</param>
        public ServerAlreadyExistsException(int serverID) {
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
        public ServerAlreadyExistsException(System.Runtime.Serialization.SerializationInfo info,
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
            return "The server with id " + serverID + " is already registered on master server";
        }

    }
}
