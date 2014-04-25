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

        /* the server's state */
        private ServerState serverState;
        private ServerState oldState;

        /* Structure that maps UID to PadInt */
        internal Dictionary<int, PadInt> padIntDictionary;

        /* Pending request list */
        //private List<Request> requestList = new List<Request>();

        private int identifier;
        IMaster masterServerReference;

        public Server() {
            serverState = (ServerState) new PrimaryServer(this);
            oldState = (ServerState) new PrimaryServer(this);
            padIntDictionary = new Dictionary<int, PadInt>();
        }

        internal int ID {
            set { this.identifier = value; }
            get { return identifier; }
        }

        internal IMaster Master {
            set { this.masterServerReference = value; }
            get { return masterServerReference; }
        }

        internal Dictionary<int, PadInt> PdInts {
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

        public void createPrimaryServer() {
            serverState = new PrimaryServer(this);
        }

        public void createBackupServer() {
            serverState = new BackupServer(this);
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
            serverState = new FreezeServer(this);
            return true;
        }

        public bool Fail() {
            oldState = serverState;
            serverState = new FailServer(this);
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
            foreach(KeyValuePair<int, PadInt> pd in padIntDictionary) {
                Console.WriteLine("PadInt with uid " + pd.Key + " and has value " + pd.Value.ActualValue);
            }
            Console.WriteLine("-----------------------");
            return true;
        }
    }
}
