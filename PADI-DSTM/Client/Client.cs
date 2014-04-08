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

        public static void testSimpleRead(int uid0) {

            Console.WriteLine("------Test: Simple read ------");

            Library library = new Library();

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.createPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                Console.WriteLine("padInt0 read: " + padInt0.read());

                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        public static void testSimpleWrite(int uid0) {

            Console.WriteLine("------Test: Simple write ------");

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.createPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                if(padInt0.write(20)) {
                    Console.WriteLine("padInt0 write done with value (20) : " + padInt0.read());
                }

                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        public static void testSimpleAbort(int uid0) {

            Console.WriteLine("------Test: Simple Abort ------");

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.createPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                if(padInt0.write(20)) {
                    Console.WriteLine("padInt0 write done with value (20) : " + padInt0.read());
                }

                Library.txAbort();
                Console.WriteLine("txAbort Done");

                /* do a read to test if the abort was successful */
                Console.WriteLine("I will test if the abort was successful...");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0A = Library.accessPadInt(uid0);

                /* the padInt's value must be equal to initialization value */
                int value = padInt0A.read();

                if(value == 0) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: abort was not successful... value = " + value);
                }
                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        public static void testSimpleCommit(int uid0) {

            Console.WriteLine("------Test: Simple Commit ------");

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.createPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                if(padInt0.write(21)) {
                    Console.WriteLine("padInt0 write done with value (21) : " + padInt0.read());
                }

                Library.txCommit();
                Console.WriteLine("txCommit Done");

                /* do a read to test if the abort was successful */
                Console.WriteLine("I will test if the commit was successful...");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                /* the padInt's value must be equal to initialization value */
                PadIntStub padInt0A = Library.accessPadInt(uid0);

                if(padInt0A.read() == 21) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: commit was not successful...");
                }

                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        public static void testMultipleRead(int uid0, int uid1, int uid2) {

            Console.WriteLine("------Test: Multiple read ------");

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.createPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);
                PadIntStub padInt1 = Library.createPadInt(uid1);
                Console.WriteLine("padInt1 created with uid: " + uid1);
                PadIntStub padInt2 = Library.createPadInt(uid2);
                Console.WriteLine("padInt2 created with uid: " + uid2);

                bool result = padInt0.read() == 0;
                result = (padInt0.read() == padInt1.read());
                result = (padInt0.read() == padInt2.read());

                if(result) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: multiple read was not successful...");
                }

                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        static void Main(string[] args) {
            Console.Title = "Client";
            Console.WriteLine("Client up and running..");

            if(Library.init()) {

                string input;

                while(true) {

                    Console.WriteLine("Please, insert the number of the test that you want to run:");
                    Console.WriteLine("1- testRandom");
                    Console.WriteLine("Tests with one client:");
                    Console.WriteLine("2- Base: testSimpleRead");
                    Console.WriteLine("3- Base: testSimpleWrite");
                    Console.WriteLine("4- Base: testSimpleAbort");
                    Console.WriteLine("5- Base: testSimpleCommit");
                    Console.WriteLine("6- Base: testMultipleReads");

                    input = Console.ReadLine();

                    if(input.Equals("1")) {
                        testRandom();
                    }

                    if(input.Equals("2")) {
                        testSimpleRead(getNextUid());
                    }

                    if(input.Equals("3")) {
                        testSimpleWrite(getNextUid());
                    }

                    if(input.Equals("4")) {
                        testSimpleAbort(getNextUid());
                    }

                    if(input.Equals("5")) {
                        testSimpleCommit(getNextUid());
                    }

                    if(input.Equals("6")) {
                        testMultipleRead(getNextUid(), getNextUid(), getNextUid());
                    }
                }

            } else {
                Logger.log(new String[] { "There are no servers available" });
            }
        }
    }
}