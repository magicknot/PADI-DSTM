using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace CommonTypes {
    /// <summary>
    /// This exception is thrown when a server is not registered on master server
    /// </summary>
    [Serializable]
    public class ServerNotFoundException : IPadiException {

        /// <summary>
        /// constant used to represent non atributed server identifier
        /// </summary>
        private const int NO_SERVER_ID = -1;
        /// <summary>
        /// constant used to represent non atributed server address
        /// </summary>
        private const string NO_SERVER_ADDRESS = "";
        /// <summary>
        /// Server identifier
        /// </summary>
        private int serverID;
        /// <summary>
        /// Server address
        /// </summary>
        private string serverAddress;

        /// <summary>
        /// Constructor
        /// </summary>
        public ServerNotFoundException(int id) {
            serverID = id;
            serverAddress = NO_SERVER_ADDRESS;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ServerNotFoundException(string address) {
            serverID = NO_SERVER_ID;
            serverAddress = address;
        }

        /// <summary>
        /// Default exception construtor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ServerNotFoundException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }

        /// <summary>
        /// Default exception constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Returns exception message
        /// </summary>
        /// <returns>message</returns>
        public override string getMessage() {
            if(serverID != NO_SERVER_ID) {
                return "Master server doesn't have server with identifier " + serverID + " registered";
            } else {
                return "Master server doesn't have server with address " + serverAddress + " registered";
            }
        }
    }
}
