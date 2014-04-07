using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;
using CommonTypes;

namespace Client {

    class Client2 {

        public Client2() {
            //nothing to do here
        }

        public void testSimpleRead(int uid0) {

            Console.WriteLine("------Test: Simple read ------");
            Logger.log(new String[] { "Client Base", "------Test: Simple read ------" });

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

        public void testSimpleWrite(int uid0) {

            Console.WriteLine("------Test: Simple write ------");
            Logger.log(new String[] { "Client Base", "------Test: Simple write ------" });

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

        public void testSimpleAbort(int uid0) {

            Console.WriteLine("------Test: Simple Abort ------");
            Logger.log(new String[] { "Client Base", "------Test: Simple Abort ------" });

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

                /* the padInt's value must be equal to initialization value */
                int value = padInt0.read();
                if(value == 0) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: abort was not successful...  value = " + value);
                }
                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
        }

        public void testSimpleCommit(int uid0) {

            Console.WriteLine("------Test: Simple Commit ------");
            Logger.log(new String[] { "Client Base", "------Test: Simple Commit ------" });

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
                if(padInt0.read() == 21) {
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

        public void testMultipleRead(int uid0, int uid1, int uid2) {

            Console.WriteLine("------Test: Multiple read ------");
            Logger.log(new String[] { "Client Base", "------Test: Multiple read ------" });

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

        /*static void Main(string[] args) {
            Console.Title = "Client Base";
            Console.WriteLine("Client Base up and running..");

            if(Library.init()) {
                testRandom();
                testSimpleRead();
                testSimpleWrite();
                testSimpleAbort();
                testSimpleCommit();
                testMultipleRead();
            } else {
                Logger.log(new String[] { "There are no servers available" });
            }

            while(true)
                ;
        }*/
    }
}
