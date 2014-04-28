using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer {
    /// <summary>
    /// This class represents the Master server
    /// </summary>
    class Master : MarshalByRefObject, IMaster {

        /// <summary>
        /// Constant used to represent non atributed server identifier
        /// </summary>
        private const int NO_SERVER_ID = -1;
        /// <summary>
        /// Constant used to represent non atributed server address
        /// </summary>
        private const string NO_SERVER_ADDRESS = "";
        /// <summary>
        /// Identifier of the last transaction started
        /// </summary>
        private int lastTransactionIdentifier;
        /// <summary>
        /// Number of servers registed on Master server
        /// </summary>
        private int numberOfRegisteredServers;
        /// <summary>
        /// List of registered servers indexed by server identifier
        /// </summary>
        private List<string> registeredServers;
        /// <summary>
        /// Identifier of the server waiting for role assignment
        /// </summary>
        private int pendingServerID;
        /// <summary>
        /// Address of the server waiting for role assignment
        /// </summary>
        private string pendingServerAddress;
        /// <summary>
        /// Dictionary storing tuple(server identifier, predicate identifying if padInt was already stored) 
        /// indexed by PadInt identifier. The predicate is relevant because it may be allocated 20 PadInts on 
        /// a single server, but only one slotted is used at a time, leaving the remaining for future requests
        /// </summary>
        private Dictionary<int, Tuple<int, bool>> padIntServers;

        internal int LastTID {
            get { return this.lastTransactionIdentifier; }
            set { this.lastTransactionIdentifier = value; }
        }

        internal int NServers {
            get { return this.numberOfRegisteredServers; }
            set { this.numberOfRegisteredServers = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Master() {
            registeredServers = new List<string>();
            padIntServers = new Dictionary<int, Tuple<int, bool>>();
            pendingServerID = NO_SERVER_ID;
            pendingServerAddress = NO_SERVER_ADDRESS;
            LastTID = 0;
            NServers = 0;
        }

        /// <summary>
        /// Returns the identifier of the next transaction
        /// </summary>
        /// <returns>transaction identifier</returns>
        public int getNextTID() {
            Logger.log(new String[] { "Master", "getNextID" });
            return LastTID++;
        }

        /// <summary>
        /// Obtains the identifier for a given server's address
        /// </summary>
        /// <param name="address">Server address</param>
        private int getServerId(string address) {
            for(int index = 0; index < registeredServers.Count; index++) {
                if(registeredServers[index] == address) {
                    return index;
                }
            }

            throw new ServerNotFoundException(address);
        }

        /// <summary>
        /// Assigns primary role to a server
        /// </summary>
        /// <param name="primaryAddress">Primary server address</param>
        /// <param name="backupAddress">Backup server address</param>
        /// <param name="id">Primary server identifier</param>
        /// <param name="padInts">Structure that maps UID to PadInt</param>
        private void createPrimaryServer(string primaryAddress, string backupAddress, int id, Dictionary<int, IPadInt> padInts) {

            IServer server = (IServer) Activator.GetObject(typeof(IServer), primaryAddress);
            server.createPrimaryServer(backupAddress, id, padInts);
        }

        /// <summary>
        /// Assigns backup role to a server
        /// </summary>
        /// <param name="primaryAddress">Primary server address</param>
        /// <param name="backupAddress">Backup server address</param>
        /// <param name="id">Backup server identifier</param>
        /// <param name="padInts">Structure that maps UID to PadInt</param>
        private void createBackupServer(string primaryAddress, string backupAddress, int id, Dictionary<int, IPadInt> padInts) {

            IServer server = (IServer) Activator.GetObject(typeof(IServer), backupAddress);
            server.createBackupServer(primaryAddress, id, padInts);
        }

        /// <summary>
        /// If possible assign a new backup server, redistribute PadInts otherwise
        /// </summary>
        /// <param name="primaryId">Primary server identifier</param>
        /// <param name="backupAddress">Backup server address</param>
        /// <param name="padInts">Structure that maps UID to PadInt</param>
        public void createNewReplica(int primaryId, string backupAddress, Dictionary<int, IPadInt> padInts) {
            Logger.log(new String[] { "Master", "createNewReplica", "primaryId", primaryId.ToString(), "backupAddress", backupAddress.ToString() });
            try {
                /* verify if the primary server exists */
                verifyServerRegistry(primaryId);

                string primaryAddress = registeredServers[primaryId];

                /* if a server is available to be the new backup server */
                if(pendingServerID != NO_SERVER_ID) {
                    createPrimaryServer(primaryAddress, pendingServerAddress, primaryId, padInts);
                    createBackupServer(primaryAddress, pendingServerAddress, primaryId, padInts);

                    /* cleans pending server variables */
                    pendingServerID = NO_SERVER_ID;
                    pendingServerAddress = NO_SERVER_ADDRESS;
                } else {
                    /* redistribute server's PadInts */
                    //TODO

                    /* puts the backup server in the pending variables */
                    pendingServerID = primaryId;
                    pendingServerAddress = primaryAddress;

                    //TODO
                    throw new NotImplementedException();
                }
            } catch(ServerNotFoundException) {
                throw;
            }
        }

        /// <summary>
        /// If possible backup server becames primary server, redistribute PadInts otherwise
        /// </summary>
        /// <param name="primaryAddress">Primary server address</param>
        /// <param name="backupAddress">Backup server address</param>
        /// <param name="padInts">Structure that maps UID to PadInt</param>
        public void becomePrimary(int primaryId, string backupAddress, Dictionary<int, IPadInt> padInts) {
            Logger.log(new String[] { "Master", "becomePrimary", "primaryId", primaryId.ToString(), "backupAddress", backupAddress.ToString() });
            try {
                /* verify if the primary server exists */
                verifyServerRegistry(primaryId);

                /* if a server is available to be the new backup server */
                if(pendingServerID != NO_SERVER_ID) {
                    createPrimaryServer(backupAddress, pendingServerAddress, primaryId, padInts);
                    createBackupServer(backupAddress, pendingServerAddress, primaryId, padInts);

                    /* cleans pending server variables */
                    pendingServerID = NO_SERVER_ID;
                    pendingServerAddress = NO_SERVER_ADDRESS;
                } else {
                    /* redistribute server's PadInts */
                    //TODO

                    /* puts the backup server in the pending variables */
                    pendingServerID = primaryId;
                    pendingServerAddress = backupAddress;

                    //TODO
                    throw new NotImplementedException();
                }
            } catch(ServerNotFoundException) {
                throw;
            }
        }

        /// <summary>
        /// Registers a new server on Master server.
        /// </summary>
        /// <param name="address">Server Address</param>
        /// <returns>Server identifier</returns>
        public int registerServer(String address) {
            Logger.log(new String[] { "Master", "registerServer", "address", address.ToString() });
            try {
                /* assign roles to the servers if exists a possible (primary,backup) pair */
                if(pendingServerID == NO_SERVER_ID) {
                    pendingServerID = NServers;
                    pendingServerAddress = address;
                    return NServers++;
                } else {
                    /* assign primary role to the pending server and backup role to the new server */
                    /* both servers start with a new padInt dictionary */
                    createPrimaryServer(pendingServerAddress, address, pendingServerID, new Dictionary<int, IPadInt>());
                    /* do the primary registry */
                    registeredServers.Insert(pendingServerID, pendingServerAddress);
                    createBackupServer(pendingServerAddress, address, pendingServerID, new Dictionary<int, IPadInt>());

                    /* cleans pending server variables */
                    int temp = pendingServerID;
                    pendingServerID = NO_SERVER_ID;
                    pendingServerAddress = NO_SERVER_ADDRESS;
                    return temp;
                }
            } catch(ArgumentException) {
                throw new ServerAlreadyExistsException(NServers);
            }
        }


        /// <summary>
        /// Returns the information of the server where the PadInt is stored
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns>Tuple containing (server identifier,server address)</returns>
        public Tuple<int, string> getPadIntServer(int uid) {
            Logger.log(new String[] { "Master", " getPadIntServer", "uid", uid.ToString() });
            try {
                int serverID = padIntServers[uid].Item1;
                verifyServerRegistry(serverID);
                return new Tuple<int, string>(serverID, registeredServers[serverID]);
            } catch(ServerNotFoundException) {
                throw;
            }
        }

        /// <summary>
        /// Returns the information of the server where the PadInt should be stored.
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns>Tuple containing (server identifier,server address)</returns>
        public Tuple<int, string> registerPadInt(int uid) {
            Logger.log(new String[] { "Master", " registerPadInt", "uid", uid.ToString() });
            try {
                int serverID = MasterServer.LoadBalancer.getAvailableServer(registeredServers);
                padIntServers.Add(uid, new Tuple<int, bool>(serverID, true));
                //registeredServers[serverID].Hits+=1;
                return new Tuple<int, string>(serverID, registeredServers[serverID]);
            } catch(ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, padIntServers[uid].Item1);
            } catch(NoServersFoundException) {
                throw;
            }
        }

        /// <summary>
        /// Requests all registered servers to dump their internal state
        /// </summary>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        public bool Status() {

            Console.WriteLine("-----------------------");
            Console.WriteLine("This is master server and the last TID given was " + LastTID);
            Console.WriteLine("Servers registered are:");
            for(int i = 0; i < registeredServers.Count; i++) {
                Console.WriteLine("Server " + i + " with address " + registeredServers[i]);
            }

            foreach(string serverAddr in registeredServers) {
                IServer server = (IServer) Activator.GetObject(typeof(IServer), serverAddr);
                server.Dump();
            }
            Console.WriteLine("-----------------------");

            return true;
        }

        /// <summary>
        /// Verifies if a server is already registered on master server
        /// </summary>
        /// <param name="serverID">server identifier</param>
        private void verifyServerRegistry(int serverID) {
            if(registeredServers.ElementAtOrDefault(serverID) == null) {
                throw new ServerNotFoundException(serverID);
            }
        }
    }
}
