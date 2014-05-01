using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    /// <summary>
    /// This exception is thrown on a PadInt is already registed on a PadInt server
    /// </summary>
    [Serializable]
    public class PadIntAlreadyExistsException : IPadiException {
        /// <summary>
        /// PadInt identifier
        /// </summary>
        private int uid;
        /// <summary>
        /// Server identifier
        /// </summary>
        private int serverID;

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
        /// <param name="uid">PadInt identifier</param>
        /// <param name="serverID">Server identifier</param>
        public PadIntAlreadyExistsException(int uid, int serverID) {
            this.uid = uid;
            this.serverID = serverID;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public PadIntAlreadyExistsException(System.Runtime.Serialization.SerializationInfo info,
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
            return "The PadInt with uid " + uid + " already exists on server " + serverID;
        }
    }
}
