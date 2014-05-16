using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer {
    class LoadBalancer {


        internal static int GetAvailableServer(List<ServerRegistry> registeredServers, bool serverIsPrimary) {
            Logger.Log(new String[] { "LoadBalancer", "GetAvailableServer" });

            List<ServerRegistry> servers = new List<ServerRegistry>(registeredServers);

            if (!serverIsPrimary) {
                servers.RemoveAt(registeredServers.Count - 1);
            }

            if (servers.Count == 0) {
                throw new NoServersFoundException();
            }

            servers.Sort(new ServerComparer());
            return servers.First<ServerRegistry>().ID;
        }

        internal static void DistributePadInts(List<ServerRegistry> registeredServers, string receiverServer) {
            Logger.Log(new String[] { "LoadBalancer", "DistributePadInts" });

            List<ServerRegistry> servers = new List<ServerRegistry>(registeredServers);
            servers.Sort(new ServerReverseComparer());

            int nPadInts = 0;
            int nServers = 0;

            foreach (ServerRegistry srvr in registeredServers) {
                nPadInts += srvr.Hits;
                nServers++;
            }

            int averageCapacity = nPadInts / nServers;

            List<int> movingPadInts = new List<int>();

            int i = 0;

            while (i < servers.Count) {

                if (servers[i].Hits < averageCapacity || receiverServer.Equals(servers[i].Address)) {
                    continue;
                }
                else if (servers[i].Hits == averageCapacity) {
                    if (movingPadInts.Count > 0) {
                        IServer server = (IServer)Activator.GetObject(typeof(IServer), servers[i].Address);
                        server.MovePadInts(movingPadInts, receiverServer);

                        foreach (int pd in movingPadInts) {
                            servers[servers.Count - 1].AddPadInt(pd);
                        }
                        movingPadInts.Clear();
                    }
                    i++;
                }
                else if (servers[i].Hits > averageCapacity) {
                    movingPadInts.Add(servers[i].RemovePadInt());
                }
            }
        }
    }
}
