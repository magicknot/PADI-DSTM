using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Timers;

namespace PadIntServer {
    /// <summary>
    /// This class represents the PadInt primary server
    /// </summary>
    class PrimaryServer : ServerState {

        /// <summary>
        /// Constant used to represent the interval after which primary server
        ///  sends I'm alive
        /// </summary>
        private const int IM_ALIVE_INTERVAL = 15;
        /// <summary>
        /// Timer used in I'm Alive mechanism
        /// </summary>
        private System.Timers.Timer imAliveTimer;
        /// <summary>
        /// Constant used to represent the interval after which primary server
        ///  requires a new backup server
        /// </summary>
        private const int BACKUP_REPLY_INTERVAL = 25;
        /// <summary>
        /// Timer used to detect that primary server does not receive a reply
        ///  from backup server
        /// </summary>
        private System.Timers.Timer BackupReplyTimer;

        internal PrimaryServer(Server server)
            : base(server) {
            // Create a timer with IM_ALIVE_INTERVAL second interval.
            imAliveTimer = new System.Timers.Timer(IM_ALIVE_INTERVAL);
            imAliveTimer.Elapsed += new ElapsedEventHandler(ImAliveEvent);

            // Create a timer with BACKUP_REPLY_INTERVAL second interval.
            imAliveTimer = new System.Timers.Timer(BACKUP_REPLY_INTERVAL);
            imAliveTimer.Elapsed += new ElapsedEventHandler(BackupReplyEvent);
        }

        /// <summary>
        /// Sends I'm alive to backup server
        /// </summary>
        internal override void ImAlive() {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "ImAlive" });

            imAliveTimer.Stop();
            Server.ReplicationServer.ImAlive();
            //re-starts the timer
            imAliveTimer.Start();
        }

        /// <summary>
        /// Deals with I'm alive timer events
        /// </summary>
        private void ImAliveEvent(object source, ElapsedEventArgs e) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "ImAliveEvent" });

            ImAlive();
        }

        /// <summary>
        /// Deals with backup reply timer events
        /// </summary>
        private void BackupReplyEvent(object source, ElapsedEventArgs e) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "BackupReplyEvent" });

            Server.Master.createNewReplica(Server.ID, Server.ReplicationServerAddr);
        }

        /// <summary>
        /// Returns the PadInt identified by uid
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns></returns>
        private PadInt getPadInt(int uid) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "getPadInt", "uid ", uid.ToString() });
            if(Server.PdInts.ContainsKey(uid)) {
                return Server.PdInts[uid];
            } else {
                throw new PadIntNotFoundException(uid, Server.ID);
            }
        }

        /// <summary>
        /// Verifies if each PadInt in padInts exist
        /// </summary>
        /// <param name="padInts">List with PadInts' identifiers</param>
        private void verifyPadInts(List<int> padInts) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "verifyPadInts" });
            try {
                foreach(int uid in padInts) {
                    getPadInt(uid);
                }
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        /// <summary>
        /// Create a PadInt identified by uid
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns></returns>
        internal override bool createPadInt(int uid) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "createPadInt", "uid ", uid.ToString() });
            try {
                Server.PdInts.Add(uid, new PadInt(uid));
                /* updates the backup server */
                Server.ReplicationServer.createPadInt(uid);
                return true;
            } catch(ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, Server.ID);
            }
        }

        internal override bool confirmPadInt(int uid) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
            try {
                getPadInt(uid);
                return true;
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        /// <summary>
        /// Returns the value of the PadInt when the transaction has the read/write lock.
        ///  Throws an exception if PadInt not found. 
        /// </summary>
        /// <param name="tid">Transaction identifier</param>
        /// <param name="uid">PadInt identifier</param>
        /// <returns></returns>
        internal override int readPadInt(int tid, int uid) {
            Logger.log(new String[] { "PrimaryServer", Server.ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = getPadInt(uid);

                while(true) {
                    if(padInt.hasWriteLock(tid) || padInt.getReadLock(tid)) {
                        /* updates the backup server */
                        Server.ReplicationServer.readPadInt(tid, uid);
                        return padInt.ActualValue;
                    }
                }
            } catch(PadIntNotFoundException) {
                throw;
            }
        }

        /// <summary>
        /// Writes a value in a PadInt
        /// </summary>
        /// <param name="tid">Transaction identifier</param>
        /// <param name="uid">PadInt identifier</param>
        /// <param name="value">Value of the write</param>
        /// <returns>True if successful</returns>
        internal override bool writePadInt(int tid, int uid, int value) {
            Logger.log(new String[] { "Server ", Server.ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = getPadInt(uid);

                while(true) {
                    if(padInt.getWriteLock(tid)) {
                        padInt.ActualValue = value;
                        /* updates the backup server */
                        Server.ReplicationServer.writePadInt(tid, uid, value);
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

            /* updates the backup server */
            Server.ReplicationServer.commit(tid, usedPadInts);

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

            /* updates the backup server */
            Server.ReplicationServer.abort(tid, usedPadInts);

            return resultAbort;
        }
    }
}
