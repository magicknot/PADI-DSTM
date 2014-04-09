using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace LogServer {
    /// <summary>
    /// This is the server used to log DSTM messages
    /// </summary>
    class Log : MarshalByRefObject, ILog, IDisposable {

        /// <summary>
        /// File where messages are logged
        /// </summary>
        System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt");
        /// <summary>
        /// Predicate that defines the moment for the stream to be closed
        /// </summary>
        bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logs">Log message arguments</param>
        public void log(String[] logs) {

            String message = DateTime.Now + " ";

            foreach(String s in logs) {
                message += s + " ";
            }

            Console.WriteLine(message);
            file.WriteLine(message);
            file.Flush();

        }

        /// <summary>
        /// Closes the log file stream
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes the log file stream
        /// </summary>
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
