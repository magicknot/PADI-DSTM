using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace LogServer {
    class LogApp {
        static void Main(string[] args) {

            Console.Title = "Log";

            TcpChannel channel = new TcpChannel(7002);
            ChannelServices.RegisterChannel(channel, false);
            Log logServer = new Log();

            RemotingServices.Marshal(logServer, "LogServer", typeof(Log));

            Console.WriteLine("Log up and running on port " + 7002);


            while(true)
                ;
        }
    }
}
