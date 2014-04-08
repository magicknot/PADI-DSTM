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
            TcpChannel channel;

            if(Library.init()) {

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
                    Console.WriteLine("------------------");
                    Console.WriteLine("Tests with more than one client:");
                    Console.WriteLine(" (you need 3 clients: c1 create the padInts; C2 tries to access them; C3 stops the loop of C1)");
                    Console.WriteLine("13- YOU MUST DO THIS in c1 BEFORE TEST WITH MORE THAN ONE CLIENT (init)");
                    Console.WriteLine("7- YOU MUST DO THIS in c3 BEFORE TEST WITH MORE THAN ONE CLIENT (create the loops)");
                    Console.WriteLine("8- If you want that client1 stops the loop");
                    Console.WriteLine("12- If you want to confirm the value of stopLoop");
                    Console.WriteLine("---------");
                    Console.WriteLine("2- If you want that client1 do a simple read");
                    Console.WriteLine("3- If you want that client1 do a simple write");
                    Console.WriteLine("6- If you want that client1 do multiple read");
                    Console.WriteLine("9- If you want that client2 do a simple read");
                    Console.WriteLine("10- If you want that client2 do a simple write");
                    Console.WriteLine("11- If you want that client2 do a multiple read");

                    input = Console.ReadLine();

                    if(input.Equals("1")) {
                        client.testRandom();
                    }

                    if(input.Equals("2")) {
                        client.testSimpleRead(client.getNextUid());
                    }

                    if(input.Equals("3")) {
                        client.testSimpleWrite(client.getNextUid());
                    }

                    if(input.Equals("4")) {
                        client.testSimpleAbort(client.getNextUid());
                    }

                    if(input.Equals("5")) {
                        client.testSimpleCommit(client.getNextUid());
                    }

                    if(input.Equals("6")) {
                        client.testMultipleRead(client.getNextUid(), client.getNextUid(), client.getNextUid());
                    }

                    /* loop setup */
                    if(input.Equals("7")) {
                        //stopLoop = false;
                        client.setStopLoop(false);
                        Console.WriteLine("loop is activated stopLoop = " + client.StopLoop);
                    }

                    /* stops the loop */
                    if(input.Equals("8")) {
                        client.StopLoop = true;
                        Console.WriteLine("loop is deactivated stopLoop = " + client.StopLoop);
                    }

                    /* client2 simple read */
                    if(input.Equals("9")) {
                        client.testSimpleReadClient2(client.getNextUid());
                    }

                    /* client2 simple write */
                    if(input.Equals("10")) {
                        client.testSimpleWriteClient2(client.getNextUid());
                    }

                    /* client2 multiple read */
                    if(input.Equals("11")) {
                        client.testMultipleReadClient2(client.getNextUid(), client.getNextUid(), client.getNextUid());
                    }

                    /* value of stopLoop */
                    if(input.Equals("12")) {
                        Console.WriteLine("stopLoop = " + client.StopLoop);
                    }

                    /* init */
                    if(input.Equals("13")) {

                        try {
                            channel = new TcpChannel(6085);
                            ChannelServices.UnregisterChannel(Library.Channel);
                            ChannelServices.RegisterChannel(channel, true);
                            RemotingServices.Marshal(client, "Client",
                                typeof(Client)
                                );
                        } catch(ServerAlreadyExistsException e) {
                            Console.WriteLine(e.getMessage());
                        }
                        Console.WriteLine("init done");
                    }
                }

            } else {
                Logger.log(new String[] { "There are no servers available" });
            }
        }
    }
}
