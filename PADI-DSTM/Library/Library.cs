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

        private IMaster masterServer;
        private Dictionary<int, String> serversList;
        private int actualTID;
        private List<int> writtenList;
        private int maxServerCapacity;

        //Qual é a ideia de guardar timers?

        public Library() {


            writtenList = new List<int>();
        }

        //porque boolean?
        public Boolean init() {
            Console.WriteLine(DateTime.Now + " Library " + " init ");

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            masterServer = (IMaster)Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
            Tuple<Dictionary<int, string>, int> serversInfo = masterServer.getServersList();
            serversList = serversInfo.Item1;
            maxServerCapacity = serversInfo.Item2;
            return serversList.Count != 0;
        }

        public bool txBegin() {
            Console.WriteLine(DateTime.Now + " Library " + " txBegin ");
            actualTID = masterServer.getNextTID();
            return true;
        }

        public bool txCommit() {
            Console.WriteLine(DateTime.Now + " Library " + " txCommit ");
            writtenList.Sort();
            throw new NotImplementedException();
        }

        public PadIntStub createPadInt(int uid) {
            Console.WriteLine(DateTime.Now + " Library " + " createPadInt " + uid);
        
            String address = serversList[getPadIntServerID(uid)];
            Console.WriteLine(address);
            IServer server = (IServer)Activator.GetObject(typeof(IServer), address);
            bool possible = server.createPadInt(uid);

            if(possible)
                return new PadIntStub(uid, actualTID, address, this);
            else
                return null;
        }


        public PadIntStub accessPadInt(int uid) {
            Console.WriteLine(DateTime.Now +  " Library " + " accessPadInt " + " uid " + uid);

            String address = serversList[getPadIntServerID(uid)];
            IServer server = (IServer)Activator.GetObject(typeof(IServer), address);
            bool accessible = server.confirmPadInt(uid);

            if(accessible)
                return new PadIntStub(uid, actualTID, address, this);
            else
                return null;
        }

        public int getNServers() {
            Console.WriteLine(DateTime.Now +  " Library " + " getNServers ");
            return serversList.Count;
        }

        public int getPadIntServerID(int uid) {
            Console.WriteLine(DateTime.Now +  " Library " + " getPadIntServerID " + " uid " + uid);
            int serverID = 0;

            for(int i = 0; i < getNServers(); i++) {
                if(uid < (i + 1) * maxServerCapacity) {
                    return serverID = i;
                }
            }

            return serverID = getNServers() - 1;
        }


        public void registerWrite(int uid) {
            Console.WriteLine(DateTime.Now + " Library " + " registerWrite " + " uid " + uid);
            writtenList.Add(uid);
        }

    }

}
