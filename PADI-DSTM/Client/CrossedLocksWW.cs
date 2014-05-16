using System;
//using PADI_DSTM;
using ClientLibrary;

class CrossedLocksWW {
    /*static void Main(string[] args) {
        bool res;
        PadInt pi_a;
        Library.Init();

        //cria os padInts
        if((args.Length > 0) && (args[0].Equals("C"))) {
            res = Library.TxBegin();
            pi_a = Library.CreatePadInt(1);
            res = Library.TxCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("Criei uid: 1. commit = " + res + " . Press enter para fazer os writes.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
        }

        res = Library.TxBegin();
        pi_a = Library.AccessPadInt(1);
        //faz writes
        for(int i = 0; i < 10; i++) {
            Console.WriteLine("####################################################################");
            Console.WriteLine("Fiz um Write no uid 1. uid() =" + pi_a.Write(i) + "Press enter para novo write.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
        }

        Console.WriteLine("####################################################################");
        Console.WriteLine("Press enter para commit.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();

        res = Library.TxCommit();
        Console.WriteLine("####################################################################");
        Console.WriteLine("Fiz o commit = " + res + " . Press enter for verification transaction.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();
    }*/
}
