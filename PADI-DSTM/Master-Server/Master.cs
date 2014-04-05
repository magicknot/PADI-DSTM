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
            Console.WriteLine(DateTime.Now + " Master " + " getNextID ");
            return lastTID++;
        }

        public Tuple<int, int> registerServer(String address) {
            Console.WriteLine(DateTime.Now + " Master " + " registerServer " + " address " + address);

            try {
                registeredServers.Add(nServers, address);
                return new Tuple<int, int>(nServers++, maxServerCapacity);
            } catch(ArgumentException) {
                return null;
            }
        }

        public Tuple<Dictionary<int, string>, int> getServersInfo(bool increase) {
            Console.WriteLine(DateTime.Now + " Master " + " getNServers " + registeredServers.Count);

            if(increase) {
                maxServerCapacity = 2* maxServerCapacity;
            }

            return new Tuple<Dictionary<int, string>, int>(registeredServers, maxServerCapacity);
        }

    }




}
