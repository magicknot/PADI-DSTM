using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;

namespace Client {
    class OldCycle {

        /*public static void Main(string[] args) {

            bool res;

            Library.Init();

            if((args.Length > 0) && (args[0].Equals("C"))) {
            res = Library.TxBegin();
            PadInt pi_a = Library.CreatePadInt(2);
            PadInt pi_b = Library.CreatePadInt(2000000001);
            PadInt pi_c = Library.CreatePadInt(1000000000);
            pi_a.Write(0);
            pi_b.Write(0);
            res = Library.TxCommit();
            }
            Console.WriteLine("####################################################################");
            Console.WriteLine("Finished creating PadInts. Press enter for 300 R/W transaction cycle.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
            for(int i = 0; i < 300; i++) {
                res = Library.TxBegin();
                PadInt pi_d = Library.AccessPadInt(2);
                PadInt pi_e = Library.AccessPadInt(2000000001);
                PadInt pi_f = Library.AccessPadInt(1000000000);
                int d = pi_d.Read();
                d++;
                pi_d.Write(d);
                int e = pi_e.Read();
                e++;
                pi_e.Write(e);
                int f = pi_f.Read();
                f++;
                pi_f.Write(f);
                Console.Write(".");
                res = Library.TxCommit();
                if(!res)
                    Console.WriteLine("$$$$$$$$$$$$$$ ABORT $$$$$$$$$$$$$$$$$");
            }
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status after cycle. Press enter for verification transaction.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
            res = Library.TxBegin();
            PadInt pi_g = Library.AccessPadInt(2);
            PadInt pi_h = Library.AccessPadInt(2000000001);
            PadInt pi_j = Library.AccessPadInt(1000000000);
            int g = pi_g.Read();
            int h = pi_h.Read();
            int j = pi_j.Read();
            res = Library.TxCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("2 = " + g);
            Console.WriteLine("2000000001 = " + h);
            Console.WriteLine("1000000000 = " + j);
            Console.WriteLine("Status post verification transaction. Press enter for exit.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
        }*/
    }
}
