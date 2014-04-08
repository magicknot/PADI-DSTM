using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;


namespace PadIntServer {

    class Server : MarshalByRefObject, IServer {

        /* Pending request list */
        //private List<Request> requestList = new List<Request>();

        /* Structure that maps UID to PadInt */
        private Dictionary<int, IPadInt> padIntDictionary;
        private int identifier;
        IMaster masterServerReference;


        public Server() {
            PdInts = new Dictionary<int, IPadInt>();
        }

        internal int ID {
            set { this.identifier = value; }
            get { return identifier; }
        }

        internal IMaster Master {
            set { this.masterServerReference = value; }
            get { return masterServerReference; }
        }

        internal Dictionary<int, IPadInt> PdInts {
            set { this.padIntDictionary = value; }
            get { return this.padIntDictionary; }
        }

        public bool init(int port) {
            try {
                Master = (IMaster) Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
                ID = Master.registerServer("tcp://localhost:" + (8000 + port) + "/PadIntServer");
            } catch(ServerAlreadyExistsException) {
                throw;
            }
            return true;
        }



        public bool createPadInt(int uid) {
            Logger.log(new String[] { "Server", ID.ToString(), "createPadInt", "uid ", uid.ToString() });
            try {
                PdInts.Add(uid, new PadInt(uid));
                return true;
            } catch(ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, ID);
            }
        }

        public bool confirmPadInt(int uid) {
            Logger.log(new String[] { "Server", ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
            try {
                getPadInt(uid);
                return true;
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        /* Returns the value of the PadInt when the transaction
         *  has the read/write lock.
         * Throw an exception if PadInt not found. 
         */
        public int readPadInt(int tid, int uid) {
            Logger.log(new String[] { "Server", ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = (PadInt) getPadInt(uid);

                while(true) {
                    if(padInt.hasWriteLock(tid) || padInt.getReadLock(tid)) {
                        return padInt.ActualValue;
                    }
                }

            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        public bool writePadInt(int tid, int uid, int value) {
            Logger.log(new String[] { "Server ", ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = (PadInt) getPadInt(uid);

                while(true) {
                    if(padInt.getWriteLock(tid)) {
                        padInt.ActualValue = value;
                        return true;
                    }
                }

            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        /* usedPadInts sao os uid usados pela transacao tid */
        public bool commit(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "Server", ID.ToString(), "commit", "tid", tid.ToString() });
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */
            bool resultCommit = true;

            try {
                verifyPadInts(usedPadInts);

                foreach(int padIntUid in usedPadInts) {
                    PadInt padInt = (PadInt) getPadInt(padIntUid);
                    resultCommit = padInt.commit(tid) && resultCommit;
                }

            } catch(PadIntNotFoundException) {
                throw;
            }
            return resultCommit;
        }

        /* usedPadInts sao os uid usados pela transacao tid */
        public bool abort(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "Server", ID.ToString(), "abort", "tid", tid.ToString() });
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */
            bool resultAbort = true;

            try {
                verifyPadInts(usedPadInts);

                foreach(int padIntUid in usedPadInts) {
                    PadInt padInt = (PadInt) getPadInt(padIntUid);
                    resultAbort = padInt.abort(tid) && resultAbort;
                }
            } catch(PadIntNotFoundException) {
                throw;
            }
            return resultAbort;
        }


        public void verifyPadInts(List<int> padInts) {
            try {
                foreach(int uid in padInts) {
                    getPadInt(uid);
                }
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        private IPadInt getPadInt(int uid) {
            if(PdInts.ContainsKey(uid)) {
                return PdInts[uid];
            } else {
                throw new PadIntNotFoundException(uid, ID);
            }
        }
    }
}
