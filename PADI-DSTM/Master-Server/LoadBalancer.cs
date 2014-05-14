using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer {
    class LoadBalancer {


        internal static int GetAvailableServer(List<ServerRegistry> registeredServers, bool serverIsPrimary) {
            List<ServerRegistry> servers = new List<ServerRegistry>(registeredServers);

            if(!serverIsPrimary) {
                servers.RemoveAt(registeredServers.Count - 1);
            }

            if(registeredServers.Count == 0) {
                throw new NoServersFoundException();
            }

            servers.Sort(new ServerComparer());
            return servers.First<ServerRegistry>().ID;
        }

        internal static void DistributePadInts(List<ServerRegistry> registeredServers) {
            List<ServerRegistry> servers = new List<ServerRegistry>(registeredServers);
            ServerRegistry receiverServer = servers[registeredServers.Count - 1];
            servers.Sort(new ServerReverseComparer());

            int nPadInts = 0;
            int nServers = 0;

            foreach(ServerRegistry srvr in registeredServers) {
                nPadInts += srvr.GetNPadInts();
                nServers++;
            }

            int averageCapacity = nPadInts / nServers;

            List<int> movingPadInts = new List<int>();

            int i = 0;
            int nMovedPadInts = 0;

            while(i < servers.Count) {
                if(nMovedPadInts == averageCapacity) {
                    return;
                } else if(servers[i].GetNPadInts() > averageCapacity) {
                    movingPadInts.Add(servers[i].RemovePadInt());
                } else if(movingPadInts.Count == averageCapacity) {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), servers[i].Address);
                    server.MovePadInts(movingPadInts, receiverServer.Address);
                    nMovedPadInts = movingPadInts.Count;
                    movingPadInts.Clear();
                    i++;
                }
            }
        }
    }
}
