using System;
//using PADI_DSTM;
using ClientLibrary;

class OldSampleApp {
    /*static void Main(string[] args) {
        bool res;

        Library.Init();
        res = Library.TxBegin();

        if((args.Length > 0) && (args[0].Equals("F"))) {
            PadInt pi_a = Library.CreatePadInt(0);
            PadInt pi_b = Library.CreatePadInt(1);
            res = Library.TxCommit();

            res = Library.TxBegin();
            pi_a = Library.AccessPadInt(0);
            pi_b = Library.AccessPadInt(1);
            pi_a.Write(36);
            pi_b.Write(37);
            Console.WriteLine("a = " + pi_a.Read());
            Console.WriteLine("b = " + pi_b.Read());
            Library.Status();
            // The following 2 lines assume we have 2 servers: one at port 2001 and another at port 2002
            res = Library.Freeze("tcp://localhost:2001/PadIntServer");
            //res = Library.Fail("tcp://localhost:2002/PadIntServer");

            pi_a = Library.AccessPadInt(0);
            pi_b = Library.AccessPadInt(1);
            pi_a.Write(36);
            pi_b.Write(37);
        } else {
            res = Library.Recover("tcp://localhost:2001/PadIntServer");
        }

        res = Library.TxCommit();
    }*/
}