using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client {
    [Serializable]
    class FailedConnectionException : System.Exception {
        private string p;

        public FailedConnectionException(string p) {
            // TODO: Complete member initialization
            this.p = p;
        }
    }
}
