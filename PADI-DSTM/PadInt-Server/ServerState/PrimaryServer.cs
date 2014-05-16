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
    class PrimaryServer : ServerState, IDisposable {

        /// <summary>
        /// Constant used to represent the interval after which primary server
        ///  sends I'm alive (1000 = 1s)
        /// </summary>
        private const int IM_ALIVE_INTERVAL = 10000;
        /// <summary>
        /// Constant used to represent the interval after which primary server
        ///  requires a new backup server (1000 = 1s)
        /// </summary>
        private const int BACKUP_REPLY_INTERVAL = 15000;

        /// <summary>
        /// Timer used to detect that primary server does not receive a reply
        ///  from backup server
        /// </summary>
        internal PadIntTimer backupReplyTimer;

        internal IServer BackupServer {
            set { this.pairServerReference = value; }
            get { return pairServerReference; }
        }

        internal string BackupAddress {
            set { this.pairServerAddress = value; }
            get { return pairServerAddress; }
        }



        internal PrimaryServer(Server server, String backupAddress, Dictionary<int, IPadInt> pdInts)
            : base(server, pdInts) {

            BackupAddress = backupAddress;
            BackupServer = (IServer) Activator.GetObject(typeof(IServer), backupAddress);

            // Create a timer with IM_ALIVE_INTERVAL second interval.
            imAliveTimer = new PadIntTimer(IM_ALIVE_INTERVAL);
            imAliveTimer.Timer.Elapsed += new ElapsedEventHandler(ImAliveEvent);

            // Create a timer with BACKUP_REPLY_INTERVAL second interval.
            backupReplyTimer = new PadIntTimer(BACKUP_REPLY_INTERVAL);
            backupReplyTimer.Timer.Elapsed += new ElapsedEventHandler(BackupReplyEvent);

            //starts im alive timer
            imAliveTimer.Start();
        }



        /// <summary>
        /// Sends I'm alive to backup server
        /// </summary>
        internal override void ImAlive() {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "ImAlive" });

            imAliveTimer.Stop();
            BackupServer.ImAlive();
            //re-starts the timer
            imAliveTimer.Start();
        }

        /// <summary>
        /// Deals with I'm alive timer events
        /// </summary>
        private void ImAliveEvent(object source, ElapsedEventArgs e) {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "ImAliveEvent" });
            ImAlive();
        }

        /// <summary>
        /// Deals with backup reply timer events
        /// </summary>
        private void BackupReplyEvent(object source, ElapsedEventArgs e) {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "BackupReplyEvent" });
            backupReplyTimer.Stop();
            IServerMachine backupServerMachine = (IServerMachine) Activator.GetObject(typeof(IServer), BackupAddress);
            backupServerMachine.restartServer(Server.ID);
            BackupServer.CreateBackupServer(Server.Address, padIntDictionary);
        }

        /// <summary>
        /// Returns the PadInt identified by uid
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns></returns>
        protected override PadInt GetPadInt(int uid) {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "getPadInt", "uid ", uid.ToString() });
            return base.GetPadInt(uid);
        }

        /// <summary>
        /// Verifies if each PadInt in padInts exist
        /// </summary>
        /// <param name="padInts">List with PadInts' identifiers</param>
        protected override void VerifyPadInts(List<int> padInts) {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "verifyPadInts" });
            base.VerifyPadInts(padInts);
        }

        /// <summary>
        /// Create a PadInt identified by uid
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <returns></returns>
        internal override bool CreatePadInt(int uid) {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "createPadInt", "uid ", uid.ToString(),
                "    NPadInts", (padIntDictionary.Count + 1).ToString() });
            try {
                padIntDictionary.Add(uid, new PadInt(uid));
                /* updates the backup server */
                backupReplyTimer.Start();
                BackupServer.CreatePadInt(uid);
                backupReplyTimer.Stop();
                return true;
            } catch(ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, Server.ID);
            }
        }

        internal override bool ConfirmPadInt(int uid) {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
            return base.ConfirmPadInt(uid);
        }

        /// <summary>
        /// Returns the value of the PadInt when the transaction has the read/write lock.
        ///  Throws an exception if PadInt not found. 
        /// </summary>
        /// <param name="tid">Transaction identifier</param>
        /// <param name="uid">PadInt identifier</param>
        /// <returns></returns>
        internal override int ReadPadInt(int tid, int uid) {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = GetPadInt(uid);

                while(true) {
                    if(padInt.HasWriteLock(tid) || padInt.GetReadLock(tid)) {
                        /* updates the backup server */
                        backupReplyTimer.Start();
                        BackupServer.ReadPadInt(tid, uid);
                        backupReplyTimer.Stop();
                        return padInt.ActualValue;
                    }
                }
            } catch(PadIntNotFoundException) {
                throw;
            } catch(AbortException) {
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
        internal override bool WritePadInt(int tid, int uid, int value) {
            Logger.Log(new String[] { "Server ", Server.ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = GetPadInt(uid);

                while(true) {
                    if(padInt.GetWriteLock(tid)) {
                        padInt.ActualValue = value;
                        /* updates the backup server */
                        backupReplyTimer.Start();
                        BackupServer.WritePadInt(tid, uid, value);
                        backupReplyTimer.Stop();
                        return true;
                    }
                }
            } catch(PadIntNotFoundException) {
                throw;
            } catch(AbortException) {
                throw;
            }
        }

        /// <summary>
        /// Commits a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool Commit(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "commit", "tid", tid.ToString() });

            bool resultCommit = true;

            try {
                VerifyPadInts(usedPadInts);

                foreach(int padIntUid in usedPadInts) {
                    PadInt padInt = GetPadInt(padIntUid);
                    resultCommit = padInt.Commit(tid) && resultCommit;
                }
            } catch(PadIntNotFoundException) {
                throw;
            }

            /* updates the backup server */
            backupReplyTimer.Start();
            BackupServer.Commit(tid, usedPadInts);
            backupReplyTimer.Stop();

            return resultCommit;
        }

        /// <summary>
        /// Aborts a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool Abort(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "PrimaryServer", Server.ID.ToString(), "abort", "tid", tid.ToString() });

            bool resultAbort = true;

            try {
                VerifyPadInts(usedPadInts);

                foreach(int padIntUid in usedPadInts) {
                    PadInt padInt = GetPadInt(padIntUid);
                    resultAbort = padInt.Abort(tid) && resultAbort;
                }
            } catch(PadIntNotFoundException) {
                throw;
            }

            /* updates the backup server */
            backupReplyTimer.Start();
            BackupServer.Abort(tid, usedPadInts);
            backupReplyTimer.Stop();

            return resultAbort;
        }

        public override void Dispose() {
            backupReplyTimer.Dispose(true);
            base.Dispose();
        }
    }
}
