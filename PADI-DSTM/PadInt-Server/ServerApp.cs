using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PadIntServer {
    class ServerApp {
        private const int MASTERADDRESS = 8001;

        static void Main(string[] args) {
            IMaster masterServer;
            Server padIntServer = new Server();

            TcpChannel channel = new TcpChannel(8001);
            ChannelServices.RegisterChannel(channel, true);

            RemotingServices.Marshal(padIntServer, "PadIntServer", typeof(IServer));
            masterServer = (IMaster)Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
            int serverID = masterServer.registerServer("tcp://localhost:8001/PadIntServer");
            if(serverID>0) {
                padIntServer.setID(serverID);
            }


            Console.WriteLine("Server up and running on port " + MASTERADDRESS);
            while(true)
                ;
        }
    }
}
