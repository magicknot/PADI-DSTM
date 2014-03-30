using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadInt_Server {
    class Program {
        static void Main(string[] args) {


            /* PadInt Tests */
            int uid = 1;
            int tid0 = 0;
            int tid1 = 1;
            PadInt padInt = new PadInt(uid);

            /*bool getReadLock(int tid, int uid)
            bool getWriteLock(int tid, int uid)
            public bool freeReadLock(int tid, int uid)
            bool freeWriteLock(int tid, int uid)
            void dequeueReadLock(int uid)
            void dequeueWriteLock(int uid)*/

            /* read */
            if(padInt.getReadLock(tid0)) {
                Console.WriteLine("obtive lock read");
            }

            /* read */
            if(padInt.getReadLock(tid0)) {
                Console.WriteLine("obtive lock read");
            }

            /* write */
            if(padInt.getWriteLock(tid1)) {
                Console.WriteLine("obtive lock write");
            }


            while(true)
                ;
        }
    }
}
