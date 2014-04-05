using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace PadIntServer {
    class PadIntNotFoundException : Exception, IPadiException {
        private int uid;

        public PadIntNotFoundException(int uid) {
            this.uid = uid;
        }

        public int getUid() {
            return uid;
        }
    }
}
