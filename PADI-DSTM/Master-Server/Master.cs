using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer {
    class Master : MarshalByRefObject, IMaster {

        private int lastTID;
        private int nServers;
        private Dictionary<int, string> registeredServers;

        public Master() {
            registeredServers = new Dictionary<int, string>();
            lastTID=0;
            nServers=0;

        }

        public int getNextTID() {
            return lastTID++;
        }

        public void registerServer(String address) {
            registeredServers.Add(nServers++, address);
        }

        public int getNServers() {
            return registeredServers.Count;
        }

        public String getServerAddress(int serverID) {
            return registeredServers[serverID];
        }



    }




}
