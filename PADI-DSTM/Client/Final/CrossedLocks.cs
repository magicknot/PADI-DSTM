using System;
//using PADI_DSTM;
using ClientLibrary;

class CrossedLocks {
    /*static void Main(string[] args) {
        bool res = false;
        PadInt pi_a, pi_b;
        PadiDstm.Init();

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
                Console.WriteLine("AFTER create commit. commit = " + res + " . Press enter for next transaction.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
            } catch(Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                PadiDstm.TxAbort();
            }

        }

        try {
            res = PadiDstm.TxBegin();
            if((args.Length > 0) && ((args[0].Equals("A")) || (args[0].Equals("C")))) {
                pi_b = PadiDstm.AccessPadInt(2000000000);
                pi_b.Write(211);
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status post first op: write. Press enter for second op.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.ReadLine();
                pi_a = PadiDstm.AccessPadInt(1);
                //pi_a.Write(212);
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status post second op: read. uid(1)= " + pi_a.Read() + ". Press enter for commit.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.ReadLine();
            } else {
                pi_a = PadiDstm.AccessPadInt(1);
                pi_a.Write(221);
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status post first op: write. Press enter for second op.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.ReadLine();
                pi_b = PadiDstm.AccessPadInt(2000000000);
                //pi_b.Write(222);
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status post second op: read. uid(1)= " + pi_b.Read() + ". Press enter for commit.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.ReadLine();
            }
            res = PadiDstm.TxCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("commit = " + res + " . Press enter for verification transaction.");
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
            Console.WriteLine("0 = " + pi_c.Read());
            Console.WriteLine("2000000000 = " + pi_d.Read());
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status after verification read. Press enter for verification commit.");
            Console.WriteLine("####################################################################");
            PadiDstm.Status();
            res = PadiDstm.TxCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("commit = " + res + " . Press enter for exit.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
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
