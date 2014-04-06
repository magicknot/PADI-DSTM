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
        private int maxServerCapacity=10;

        public Master() {
            registeredServers = new Dictionary<int, string>();
            lastTID = 0;
            nServers = 0;
        }

        public int getNextTID() {
            Logger.log(new String[] { "Master", "getNextID" });
            return lastTID++;
        }

        public Tuple<int, int> registerServer(String address) {
            Logger.log(new String[] { "Master", " registerServer", " address ", address.ToString() });

            try {
                registeredServers.Add(nServers, address);
                return new Tuple<int, int>(nServers++, maxServerCapacity);
            } catch(ArgumentException) {
                throw new AlreadyRegisteredException(nServers);
            }
        }

        public Tuple<Dictionary<int, string>, int> getServersInfo(bool increase) {
            Logger.log(new String[] { "Master", "getNServers", registeredServers.Count.ToString() });

            if(increase) {
                maxServerCapacity = 2* maxServerCapacity;
            }

            if(registeredServers.Count==0) {
                throw new NoServersFoundException();
            } else {
                return new Tuple<Dictionary<int, string>, int>(registeredServers, maxServerCapacity);
            }
        }
    }




}
