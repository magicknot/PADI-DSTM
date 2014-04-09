using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace MasterServer {
    /// <summary>
    /// This class represents the master server application
    /// </summary>
    class MasterApp {
        private const int MASTERADDRESS = 8086;

        static void Main(string[] args) {

            Console.Title = "Master";

            TcpChannel channel = new TcpChannel(MASTERADDRESS);
            ChannelServices.RegisterChannel(channel, false);
            Master masterServer = new Master();
            RemotingServices.Marshal(masterServer, "MasterServer", typeof(Master));

            Console.WriteLine("Master up and running on port " + MASTERADDRESS);

            while(true)
                ;

        }

    }
}
