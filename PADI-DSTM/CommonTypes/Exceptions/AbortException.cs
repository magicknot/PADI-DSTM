using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    /// <summary>
    /// This exception is thrown when a abort occurs in a PadInt
    /// </summary>
    [Serializable]
    public class AbortException : IPadiException {
        /// <summary>
        /// Transaction identifier
        /// </summary>
        private int tid;
        /// <summary>
        /// PadInt identifier
        /// </summary>
        private int uid;

        internal int TID {
            get { return this.uid; }
        }

        internal int UID {
            get { return this.uid; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uid">Transaction identifier</param>
        /// <param name="uid">PadInt identifier</param>
        public AbortException(int tid, int uid) {
            this.tid = tid;
            this.uid = uid;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AbortException(System.Runtime.Serialization.SerializationInfo info,
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
            return "Transaction" + tid + "abort in PadInt with identifier " + uid;
        }
    }
}
