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
        bool commit(int tid, List<int> usedPadInts);
        bool abort(int tid, List<int> usedPadInts);
    }

    public interface IMaster {

        int getNextTID();
        Tuple<int, int> registerServer(String address);
        Tuple<Dictionary<int, string>, int> getServersInfo(bool increase);
    }

    public interface ILog {
        void log(String[] logs);
    }

    public abstract class IPadiException : System.Exception {
        public abstract string getMessage();
    }

    public static class Logger {
        static ILog logServer = (ILog) Activator.GetObject(typeof(ILog), "tcp://localhost:7002/LogServer");

        public static void log(String[] args) {
            logServer.log(args);
        }

    }

}
