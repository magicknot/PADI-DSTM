using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    public interface IClient {
    }

    public interface IServer {
        bool createPadInt(int uid);
        bool confirmPadInt(int uid);
        int readPadInt(int tid, int uid);
        bool writePadInt(int tid, int uid, int value);
    }

    public interface IMaster {
        bool registerServer(String address);
        int getNextTID();
        Dictionary<int, string> getServersList();
        //String getServerAddress(int serverID);
    }
}
