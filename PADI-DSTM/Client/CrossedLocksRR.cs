using System;
//using PADI_DSTM;
using ClientLibrary;

class CrossedLocksRR {
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
            Console.WriteLine("Criei uid: 1. commit = " + res + " . Press enter para fazer os reads.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
        }

        res = Library.TxBegin();
        pi_a = Library.AccessPadInt(1);
        //faz read
        for(int i = 0; i < 10; i++) {
            Console.WriteLine("####################################################################");
            Console.WriteLine("Fiz um Read no uid 1. uid(1) =" + pi_a.Read() + "Press enter para novo read.");
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
