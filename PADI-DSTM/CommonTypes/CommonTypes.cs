﻿using System;
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

    public interface IPadiException {
        int getUid();
    }

}
