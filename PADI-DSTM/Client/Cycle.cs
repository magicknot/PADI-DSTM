using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;

namespace Client {
    class Cycle {

        public static void Main(string[] args) {

            bool res;

            Library.init();

            // if((args.Length > 0) && (args[0].Equals("C"))) {
            res = Library.txBegin();
            PadIntStub pi_a = Library.createPadInt(2);
            PadIntStub pi_b = Library.createPadInt(2000000001);
            PadIntStub pi_c = Library.createPadInt(1000000000);
            pi_a.write(0);
            pi_b.write(0);
            res = Library.txCommit();
            //}
            Console.WriteLine("####################################################################");
            Console.WriteLine("Finished creating PadInts. Press enter for 300 R/W transaction cycle.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
            for(int i = 0; i < 300; i++) {
                res = Library.txBegin();
                PadIntStub pi_d = Library.accessPadInt(2);
                PadIntStub pi_e = Library.accessPadInt(2000000001);
                PadIntStub pi_f = Library.accessPadInt(1000000000);
                int d = pi_d.read();
                d++;
                pi_d.write(d);
                int e = pi_e.read();
                e++;
                pi_e.write(e);
                int f = pi_f.read();
                f++;
                pi_f.write(f);
                Console.Write(".");
                res = Library.txCommit();
                if(!res)
                    Console.WriteLine("$$$$$$$$$$$$$$ ABORT $$$$$$$$$$$$$$$$$");
            }
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status after cycle. Press enter for verification transaction.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
            res = Library.txBegin();
            PadIntStub pi_g = Library.accessPadInt(2);
            PadIntStub pi_h = Library.accessPadInt(2000000001);
            PadIntStub pi_j = Library.accessPadInt(1000000000);
            int g = pi_g.read();
            int h = pi_h.read();
            int j = pi_j.read();
            res = Library.txCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("2 = " + g);
            Console.WriteLine("2000000001 = " + h);
            Console.WriteLine("1000000000 = " + j);
            Console.WriteLine("Status post verification transaction. Press enter for exit.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
        }

    }
}
