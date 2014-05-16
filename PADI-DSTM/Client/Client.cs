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

        public Client() {
            random = new Random();
            nextUid = 0;
        }

        internal int NextUID {
            set { this.nextUid = value; }
            get { return nextUid; }

        }

        public int LastUid() {
            return nextUid;
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

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                Console.WriteLine("padInt0 read: " + padInt0.Read());

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

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
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
            Logger.Log(new String[] { "---------Simple write end----------" });
        }

        public void TestSimpleAbort(int uid0) {
            Console.WriteLine("------ Test: Simple Abort ------");
            Logger.Log(new String[] { "Client", "------ Test: Simple Abort ------" });

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
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

                PadInt padInt0A = Library.AccessPadInt(uid0);

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

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
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
                PadInt padInt0A = Library.AccessPadInt(uid0);

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

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);
                PadInt padInt1 = Library.CreatePadInt(uid1);
                Console.WriteLine("padInt1 created with uid: " + uid1);
                PadInt padInt2 = Library.CreatePadInt(uid2);
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
            Logger.Log(new String[] { "---------multiple read end----------" });
        }

        public void TestReadWrite(int uid0) {
            Console.WriteLine("------ Test: Read write ------");
            Logger.Log(new String[] { "Client", "------ Test: Read write ------" });

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                //read
                Console.WriteLine("padInt0 read: " + padInt0.Read());

                //Library.txCommit();
                //Console.WriteLine("txCommit Done");

                // write
                Console.WriteLine("now I will do a write...");

                //Library.TxBegin();
                //Console.WriteLine("txBegin Done");

                /* the padInt's value must be equal to initialization value */
                //PadInt padInt0A = Library.AccessPadInt(uid0);

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

        public void TestWriteRead(int uid0) {
            Console.WriteLine("------ Test: write read ------");
            Logger.Log(new String[] { "Client", "------ Test: write read ------" });

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);

                if(padInt0.Write(211)) {
                    Console.WriteLine("padInt0 write done with value (211) : " + padInt0.Read());
                }

                //read
                Console.WriteLine("padInt0 read: " + padInt0.Read());

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------write read end----------" });
        }

        public void TestFreezeCreate(int uid0) {
            Console.WriteLine("------ Test: Freeze Create ------");
            Logger.Log(new String[] { "Client", "------ Test: Freeze Create ------" });

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);


                Console.WriteLine("####################################################################");
                Console.WriteLine("Vou fazer o primeiro ciclo de 5 writes. Depois vem o freeze");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                int i = 0;
                for(; i <= 5; i++) {
                    Console.WriteLine("Fiz um Write no uid 1. uid() = " + padInt0.Write(i).ToString());

                }

                Console.WriteLine("####################################################################");
                Console.WriteLine("Vou fazer o freeze Nota: Podem ser imprimidas writas que estao a ser feitas em cache! so vai parar quando fizer pedido ao server");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                // The following 2 lines assume we have 2 servers: one at port 2001 and another at port 2002
                bool res = Library.Freeze("tcp://localhost:2001/PadIntServer");
                //res = Library.Fail("tcp://localhost:2002/PadIntServer");
                Console.WriteLine("####################################################################");
                Console.WriteLine("Fiz o freeze res= " + res + "Vou fazer o segundo ciclo de 5 writes. Depois vem o commit");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                for(; i <= 10; i++) {
                    Console.WriteLine("Fiz um Write no uid 1. uid() = " + padInt0.Write(i).ToString());
                }

                Console.WriteLine("####################################################################");
                Console.WriteLine("Fiz os 10 writes. (valor deve ser 10) padInt0.Read() =" + padInt0.Read() + "Press enter para commit.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Read Freeze end----------" });
        }

        public void TestFailCreate(int uid0) {
            Console.WriteLine("------ Test: Fail Create ------");
            Logger.Log(new String[] { "Client", "------ Test: Fail Create ------" });

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);


                Console.WriteLine("####################################################################");
                Console.WriteLine("Vou fazer o primeiro ciclo de 5 writes. Depois vem o fail");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                int i = 0;
                for(; i <= 5; i++) {
                    Console.WriteLine("Fiz um Write no uid 1. uid() = " + padInt0.Write(i).ToString());

                }

                Console.WriteLine("####################################################################");
                Console.WriteLine("Vou fazer o fail");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                // The following 2 lines assume we have 2 servers: one at port 2001 and another at port 2002
                //bool res = Library.Freeze("tcp://localhost:2001/PadIntServer");
                bool res = Library.Fail("tcp://localhost:2001/PadIntServer");
                Console.WriteLine("####################################################################");
                Console.WriteLine("Fiz o fail res= " + res + "Vou fazer o segundo ciclo de 5 writes. Depois vem o commit");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                for(; i <= 10; i++) {
                    Console.WriteLine("Fiz um Write no uid 1. uid() = " + padInt0.Write(i).ToString());
                }

                Console.WriteLine("####################################################################");
                Console.WriteLine("Fiz os 10 writes. (valor deve ser 10) padInt0.Read() =" + padInt0.Read() + "Press enter para commit.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Read Fail end----------" });
        }

        public void TestFreezeFail(int uid0) {
            Console.WriteLine("------ Test: Freeze (o cliente1 tem que ja ter feito o freeze/fail) ------");
            Logger.Log(new String[] { "Client", "------ Test: Freeze/Fail ------" });

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                PadInt padInt0 = Library.CreatePadInt(uid0);
                Console.WriteLine("padInt0 created with uid: " + uid0);


                Console.WriteLine("####################################################################");
                Console.WriteLine("Vou fazer ciclo de 10 writes.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                for(int i = 0; i <= 10; i++) {
                    Console.WriteLine("Fiz um Write no uid 2. uid() = " + padInt0.Write(i).ToString());
                }

                Console.WriteLine("####################################################################");
                Console.WriteLine("Fiz os 10 writes. (valor deve ser 10) padInt0.Read() =" + padInt0.Read() + "Press enter para commit.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Read Freeze end----------" });
        }

        public void TestRecover() {
            Console.WriteLine("------ Test: Recover ------");
            Logger.Log(new String[] { "Client", "------ Test: Recover ------" });

            Console.WriteLine("library created");

            try {
                Console.WriteLine("init() Done");

                Library.TxBegin();
                Console.WriteLine("txBegin Done");

                // The following 2 lines assume we have 2 servers: one at port 2001 and another at port 2002
                bool res = Library.Recover("tcp://localhost:2001/PadIntServer");
                Console.WriteLine("####################################################################");
                Console.WriteLine("Fiz o recover res= " + res + " Depois vem o commit");
                Console.WriteLine("####################################################################");
                Console.ReadLine();

                Library.TxCommit();
                Console.WriteLine("txCommit Done");

                Console.WriteLine("closeChannel Done");
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("------------");
            Logger.Log(new String[] { "---------Read Freeze end----------" });
        }
    }
}