using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace CommonTypes {

    /// <summary>
    /// This exception is thrown when a library tries to involve in commit or abort requests,
    /// the uid of a PadInt not accessed nor created during the current transaction 
    /// </summary>
    [Serializable]
    public class WrongPadIntRequestException : IPadiException {

        /// <summary>
        /// PadInt identifier
        /// </summary>
        private int uid;
        /// <summary>
        /// Transaction identifier
        /// </summary>
        private int tid;

        internal int UID {
            set { this.uid = value; }
            get { return this.uid; }
        }

        internal int TID {
            set { this.TID = value; }
            get { return this.TID; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <param name="tid">Transaction identifier</param>
        public WrongPadIntRequestException(int uid, int tid) {
            this.uid = uid;
            this.tid = tid;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public WrongPadIntRequestException(System.Runtime.Serialization.SerializationInfo info,
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
        public override String getMessage() {
            return "The padInt with uid " + uid + " was not accessed nor created on the context of transaction with tid " + tid;
        }

    }
}
