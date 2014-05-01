using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer {
    public static class LoadBalancer {

        internal static int GetAvailableServer(List<string> registeredServers) {
            if(registeredServers.Count != 0) {
                Random random = new Random();
                return random.Next(0, registeredServers.Count);
            } else {
                throw new NoServersFoundException();
            }
        }
    }
}
