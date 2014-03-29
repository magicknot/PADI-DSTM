using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadInt_Server {
    class PadInt {

        /* PadInt's uid */
        private int uid;

        /* PadInt's value during the transaction */
        private int actualValue;

        /* PadInt's value in the begining of the transaction */
        private int originalValue;

        /* Timer used in deadlock detection */
        //timer...

        /* uid of the next transaction to be promoted */
        private int promotion;

        /* Queue with transactions' tid with atributed read locks */
        private List<int> readers;

        /* Transaction' tid with atributed write lock */
        private int writer;

        /* Queue with transactions' tid with pending read locks */
        private List<int> pendingReaders;

        /* Queue with transactions' tid with pending write locks */
        private List<int> pendingWriters;

        /* uid represetns the PadInt's uid */
        protected PadInt (int uid)
        {

            this.uid = uid;
            this.actualValue = 0;
            this.originalValue = 0;
            //this.timer = ...
            this.promotion = 0;//confirmar???
            this.readers = new List<int>();
            this.writer = 0; //confirmar???
            this.pendingReaders = new List<int>();
            this.pendingWriters = new List<int>();
        }

        public int Uid
        {
            get { return uid; }
            set { this.uid = value; }
        }

        public int ActualValue
        {
            get { return actualValue; }
            set { this.actualValue = value; }
        }

        public int OriginalValue
        {
            get { return originalValue; }
            set { this.originalValue = value; }
        }

        /* public int Timer
        {
            get { return timer; }
            set { this.timer = value; }
        }*/

        /* Assigns to the transaction identified with tid
         * a read lock over the PadInt identified with uid,
         * as soon as possible.
         * 
         * Returns true if successful */
        public bool getReadLock (int tid, int uid)
        {
            return true;
        }

        /* Assigns to the transaction identified with tid
         * a write lock over the PadInt identified with uid,
         * as soon as possible.
         * 
         * Returns true if successful */
        public bool getWriteLock (int tid, int uid)
        {
            return true;
        }

        /* Frees a read lock over the PadInt, identified with uid,
        * owned by a transaction identified with tid.
        *
        * Returns true if successful */
        public bool freeReadLock (int tid, int uid)
        {
            dequeueReadLock(uid);
            return true;
        }

        /* Frees a write lock over the PadInt, identified with uid,
        * owned by a transaction identified with tid.
        *
        * Returns true if successful */
        public bool freeWriteLock (int tid, int uid)
        {
            dequeueWriteLock(uid);
            return true;
        }

        /* 
        *  */
        private void dequeueReadLock (int uid)
        {

        }

        /* 
        *  */
        private void dequeueWriteLock (int uid)
        {

        }
    }
}
