using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;
using CommonTypes;

namespace Client {

    class Client {

        private static int uid0;
        private static int uid1;
        private static int uid2;

        public Client() {
            uid0 = 0;
            uid1 = 1;
            uid2 = 2;
        }

        public static void testSimpleRead() {

            Console.WriteLine("------Test: Simple read ------");

            Library library = new Library();

            Console.WriteLine("library created");

            if(library.init()) {
                Console.WriteLine("init() Done");

                library.txBegin();

                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = library.createPadInt(uid0);

                Console.WriteLine("padInts created");

                Console.WriteLine("padInt0 read: " + padInt0.read());

                library.txCommit();

                Console.WriteLine("txBegin Done");
            } else {
                Logger.log(new String[] { "There are no servers available" });
            }

        }

        public static void testSimpleWrite() {

            Console.WriteLine("------Test: Simple write ------");

            Library library = new Library();

            Console.WriteLine("library created");

            if(library.init()) {
                Console.WriteLine("init() Done");

                library.txBegin();

                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = library.createPadInt(uid0);

                Console.WriteLine("padInts created");

                if(padInt0.write(20)) {
                    Console.WriteLine("padInt0 write done with value (20) : " + padInt0.read());
                }

                library.txCommit();

                Console.WriteLine("txCommit Done");
            } else {
                Logger.log(new String[] { "There are no servers available" });
            }

        }

        public static void testSimpleAbort() {

            Console.WriteLine("------Test: Simple Abort ------");

            Library library = new Library();

            Console.WriteLine("library created");

            if(library.init()) {
                Console.WriteLine("init() Done");

                library.txBegin();

                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = library.createPadInt(uid0);

                Console.WriteLine("padInts created");

                if(padInt0.write(20)) {
                    Console.WriteLine("padInt0 write done with value (20) : " + padInt0.read());
                }

                library.txAbort();

                Console.WriteLine("txAbort Done");

                /* do a read to test if the abort was successful */
                Console.WriteLine("I will test if the abort was successful...");

                library.txBegin();
                Console.WriteLine("txBegin Done");

                /* the padInt's value must be equal to initialization value */
                if(padInt0.read() == 0) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: abort was not successful...");
                }

                library.txCommit();

                Console.WriteLine("txBegin Done");
            } else {
                Logger.log(new String[] { "There are no servers available" });
            }

        }

        public static void testSimpleCommit() {

            Console.WriteLine("------Test: Simple Commit ------");

            Library library = new Library();

            Console.WriteLine("library created");

            if(library.init()) {
                Console.WriteLine("init() Done");

                library.txBegin();

                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = library.createPadInt(uid0);

                Console.WriteLine("padInts created");

                if(padInt0.write(21)) {
                    Console.WriteLine("padInt0 write done with value (20) : " + padInt0.read());
                }

                library.txCommit();

                Console.WriteLine("txCommit Done");

                /* do a read to test if the abort was successful */
                Console.WriteLine("I will test if the commit was successful...");

                library.txBegin();
                Console.WriteLine("txBegin Done");

                /* the padInt's value must be equal to initialization value */
                if(padInt0.read() == 21) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: abort was not successful...");
                }

                library.txCommit();

                Console.WriteLine("txBegin Done");
            } else {
                Logger.log(new String[] { "There are no servers available" });
            }

        }

        static void Main(string[] args) {
            Console.Title = "Client";
            Console.WriteLine("Client up and running..");

            testSimpleRead();
            testSimpleWrite();
            testSimpleAbort();
            testSimpleCommit();
        }
    }
}
