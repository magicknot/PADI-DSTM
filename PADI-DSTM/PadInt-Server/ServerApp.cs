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

            Console.Title = "Server";
            Random random = new Random();
            int randomNumber = random.Next(0, 100);

            Server padIntServer = new Server();
            TcpChannel channel = new TcpChannel(8000 + randomNumber);
            ChannelServices.RegisterChannel(channel, false);

            if(padIntServer.init(randomNumber)) {
                RemotingServices.Marshal(padIntServer, "PadIntServer", typeof(IServer));
                Console.WriteLine("Server up and running on port " + (8000 + randomNumber));
            }

            while(true)
                ;
        }
    }
}
