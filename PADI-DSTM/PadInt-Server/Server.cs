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
         * Returns null if not found. 
         */
        private PadInt getPadInt(int uid) {

            foreach(KeyValuePair<int, PadInt> entry in padIntDict) {
                if(entry.Key == uid) {
                    return entry.Value;
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

        public void commit(int tid) {
            throw new NotImplementedException();
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */

            /* ainda nao esta acabado */

            foreach(KeyValuePair<int, PadInt> entry in padIntDict) {
                //freeReadLock(int tid)
                //freeWriteLock(int tid)

                /* apenas precisa de fazer isto apenas quando sao escritas */
                entry.Value.OriginalValue = entry.Value.ActualValue;
            }

            /* retorna algum tipo de msg a indicar que fez o commit? */
        }

        public void abort(int tid) {
            throw new NotImplementedException();
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */

            /* ainda nao esta acabado */
            foreach(KeyValuePair<int, PadInt> entry in padIntDict) {
                //freeReadLock(int tid)
                //freeWriteLock(int tid)

                /* apenas e so´ no caso em que era lock de write e´
                 * que faz isto
                 */
                entry.Value.ActualValue = entry.Value.OriginalValue;
            }

            /* retorna algum tipo de msg a indicar que fez o commit? */
        }
    }
}
