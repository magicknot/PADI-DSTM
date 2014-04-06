using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    public class WrongServerRequestException : IPadiException {
        private int uid;
        private int serverID;

        public WrongServerRequestException(int uid, int serverID) {
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
            return "The request envolving " + uid + " on server " + serverID + " can't be fulfilled";
        }
    }
}
