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
            Random random = new Random();
int randomNumber = random.Next(0, 100);


            TcpChannel channel = new TcpChannel(8000+randomNumber);
            ChannelServices.RegisterChannel(channel, true);

            RemotingServices.Marshal(padIntServer, "PadIntServer", typeof(IServer));
            masterServer = (IMaster)Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
            int serverID = masterServer.registerServer("tcp://localhost:"+(8000+randomNumber)+"/PadIntServer");
            if(serverID>0) {
                padIntServer.setID(serverID);
            }


            Console.WriteLine("Server up and running on port " + (8000+randomNumber));
            while(true)
                ;
        }
    }
}
