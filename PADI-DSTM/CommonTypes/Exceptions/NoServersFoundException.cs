using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace CommonTypes {
    /// <summary>
    /// This exception is thrown when there are no servers registered on master server
    /// </summary>
    [Serializable]
    public class NoServersFoundException : IPadiException {

        /// <summary>
        /// Constructor
        /// </summary>
        public NoServersFoundException() {
        }

        /// <summary>
        /// Default exception construtor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public NoServersFoundException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }

        /// <summary>
        /// Default exception constructor
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
        public override string GetMessage() {
            return "Master server doesn't have any server registered";
        }
    }
}
