using System;
//using PADI_DSTM;
using ClientLibrary;

class TesteCache {
    static void Main(string[] args) {
        bool res;
        PadInt pi_a;
        Library.Init();

        //cria os padInts
        res = Library.TxBegin();
        pi_a = Library.CreatePadInt(1);
        res = Library.TxCommit();
        Console.WriteLine("####################################################################");
        Console.WriteLine("Criei uid: 1. commit = " + res + " . Press enter for next transaction.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();


        res = Library.TxBegin();
        //testa os reads
        if((args.Length > 0) && (args[0].Equals("R"))) {
            pi_a = Library.AccessPadInt(1);
            pi_a.Read();
            pi_a.Read();
            pi_a.Read();
        }

        //testa os writes
        if((args.Length > 0) && (args[0].Equals("W"))) {
            pi_a = Library.AccessPadInt(1);
            pi_a.Write(1);
            pi_a.Write(2);
            pi_a.Write(3);
        }

        res = Library.TxCommit();
        Console.WriteLine("####################################################################");
        Console.WriteLine("End: Fiz o commit = " + res + " . Press enter for verification transaction.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();
    }
}
