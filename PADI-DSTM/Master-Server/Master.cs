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
            lastTID = 0;
            nServers = 0;

        }

        public int getNextTID() {
            Console.WriteLine(DateTime.Now + " Master " + " getNextID ");
            return lastTID++;
        }

        public bool registerServer(String address) {
            Console.WriteLine(DateTime.Now + " Master " + " registerServer " + " address " + address);

            try {
                registeredServers.Add(nServers++, address);
                return true;
            } catch(ArgumentException) {
                return false;
            }
        }

        public Dictionary<int, string> getServersList() {
            Console.WriteLine(DateTime.Now + " Master " + " getNServers " + registeredServers.Count);
            return registeredServers;
        }

        /*public String getServerAddress(int serverID) {
            Console.WriteLine(DateTime.Now + " Master " + " getServerAddress " + " serverID " + serverID);
            return registeredServers[serverID];
        }*/



    }




}
