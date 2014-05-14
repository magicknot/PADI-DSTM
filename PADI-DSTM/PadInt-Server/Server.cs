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
    class Server : MarshalByRefObject, IServer, IDisposable {

        /// <summary>
        /// Constant used to represent non atributed server address
        /// </summary>
        private const string NO_SERVER_ADDRESS = "";
        /// <summary>
        /// Server's state
        /// </summary>
        private ServerState serverState;
        /// <summary>
        /// Structure that maps UID to PadInt
        /// </summary>
        internal Dictionary<int, IPadInt> padIntDictionary;
        /// <summary>
        /// Server identifier
        /// </summary>
        private int identifier;
        /// <summary>
        /// Server address
        /// </summary>
        private string serverAddress;

        public Server(string address) {
            serverState = new FailedState(this);
            padIntDictionary = new Dictionary<int, IPadInt>();
            Address = address;
        }

        internal int ID {
            set { this.identifier = value; }
            get { return identifier; }
        }

        internal string Address {
            set { this.serverAddress = value; }
            get { return serverAddress; }
        }

        internal Dictionary<int, IPadInt> PdInts {
            set { this.padIntDictionary = value; }
            get { return this.padIntDictionary; }
        }

        internal ServerState State {
            set { this.serverState = value; }
            get { return this.serverState; }
        }

        public bool Init(int port) {
            try {
                IMaster master = (IMaster) Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
                Tuple<int, string> info = master.RegisterServer(Address);
                ID = info.Item1;
                string primaryServerAddr = info.Item2;
                if(primaryServerAddr != NO_SERVER_ADDRESS) {
                    CreateBackupServer(primaryServerAddr, new Dictionary<int, IPadInt>());
                }
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
        public void CreatePrimaryServer(string backupAddress, Dictionary<int, IPadInt> padInts) {
            Logger.Log(new String[] { "Server", ID.ToString(), "createPrimaryServer", "backupAddress ", backupAddress, "id ", ID.ToString(), "padInts ", padInts.Count.ToString() });
            serverState = new PrimaryServer(this, backupAddress);
            padIntDictionary = padInts;
        }

        /// <summary>
        /// Changes the server's role to backup role.
        /// </summary>
        /// <param name="primaryAddress">Primary server address</param>
        /// <param name="id">Server identifier</param>
        /// <param name="padInts">Structure that maps UID to PadInt</param>
        public void CreateBackupServer(string primaryAddress, Dictionary<int, IPadInt> padInts) {
            Logger.Log(new String[] { "Server", ID.ToString(), "createBackupServer", "primaryAddress ", primaryAddress, "id ", ID.ToString(), "padInts ", padInts.Count.ToString() });
            serverState = new BackupServer(this, primaryAddress);
            padIntDictionary = padInts;
        }

        public void ImAlive() {
            Logger.Log(new String[] { "Server", ID.ToString(), "ImAlive" });
            serverState.ImAlive();
        }

        public bool CreatePadInt(int uid) {
            Logger.Log(new String[] { "Server", ID.ToString(), "createPadInt", "uid ", uid.ToString() });
            try {
                return serverState.CreatePadInt(uid);
            } catch(PadIntAlreadyExistsException) {
                throw;
            } catch(ServerDoesNotReplyException) {
                throw;
            }
        }

        public bool ConfirmPadInt(int uid) {
            Logger.Log(new String[] { "Server", ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
            try {
                return serverState.ConfirmPadInt(uid);
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
        public int ReadPadInt(int tid, int uid) {
            Logger.Log(new String[] { "Server", ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            try {
                return serverState.ReadPadInt(tid, uid);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(AbortException) {
                throw;
            } catch(ServerDoesNotReplyException) {
                throw;
            }
        }

        public bool WritePadInt(int tid, int uid, int value) {
            Logger.Log(new String[] { "Server ", ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            try {
                return serverState.WritePadInt(tid, uid, value);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(AbortException) {
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
        public bool Commit(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "Server", ID.ToString(), "commit", "tid", tid.ToString() });
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */
            try {
                return serverState.Commit(tid, usedPadInts);
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
        public bool Abort(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "Server", ID.ToString(), "abort", "tid", tid.ToString() });
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */

            try {
                return serverState.Abort(tid, usedPadInts);
            } catch(PadIntNotFoundException) {
                throw;
            } catch(ServerDoesNotReplyException) {
                throw;
            }
        }

        public bool Freeze() {
            Logger.Log(new String[] { "Server", "Freeze" });
            serverState = new FrozeState(this);
            return true;
        }

        public bool Fail() {
            Logger.Log(new String[] { "Server", "Fail" });
            serverState = new FailedState(this);
            ServerMachine.killServer();
            return true;
        }

        public bool Recover() {
            Logger.Log(new String[] { "Server", "Recover" });
            serverState.Recover();
            return true;
        }

        public bool Status() {
            Logger.Log(new String[] { "Server", "Status" });
            Console.WriteLine("-----------------------");
            Console.WriteLine("This server has id " + ID);
            Console.WriteLine("PadInts stored on this server are:");
            foreach(KeyValuePair<int, IPadInt> pd in padIntDictionary) {
                Console.WriteLine("PadInt with uid " + pd.Key + " and has value " + ((PadInt) pd.Value).ActualValue);
            }
            Console.WriteLine("-----------------------");
            return true;
        }

        public void MovePadInts(List<int> padInts, string receiverAddress) {
            serverState.MovePadInts(padInts, receiverAddress);
        }

        public void ReceivePadInts(Dictionary<int, IPadInt> receivedPadInts) {
            serverState.ReceivePadInts(receivedPadInts);
        }

        public void Dispose() {
            serverState.Dispose();
        }
    }
}
