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
                registeredServers.Insert(nServers, new ServerRegistry(address));
                return nServers++;
            } catch(ArgumentException) {
                throw new ServerAlreadyExistsException(nServers);
            }
        }

        public Tuple<int, string> getPadIntServer(int uid) {
            Logger.log(new String[] { "Master", " getPadIntServer", "uid", uid.ToString() });
            try {
                int serverID = padIntServers[uid];
                verifyServerRegistry(serverID);
                return new Tuple<int, string>(serverID, registeredServers[serverID].Address);
            } catch(NoServersFoundException) {
                throw;
            }
        }

        public Tuple<int, string> registerPadInt(int uid) {
            Logger.log(new String[] { "Master", " registerPadInt", "uid", uid.ToString() });
            try {
                int serverID = MasterServer.LoadBalancer.getAvailableServer(registeredServers);
                padIntServers.Add(uid, serverID);
                registeredServers[serverID].Hits+=1;
                return new Tuple<int, string>(serverID, registeredServers[serverID].Address);
            } catch(ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, padIntServers[uid]);
            } catch(NoServersFoundException) {
                throw;
            }
        }

        private void verifyServerRegistry(int serverID) {
            if(registeredServers.ElementAtOrDefault(serverID)==null) {
                throw new NoServersFoundException();
            }
        }
    }
}
