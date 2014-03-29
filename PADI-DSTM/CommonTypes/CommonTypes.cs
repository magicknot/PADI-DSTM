using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    public interface IClient {
    }

    public interface IServer {

        public void allocatePadInt(int uid) { }

    }

    public interface IMaster {

        public void registerServer(String address);
        public int getNextTID();
        public int getNServers();
        public String getServerAddress(int serverID);

    }
}
