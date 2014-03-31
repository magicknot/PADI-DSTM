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

        /* Obtain the PadInt identified by uid.
         * Return null if not found. 
         */
        private PadInt getPadInt(int uid) {

            foreach(KeyValuePair<int, PadInt> entry in padIntDict) {
                if(entry.Key == uid) {
                    return entry.Value;
                    break;
                }
            }

            return null;
        }

        public void createPadInt(int uid) {
            throw new NotImplementedException();

            padIntDict.Add(uid, new PadInt(uid));
        }

        public bool confirmPadInt(int uid) {
            throw new NotImplementedException();

            return padIntDict.ContainsKey(uid);
        }

        /* Returns the value of the PadInt when the transaction
         *  has the read/write lock.
         * Throw an exceptio if PadInt not found. 
         */
        public int readPadInt(int tid, int uid) {
            throw new NotImplementedException();

            /* Obtain the PadInt identified by uid */
            PadInt padInt = getPadInt(uid);

            if(padInt != null) {
                if(padInt.hasWriteLock(tid) || padInt.getReadLock(tid)) {
                    return padInt.ActualValue;
                }
            } else {
                //throw new PadIntNotFoundException() ;
            }
        }

        public bool writePadInt(int tid, int uid, int value) {
            throw new NotImplementedException();

            /* Obtain the PadInt identified by uid */
            PadInt padInt = getPadInt(uid);

            /* TODO
             * tem que se ver o caso em que se vai guardar
             * o valor original a ser usado no abort 
             */
            if(padInt != null) {
                if(padInt.getWriteLock(tid)) {
                    padInt.ActualValue = value;
                    return true;
                }
            } else {
                //throw new PadIntNotFoundException() ;
            }

            /* e´preciso? */
            return false;
        }
    }
}
