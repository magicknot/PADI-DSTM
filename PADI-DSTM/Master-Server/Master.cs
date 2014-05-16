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
        private List<ServerRegistry> registeredServers;
        /// <summary>
        /// It identifies if next server added will be a primary or backup
        /// </summary>
        private bool serverIsPrimary;
        /// <summary>
        /// It maps uids into padInt server identifiers
        /// </summary>
        // private Dictionary<int, int> padIntServers;

        internal int LastTID {
            get { return this.lastTransactionIdentifier; }
            set { this.lastTransactionIdentifier = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Master() {
            registeredServers = new List<ServerRegistry>();
            // padIntServers = new Dictionary<int, int>();
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
        /// Registers a new server on Master server.
        /// </summary>
        /// <param name="address">Server Address</param>
        /// <returns>Server identifier</returns>
        public Tuple<int, string> RegisterServer(String address) {
            Logger.Log(new String[] { "Master", "registerServer", "address", address.ToString() });
            if (serverIsPrimary) {
                registeredServers.Insert(registeredServers.Count, new ServerRegistry(registeredServers.Count, address));
                serverIsPrimary = false;
                return new Tuple<int, string>(registeredServers.Count - 1, NO_SERVER_ADDRESS);
            }
            else {
                serverIsPrimary = true;
                LoadBalancer.DistributePadInts(registeredServers, address);
                return new Tuple<int, string>(registeredServers.Count - 1, registeredServers[registeredServers.Count - 1].Address);
            }
        }

        private ServerRegistry getServerRegistry(int uid) {
            foreach (ServerRegistry srvr in registeredServers) {
                if (srvr.HasPadInt(uid)) {
                    return srvr;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the information of the server where the PadInt is stored
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns>Tuple containing (server identifier,server address)</returns>
        public Tuple<int, string> GetPadIntServer(int uid) {
            Logger.Log(new String[] { "Master", " getPadIntServer", "uid", uid.ToString() });

            ServerRegistry srvr = getServerRegistry(uid);
            if (srvr != null) {
                return new Tuple<int, string>(srvr.ID, srvr.Address);
            }
            else {
                throw new PadIntNotFoundException(uid);
            }
        }

        /// <summary>
        /// Returns the information of the server where the PadInt should be stored.
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns>Tuple containing (server identifier,server address)</returns>
        public Tuple<int, string> RegisterPadInt(int uid) {
            Logger.Log(new String[] { "Master", " registerPadInt", "uid", uid.ToString() });
            int newServerID;
            try {
                newServerID = LoadBalancer.GetAvailableServer(registeredServers, serverIsPrimary);
            }
            catch (NoServersFoundException) {
                throw;
            }

            ServerRegistry oldServer = getServerRegistry(uid);

            if (oldServer == null) {
                registeredServers[newServerID].AddPadInt(uid);
                return new Tuple<int, string>(newServerID, registeredServers[newServerID].Address);
            }
            else {
                throw new PadIntAlreadyExistsException(uid, oldServer.ID);
            }
        }

        public void UpdateServerAddress(int id, string address) {
            Logger.Log(new String[] { "Master", "UpdateServerAddress", "server id", id.ToString(), "address", address.ToString() });
            registeredServers[id].Address = address;
        }

        /// <summary>
        /// Requests all registered servers to dump their internal state
        /// </summary>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        public bool Status() {

            Console.WriteLine("-----------------------");
            Console.WriteLine("This is master server and the last TID given was " + LastTID);
            Console.WriteLine("Primary Servers registered are:");
            for (int i = 0; i < registeredServers.Count; i++) {
                Console.WriteLine("Server " + i + " with address " + registeredServers[i].Address + " and carrying PadInts " + registeredServers[i].DumpPadInts());
            }

            foreach (ServerRegistry srvr in registeredServers) {
                IServer server = (IServer)Activator.GetObject(typeof(IServer), srvr.Address);
                server.Status();
            }
            Console.WriteLine("-----------------------");

            return true;
        }

        public override object InitializeLifetimeService() {
            return null;
        }
    }
}
