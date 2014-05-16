using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PadIntServer {
    class ServerMachine : MarshalByRefObject, IServerMachine, IDisposable {

        private const int MASTERADDRESS = 8001;
        private static Server padIntServer;
        private string serverAddress;
        private TcpChannel channel;

        public Server PdServer {
            get { return padIntServer; }
            set { padIntServer = value; }
        }

        public ServerMachine(string address, int port) {
            padIntServer = new Server(address, this);
            channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
        }

        public void getNewPort() {
            Random random = new Random();
            int port = 5000 + random.Next(0, 100);
            string address = "tcp://localhost:" + (port) + "/PadIntServer";
            padIntServer.Address = address;
            channel.StopListening(null);
            ChannelServices.UnregisterChannel(channel);
            channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);

        }

        public void KillServer() {
            Logger.Log(new String[] { "Server Machine", "Kill Server" });
            //  channel.StopListening(null);
            RemotingServices.Disconnect(PdServer);
            //  ChannelServices.UnregisterChannel(channel);
        }

        public bool RestartServer() {
            Logger.Log(new String[] { "Server Machine", "Restart Server" });
            //  channel.StartListening(null);
            //  ChannelServices.RegisterChannel(channel, false);
            PdServer.State.Recover();
            return true;
        }

        public void Dispose() {
            padIntServer.Dispose();
        }
    }
}
