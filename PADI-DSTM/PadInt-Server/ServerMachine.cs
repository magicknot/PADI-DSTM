using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadIntServer {
    class ServerMachine : MarshalByRefObject, IServerMachine, IDisposable {

        private const int MASTERADDRESS = 8001;
        private static Server padIntServer;
        private string serverAddress;

        public string Address {
            get { return serverAddress; }
            set { serverAddress = value; }
        }

        public Server Server {
            get { return padIntServer; }
            set { padIntServer = value; }
        }

        public ServerMachine(string address) {
            padIntServer = new Server(address);
            Address = address;
        }

        public static void killServer() {
            padIntServer = null;
        }

        public void restartServer(int id) {
            if(padIntServer == null) {
                padIntServer = new Server(Address);
                padIntServer.ID = id;
            }
        }

        public void Dispose() {
            padIntServer.Dispose();
        }
    }
}
