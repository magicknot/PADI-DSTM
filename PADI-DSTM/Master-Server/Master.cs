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
        /// Constant used to represent non atributed server address
        /// </summary>
        private const string NO_SERVER_ADDRESS = "";
        /// <summary>
        /// Identifier of the last transaction started
        /// </summary>
        private int lastTransactionIdentifier;
        /// <summary>
        /// List of registered servers indexed by server identifier
        /// </summary>
        private List<string> registeredServers;
        private bool serverIsPrimary;
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

        /// <summary>
        /// Constructor
        /// </summary>
        public Master() {
            registeredServers = new List<string>();
            padIntServers = new Dictionary<int, Tuple<int, bool>>();
            LastTID = 0;
            serverIsPrimary = true;
        }

        /// <summary>
        /// Returns the identifier of the next transaction
        /// </summary>
        /// <returns>transaction identifier</returns>
        public int GetNextTID() {
            Logger.Log(new String[] { "Master", "getNextID" });
            return LastTID++;
        }

        /// <summary>
        /// Verifies if a server is already registered on master server
        /// </summary>
        /// <param name="serverID">server identifier</param>
        private void VerifyServerRegistry(int serverID) {
            if(registeredServers.ElementAtOrDefault(serverID) == null) {
                throw new ServerNotFoundException(serverID);
            }
        }

        /// <summary>
        /// Registers a new server on Master server.
        /// </summary>
        /// <param name="address">Server Address</param>
        /// <returns>Server identifier</returns>
        public Tuple<int, string> RegisterServer(String address) {
            Logger.Log(new String[] { "Master", "registerServer", "address", address.ToString() });
            try {
                if(serverIsPrimary) {
                    registeredServers.Insert(registeredServers.Count, address);
                    serverIsPrimary = false;
                    return new Tuple<int, string>(registeredServers.Count - 1, NO_SERVER_ADDRESS);
                } else {
                    serverIsPrimary = true;
                    return new Tuple<int, string>(registeredServers.Count - 1, registeredServers[registeredServers.Count - 1]);
                }
            } catch(ArgumentException) {
                throw new ServerAlreadyExistsException(registeredServers.Count - 1);
            }
        }

        /// <summary>
        /// Returns the information of the server where the PadInt is stored
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns>Tuple containing (server identifier,server address)</returns>
        public Tuple<int, string> GetPadIntServer(int uid) {
            Logger.Log(new String[] { "Master", " getPadIntServer", "uid", uid.ToString() });
            try {
                int serverID = padIntServers[uid].Item1;
                VerifyServerRegistry(serverID);
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
        public Tuple<int, string> RegisterPadInt(int uid) {
            Logger.Log(new String[] { "Master", " registerPadInt", "uid", uid.ToString() });
            try {
                int serverID = MasterServer.LoadBalancer.GetAvailableServer(registeredServers);
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
    }
}
