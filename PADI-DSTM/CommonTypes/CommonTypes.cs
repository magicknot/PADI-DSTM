using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    public interface IClient {
    }

    public interface IPadInt {
    }

    public interface IServer {
        bool createPadInt(int uid);
        bool confirmPadInt(int uid);
        int readPadInt(int tid, int uid);
        bool writePadInt(int tid, int uid, int value);
        void attachPadInts(Dictionary<int, String> serverAddresses, Dictionary<int, IPadInt> sparedPadInts);
    }

    public interface IMaster {
        int registerServer(String address);
        int getNextTID();
        Tuple<Dictionary<int, string>,int> getServersList();
        //String getServerAddress(int serverID);

        Dictionary<int, string> updateMaxCapacity();
    }

}
