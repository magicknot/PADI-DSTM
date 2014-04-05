using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadIntServer {
    class PadIntNotFoundException : Exception {
        private int uid;

        public PadIntNotFoundException(int uid) {
            this.uid = uid;
        }
    }
}
