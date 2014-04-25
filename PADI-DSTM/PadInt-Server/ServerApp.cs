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
            Random random = new Random();
            int randomNumber = random.Next(0, 100);

            Server padIntServer = new Server();
            /* if the argument received is P then create a primary server.
             * if the argument received is B then create a backup server.
             */
            /*if((args.Length > 0) && (args[0].Equals("P"))) {
                padIntServer.createPrimaryServer();
            } else if((args.Length > 0) && (args[0].Equals("B"))) {
                padIntServer.createBackupServer();
            }*/

            TcpChannel channel = new TcpChannel(8000 + randomNumber);
            ChannelServices.RegisterChannel(channel, false);

            try {
                padIntServer.init(randomNumber);
                RemotingServices.Marshal(padIntServer, "PadIntServer", typeof(IServer));
                Console.WriteLine("Server up and running on port " + (8000 + randomNumber));
            } catch(ServerAlreadyExistsException e) {
                Console.WriteLine(e.getMessage());
            }

            while(true)
                ;
        }
    }
}
