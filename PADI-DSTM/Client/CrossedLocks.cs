using System;
//using PADI_DSTM;
using ClientLibrary;

class CrossedLocks {
    /*static void Main(string[] args) {
        bool res;
        PadInt pi_a, pi_b;
        Library.Init();

        if((args.Length > 0) && (args[0].Equals("C"))) {
            res = Library.TxBegin();
            pi_a = Library.CreatePadInt(1);
            pi_b = Library.CreatePadInt(2000000000);
            Console.WriteLine("####################################################################");
            Console.WriteLine("BEFORE create commit. Press enter for commit.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
            res = Library.TxCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("AFTER create commit. commit = " + res + " . Press enter for next transaction.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
        }

        res = Library.TxBegin();
        if((args.Length > 0) && ((args[0].Equals("A")) || (args[0].Equals("C")))) {
            pi_b = Library.AccessPadInt(2000000000);
            pi_b.Write(211);
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status post first op: write. Press enter for second op.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
            pi_a = Library.AccessPadInt(1);
            //pi_a.Write(212);
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status post second op: read. uid(1)= " + pi_a.Read() + ". Press enter for commit.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
        } else {
            pi_a = Library.AccessPadInt(1);
            pi_a.Write(221);
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status post first op: write. Press enter for second op.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
            pi_b = Library.AccessPadInt(2000000000);
            //pi_b.Write(222);
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status post second op: read. uid(1)= " + pi_b.Read() + ". Press enter for commit.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
        }
        res = Library.TxCommit();
        Console.WriteLine("####################################################################");
        Console.WriteLine("commit = " + res + " . Press enter for verification transaction.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();
        res = Library.TxBegin();
        PadInt pi_c = Library.AccessPadInt(1);
        PadInt pi_d = Library.AccessPadInt(2000000000);
        Console.WriteLine("0 = " + pi_c.Read());
        Console.WriteLine("2000000000 = " + pi_d.Read());
        Console.WriteLine("####################################################################");
        Console.WriteLine("Status after verification read. Press enter for verification commit.");
        Console.WriteLine("####################################################################");
        Library.Status();
        res = Library.TxCommit();
        Console.WriteLine("####################################################################");
        Console.WriteLine("commit = " + res + " . Press enter for exit.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();
    }*/
}
