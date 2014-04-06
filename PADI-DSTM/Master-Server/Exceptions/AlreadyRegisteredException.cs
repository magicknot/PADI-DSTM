using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer {
    class AlreadyRegisteredException : IPadiException {
        int serverID;

        public AlreadyRegisteredException(int serverID) {
            this.serverID = serverID;
        }

        public int getServerID() {
            return serverID;
        }

        public override string getMessage() {
            return "The server with id " + serverID + " is already registered on master server";
        }


        public int getUid() {
            throw new NotImplementedException();
        }
    }
}
