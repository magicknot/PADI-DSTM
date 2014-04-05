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
            log(new String[] { "Master", "getNextID" });
            return lastTID++;
        }

        public Tuple<int, int> registerServer(String address) {
            log(new String[] { "Master", " registerServer", " address ", address.ToString() });

            try {
                registeredServers.Add(nServers, address);
                return new Tuple<int, int>(nServers++, maxServerCapacity);
            } catch(ArgumentException) {
                return null;
            }
        }

        public Tuple<Dictionary<int, string>, int> getServersInfo(bool increase) {
            log(new String[] { "Master", "getNServers", registeredServers.Count.ToString() });

            if(increase) {
                maxServerCapacity = 2* maxServerCapacity;
            }

            return new Tuple<Dictionary<int, string>, int>(registeredServers, maxServerCapacity);
        }

        internal void log(String[] args) {
            ILog logServer =  (ILog)Activator.GetObject(typeof(ILog), "tcp://localhost:7002/LogServer");
            logServer.log(args);
        }

    }




}
