using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace LogServer {
    class Log : MarshalByRefObject, ILog {

        System.IO.StreamWriter file =  new System.IO.StreamWriter("log.txt");

        public void log(String[] logs) {

            String message = DateTime.Now + " ";

            foreach(String s in logs) {
                message += s + " ";
            }

            Console.WriteLine(message);
            file.WriteLine(message);
            file.Flush();
            //file.Close();

        }


    }


}
