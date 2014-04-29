using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Timers;

namespace PadIntServer {
    /// <summary>
    /// This class represents the PadInt backup server.
    /// Note that the loops (pending queue) in PadInt class do not occur when the backup server
    ///  is called because the primary server only calls this server when it
    ///  obtains the read/write locks
    /// </summary>
    class BackupServer : ServerState {

        /// <summary>
        /// Constant used to represent the interval after which
        ///  backup server tries to be a primary server (1000 = 1s)
        /// </summary>
        private const int IM_ALIVE_INTERVAL = 35000;
        /// <summary>
        /// Timer used in I'm Alive mechanism
        /// </summary>
        private System.Timers.Timer imAliveTimer;

        internal BackupServer(Server server)
            : base(server) {
            // Create a timer with inAliveInterval second interval.
            imAliveTimer = new System.Timers.Timer(IM_ALIVE_INTERVAL);
            imAliveTimer.Elapsed += new ElapsedEventHandler(ImAliveEvent);

            //starts im alive timer
            imAliveTimer.Start();
        }

        /// <summary>
        /// Receives I'm alive from primary server
        /// </summary>
        internal override void ImAlive() {
            Logger.log(new String[] { "BackupServer", Server.ID.ToString(), "ImAlive" });
            //re-starts the timer
            imAliveTimer.Stop();
            imAliveTimer.Start();
        }

        private void ImAliveEvent(object source, ElapsedEventArgs e) {
            Logger.log(new String[] { "BackupServer", Server.ID.ToString(), "ImAliveEvent" });
            Server.Master.becomePrimary(Server.ID, Server.ReplicationServerAddr, Server.PdInts);
        }

        private PadInt getPadInt(int uid) {
            if(Server.PdInts.ContainsKey(uid)) {
                return (PadInt) Server.PdInts[uid];
            } else {
                throw new PadIntNotFoundException(uid, Server.ID);
            }
        }

        private void verifyPadInts(List<int> padInts) {
            try {
                foreach(int uid in padInts) {
                    getPadInt(uid);
                }
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        internal override bool createPadInt(int uid) {
            Logger.log(new String[] { "BackupServer", Server.ID.ToString(), "createPadInt", "uid ", uid.ToString() });
            try {
                Server.PdInts.Add(uid, (IPadInt) new PadInt(uid));
                return true;
            } catch(ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, Server.ID);
            }
        }

        internal override bool confirmPadInt(int uid) {
            Logger.log(new String[] { "BackupServer", Server.ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
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
        internal override int readPadInt(int tid, int uid) {
            Logger.log(new String[] { "BackupServer", Server.ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = getPadInt(uid);

                if(padInt.hasWriteLock(tid) || padInt.getReadLock(tid)) {
                    return padInt.ActualValue;
                }
            } catch(PadIntNotFoundException) {
                throw;
            } catch(AbortException) {
                throw;
            }

            return -1;
        }

        internal override bool writePadInt(int tid, int uid, int value) {
            Logger.log(new String[] { "Server ", Server.ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = getPadInt(uid);

                if(padInt.getWriteLock(tid)) {
                    padInt.ActualValue = value;
                    return true;
                }
            } catch(PadIntNotFoundException) {
                throw;
            } catch(AbortException) {
                throw;
            }

            return false;
        }

        /// <summary>
        /// Commits a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool commit(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "BackupServer", Server.ID.ToString(), "commit", "tid", tid.ToString() });

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

            return resultCommit;
        }

        /// <summary>
        /// Aborts a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool abort(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "BackupServer", Server.ID.ToString(), "abort", "tid", tid.ToString() });

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

            return resultAbort;
        }
    }
}
