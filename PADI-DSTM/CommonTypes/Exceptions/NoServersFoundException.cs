using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    [Serializable]
    public class NoServersFoundException : IPadiException {

        public NoServersFoundException() {
        }

        public NoServersFoundException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }

        public override string getMessage() {
            return "Master server doesn't have any server registered";
        }
    }
}
