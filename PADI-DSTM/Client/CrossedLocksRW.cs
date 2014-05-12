using System;
//using PADI_DSTM;
using ClientLibrary;

class CrossedLocksRW {
    static void Mainaa(string[] args) {
        bool res;
        PadInt pi_a;
        Library.Init();

        //cria os padInts
        if((args.Length > 0) && (args[0].Equals("C"))) {
            res = Library.TxBegin();
            pi_a = Library.CreatePadInt(1);
            res = Library.TxCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("Criei uid: 1. commit = " + res + " . Press enter for next transaction.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
        }

        res = Library.TxBegin();
        //o que criou faz read
        if((args.Length > 0) && (args[0].Equals("C"))) {
            pi_a = Library.AccessPadInt(1);
            Console.WriteLine("####################################################################");
            Console.WriteLine("C: Aqui Read no uid 1. uid(1) =" + pi_a.Read() + "Press enter para commit.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
        }

        //o que acede faz write
        if((args.Length > 0) && (args[0].Equals("A"))) {
            pi_a = Library.AccessPadInt(1);
            pi_a.Write(20);
            Console.WriteLine("####################################################################");
            Console.WriteLine("A: Fiz write no uid 1 com valor 20. Agora fiz read para ler o valor uid(1) =" + pi_a.Read());
            Console.WriteLine("Press enter para commit.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
        }

        res = Library.TxCommit();
        Console.WriteLine("####################################################################");
        Console.WriteLine("Fiz o commit = " + res + " . Press enter for verification transaction.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();
    }
}
