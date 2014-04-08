using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace CommonTypes {
    [Serializable]
    public class WrongPadIntRequestException : IPadiException {
        private int uid;
        private int tid;

        public WrongPadIntRequestException(int uid, int tid) {
            this.uid = uid;
            this.tid = tid;
        }

        public WrongPadIntRequestException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }

        public int getUid() {
            return uid;
        }

        public int getTid() {
            return tid;
        }

        public override String getMessage() {
            return "The padInt with uid " + uid + " was not accessed nor created on the context of transaction with tid " + tid;
        }

    }
}
