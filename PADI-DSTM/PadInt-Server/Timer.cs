using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadIntServer {
    class PadIntTimer : IDisposable {


        internal System.Timers.Timer timer;

        /// <summary>
        /// Predicate that defines the moment for the stream to be closed
        /// </summary>
        bool disposed = false;

        public PadIntTimer(double interval) {
            timer = new System.Timers.Timer(interval);
        }

        public System.Timers.Timer Timer {
            get { return timer; }
            set { timer = value; }
        }

        public void Stop() {
            timer.Stop();
        }

        public void Start() {
            timer.Start();
        }

        public void Close() {
            timer.Close();
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
        public virtual void Dispose(bool disposing) {
            if(disposed)
                return;

            if(disposing) {
                timer.Dispose();
            }

            disposed = true;
        }
    }
}
