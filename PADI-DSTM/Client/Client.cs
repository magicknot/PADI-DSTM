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

        public void SetStop(bool value) {
            this.stopLoop = value;
        }

        public void SetStopLoop(bool value) {
            //TcpChannel channel = new TcpChannel();
            //ChannelServices.RegisterChannel(channel, true);
            Client c = (Client) Activator.GetObject(typeof(Client), "tcp://localhost:6085/Client");
            c.SetStop(value);
            Console.WriteLine("client.StopLoop = " + StopLoop);
        }

        public int LastUid() {
            return nextUid;
        }

        public int GetLastUid() {
            //TcpChannel channel = new TcpChannel();
            //ChannelServices.RegisterChannel(channel, true);
            Client c = (Client) Activator.GetObject(typeof(Client), "tcp://localhost:6085/Client");
            return c.LastUid();
        }

        public int GetNextUid() {
            /* first arg of Next its the minimum and the second arg is the maximum */
            nextUid = random.Next(0, 100);
            return nextUid;
        }

        public void TestRandom() {

            Console.WriteLine("------Test random ------");
            Logger.Log(new String[] { "Client", "------Test random begin------" });

            for(int i = 0; i < 20; i++)
                Console.WriteLine("number = " + GetNextUid());

            Logger.Log(new String[] { "-------------Test random end------------------" });
        }

        public void TestSimpleRead(int uid0) {

            Console.WriteLine("------ Test: Simple read ------");
            Logger.Log(new String[] { "Client", "------ Test: Simple read begin ------" });

            Library library = new Library();

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                Console.WriteLine("padInt0 read: " + padInt0.Read());

                /* to test with more than one client */
                while(!stopLoop) {
                    Console.WriteLine("loop stopLoop = " + stopLoop);
                }

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Simple read end----------" });
        }

        public void TestSimpleWrite(int uid0) {

            Console.WriteLine("------ Test: Simple write ------");
            Logger.Log(new String[] { "Client", "------ Test: Simple write ------" });

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                if(padInt0.Write(20)) {
                    Console.WriteLine("padInt0 write done with value (20) : " + padInt0.Read());
                }

                /* to test with more than one client */
                while(!stopLoop)
                    ;

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Simple write end----------" });
        }

        public void TestSimpleAbort(int uid0) {

            Console.WriteLine("------ Test: Simple Abort ------");
            Logger.Log(new String[] { "Client", "------ Test: Simple Abort ------" });

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                if(padInt0.Write(20)) {
                    Console.WriteLine("padInt0 write done with value (20) : " + padInt0.Read());
                }

                Library.TxAbort();
                Console.WriteLine("txAbort Done");

                /* do a read to test if the abort was successful */
                Console.WriteLine("I will test if the abort was successful...");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0A = Library.AccessPadInt(uid0);

                /* the padInt's value must be equal to initialization value */
                int value = padInt0A.Read();

                if(value == 0) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: abort was not successful... value = " + value);
                }
                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Simple abort end----------" });
        }

        public void TestSimpleCommit(int uid0) {

            Console.WriteLine("------ Test: Simple Commit ------");
            Logger.Log(new String[] { "Client", "------ Test: Simple Commit ------" });

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                if(padInt0.Write(21)) {
                    Console.WriteLine("padInt0 write done with value (21) : " + padInt0.Read());
                }

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                /* do a read to test if the abort was successful */
                Console.WriteLine("I will test if the commit was successful...");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                /* the padInt's value must be equal to initialization value */
                PadIntStub padInt0A = Library.AccessPadInt(uid0);

                if(padInt0A.Read() == 21) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: commit was not successful...");
                }

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Simple commit end----------" });
        }

        public void TestMultipleRead(int uid0, int uid1, int uid2) {

            Console.WriteLine("------ Test: Multiple read ------");
            Logger.Log(new String[] { "Client", "------ Test: Multiple read ------" });

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);
                PadIntStub padInt1 = Library.CreatePadInt(uid1);
                Console.WriteLine("padInt1 created with uid: " + uid1);
                PadIntStub padInt2 = Library.CreatePadInt(uid2);
                Console.WriteLine("padInt2 created with uid: " + uid2);

                bool result = padInt0.Read() == 0;
                result = (padInt0.Read() == padInt1.Read());
                result = (padInt0.Read() == padInt2.Read());

                if(result) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: multiple read was not successful...");
                }

                /* to test with more than one client */
                while(!stopLoop)
                    ;

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------multiple read end----------" });
        }

        public void TestSampleApp() {
            Console.WriteLine("------ Test: Sample App ------");
            Logger.Log(new String[] { "Client", "------ Test: Sample App ------" });

            bool res;
            res = Library.TxBegin();
            PadIntStub pi_a = Library.CreatePadInt(0);
            PadIntStub pi_b = Library.CreatePadInt(1);
            res = Library.TxCommit();

            res = Library.TxBegin();
            pi_a = Library.AccessPadInt(0);
            pi_b = Library.AccessPadInt(1);
            pi_a.Write(36);
            pi_b.Write(37);
            Console.WriteLine("a = " + pi_a.Read());
            Console.WriteLine("b = " + pi_b.Read());
            Library.Status();

            // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
            /*res = PadiDstm.Freeze("tcp://localhost:2001/Server");
            res = PadiDstm.Recover("tcp://localhost:2001/Server");
            res = PadiDstm.Fail("tcp://localhost:2002/Server");*/

            res = Library.TxCommit();
            Logger.Log(new String[] { "---------Sample App end----------" });
        }

        public void TestReadWrite(int uid0) {

            Console.WriteLine("------ Test: Read write ------");
            Logger.Log(new String[] { "Client", "------ Test: Read write ------" });

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                //read
                Console.WriteLine("padInt0 read: " + padInt0.Read());

                //Library.txCommit();
                //Console.WriteLine("txCommit Done");

                // write
                Console.WriteLine("now I will do a write...");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                /* the padInt's value must be equal to initialization value */
                PadIntStub padInt0A = Library.AccessPadInt(uid0);

                if(padInt0.Write(211)) {
                    Console.WriteLine("padInt0 write done with value (211) : " + padInt0.Read());
                }

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Read write end----------" });
        }

        //-----------------------------------------------------
        //client 2
        public void TestSimpleReadClient2(int uid0) {

            Console.WriteLine("------ Test: client2 Simple read ------");
            Logger.Log(new String[] { "Client", "------ Test: client2 Simple read ------" });

            Library library = new Library();

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.AccessPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                Console.WriteLine("padInt0 read: " + padInt0.Read());

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Client2 Simple read end----------" });
        }

        public void TestSimpleWriteClient2(int uid0) {

            Console.WriteLine("------ Test: client2 Simple write ------");
            Logger.Log(new String[] { "Client", "------ Test: client2 Simple write ------" });

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.AccessPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                if(padInt0.Write(20)) {
                    Console.WriteLine("padInt0 write done with value (20) : " + padInt0.Read());
                }

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Client2 Simple write end----------" });
        }

        public void TestMultipleReadClient2(int uid0, int uid1, int uid2) {

            Console.WriteLine("------ Test: Client2 Multiple read ------");
            Logger.Log(new String[] { "Client", "------ Test: Client2 Multiple read ------" });

            Library library = new Library();
            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = Library.AccessPadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);
                PadIntStub padInt1 = Library.AccessPadInt(uid1);
                Console.WriteLine("padInt1 created with uid: " + uid1);
                PadIntStub padInt2 = Library.AccessPadInt(uid2);
                Console.WriteLine("padInt2 created with uid: " + uid2);

                bool result = padInt0.Read() == 0;
                result = (padInt0.Read() == padInt1.Read());
                result = (padInt0.Read() == padInt2.Read());

                if(result) {
                    Console.WriteLine("it's OK");
                } else {
                    Console.WriteLine("BUG!!!!!: multiple read was not successful...");
                }

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Client2 Multiple read end----------" });
        }

    }
}