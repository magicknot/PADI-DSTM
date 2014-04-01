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
                library.createPadInt(1);
            } throw new FailedConnectionException("Master");


        }
    }
}
