using System;
//using PADI_DSTM;
using ClientLibrary;

class CrossedLocks {
    /*static void Main(string[] args) {
        bool res;
        PadIntStub pi_a, pi_b;
        Library.init();

        if((args.Length > 0) && (args[0].Equals("C"))) {
            res = Library.txBegin();
            pi_a = Library.createPadInt(1);
            pi_b = Library.createPadInt(2000000000);
            Console.WriteLine("####################################################################");
            Console.WriteLine("BEFORE create commit. Press enter for commit.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
            res = Library.txCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("AFTER create commit. commit = " + res + " . Press enter for next transaction.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
        }

        res = Library.txBegin();
        if((args.Length > 0) && ((args[0].Equals("A")) || (args[0].Equals("C")))) {
            pi_b = Library.accessPadInt(2000000000);
            pi_b.write(211);
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status post first op: write. Press enter for second op.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
            pi_a = Library.accessPadInt(1);
            //pi_a.Write(212);
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status post second op: read. uid(1)= " + pi_a.read() + ". Press enter for commit.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
        } else {
            pi_a = Library.accessPadInt(1);
            pi_a.write(221);
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status post first op: write. Press enter for second op.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
            pi_b = Library.accessPadInt(2000000000);
            //pi_b.Write(222);
            Console.WriteLine("####################################################################");
            Console.WriteLine("Status post second op: read. uid(1)= " + pi_b.read() + ". Press enter for commit.");
            Console.WriteLine("####################################################################");
            Library.Status();
            Console.ReadLine();
        }
        res = Library.txCommit();
        Console.WriteLine("####################################################################");
        Console.WriteLine("commit = " + res + " . Press enter for verification transaction.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();
        res = Library.txBegin();
        PadIntStub pi_c = Library.accessPadInt(1);
        PadIntStub pi_d = Library.accessPadInt(2000000000);
        Console.WriteLine("0 = " + pi_c.read());
        Console.WriteLine("2000000000 = " + pi_d.read());
        Console.WriteLine("####################################################################");
        Console.WriteLine("Status after verification read. Press enter for verification commit.");
        Console.WriteLine("####################################################################");
        Library.Status();
        res = Library.txCommit();
        Console.WriteLine("####################################################################");
        Console.WriteLine("commit = " + res + " . Press enter for exit.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();
    }*/
}
