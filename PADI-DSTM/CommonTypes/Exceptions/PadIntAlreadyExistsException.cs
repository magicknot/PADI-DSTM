using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    [Serializable]
    public class PadIntAlreadyExistsException : IPadiException {
        private int uid;
        private int serverID;

        public PadIntAlreadyExistsException(int uid, int serverID) {
            this.uid = uid;
            this.serverID = serverID;
        }

        public PadIntAlreadyExistsException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }

        public int getUid() {
            return uid;
        }

        public int getServerID() {
            return serverID;
        }

        public override String getMessage() {
            return "The PadInt with uid " + uid + " already exists on server " + serverID;
        }
    }
}
