using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadInt_Server {

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
        public PadInt(int uid) {

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

        public int Uid {
            get { return uid; }
            set { this.uid = value; }
        }

        public int ActualValue {
            get { return actualValue; }
            set { this.actualValue = value; }
        }

        public int OriginalValue {
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
        public bool getReadLock(int tid) {

            /* ve se não há algum escritor 
             *  se nao existir mete nos leitores
               se existir escritor mete na fila de espera dos leitores
             */

            /* if there is no writer */
            if(writer == INITIALIZATION) {
                readers.Add(tid);
            } else {
                pendingReaders.Add(tid);

                while(pendingReaders.Contains(tid))
                    Console.WriteLine("espera...");
            }

            /* TODO !!!!!
             * 
             * VER COMO E HISTORIA DE FICAR PARADO `A ESPERA SE SO´ 
             * RESPONDER DEPOIS DE TER O LOCK. MESMO QUE ISSO SEJA NO
             * SERVER TB TEM QUE SER VISTO AQUI DE ALGUMA FORMA
             */

            return true;
            /* retorna false caso abort??? depois com os timer? */
        }

        /* Returns true if the transaction identified by tid
         *  has a write lock over the PadInt identified by uid */
        public bool hasWriteLock(int tid) {
            return writer == tid;
        }

        /* Assigns to the transaction identified by tid
         * a write lock over the PadInt identified by uid,
         * as soon as possible.
         * 
         * Returns true if successful */
        public bool getWriteLock(int tid) {

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
                        while(pendingWriters.Contains(tid))
                            Console.WriteLine("espera...");
                    }
                } else {
                    /* if the locks are read locks */

                    /* if the transaction, identified by tid,
                     *  does not have a read lock */
                    if(!readers.Contains(tid)) {
                        pendingWriters.Add(tid);
                        while(pendingWriters.Contains(tid))
                            Console.WriteLine("espera...");
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
        public void freeReadLock(int tid) {
            readers.Remove(tid);
            dequeueReadLock();
        }

        /* Frees a write lock over the PadInt, identified by uid,
        * owned by a transaction identified with tid.
        *
        * Returns true if successful */
        public void freeWriteLock(int tid) {
            /* "frees" writer variable */
            writer = INITIALIZATION;
            dequeueWriteLock();
        }

        /* If called when does not exist any reader,
         *  see if exists any transaction waiting for
         *  a promotion then do the promotion. If only
         *  exists pending writers, assigns the lock to
         *  the first one.
         */
        private void dequeueReadLock() {
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
        private void dequeueWriteLock() {
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

        public void commit(int tid) {
            /* TODO */

            /* ver se da´ para fazer fusao com o metodo abort */

            /* liberta locks de read: freeReadLock(int tid)
                 * liberta locks de write: freeWriteLock(int tid)
                 * 
                 * verifica se o tid da transaccao nao esta na promotion
                 *  - se estiver e for commit manda abort????
                 *  - se estiver e for abort limpa apenas
                 * 
                 * apenas precisa de fazer isto apenas quando sao escritas:
                 * entry.Value.OriginalValue = entry.Value.ActualValue; */
        }

        public void abort(int tid) {
            /* TODO */


            /* liberta locks de read: freeReadLock(int tid)
                 * liberta locks de write: freeWriteLock(int tid)
                 * 
                 * verifica se o tid da transaccao nao esta na promotion
                 *  - se estiver e for commit manda abort????
                 *  - se estiver e for abort limpa apenas
                 * 
                 * apenas e so´ no caso em que era lock de write e´ que faz isto:
                 * entry.Value.ActualValue = entry.Value.OriginalValue; */
        }
    }
}
