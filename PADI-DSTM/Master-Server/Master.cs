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
        /// Registers a new server on Master server.
        /// </summary>
        /// <param name="address">Server Address</param>
        /// <returns>Server identifier</returns>
        public int registerServer(String address) {
            Logger.log(new String[] { "Master", " registerServer", "address", address.ToString() });
            try {
                registeredServers.Insert(NServers, address);
                return NServers++;
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
            } catch(NoServersFoundException) {
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
        /*FIXME exception used is  the wrong one */
        private void verifyServerRegistry(int serverID) {
            if(registeredServers.ElementAtOrDefault(serverID)==null) {
                throw new NoServersFoundException();
            }
        }
    }
}
