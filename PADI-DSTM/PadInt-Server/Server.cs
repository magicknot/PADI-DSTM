using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;


namespace PadIntServer {
    /// <summary>
    /// This class represents the PadInt server
    /// </summary>
    class Server : MarshalByRefObject, IServer {

        /// <summary>
        /// Server's state
        /// </summary>
        private ServerState serverState;
        /// <summary>
        /// Server's old state
        /// </summary>
        private ServerState oldState;
        /// <summary>
        /// Structure that maps UID to PadInt
        /// </summary>
        internal Dictionary<int, IPadInt> padIntDictionary;
        /// <summary>
        /// Pending request list
        /// </summary>
        //private List<Request> requestList = new List<Request>();
        /// <summary>
        /// Server identifier
        /// </summary>
        private int identifier;
        /// <summary>
        /// Reference to master server
        /// </summary>
        IMaster masterServerReference;
        /// <summary>
        /// Primary/backup server
        ///  (backup if this server is the primary server, primary otherwise)
        /// </summary>
        private IServer replicationServer;
        /// <summary>
        /// Primary/backup server's address
        ///  (backup server's address if this server is the primary server,
        ///   primary server's address otherwise)
        /// </summary>
        private string replicationServerAddress;

        public Server() {
            serverState = (ServerState) new FailedServer(this);
            oldState = (ServerState) new FailedServer(this);
            padIntDictionary = new Dictionary<int, IPadInt>();
        }

        internal int ID {
            set { this.identifier = value; }
            get { return identifier; }
        }

        internal IMaster Master {
            set { this.masterServerReference = value; }
            get { return masterServerReference; }
        }

        internal IServer ReplicationServer {
            set { this.replicationServer = value; }
            get { return replicationServer; }
        }

        internal string ReplicationServerAddr {
            set { this.replicationServerAddress = value; }
            get { return replicationServerAddress; }
        }

        internal Dictionary<int, IPadInt> PdInts {
            set { this.padIntDictionary = value; }
            get { return this.padIntDictionary; }
        }

        public bool init(int port) {
            try {
                Master = (IMaster) Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
                ID = Master.registerServer("tcp://localhost:" + (8000 + port) + "/PadIntServer");
            } catch(ServerAlreadyExistsException) {
                throw;
            }
            return true;
        }

        /// <summary>
        /// Changes the server's role to primary role.
        /// </summary>
        /// <param name="backupAddress">Backup server address</param>
        /// <param name="id">Server identifier</param>
        /// <param name="padInts">Structure that maps UID to PadInt</param>
        public void createPrimaryServer(string backupAddress, int id, Dictionary<int, IPadInt> padInts) {
            Logger.log(new String[] { "Server", ID.ToString(), "createPrimaryServer", "backupAddress ", backupAddress, "id ", id.ToString(), "padInts ", padInts.Count.ToString() });
            serverState = new PrimaryServer(this);
            ReplicationServerAddr = backupAddress;
            ReplicationServer = (IServer) Activator.GetObject(typeof(IServer), backupAddress);
            ID = id;
            padIntDictionary = padInts;
        }

        /// <summary>
        /// Changes the server's role to backup role.
        /// </summary>
        /// <param name="primaryAddress">Primary server address</param>
        /// <param name="id">Server identifier</param>
        /// <param name="padInts">Structure that maps UID to PadInt</param>
        public void createBackupServer(string primaryAddress, int id, Dictionary<int, IPadInt> padInts) {
            Logger.log(new String[] { "Server", ID.ToString(), "createBackupServer", "primaryAddress ", primaryAddress, "id ", id.ToString(), "padInts ", padInts.Count.ToString() });
            serverState = new BackupServer(this);
            ReplicationServerAddr = primaryAddress;
            ReplicationServer = (IServer) Activator.GetObject(typeof(IServer), primaryAddress);
            ID = id;
            padIntDictionary = padInts;
        }

        public void ImAlive() {
            Logger.log(new String[] { "Server", ID.ToString(), "ImAlive" });
            serverState.ImAlive();
        }

        public bool createPadInt(int uid) {
            Logger.log(new String[] { "Server", ID.ToString(), "createPadInt", "uid ", uid.ToString() });
            try {
                return serverState.createPadInt(uid);
            } catch(PadIntAlreadyExistsException) {
                throw;
            } catch(ServerDoesNotReplyException) {
                throw;
            }
        }

        public bool confirmPadInt(int uid) {
            Logger.log(new String[] { "Server", ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
            try {
                return serverState.confirmPadInt(uid);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(ServerDoesNotReplyException) {
                throw;
            }
        }

        /* Returns the value of the PadInt when the transaction
         *  has the read/write lock.
         * Throw an exception if PadInt not found. 
         */
        public int readPadInt(int tid, int uid) {
            Logger.log(new String[] { "Server", ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            try {
                return serverState.readPadInt(tid, uid);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(ServerDoesNotReplyException) {
                throw;
            }
        }

        public bool writePadInt(int tid, int uid, int value) {
            Logger.log(new String[] { "Server ", ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            try {
                return serverState.writePadInt(tid, uid, value);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(ServerDoesNotReplyException) {
                throw;
            }
        }

        /// <summary>
        /// Commits a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        public bool commit(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "Server", ID.ToString(), "commit", "tid", tid.ToString() });
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */
            try {
                return serverState.commit(tid, usedPadInts);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(ServerDoesNotReplyException) {
                throw;
            }
        }

        /// <summary>
        /// Aborts a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        public bool abort(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "Server", ID.ToString(), "abort", "tid", tid.ToString() });
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */

            try {
                return serverState.abort(tid, usedPadInts);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(ServerDoesNotReplyException) {
                throw;
            }
        }

        public bool Freeze() {
            oldState = serverState;
            serverState = new FreezedServer(this);
            return true;
        }

        public bool Fail() {
            oldState = serverState;
            serverState = new FailedServer(this);
            return true;
        }
        public bool Recover() {
            serverState = oldState;

            //TODO
            //Trata os pedidos que tinha pendentes

            return true;
        }

        public bool Dump() {
            Console.WriteLine("-----------------------");
            Console.WriteLine("This server has id " + ID);
            Console.WriteLine("PadInts stored on this server are:");
            foreach(KeyValuePair<int, IPadInt> pd in padIntDictionary) {
                Console.WriteLine("PadInt with uid " + pd.Key + " and has value " + ((PadInt) pd.Value).ActualValue);
            }
            Console.WriteLine("-----------------------");
            return true;
        }
    }
}
