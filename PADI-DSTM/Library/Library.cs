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
using System.Runtime.Serialization;


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
            masterServer = (IMaster) Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");


            Logger.log(new String[] { "Library", "init", "\r\n" });
            bool result = updateServersInfo();
            Logger.log(new String[] { " " });

            return result;
        }

        private bool updateServersInfo() {
            try {
                Tuple<Dictionary<int, string>, int> serversInfo = masterServer.getServersInfo(false);
                serversList = serversInfo.Item1;
                maxServerCapacity = serversInfo.Item2;
            } catch(SerializationException e) {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            } catch(IPadiException e) {
                e.getMessage();
                return false;
            }

            return true;
        }

        public bool txBegin() {
            Logger.log(new String[] { "Library", "txBegin" });
            actualTID = masterServer.getNextTID();
            Logger.log(new String[] { " " });
            return true;
        }

        public bool txCommit() {
            Logger.log(new String[] { "Library", "txCommit" });

            if(writtenList.Count==0) {
                Logger.log(new String[] { "Library", "txCommit", "nothing to commit" });
            }

            updateServersInfo();
            writtenList.Sort();
            int serverID = getPadIntServerID(writtenList.First());
            int tempServerID;
            List<int> toCommitList = new List<int>();
            bool result = false;

            foreach(int i in writtenList) {
                tempServerID = getPadIntServerID(i);

                if(tempServerID != serverID) {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), serversList[serverID]);
                    result = result && server.commit(actualTID, toCommitList);
                    serverID = tempServerID;
                    toCommitList = new List<int>();
                }
                toCommitList.Add(i);
            }

            Logger.log(new String[] { " " });
            return result;
        }

        public bool txAbort() {
            Logger.log(new String[] { "Library", "txCommit" });

            if(writtenList.Count==0) {
                Logger.log(new String[] { "Library", "txCommit", "nothing to abort" });
            }

            updateServersInfo();
            writtenList.Sort();
            int serverID = getPadIntServerID(writtenList.First());
            int tempServerID;
            List<int> toCommitList = new List<int>();
            bool result = false;

            foreach(int i in writtenList) {
                tempServerID = getPadIntServerID(i);

                if(tempServerID != serverID) {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), serversList[serverID]);
                    result = server.abort(actualTID, toCommitList) && result;
                    serverID = tempServerID;
                    toCommitList = new List<int>();
                }
                toCommitList.Add(i);
            }

            Logger.log(new String[] { " " });
            return result;
        }

        public PadIntStub createPadInt(int uid) {
            Logger.log(new String[] { "Library", "createPadInt", uid.ToString() });

            String address = serversList[getPadIntServerID(uid)];
            IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
            bool possible = server.createPadInt(uid);

            Logger.log(new String[] { " " });

            if(possible)
                return new PadIntStub(uid, actualTID, address, this);
            else
                return null;
        }

        public PadIntStub accessPadInt(int uid) {
            Logger.log(new String[] { "Library", "accessPadInt", "uid", uid.ToString() });

            String address = serversList[getPadIntServerID(uid)];
            IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
            bool accessible = server.confirmPadInt(uid);

            Logger.log(new String[] { " " });

            if(accessible)
                return new PadIntStub(uid, actualTID, address, this);
            else
                return null;
        }

        public int getNServers() {
            Logger.log(new String[] { "Library", "getNServers" });
            return serversList.Count;
        }

        public int getPadIntServerID(int uid) {
            Logger.log(new String[] { "Library", "getPadIntServerID", "uid", uid.ToString() });
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
            Logger.log(new String[] { "Library", "registerWrite", "uid", uid.ToString() });
            writtenList.Add(uid);
        }
    }
}
