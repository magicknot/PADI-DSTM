using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client {
    class ClientSetup {

        private static ClientBase clientBase = new ClientBase();
        private static Client1 client1 = new Client1();
        private static Client2 client2 = new Client2();

        private static Random random = new Random();
        private static int nextUid = 0;

        public ClientSetup() {
            //this.clientBase = new ClientBase();
            //this.client1 = new Client1();
        }

        private static int getNextUid() {
            /* first arg of Next its the minimum and the second arg is the maximum */
            nextUid = random.Next(0, 100);
            return nextUid;
        }

        public static void testRandom() {

            Console.WriteLine("------Test random ------");
            //Logger.log(new String[] { "Client Base", "------Test random ------" });

            for(int i = 0; i < 20; i++)
                Console.WriteLine("number = " + getNextUid());
        }

        static void Main(string[] args) {
            Console.Title = "Client Setup";
            Console.WriteLine("Client Setup up and running..");

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
                Console.WriteLine("7- ALL BASE TESTS");
                Console.WriteLine("Tests with two clients (Base & Client1):");
                Console.WriteLine("8- Base & Client1: testMultipleWrite");

                input = Console.ReadLine();

                if(input.Equals("1")) {
                    testRandom();
                }

                if(input.Equals("2")) {
                    clientBase.testSimpleRead();
                }

                if(input.Equals("3")) {
                    clientBase.testSimpleWrite();
                }

                if(input.Equals("4")) {
                    clientBase.testSimpleAbort();
                }

                if(input.Equals("5")) {
                    clientBase.testSimpleCommit();
                }

                if(input.Equals("6")) {
                    clientBase.testMultipleRead();
                }

                if(input.Equals("7")) {
                    clientBase.testSimpleRead();
                    clientBase.testSimpleWrite();
                    clientBase.testSimpleAbort();
                    clientBase.testSimpleCommit();
                    clientBase.testMultipleRead();
                }
            }
        }
    }
}
