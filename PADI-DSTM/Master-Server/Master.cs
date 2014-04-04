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
        private int maxServerCapacity;

        public Master() {
            registeredServers = new Dictionary<int, string>();
            lastTID = 0;
            nServers = 0;

        }

        public int getNextTID() {
            Console.WriteLine(DateTime.Now + " Master " + " getNextID ");
            return lastTID++;
        }

        public int registerServer(String address) {
            Console.WriteLine(DateTime.Now + " Master " + " registerServer " + " address " + address);

            try {
                registeredServers.Add(nServers++, address);
                return nServers;
            } catch(ArgumentException) {
                return -1;
            }
        }

        public Tuple<Dictionary<int, string>, int> getServersList() {
            Console.WriteLine(DateTime.Now + " Master " + " getNServers " + registeredServers.Count);
            return new Tuple<Dictionary<int, string>, int>(registeredServers, maxServerCapacity);
        }

        public Dictionary<int, string> updateMaxCapacity() {
            maxServerCapacity = 2* maxServerCapacity;
            return getServersList().Item1;
        }

        /*public String getServerAddress(int serverID) {
            Console.WriteLine(DateTime.Now + " Master " + " getServerAddress " + " serverID " + serverID);
            return registeredServers[serverID];
        }*/



    }




}
