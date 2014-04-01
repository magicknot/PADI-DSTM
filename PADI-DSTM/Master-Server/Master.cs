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

        public void registerServer(String address) {
            Console.WriteLine(DateTime.Now + " Master " + " registerServer " + " address " + address);
            registeredServers.Add(nServers++, address);
        }

        public int getNServers() {
            Console.WriteLine(DateTime.Now + " Master " + " getNServers ");
            return registeredServers.Count;
        }

        public String getServerAddress(int serverID) {
            Console.WriteLine(DateTime.Now + " Master " + " getServerAddress " + " serverID " + serverID);
            return registeredServers[serverID];
        }



    }




}
