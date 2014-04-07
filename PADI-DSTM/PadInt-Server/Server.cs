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
        Dictionary<int, String> serverAddresses;
        private int identifier;
        private int maxServerCapacity;
        IMaster masterServerReference;
        private bool isTailServer;


        public Server() {
            PdInts = new Dictionary<int, IPadInt>();
            IsTail = true;
        }

        internal int ID {
            set { this.identifier = value; }
            get { return identifier; }
        }

        internal int MaxCpt {
            set { this.maxServerCapacity = value; }
            get { return this.maxServerCapacity; }
        }

        internal IMaster Master {
            set { this.masterServerReference = value; }
            get { return masterServerReference; }
        }

        internal bool IsTail {
            set { this.isTailServer = value; }
            get { return this.isTailServer; }
        }

        internal Dictionary<int, IPadInt> PdInts {
            set { this.padIntDictionary = value; }
            get { return this.padIntDictionary; }
        }

        internal Dictionary<int, String> ServerAddrs {
            set { this.serverAddresses = value; }
            get { return this.serverAddresses; }
        }


        public bool init(int port) {
            try {
                Master = (IMaster) Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
                Tuple<int, int> serverInfo = Master.registerServer("tcp://localhost:" + (8000 + port) + "/PadIntServer");
                Dictionary<int, String> serverAddresses = Master.getServersInfo(true).Item1;
                ID = serverInfo.Item1;
                MaxCpt = serverInfo.Item2;

                if(ID != 0) {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), ServerAddrs[ID - 1]);
                    Dictionary<int, IPadInt> importedPds = server.removeFromTail();
                    foreach(KeyValuePair<int, IPadInt> entry in importedPds) {
                        PdInts.Add(entry.Key, entry.Value);
                    }
                }

            } catch(IPadiException e) {
                Console.WriteLine(e.getMessage());
                return false;
            }
            return true;
        }

        public Dictionary<int, IPadInt> removeFromTail() {
            IsTail = false;

            Dictionary<int, IPadInt> sparePadInts = new Dictionary<int, IPadInt>();

            foreach(KeyValuePair<int, IPadInt> entry in PdInts) {
                if(entry.Key > MaxCpt * ID + MaxCpt) {
                    sparePadInts.Add(entry.Key, entry.Value);
                }
            }
            return sparePadInts;
        }

        public Boolean isValidRequest(int uid) {
            if(!isTailServer && (uid < MaxCpt * ID || uid > MaxCpt * ID + MaxCpt)) {
                throw new WrongServerRequestException(uid, ID);
            } else {
                return true;
            }
        }


        /* Obtain the PadInt identified by uid.
         * Returns null if not found. 
         */
        private IPadInt getPadInt(int uid) {
            Logger.log(new String[] { "server", "getPadInt", "uid", uid.ToString() });

            try {
                isValidRequest(uid);
            } catch(WrongServerRequestException) {
                throw;
            }

            foreach(KeyValuePair<int, IPadInt> entry in PdInts) {
                if(entry.Key == uid) {
                    return entry.Value;
                }
            }

            throw new PadIntNotFoundException(uid, ID);
        }

        public bool createPadInt(int uid) {
            Logger.log(new String[] { "server", ID.ToString(), "createPadInt", "uid ", uid.ToString() });

            try {
                isValidRequest(uid);

                PdInts.Add(uid, new PadInt(uid));

                if(PdInts.Count > 2 * MaxCpt) {
                    movePadInts(serverAddresses);
                }

                return true;

            } catch(WrongServerRequestException) {
                throw;
            } catch(ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, ID);
            }
        }

        public void movePadInts(Dictionary<int, String> serverAddresses) {
            Logger.log(new String[] { "server", ID.ToString(), "movePadInts" });

            //fica a faltar fazer para os highers, usando a lista de servidores para
            //saber se é o ultimo ou nao e dai decidir se chuta pra frente ou nao
            Dictionary<int, IPadInt> lowerPadInts = new Dictionary<int, IPadInt>();
            int originalCapacity = MaxCpt;

            if(ID == 0)
                return;

            while(lowerPadInts.Count < originalCapacity / 2) {
                MaxCpt = 2 * MaxCpt;

                foreach(int key in PdInts.Keys) {
                    if(key < MaxCpt * ID) {
                        lowerPadInts.Add(key, PdInts[key]);
                    }
                }

                foreach(int key in lowerPadInts.Keys) {
                    PdInts.Remove(key);
                }
            }

            string leftServerAddress = serverAddresses[ID - 1];
            Logger.log(new String[] { "left server Address", "new capacity", MaxCpt.ToString() });

            IServer server = (IServer) Activator.GetObject(typeof(IServer), leftServerAddress);
            server.attachPadInts(serverAddresses, lowerPadInts);
        }

        public void attachPadInts(Dictionary<int, String> serverAddresses, Dictionary<int, IPadInt> sparedPadInts) {
            Logger.log(new String[] { "Server", ID.ToString(), "attachPadInts", sparedPadInts.Count.ToString() });

            foreach(int key in sparedPadInts.Keys) {
                Logger.log(new String[] { "attached padint", key.ToString() });
                PdInts.Add(key, sparedPadInts[key]);
            }

            if(ID == 0)
                return;

            movePadInts(serverAddresses);
        }

        public bool confirmPadInt(int uid) {
            Logger.log(new String[] { "Server", ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
            return PdInts.ContainsKey(uid);
        }

        /* Returns the value of the PadInt when the transaction
         *  has the read/write lock.
         * Throw an exception if PadInt not found. 
         */
        public int readPadInt(int tid, int uid) {
            Logger.log(new String[] { "Server", ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            try {
                isValidRequest(uid);
                /* Obtain the PadInt identified by uid */
                PadInt padInt = (PadInt) getPadInt(uid);

                while(true) {

                    if(padInt.hasWriteLock(tid) || padInt.getReadLock(tid)) {
                        return padInt.ActualValue;
                    }

                }

            } catch(WrongServerRequestException) {
                throw;
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        public bool writePadInt(int tid, int uid, int value) {
            Logger.log(new String[] { " Server ", ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            try {
                isValidRequest(uid);
                /* Obtain the PadInt identified by uid */
                PadInt padInt = (PadInt) getPadInt(uid);

                while(true) {
                    if(padInt.getWriteLock(tid)) {
                        padInt.ActualValue = value;
                        return true;
                    }

                }

            } catch(WrongServerRequestException) {
                throw;
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
                foreach(int uid in usedPadInts) {
                    PadInt padInt = (PadInt) getPadInt(uid);
                    isValidRequest(uid);
                }
            } catch(PadIntNotFoundException) {
                throw;
            } catch(WrongServerRequestException) {
                throw;
            }

            foreach(int padIntUid in usedPadInts) {
                PadInt padInt = (PadInt) getPadInt(padIntUid);
                resultCommit = padInt.commit(tid) && resultCommit;
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
                foreach(int uid in usedPadInts) {
                    PadInt padInt = (PadInt) getPadInt(uid);
                    isValidRequest(uid);
                }
            } catch(PadIntNotFoundException) {
                throw;
            } catch(WrongServerRequestException) {
                throw;
            }

            foreach(int padIntUid in usedPadInts) {
                PadInt padInt = (PadInt) getPadInt(padIntUid);
                resultAbort = padInt.commit(tid) && resultAbort;
            }

            return resultAbort;
        }
    }
}
