using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    public interface IClient {
    }

    public interface IServer {
        void createPadInt(int uid);

        bool confirmPadInt(int uid);

        int readPadInt(int tid, int uid);
        bool writePadInt(int tid, int uid, int value);
    }

    public interface IMaster {
        void registerServer(String address);
        int getNextTID();
        int getNServers();
        String getServerAddress(int serverID);
    }
}
