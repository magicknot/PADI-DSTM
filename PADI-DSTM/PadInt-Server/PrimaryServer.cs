using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace PadIntServer {
    /// <summary>
    /// This class represents the PadInt primary server
    /// </summary>
    class PrimaryServer : ServerState {

        internal PrimaryServer(Server server)
            : base(server) {
            // Nothing to do here
        }

        private PadInt getPadInt(int uid) {
            if(Server.PdInts.ContainsKey(uid)) {
                return Server.PdInts[uid];
            } else {
                throw new PadIntNotFoundException(uid, Server.ID);
            }
        }

        internal void verifyPadInts(List<int> padInts) {
            try {
                foreach(int uid in padInts) {
                    getPadInt(uid);
                }
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        internal override bool createPadInt(int uid) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "createPadInt", "uid ", uid.ToString() });
            try {
                Server.PdInts.Add(uid, new PadInt(uid));
                //TODO actualiza o backup
                return true;
            } catch(ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, Server.ID);
            }
        }

        internal override bool confirmPadInt(int uid) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
            try {
                getPadInt(uid);
                //TODO actualiza o backup
                return true;
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        /* Returns the value of the PadInt when the transaction
         *  has the read/write lock.
         * Throw an exception if PadInt not found. 
         */
        internal override int readPadInt(int tid, int uid) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = getPadInt(uid);

                while(true) {
                    if(padInt.hasWriteLock(tid) || padInt.getReadLock(tid)) {
                        //TODO actualiza o backup
                        return padInt.ActualValue;
                    }
                }
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        internal override bool writePadInt(int tid, int uid, int value) {
            Logger.log(new String[] { "Server ", Server.ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = getPadInt(uid);

                while(true) {
                    if(padInt.getWriteLock(tid)) {
                        padInt.ActualValue = value;
                        //TODO actualiza o backup
                        return true;
                    }
                }
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        /// <summary>
        /// Commits a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool commit(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "commit", "tid", tid.ToString() });

            bool resultCommit = true;

            try {
                verifyPadInts(usedPadInts);

                foreach(int padIntUid in usedPadInts) {
                    PadInt padInt = getPadInt(padIntUid);
                    resultCommit = padInt.commit(tid) && resultCommit;
                }

            } catch(PadIntNotFoundException) {
                throw;
            }

            //TODO actualiza o backup

            return resultCommit;
        }

        /// <summary>
        /// Aborts a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool abort(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "abort", "tid", tid.ToString() });

            bool resultAbort = true;

            try {
                verifyPadInts(usedPadInts);

                foreach(int padIntUid in usedPadInts) {
                    PadInt padInt = getPadInt(padIntUid);
                    resultAbort = padInt.abort(tid) && resultAbort;
                }
            } catch(PadIntNotFoundException) {
                throw;
            }

            //TODO actualiza o backup

            return resultAbort;
        }
    }
}
