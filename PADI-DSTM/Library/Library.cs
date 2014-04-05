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

        public Library() {

            writtenList = new List<int>();
        }

        //porque boolean?
        public bool init() {

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            log(new String[] { "Library", "init" });

            masterServer = (IMaster) Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
            Tuple<Dictionary<int, string>, int> serversInfo = masterServer.getServersInfo(false);
            serversList = serversInfo.Item1;
            maxServerCapacity = serversInfo.Item2;

            log(new String[] { " " });

            return serversList.Count != 0;
        }

        public bool txBegin() {
            log(new String[] { "Library", "txBegin" });
            actualTID = masterServer.getNextTID();
            log(new String[] { " " });
            return true;
        }

        public bool txCommit() {
            log(new String[] { "Library", "txCommit" });

            int serverID = getPadIntServerID(writtenList.First());
            int tempServerID;
            List<int> toCommitList = new List<int>();
            writtenList.Sort();

            foreach(int i in writtenList) {
                tempServerID = getPadIntServerID(i);

                if(tempServerID != serverID) {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), serversList[serverID]);
                    server.commit(actualTID, toCommitList);
                    serverID = tempServerID;
                    toCommitList= new List<int>();
                }
                toCommitList.Add(i);
            }

            log(new String[] { " " });
            return true;
        }

        public bool txAbort() {
            log(new String[] { "Library", "txCommit" });

            int serverID = getPadIntServerID(writtenList.First());
            int tempServerID;
            List<int> toCommitList = new List<int>();
            writtenList.Sort();

            foreach(int i in writtenList) {
                tempServerID = getPadIntServerID(i);

                if(tempServerID != serverID) {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), serversList[serverID]);
                    server.abort(actualTID, toCommitList);
                    serverID = tempServerID;
                    toCommitList= new List<int>();
                }
                toCommitList.Add(i);
            }

            log(new String[] { " " });
            return true;
        }

        public PadIntStub createPadInt(int uid) {
            log(new String[] { "Library", "createPadInt", uid.ToString() });

            String address = serversList[getPadIntServerID(uid)];
            IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
            bool possible = server.createPadInt(uid);

            log(new String[] { " " });

            if(possible)
                return new PadIntStub(uid, actualTID, address, this);
            else
                return null;
        }

        public PadIntStub accessPadInt(int uid) {
            log(new String[] { "Library", "accessPadInt", "uid", uid.ToString() });

            String address = serversList[getPadIntServerID(uid)];
            IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
            bool accessible = server.confirmPadInt(uid);

            log(new String[] { " " });

            if(accessible)
                return new PadIntStub(uid, actualTID, address, this);
            else
                return null;
        }

        public int getNServers() {
            log(new String[] { "Library", "getNServers" });
            return serversList.Count;
        }

        public int getPadIntServerID(int uid) {
            log(new String[] { "Library", "getPadIntServerID", "uid", uid.ToString() });
            int serverID = 0;
            int nServers = getNServers();

            for(int i = 0; i < nServers; i++) {
                if(uid < (i + 1) * maxServerCapacity) {
                    return serverID = i;
                }
            }

            return serverID = nServers - 1;
        }

        public void registerWrite(int uid) {
            log(new String[] { "Library", "registerWrite", "uid", uid.ToString() });
            writtenList.Add(uid);
        }

        public void log(String[] args) {
            ILog logServer = (ILog) Activator.GetObject(typeof(ILog), "tcp://localhost:7002/LogServer");
            logServer.log(args);
        }
    }
}
