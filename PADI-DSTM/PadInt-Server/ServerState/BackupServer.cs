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
        private const int IM_ALIVE_INTERVAL = 15000;

        internal IServer PrimaryServer {
            set { this.pairServerReference = value; }
            get { return pairServerReference; }
        }

        internal string PrimaryAddress {
            set { this.pairServerAddress = value; }
            get { return pairServerAddress; }
        }

        internal BackupServer(Server server, string primaryAddress, Dictionary<int, IPadInt> pdInts)
            : base(server, pdInts) {

            PrimaryAddress = primaryAddress;
            PrimaryServer = (IServer)Activator.GetObject(typeof(IServer), primaryAddress);
            PrimaryServer.CreatePrimaryServer(Server.Address, padIntDictionary, false);

            // Create a timer with inAliveInterval second interval.
            imAliveTimer = new PadIntTimer(IM_ALIVE_INTERVAL);
            imAliveTimer.Timer.Elapsed += new ElapsedEventHandler(ImAliveEvent);

            //starts im alive timer
            imAliveTimer.Start();
            StateMsg = "BACKUP STATE";
        }

        /// <summary>
        /// Receives I'm alive from primary server
        /// </summary>
        internal override void ImAlive() {
            Logger.Log(new String[] { "BackupServer", Server.ID.ToString(), "ImAlive" });
            //re-starts the timer
            imAliveTimer.Stop();
            imAliveTimer.Start();
        }

        private void ImAliveEvent(object source, ElapsedEventArgs e) {
            Logger.Log(new String[] { "BackupServer", Server.ID.ToString(), "ImAliveEvent" });
            Console.WriteLine("PrimaryAddress: " + PrimaryAddress + " " + stateMessage);
            imAliveTimer.Stop();
            //imAliveTimer.Close();
            IServerMachine primaryServerMachine = (IServerMachine)Activator.GetObject(typeof(IServerMachine), PrimaryAddress + "Machine");

            primaryServerMachine.RestartServer();
            PrimaryServer.CreateBackupServer(Server.Address, padIntDictionary, true);
            //Server.CreatePrimaryServer(newBackupAddress, padIntDictionary, false);
            IMaster master = (IMaster)Activator.GetObject(typeof(IMaster), "tcp://localhost:8086/MasterServer");
            master.UpdateServerAddress(Server.ID, Server.Address);
        }

        protected override PadInt GetPadInt(int uid) {
            Logger.Log(new String[] { "BackupServer", Server.ID.ToString(), "getPadInt", "uid ", uid.ToString() });
            return base.GetPadInt(uid);
        }

        protected override void VerifyPadInts(List<int> padInts) {
            Logger.Log(new String[] { "BackupServer", Server.ID.ToString(), "verifyPadInts" });
            base.VerifyPadInts(padInts);
        }

        internal override bool CreatePadInt(int uid) {
            Logger.Log(new String[] { "BackupServer", Server.ID.ToString(), "createPadInt", "uid ", uid.ToString(),
                "    NPadInts", (padIntDictionary.Count + 1).ToString() });
            try {
                padIntDictionary.Add(uid, (IPadInt)new PadInt(uid));
                return true;
            }
            catch (ArgumentException) {
                throw new PadIntAlreadyExistsException(uid, Server.ID);
            }
        }

        internal override bool ConfirmPadInt(int uid) {
            Logger.Log(new String[] { "BackupServer", Server.ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });
            return base.ConfirmPadInt(uid);
        }

        /* Returns the value of the PadInt when the transaction
         *  has the read/write lock.
         * Throw an exception if PadInt not found. 
         */
        internal override int ReadPadInt(int tid, int uid) {
            Logger.Log(new String[] { "BackupServer", Server.ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = GetPadInt(uid);

                if (padInt.HasWriteLock(tid) || padInt.GetReadLock(tid)) {
                    return padInt.ActualValue;
                }
            }
            catch (PadIntNotFoundException) {
                throw;
            }
            catch (AbortException) {
                throw;
            }

            return -1;
        }

        internal override bool WritePadInt(int tid, int uid, int value) {
            Logger.Log(new String[] { "Server ", Server.ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            try {
                /* Obtain the PadInt identified by uid */
                PadInt padInt = GetPadInt(uid);

                if (padInt.GetWriteLock(tid)) {
                    padInt.ActualValue = value;
                    return true;
                }
            }
            catch (PadIntNotFoundException) {
                throw;
            }
            catch (AbortException) {
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
        internal override bool Commit(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "BackupServer", Server.ID.ToString(), "commit", "tid", tid.ToString() });

            bool resultCommit = true;

            try {
                VerifyPadInts(usedPadInts);

                foreach (int padIntUid in usedPadInts) {
                    PadInt padInt = GetPadInt(padIntUid);
                    resultCommit = padInt.Commit(tid) && resultCommit;
                }

            }
            catch (PadIntNotFoundException) {
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
        internal override bool Abort(int tid, List<int> usedPadInts) {
            Logger.Log(new String[] { "BackupServer", Server.ID.ToString(), "abort", "tid", tid.ToString() });

            bool resultAbort = true;

            try {
                VerifyPadInts(usedPadInts);

                foreach (int padIntUid in usedPadInts) {
                    PadInt padInt = GetPadInt(padIntUid);
                    resultAbort = padInt.Abort(tid) && resultAbort;
                }
            }
            catch (PadIntNotFoundException) {
                throw;
            }

            return resultAbort;
        }

        public override bool Status() {
            base.Status();
            return true;
        }
    }
}
