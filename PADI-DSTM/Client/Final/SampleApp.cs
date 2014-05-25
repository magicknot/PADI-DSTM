using System;
//using PADI_DSTM;
using ClientLibrary;

class SampleApp {
    /*static void Main(string[] args) {
        bool res = false;
        PadInt pi_a, pi_b;
        PadiDstm.Init();

        // Create 2 PadInts
        if((args.Length > 0) && (args[0].Equals("C"))) {
            try {
                res = PadiDstm.TxBegin();
                pi_a = PadiDstm.CreatePadInt(1);
                pi_b = PadiDstm.CreatePadInt(2000000000);
                Console.WriteLine("####################################################################");
                Console.WriteLine("BEFORE create commit. Press enter for commit.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.ReadLine();
                res = PadiDstm.TxCommit();
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER create commit returned " + res + " . Press enter for next transaction.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
            } catch(Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for next transaction.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                PadiDstm.TxAbort();
            }
        }

        try {
            res = PadiDstm.TxBegin();
            pi_a = PadiDstm.AccessPadInt(1);
            pi_b = PadiDstm.AccessPadInt(2000000000);
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status after AccessPadint");
            Console.WriteLine("####################################################################");
            PadiDstm.Status();
            if((args.Length > 0) && ((args[0].Equals("C")) || (args[0].Equals("A")))) {
                pi_a.Write(11);
                pi_b.Write(12);
            } else {
                pi_a.Write(21);
                pi_b.Write(22);
            }
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status after write. Press enter for read.");
            Console.WriteLine("####################################################################");
            PadiDstm.Status();
            Console.WriteLine("1 = " + pi_a.Read());
            Console.WriteLine("2000000000 = " + pi_b.Read());
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status after read. Press enter for commit.");
            Console.WriteLine("####################################################################");
            PadiDstm.Status();
            Console.ReadLine();
            res = PadiDstm.TxCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status after commit. commit = " + res + "Press enter for verification transaction.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
        } catch(Exception e) {
            Console.WriteLine("Exception: " + e.Message);
            Console.WriteLine("####################################################################");
            Console.WriteLine("AFTER r/w ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
            PadiDstm.TxAbort();
        }

        try {
            res = PadiDstm.TxBegin();
            PadInt pi_c = PadiDstm.AccessPadInt(1);
            PadInt pi_d = PadiDstm.AccessPadInt(2000000000);
            Console.WriteLine("####################################################################");
            Console.WriteLine("1 = " + pi_c.Read());
            Console.WriteLine("2000000000 = " + pi_d.Read());
            Console.WriteLine("Status after verification read. Press enter for commit and exit.");
            Console.WriteLine("####################################################################");
            PadiDstm.Status();
            Console.ReadLine();
            res = PadiDstm.TxCommit();
        } catch(Exception e) {
            Console.WriteLine("Exception: " + e.Message);
            Console.WriteLine("####################################################################");
            Console.WriteLine("AFTER verification ABORT. Commit returned " + res + " . Press enter for abort and exit.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
            PadiDstm.TxAbort();
        }
    }*/
}
