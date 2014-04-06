using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer {
    class NoServersFoundException : IPadiException {

        public NoServersFoundException() {
        }

        public override string getMessage() {
            return "Master server doesn't have any server registered";
        }

    }
}
