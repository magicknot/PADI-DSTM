using System;
using System.Collections.Generic;
using System.Text;
//using PADI_DSTM;
using ClientLibrary;

class TenThousand {
    static void Main(string[] args) {
        bool res = false;
        int aborted = 0, committed = 0;

        PadiDstm.Init();
        try {
            if((args.Length > 0) && (args[0].Equals("C"))) {
                res = PadiDstm.TxBegin();
                for(int i = 0; i < 10001; i++) {
                    PadInt pi_a = PadiDstm.CreatePadInt(i);
                    pi_a.Write(i);
                }
                res = PadiDstm.TxCommit();
            }
            Console.WriteLine("####################################################################");
            Console.WriteLine("Finished creating PadInts. Press enter for sum transaction.");
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
        try {
            int sum = 0;
            PadInt pi_a;
            res = PadiDstm.TxBegin();
            for(int i = 0; i < 9999; i++) {
                pi_a = PadiDstm.AccessPadInt(i);
                sum += pi_a.Read();
            }
            Console.WriteLine("sum= " + sum);
            if(args[0].Equals("D1")) {
                pi_a = PadiDstm.AccessPadInt(10000);
                pi_a.Write(sum);
            }
            if(args[0].Equals("D2")) {
                pi_a = PadiDstm.AccessPadInt(10001);
                pi_a.Write(sum);
            }
            res = PadiDstm.TxCommit();
            if(res) { committed++; Console.Write("."); } else {
                aborted++;
                Console.WriteLine("$$$$$$$$$$$$$$ ABORT $$$$$$$$$$$$$$$$$");
            }
        } catch(Exception e) {
            Console.WriteLine("Exception: " + e.Message);
            Console.WriteLine("####################################################################");
            Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
            PadiDstm.TxAbort();
            aborted++;
        }


        Console.WriteLine("####################################################################");
        Console.WriteLine("committed = " + committed + " ; aborted = " + aborted);
        Console.WriteLine("Press enter for status.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();
        PadiDstm.Status();
        Console.WriteLine("####################################################################");
        Console.WriteLine("Press enter for verification transaction.");
        Console.WriteLine("####################################################################");

        try {
            res = PadiDstm.TxBegin();
            PadInt pi_g = PadiDstm.AccessPadInt(10000);
            int g = pi_g.Read();
            res = PadiDstm.TxCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("sum = " + g);
            Console.WriteLine("Status post verification transaction. Press enter for exit.");
            Console.WriteLine("####################################################################");
            PadiDstm.Status();
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
}
