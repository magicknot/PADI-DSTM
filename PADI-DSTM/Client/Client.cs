using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;

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

        public static void test1() {

            Console.WriteLine("------Test1 Begin------");

            Library library = new Library();

            Console.WriteLine("library created");

            if(library.init()) {
                library.txBegin();

                Console.WriteLine("txBegin Done");

                PadIntStub padInt0 = library.createPadInt(uid0);
                PadIntStub padInt1 = library.createPadInt(uid1);

                Console.WriteLine("padInts created");

                Console.WriteLine("padInt0 read: " + padInt0.read());

                //tirar comentado quando estiver feito
                //library.txCommit();

                Console.WriteLine("txBegin Done");


            } else {
                library.log(new String[] { "There are no servers available" });
            }

        }

        static void Main(string[] args) {
            Console.Title = "Client";
            Console.WriteLine("Client up and running..");

            test1();

            while(true)
                ;
        }
    }
}
