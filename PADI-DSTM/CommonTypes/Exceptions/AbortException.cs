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
        /// PadInt identifier
        /// </summary>
        private int uid;

        internal int UID {
            set { this.uid = value; }
            get { return this.uid; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        public AbortException(int uid) {
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
        public override String getMessage() {
            return "Abort in PadInt with identifier " + uid;
        }
    }
}
