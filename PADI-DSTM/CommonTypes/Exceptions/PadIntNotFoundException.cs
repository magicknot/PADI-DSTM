using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    /// <summary>
    /// This exception is thrown when a PadInt is not found on a server
    /// </summary>
    [Serializable]
    public class PadIntNotFoundException : IPadiException {

        /// <summary>
        /// PadInt identifier
        /// </summary>
        private int uid;
        /// <summary>
        /// Server identifier
        /// </summary>
        private int serverID;

        private const int NO_SERVER = -1;

        internal int UID {
            set { this.uid = value; }
            get { return this.uid; }
        }

        internal int ServerID {
            set { this.serverID = value; }
            get { return this.serverID; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="serverID"></param>
        public PadIntNotFoundException(int uid, int serverID) {
            this.uid = uid;
            this.serverID = serverID;
        }

        public PadIntNotFoundException(int uid) {
            this.uid = uid;
            this.serverID = NO_SERVER;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public PadIntNotFoundException(System.Runtime.Serialization.SerializationInfo info,
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
        public override String GetMessage() {
            if(serverID == NO_SERVER) {
                return "The PadInt with uid " + uid + " was not found on any server ";
            } else {
                return "The PadInt with uid " + uid + " was not found on server " + serverID;
            }
        }
    }
}
