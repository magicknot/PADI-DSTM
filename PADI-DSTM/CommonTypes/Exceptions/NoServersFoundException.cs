using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    public class NoServersFoundException : IPadiException {

        public NoServersFoundException() {
        }

        public override string getMessage() {
            return "Master server doesn't have any server registered";
        }

        public String Message {
            get { return "Master server doesn't have any server registered"; }
        }
    }
}
