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
    class MasterApp {

        private const int MASTERADDRESS = 8086;

        static void Main(string[] args) {

            TcpChannel channel = new TcpChannel(MASTERADDRESS);
            ChannelServices.RegisterChannel(channel, true);
            Master masterServer = new Master();

            RemotingServices.Marshal(masterServer, "MasterServer", typeof(Master));

            Console.WriteLine("Master up and running on port " + MASTERADDRESS);

            while(true)
                ;

        }

    }
}
