using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadIntServer {
    class TestClass {

        /* PadInt Tests */
        private static int uid = 1;
        private static int tid0 = 0;
        private static int tid1 = 1;
        private static int tid2 = 2;
        private static PadInt padInt = new PadInt(uid);

        static void testeWrite1() {
            /* write */
            if(padInt.GetWriteLock(tid0)) {
                Console.WriteLine("tid0: obtive lock write");
            }

            /* read */
            if(padInt.GetReadLock(tid1)) {
                Console.WriteLine("tid1: obtive lock read");
            }

            /* liberta write */
            padInt.FreeWriteLock(tid0);
            Console.WriteLine("tid0: libertei lock write");

            /* read */
            if(padInt.GetReadLock(tid2)) {
                Console.WriteLine("tid2: obtive lock read");
            }
        }

        public static void MainTeste(String[] args) {

            /*bool getReadLock(int tid, int uid)
bool getWriteLock(int tid, int uid)
public bool freeReadLock(int tid, int uid)
bool freeWriteLock(int tid, int uid)
void dequeueReadLock(int uid)
void dequeueWriteLock(int uid)*/


            /* PadInt Test */
            //testeWrite1();

        }
    }

}
