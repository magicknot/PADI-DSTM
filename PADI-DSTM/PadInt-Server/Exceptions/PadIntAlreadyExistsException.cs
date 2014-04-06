using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace PadIntServer {
    class PadIntAlreadyExistsException : IPadiException {
        private int uid;
        private int serverID;

        public PadIntAlreadyExistsException(int uid, int serverID) {
            this.uid = uid;
            this.serverID = serverID;
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
