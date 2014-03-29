using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using CommonTypes;


namespace ClientLibrary {



    class Library {

        private const int NINTSPERSERVER = 10;
        private IMaster masterServer;
        private int nServers;
        private int actualTID;

        public Library() {

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            masterServer = (IMaster)Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
        }

        //porque boolean?
        public Boolean init() {
            nServers = masterServer.getNServers();

            return true;
        }

        public bool txBegin() {
            actualTID = masterServer.getNextTID();
            return true;
        }

        public PadIntStub createPadInt(int uid) {
            int serverID=0;
            if(uid>=NINTSPERSERVER*nServers) {
                serverID=nServers-1;
            } else {
                for(int i=0; i<nServers; i++) {
                    if(uid<(i+1)*NINTSPERSERVER) {
                        serverID=i;
                    }
                }
            }

            IServer server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:"+masterServer.getServerAddress(serverID) + "/PadIntServer");
            server.allocatePadInt(uid);

            return new PadIntStub(uid, this);


        }

    }

}
