using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;
using CommonTypes;

namespace Client {

    class Client {

        private static Random random = new Random();
        private static int nextUid = 0;

        public Client() {
            //random = new Random();
            //nextUid = random.Next();
        }

        private static int getNextUid() {
            /* first arg of Next its the minimum and the second arg is the maximum */
            nextUid = random.Next(0, 100);
            return nextUid;
        }

        public static void testRandom() {

            Console.WriteLine("------Test random ------");

            for(int i = 0; i < 20; i++)
                Console.WriteLine("number = " + getNextUid());
        }

        public static void testSimpleRead() {

            Console.WriteLine("------Test: Simple read ------");

            Library library = new Library();

            Console.WriteLine("library created");

            try {
                if(library.init()) {
                    Console.WriteLine("init() Done");

                    library.txBegin();
                    Console.WriteLine("txBegin Done");

                    PadIntStub padInt0 = library.createPadInt(getNextUid());
                    Console.WriteLine("padInt0 created with uid: " + nextUid);

                    Console.WriteLine("padInt0 read: " + padInt0.read());

                    library.txCommit();
                    Console.WriteLine("txCommit Done");

                    library.closeChannel();
                    Console.WriteLine("closeChannel Done");
                } else {
                    Logger.log(new String[] { "There are no servers available" });
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        public static void testSimpleWrite() {

            Console.WriteLine("------Test: Simple write ------");

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                if(library.init()) {
                    Console.WriteLine("init() Done");

                    library.txBegin();
                    Console.WriteLine("txBegin Done");

                    PadIntStub padInt0 = library.createPadInt(getNextUid());
                    Console.WriteLine("padInt0 created with uid: " + nextUid);

                    if(padInt0.write(20)) {
                        Console.WriteLine("padInt0 write done with value (20) : " + padInt0.read());
                    }

                    library.txCommit();
                    Console.WriteLine("txCommit Done");

                    library.closeChannel();
                    Console.WriteLine("closeChannel Done");
                } else {
                    Logger.log(new String[] { "There are no servers available" });
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        public static void testSimpleAbort() {

            Console.WriteLine("------Test: Simple Abort ------");

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                if(library.init()) {
                    Console.WriteLine("init() Done");

                    library.txBegin();
                    Console.WriteLine("txBegin Done");

                    PadIntStub padInt0 = library.createPadInt(getNextUid());
                    Console.WriteLine("padInt0 created with uid: " + nextUid);

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
                    int value = padInt0.read();
                    if(value == 0) {
                        Console.WriteLine("it's OK");
                    } else {
                        Console.WriteLine("BUG!!!!!: abort was not successful...  value = " + value);
                    }
                    library.txCommit();
                    Console.WriteLine("txCommit Done");

                    library.closeChannel();
                    Console.WriteLine("closeChannel Done");
                } else {
                    Logger.log(new String[] { "There are no servers available" });
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        public static void testSimpleCommit() {

            Console.WriteLine("------Test: Simple Commit ------");

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                if(library.init()) {
                    Console.WriteLine("init() Done");

                    library.txBegin();
                    Console.WriteLine("txBegin Done");

                    PadIntStub padInt0 = library.createPadInt(getNextUid());
                    Console.WriteLine("padInt0 created with uid: " + nextUid);

                    if(padInt0.write(21)) {
                        Console.WriteLine("padInt0 write done with value (21) : " + padInt0.read());
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
                    Console.WriteLine("txCommit Done");

                    library.closeChannel();
                    Console.WriteLine("closeChannel Done");
                } else {
                    Logger.log(new String[] { "There are no servers available" });
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        public static void testMultipleRead() {

            Console.WriteLine("------Test: Multiple read ------");

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                if(library.init()) {
                    Console.WriteLine("init() Done");

                    library.txBegin();
                    Console.WriteLine("txBegin Done");

                    PadIntStub padInt0 = library.createPadInt(getNextUid());
                    Console.WriteLine("padInt0 created with uid: " + nextUid);
                    PadIntStub padInt1 = library.createPadInt(getNextUid());
                    Console.WriteLine("padInt1 created with uid: " + nextUid);
                    PadIntStub padInt2 = library.createPadInt(getNextUid());
                    Console.WriteLine("padInt2 created with uid: " + nextUid);

                    bool result = padInt0.read() == 0;
                    result = (padInt0.read() == padInt1.read());
                    result = (padInt0.read() == padInt2.read());

                    if(result) {
                        Console.WriteLine("it's OK");
                    } else {
                        Console.WriteLine("BUG!!!!!: multiple read was not successful...");
                    }

                    library.txCommit();
                    Console.WriteLine("txCommit Done");

                    library.closeChannel();
                    Console.WriteLine("closeChannel Done");
                } else {
                    Logger.log(new String[] { "There are no servers available" });
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        static void Main(string[] args) {
            Console.Title = "Client";
            Console.WriteLine("Client up and running..");

            //testRandom();
            //testSimpleRead();
            //testSimpleWrite();
            testSimpleAbort();
            //testSimpleCommit();
            //testMultipleRead();

            while(true)
                ;
        }
    }
}
