using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;

namespace Client {
    class Client {
        static void Main(string[] args) {
            Console.WriteLine("Client up and running..");

            Library library = new Library();
            if(library.init()) {
                library.txBegin();
                for(int i = 0; i<40; i++) {
                    library.createPadInt(i);
                }

            } else {
                library.log(new String[] { "There are no servers available" });
            }

            while(true)
                ;

        }
    }
}
