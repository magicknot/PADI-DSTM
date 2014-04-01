using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;


namespace PadIntServer {

    class Server : MarshalByRefObject, IServer {

        /* Pending request list */
        //private List<Request> requestList = new List<Request>();

        /* Structure that maps UID to PadInt */
        private Dictionary<int, PadInt> padIntDict = new Dictionary<int, PadInt>();

        /* Obtain the PadInt identified by uid.
         * Returns null if not found. 
         */
        private PadInt getPadInt(int uid) {
            Console.WriteLine(DateTime.Now + " Server " + " operation " + " getPadInt " + " uid " + uid);


            foreach(KeyValuePair<int, PadInt> entry in padIntDict) {
                if(entry.Key == uid) {
                    return entry.Value;
                }
            }

            return null;
        }

        public void createPadInt(int uid) {
            Console.WriteLine(DateTime.Now + " Server " + " operation " + " createPadInt " + " uid " + uid);


            padIntDict.Add(uid, new PadInt(uid));
        }

        public bool confirmPadInt(int uid) {
            Console.WriteLine(DateTime.Now + " Server " + " operation " + " confirmPadInt " + " uid " + uid);
            return padIntDict.ContainsKey(uid);
        }

        /* Returns the value of the PadInt when the transaction
         *  has the read/write lock.
         * Throw an exceptio if PadInt not found. 
         */
        public int readPadInt(int tid, int uid) {
            throw new NotImplementedException();
            Console.WriteLine(DateTime.Now + " Server " + " operation " + " readPadInt " + " tid " + tid  + " uid " + uid );


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
            Console.WriteLine(DateTime.Now + " Server " + " operation " + " writePadInt " + " tid " + tid + " uid " + uid + " value " + value);


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

        /* usedPadInts sao os uid usados pela transacao tid */
        public void commit(int tid, List<int> usedPadInts) {
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

            foreach(int padInt in usedPadInts) {

                /* PadInt padInt = getPadInt(uid);
                 * 
                 * chama metodo commit do PadInt que faz:
                 * 
                 * liberta locks de read: freeReadLock(int tid)
                 * liberta locks de write: freeWriteLock(int tid)
                 * 
                 * verifica se o tid da transaccao nao esta na promotion
                 *  - se estiver e for commit manda abort????
                 *  - se estiver e for abort limpa apenas
                 * 
                 * apenas precisa de fazer isto apenas quando sao escritas:
                 * entry.Value.OriginalValue = entry.Value.ActualValue;
                 
                 */

            }

            /* retorna algum tipo de msg a indicar que fez o commit? */
        }

        /* usedPadInts sao os uid usados pela transacao tid */
        public void abort(int tid, List<int> usedPadInts) {
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
            foreach(int padInt in usedPadInts) {

                /* PadInt padInt = getPadInt(uid);
                 * 
                 * chama metodo abort do PadInt que faz:
                 * 
                 * liberta locks de read: freeReadLock(int tid)
                 * liberta locks de write: freeWriteLock(int tid)
                 * 
                 * verifica se o tid da transaccao nao esta na promotion
                 *  - se estiver e for commit manda abort????
                 *  - se estiver e for abort limpa apenas
                 * 
                 * apenas e so´ no caso em que era lock de write e´ que faz isto:
                 * entry.Value.ActualValue = entry.Value.OriginalValue;
                 
                 */
            }

            /* retorna algum tipo de msg a indicar que fez o abort? */
        }
    }
}
