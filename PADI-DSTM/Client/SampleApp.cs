using System;
//using PADI_DSTM;
using ClientLibrary;

class SampleApp {
    /*static void Main(string[] args) {
        bool res;

        Library.Init();

        res = Library.TxBegin();
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
        // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
        res = Library.Freeze("tcp://localhost:2001/Server");

        pi_a = Library.AccessPadInt(0);
        pi_b = Library.AccessPadInt(1);
        pi_a.Write(36);
        pi_b.Write(37);

        res = Library.Recover("tcp://localhost:2001/Server");

        //res = Library.Fail("tcp://localhost:2002/Server");
        res = Library.TxCommit();
    }*/
}