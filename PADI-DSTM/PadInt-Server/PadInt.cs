using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace PadIntServer {
    [Serializable]
    class PadInt {

        /* constante used in the initialization of int variables */
        private const int INITIALIZATION = -1;

        /* PadInt's uid */
        private int uid;

        /* PadInt's value during the transaction */
        private int actualValue;

        /* PadInt's value in the begining of the transaction */
        private int originalValue;

        /* Timer used in deadlock detection */
        //timer...

        /* uid of the next transaction to be promoted.
         *
         * Value INITIALIZATION means that there is no transaction,
         *  identified by tid, waiting for promotion. 
         */
        private int promotion;

        /* Queue with transactions' tid with atributed read locks */
        private List<int> readers;

        /* Transaction' tid with atributed write lock.
         * 
         * Value INITIALIZATION means that there is no transaction,
         *  identified by tid, writing. 
         */
        private int writer;

        /* Queue with transactions' tid with pending read locks */
        private List<int> pendingReaders;

        /* Queue with transactions' tid with pending write locks */
        private List<int> pendingWriters;

        /* uid represetns the PadInt's uid */
        internal PadInt(int uid) {

            this.uid = uid;
            this.actualValue = 0;
            this.originalValue = 0;
            //this.timer = ...
            this.promotion = INITIALIZATION;
            this.readers = new List<int>();
            this.writer = INITIALIZATION;
            this.pendingReaders = new List<int>();
            this.pendingWriters = new List<int>();
        }

        internal int Uid {
            get { return uid; }
            set { this.uid = value; }
        }

        internal int ActualValue {
            get { return actualValue; }
            set { this.actualValue = value; }
        }

        internal int OriginalValue {
            get { return originalValue; }
            set { this.originalValue = value; }
        }

        /* public int Timer
        {
            get { return timer; }
            set { this.timer = value; }
        }*/

        /* Assigns to the transaction identified by tid
         * a read lock over the PadInt identified by uid,
         * as soon as possible.
         * 
         * Returns true if successful */
        internal bool getReadLock(int tid) {

            Logger.log(new String[] { "PadInt", "getReadLock" });

            /* ve se não há algum escritor 
             *  se nao existir mete nos leitores
               se existir escritor mete na fila de espera dos leitores
             */

            /* if there is no writer */
            if(writer == INITIALIZATION) {
                readers.Add(tid);
            } else {
                pendingReaders.Add(tid);

                while(pendingReaders.Contains(tid)) {
                    Logger.log(new String[] { "PadInt", "espera read... writter: ", writer.ToString() });
                }
            }

            /* TODO !!!!!
             * 
             * VER COMO E HISTORIA DE FICAR PARADO `A ESPERA SE SO´ 
             * RESPONDER DEPOIS DE TER O LOCK. MESMO QUE ISSO SEJA NO
             * 
             * SERVER TB TEM QUE SER VISTO AQUI DE ALGUMA FORMA
             */

            return true;
            /* retorna false caso abort??? depois com os timer? */
        }

        /* Returns true if the transaction identified by tid
         *  has a write lock over the PadInt identified by uid */
        internal bool hasWriteLock(int tid) {
            return writer == tid;
        }

        /* Assigns to the transaction identified by tid
         * a write lock over the PadInt identified by uid,
         * as soon as possible.
         * 
         * Returns true if successful */
        internal bool getWriteLock(int tid) {

            Logger.log(new String[] { "PadInt", "getWriteLock" });

            /* TODO
             * 
             * ver como e´ o caso em que existe alguma
             * no promotion e depois vem para aqui.
             * Ver caso da tiraQueueLeitura.
             * 
             */

            /* if don't exists a writer or readers */
            if(writer == INITIALIZATION || readers.Count == 0) {
                writer = tid;
            } else {
                /* if the lock is a write lock */
                if(readers.Count == 0) {
                    /* if the lock is not assigned to the transaction
                     *  identified by tid */
                    if(writer != tid) {
                        pendingWriters.Add(tid);
                        while(pendingWriters.Contains(tid)) {
                            Logger.log(new String[] { "PadInt", "espera write 1... writer: ", writer.ToString() });
                        }
                    }
                } else {
                    /* if the locks are read locks */

                    /* if the transaction, identified by tid,
                     *  does not have a read lock */
                    if(!readers.Contains(tid)) {
                        pendingWriters.Add(tid);
                        while(pendingWriters.Contains(tid)) {
                            Logger.log(new String[] { "PadInt", "espera write 2...writer: ", writer.ToString() });
                        }
                    } else {
                        /* if there is only a
                         *  reader (transaction identified by tid) */
                        readers.Remove(tid);
                        if(readers.Count == 1) {
                            writer = tid;
                        } else {
                            /* if there is no transaction wainting
                             * for promotion */
                            if(promotion == INITIALIZATION) {
                                promotion = tid;
                            } else {
                                /* abort */
                                //TODO confirmar se basta retornar falso
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /* Frees a read lock over the PadInt, identified by uid,
        * owned by a transaction identified by tid.
        *
        * Returns true if successful */
        internal bool freeReadLock(int tid) {
            Logger.log(new String[] { "PadInt", "freeReadLock" });

            if(readers.Remove(tid)) {
                dequeueReadLock();
                return true;
            } else {
                return false;
            }
        }

        /* Frees a write lock over the PadInt, identified by uid,
        * owned by a transaction identified with tid.
        *
        * Returns true if successful */
        internal bool freeWriteLock(int tid) {
            Logger.log(new String[] { "PadInt", "freeWriteLock" });

            /* "frees" writer variable */
            if(writer != INITIALIZATION) {
                writer = INITIALIZATION;
                dequeueWriteLock();
                return true;
            } else {
                return false;
            }
        }

        /* If called when does not exist any reader,
         *  see if exists any transaction waiting for
         *  a promotion then do the promotion. If only
         *  exists pending writers, assigns the lock to
         *  the first one.
         */
        internal void dequeueReadLock() {
            Logger.log(new String[] { "PadInt", "dequeueReadLock" });

            int temp = INITIALIZATION;

            if(readers.Count == 0) {
                if(promotion != INITIALIZATION) {
                    /* "frees" promotion variable */
                    temp = promotion;
                    promotion = INITIALIZATION;
                    getWriteLock(temp);
                } else {
                    if(pendingWriters.Count > 0) {
                        /* removes the first writer in the queue */
                        temp = pendingWriters[0];
                        pendingWriters.RemoveAt(0);
                        getWriteLock(temp);
                    }
                }
            }
        }

        /* If exists any transaction waiting for a promotion
         *  then do the promotion. Otherwise if exists pending
         *  writers, assigns the lock to the first one.
         * If none of the last cenarios was true see if exists
         *  any pending reader and assigns the lock to the
         *  first one.
         */
        internal void dequeueWriteLock() {
            Logger.log(new String[] { "PadInt", "dequeueWriteLock" });

            int temp = INITIALIZATION;

            if(promotion != INITIALIZATION) {
                temp = promotion;
                promotion = INITIALIZATION;
                getWriteLock(temp);
            } else {
                if(pendingWriters.Count > 0) {
                    /* removes the first writer in the queue */
                    temp = pendingWriters[0];
                    pendingWriters.RemoveAt(0);
                    getWriteLock(temp);
                } else {
                    if(pendingReaders.Count > 0) {
                        /* removes the first reader in the queue */
                        temp = pendingReaders[0];
                        pendingReaders.RemoveAt(0);
                        getReadLock(temp);
                    }
                }
            }
        }

        /* Receives the identifier of a transaction and does a commit.
         * 
         * Free promotion variable, removes all read/write locks atributed
         *  to this transaction and removes all possible pending locks.
         * 
         * Finally, updates PadInt's originalValue because we do a successful
         *  commit.
         * 
         * Returns true if successful.
         */
        internal bool commit(int tid) {
            Logger.log(new String[] { "PadInt", "commit" });

            bool commitSuccessful = true;

            if(pendingReaders.Remove(tid)) {
                commitSuccessful = false;
            }

            if(pendingWriters.Remove(tid)) {
                commitSuccessful = false;
            }

            freeReadLock(tid);

            if(freeWriteLock(tid)) {
                OriginalValue = ActualValue;
                Logger.log(new String[] { "PadInt", "commit", " Cleaned writer= ", writer.ToString() });
            }

            /* this must be done in the end because if we do this before
             *  we return false and don't free the locks or pending locks.
             */
            if(promotion == tid) {
                promotion = INITIALIZATION;
                commitSuccessful = false;
                //lanca excepcao NaoFezAindaEscrita
            }

            return commitSuccessful;
        }

        /* Receives the identifier of a transaction and does an abort.
         * 
         * Free promotion variable, removes all read/write locks atributed
         *  to this transaction and removes all possible pending locks.
         * 
         * Finally, does the rollback of the PadInt's value.
         * 
         * Returns true if successful.
         */
        internal bool abort(int tid) {
            Logger.log(new String[] { "PadInt", "abort" });

            pendingReaders.Remove(tid);
            pendingWriters.Remove(tid);

            if(promotion == tid) {
                promotion = INITIALIZATION;
            }

            freeReadLock(tid);

            /* only if exists a write lock we do the rollback of the PadInt's value */
            if(freeWriteLock(tid)) {
                ActualValue = OriginalValue;
                Logger.log(new String[] { "PadInt", "abort", " Cleaned writer= ", writer.ToString() });
            }

            return true;
        }
    }
}
