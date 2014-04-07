using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_Server {
    public static class LoadBalancer {

        internal static int getAvailableServer(int nServers) {
            Random random = new Random();
            return random.Next(0, nServers);
        }
    }
}
