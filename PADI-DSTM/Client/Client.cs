using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;
using CommonTypes;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace Client {

    class Client : MarshalByRefObject {

        private Random random;
        private int nextUid;
        /* if true stops the loop */
        private bool stopLoop;

        public Client() {
            random = new Random();
            nextUid = 0;
            stopLoop = true;
        }

        internal int NextUID {
            set { this.nextUid = value; }
            get { return nextUid; }

        }

        internal bool StopLoop {
            set { this.stopLoop = value; }
            get { return stopLoop; }

        }

        public void setStop(bool value) {
            this.stopLoop = value;
        }

        public void setStopLoop(bool value) {
            //TcpChannel channel = new TcpChannel();
            //ChannelServices.RegisterChannel(channel, true);
            Client c = (Client) Activator.GetObject(typeof(Client), "tcp://localhost:6085/Client");
            c.setStop(value);
            Console.WriteLine("client.StopLoop = " + StopLoop);
        }

        public int lastUid() {
            return nextUid;
        }

        public int getLastUid() {
            //TcpChannel channel = new TcpChannel();
            //ChannelServices.RegisterChannel(channel, true);
            Client c = (Client) Activator.GetObject(typeof(Client), "tcp://localhost:6085/Client");
            return c.lastUid();
        }

        public int getNextUid() {
            /* first arg of Next its the minimum and the second arg is the maximum */
            nextUid = random.Next(0, 100);
            return nextUid;
        }

        public void testRandom() {

            Console.WriteLine("------Test random ------");
            Logger.log(new String[] { "Client", "------Test random begin------" });

            for(int i = 0; i < 20; i++)
                Console.WriteLine("number = " + getNextUid());

            Logger.log(new String[] { "-------------Test random end------------------" });
        }

        public void testSimpleRead(int uid0) {

            Console.WriteLine("------ Test: Simple read ------");
            Logger.log(new String[] { "Client", "------ Test: Simple read begin ------" });

            Library library = new Library();

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.createPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                Console.WriteLine("padInt0 read: " + padInt0.read());

                /* to test with more than one client */
                while(!stopLoop) {
                    Console.WriteLine("loop stopLoop = " + stopLoop);
                }

                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.log(new String[] { "---------Simple read end----------" });
        }

        public void testSimpleWrite(int uid0) {

            Console.WriteLine("------ Test: Simple write ------");
            Logger.log(new String[] { "Client", "------ Test: Simple write ------" });

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

                /* to test with more than one client */
                while(!stopLoop)
                    ;

                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.log(new String[] { "---------Simple write end----------" });
        }

        public void testSimpleAbort(int uid0) {

            Console.WriteLine("------ Test: Simple Abort ------");
            Logger.log(new String[] { "Client", "------ Test: Simple Abort ------" });

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
            Logger.log(new String[] { "---------Simple abort end----------" });
        }

        public void testSimpleCommit(int uid0) {

            Console.WriteLine("------ Test: Simple Commit ------");
            Logger.log(new String[] { "Client", "------ Test: Simple Commit ------" });

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
            Logger.log(new String[] { "---------Simple commit end----------" });
        }

        public void testMultipleRead(int uid0, int uid1, int uid2) {

            Console.WriteLine("------ Test: Multiple read ------");
            Logger.log(new String[] { "Client", "------ Test: Multiple read ------" });

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

                /* to test with more than one client */
                while(!stopLoop)
                    ;

                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.log(new String[] { "---------multiple read end----------" });
        }

        public void testSampleApp() {
            Console.WriteLine("------ Test: Sample App ------");
            Logger.log(new String[] { "Client", "------ Test: Sample App ------" });

            bool res;
            res = Library.txBegin();
            PadIntStub pi_a = Library.createPadInt(0);
            PadIntStub pi_b = Library.createPadInt(1);
            res = Library.txCommit();

            res = Library.txBegin();
            pi_a = Library.accessPadInt(0);
            pi_b = Library.accessPadInt(1);
            pi_a.write(36);
            pi_b.write(37);
            Console.WriteLine("a = " + pi_a.read());
            Console.WriteLine("b = " + pi_b.read());
            //Library.status();

            // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
            /*res = PadiDstm.Freeze("tcp://localhost:2001/Server");
            res = PadiDstm.Recover("tcp://localhost:2001/Server");
            res = PadiDstm.Fail("tcp://localhost:2002/Server");*/

            res = Library.txCommit();
            Logger.log(new String[] { "---------Sample App end----------" });
        }

        //-----------------------------------------------------
        //client 2
        public void testSimpleReadClient2(int uid0) {

            Console.WriteLine("------ Test: client2 Simple read ------");
            Logger.log(new String[] { "Client", "------ Test: client2 Simple read ------" });

            Library library = new Library();

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.accessPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                Console.WriteLine("padInt0 read: " + padInt0.read());

                Library.txCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.log(new String[] { "---------Client2 Simple read end----------" });
        }

        public void testSimpleWriteClient2(int uid0) {

            Console.WriteLine("------ Test: client2 Simple write ------");
            Logger.log(new String[] { "Client", "------ Test: client2 Simple write ------" });

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.accessPadInt(uid0);
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
            Logger.log(new String[] { "---------Client2 Simple write end----------" });
        }

        public void testMultipleReadClient2(int uid0, int uid1, int uid2) {

            Console.WriteLine("------ Test: Client2 Multiple read ------");
            Logger.log(new String[] { "Client", "------ Test: Client2 Multiple read ------" });

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.txBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.accessPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);
                PadIntStub padInt1 = Library.accessPadInt(uid1);
                Console.WriteLine("padInt1 created with uid: " + uid1);
                PadIntStub padInt2 = Library.accessPadInt(uid2);
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
            Logger.log(new String[] { "---------Client2 Multiple read end----------" });
        }

    }
}