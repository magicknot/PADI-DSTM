using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;


namespace PadInt_Server {

    class Server : MarshalByRefObject, IServer {

        /* Pending request list */
        //private List<Request> requestList = new List<Request>();

        /* Structure that maps UID to PadInt */
        private Dictionary<int, PadInt> padIntDict = new Dictionary<int, PadInt>();
        //arranjar nome melhor???

        public void createPadInt(int uid) {
            throw new NotImplementedException();
        }

        public Boolean confirmPadInt(int uid) {
            throw new NotImplementedException();
        }

        public bool writePadInt(int tid, int uid, int value) {
            throw new NotImplementedException();
        }

        public int readPadInt(int tid, int uid) {
            throw new NotImplementedException();
        }

        public bool confirmPadInt() {
            throw new NotImplementedException();
        }
    }
}
