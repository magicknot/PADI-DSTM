using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace LogServer {
    class Log : MarshalByRefObject, ILog, IDisposable {

        System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt");
        bool disposed = false;

        public void log(String[] logs) {

            String message = DateTime.Now + " ";

            foreach(String s in logs) {
                message += s + " ";
            }

            Console.WriteLine(message);
            file.WriteLine(message);
            file.Flush();

        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if(disposed)
                return;

            if(disposing) {
                file.Close();
            }

            disposed = true;
        }

    }


}
