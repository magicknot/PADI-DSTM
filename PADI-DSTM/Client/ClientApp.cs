using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using ClientLibrary;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace Client {
    class ClientApp {

        public static void Main(string[] args) {

            Client client = new Client();

            Console.Title = "Client";
            Console.WriteLine("Client up and running..");

            if(Library.Init()) {

                string input;

                while(true) {
                    Console.WriteLine("----------------------HELP-------------------------");
                    Console.WriteLine("Please, insert the number of the test that you want to run:");
                    Console.WriteLine("Tests with just one client");
                    Console.WriteLine("1- testRandom");
                    Console.WriteLine("Tests with one client:");
                    Console.WriteLine("2- Base: testSimpleRead");
                    Console.WriteLine("3- Base: testSimpleWrite");
                    Console.WriteLine("4- Base: testSimpleAbort");
                    Console.WriteLine("5- Base: testSimpleCommit");
                    Console.WriteLine("6- Base: testMultipleReads");
                    Console.WriteLine("7- Base: testReadWrite");
                    Console.WriteLine("8- Base: testWriteRead");
                    Console.WriteLine("9- Base: testFreezeCreate (client1)");
                    Console.WriteLine("10- Base: testFreeze (client2)");
                    Console.WriteLine("11- Base: testRecover");
                    Console.WriteLine("12- status");
                    Console.WriteLine("---------");

                    Console.Write(">");

                    input = Console.ReadLine();

                    if(input.Equals("1")) {
                        client.TestRandom();
                    }

                    if(input.Equals("2")) {
                        client.TestSimpleRead(client.GetNextUid());
                    }

                    if(input.Equals("3")) {
                        client.TestSimpleWrite(client.GetNextUid());
                    }

                    if(input.Equals("4")) {
                        client.TestSimpleAbort(client.GetNextUid());
                    }

                    if(input.Equals("5")) {
                        client.TestSimpleCommit(client.GetNextUid());
                    }

                    if(input.Equals("6")) {
                        client.TestMultipleRead(client.GetNextUid(), client.GetNextUid(), client.GetNextUid());
                    }

                    if(input.Equals("7")) {
                        client.TestReadWrite(client.GetNextUid());
                    }

                    if(input.Equals("8")) {
                        client.TestWriteRead(client.GetNextUid());
                    }

                    if(input.Equals("9")) {
                        client.TestFreezeCreate(1);
                    }

                    if(input.Equals("10")) {
                        client.TestFreeze(2);
                    }

                    if(input.Equals("11")) {
                        client.TestRecover();
                    }


                    if(input.Equals("12")) {
                        Library.Status();
                    }
                }

            } else {
                Logger.Log(new String[] { "There are no servers available" });
            }
        }
    }
}
