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
    /// <summary>
    /// This class represents the PadInt server application
    /// </summary>
    class ServerApp {
        private const int MASTERADDRESS = 8001;

        static void Main(string[] args) {

            Console.Title = "Server";
            int port;

            if(args.Length > 0) {
                port = Int32.Parse(args[0]);
            } else {
                Random random = new Random();
                port = 8000 + random.Next(0, 100);
            }

            Server padIntServer = new Server();

            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);

            try {
                RemotingServices.Marshal(padIntServer, "PadIntServer", typeof(IServer));
                padIntServer.Init(port);
                Console.WriteLine("Server up and running on port " + (port));
            } catch(ServerAlreadyExistsException e) {
                Console.WriteLine(e.GetMessage());
            }

            while(true)
                ;
        }
    }
}
