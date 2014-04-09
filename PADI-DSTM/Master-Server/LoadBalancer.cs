using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer {
    public static class LoadBalancer {

		/** A map between the PadInt UID and the server where it's stored. */ 
		private Dictionary<int, ServerRegistry> location;

		/** List of servers without any PadInt */
		private List<ServerRegistry> availableServers;

		/**
		 * Instantiates a new Load Balancer. It can receive a list of servers or be initialized without 
		 * any server.
		 */
		public LoadBalancer(List<ServerRegistry> servers = new List<ServerRegistry>()) {
			this.location = new Dictionary<int, ServerRegistry>();
			this.availableServers = servers;
		}

		/**
		 * Returns the server where the PadInt is stored and the hit value is updated. If the PadInt doesn't exist, then it is thrown an
		 * exception.
		 */
		public ServerRegistry Server(int padint) {
			ServerRegistry server = location[padint];

			if(server == null) {
				throw new PadIntNotFoundException(padint, null); //FIXME: Remove null from here! See issue #9
			}

			server.Hits(server.Hits()++); //FIXME: The increment should be done in ServerRegistry class.
		}

		/**
		 * Deploys the first server in the available server list. If the server isn't sucessfully
		 * removed from the list an exception is thrown and the deployment aborted.
		 */
		private void deployServer() {
			if (availableServers.Count < 1) {
				// TODO: Ask the Master to create a new server (maybe it should just send an exception)
				return;
			}

			ServerRegistry server = availableServers.First;

			if (availableServers.Remove(server)) {
				hits.Add(availableServers.Remove);
			} else {
				throw new ServerCouldNotBeDeployed(server.Address);
			}
		}

        internal static int getAvailableServer(List<MasterServer.ServerRegistry> registeredServers) {
            if(registeredServers.Count!=0) {
                Random random = new Random();
                return random.Next(0, registeredServers.Count);
            } else {
                throw new NoServersFoundException();
            }
        }
    }
}
