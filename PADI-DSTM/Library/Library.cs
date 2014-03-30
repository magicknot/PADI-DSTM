using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using CommonTypes;
using System.Collections;


namespace ClientLibrary {



    public class Library {

        private const int NINTSPERSERVER = 10;
        private IMaster masterServer;
        private int nServers;
        private int actualTID;
        private List<int> writtenList;

        //Qual é a ideia de guardar timers?

        public Library() {

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            masterServer = (IMaster)Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
            writtenList = new List<int>();
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

        public bool txCommit() {
            writtenList.Sort();
            throw new NotImplementedException();
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

            String address = masterServer.getServerAddress(serverID);

            IServer server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:"+address + "/PadIntServer");
            server.createPadInt(uid);

            return new PadIntStub(uid, actualTID, address, this);


        }

        public PadIntStub accessPadInt(int uid) {

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

            String address = masterServer.getServerAddress(serverID);

            IServer server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:"+ address + "/PadIntServer");
            if(server.confirmPadInt())
                return new PadIntStub(uid, actualTID, address, this);
            else
                return null;
        }

        protected void registerWrite(int uid) {
            writtenList.Add(uid);
        }

    }

}
