using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    public interface IClient {
    }

    public interface IServer {
        void allocatePadInt (int uid);
    }

    public interface IMaster {
        void registerServer (String address);
        int getNextTID ();
        int getNServers ();
        String getServerAddress (int serverID);
    }
}
