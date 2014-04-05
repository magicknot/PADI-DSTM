using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace LogServer {
    class Log : MarshalByRefObject, ILog {

        public void log(String[] logs) {

            String message = DateTime.Now + " ";

            foreach(String s in logs) {
                message += s + " ";
            }

            Console.WriteLine(message,1000,1000);

        }


    }


}
