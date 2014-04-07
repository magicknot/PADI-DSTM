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
        private List<ServerRegistry> registeredServers;
        private Dictionary<int, int> padIntServers;

        public Master() {
            registeredServers = new List<ServerRegistry>();
            padIntServers = new Dictionary<int, int>();
            lastTID = 0;
            nServers = 0;
        }

        public int getNextTID() {
            Logger.log(new String[] { "Master", "getNextID" });
            return lastTID++;
        }

        public int registerServer(String address) {
            Logger.log(new String[] { "Master", " registerServer", "address", address.ToString() });
            try {
                registeredServers.Insert(nServers,new ServerRegistry(address));
                return nServers++;
            } catch(ArgumentException) {
                throw new ServerAlreadyExistsException(nServers);
            }
        }

        public string getPadIntServer(int uid) {
            Logger.log(new String[] { "Master", " getPadIntServer", "uid", uid.ToString() });
            if(padIntServers.ContainsKey(uid)) {
                return registeredServers[padIntServers[uid]].Address;
            } else {
                throw new NoServersFoundException();
            }
        }

        public string registerPadInt(int uid) {
            Logger.log(new String[] { "Master", " registerPadInt", "uid", uid.ToString() });
            try {
                int serverID = Master_Server.LoadBalancer.getAvailableServer(registeredServers.Count);
                padIntServers.Add(uid, serverID);
                registeredServers[serverID].Hits+=1;
                return registeredServers[serverID].Address;
            } catch(ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, padIntServers[uid]);
            }
        }
    }
}
