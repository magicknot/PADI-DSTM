using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommonTypes;

namespace ClientLibrary {
    public class PadInt {

        /// <summary>
        /// Identifier of remote PadInt
        /// </summary>
        private int uid;
        /// <summary>
        /// Address of server where remote PadInt is stored
        /// </summary>
        private string address;
        /// <summary>
        /// Identifier of transaction
        /// </summary>
        private int tid;
        /// <summary>
        /// Identifer of server where remote PadInt is stored
        /// </summary>
        private int serverID;
        /// <summary>
        /// Reference to the PadInt's cache
        /// </summary>
        private ClientCache cache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uid">PadInt identifier</param>
        /// <param name="tid">Transaction identifier</param>
        /// <param name="serverID">Server identifier</param>
        /// <param name="address">Server address</param>
        internal PadInt(int uid, int tid, int serverID, string address, ClientCache cache) {
            this.uid = uid;
            this.tid = tid;
            this.address = address;
            this.serverID = serverID;
            this.cache = cache;
        }

        /// <summary>
        /// Reads the value of remote PadInt
        /// </summary>
        /// <returns>The value of the remote PadInt</returns>
        public int Read() {
            Logger.Log(new String[] { "PadIntStub", "read" });

            try {
                int result;
                if(cache.HasWritePadInt(serverID, uid) || cache.HasReadPadInt(serverID, uid)) {
                    result = cache.GetPadIntValue(serverID, uid);
                    cache.UpdatePadIntValue(serverID, uid, result);
                } else {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
                    result = server.ReadPadInt(tid, uid);
                }
                cache.setPadIntAsRead(serverID, uid);
                return result;
            } catch(PadIntNotFoundException) {
                try {
                    cache.UpdatePadIntServer(serverID, uid);
                    return Read();
                } catch(PadIntNotFoundException) {
                    throw;
                }
            } catch(WrongPadIntRequestException) {
                throw;
            } catch(AbortException) {
                Console.WriteLine("Abort Exception: cannot obtain the read lock.");
                PadiDstm.TxAbort();
                throw;
            } catch(SocketException) {
                try {
                    cache.UpdatePadIntServer(serverID, uid);
                    return Read();
                } catch(PadIntNotFoundException) {
                    throw;
                }
            }
        }

        /// <summary>
        /// Writes the value on remote PadInt
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        public bool Write(int value) {
            Logger.Log(new String[] { "PadIntStub", "write" + "value" + value.ToString() });

            try {
                if(!cache.HasWritePadInt(serverID, uid)) {
                    IServer server = (IServer) Activator.GetObject(typeof(IServer), address);
                    server.WritePadInt(tid, uid, value);
                    cache.setPadIntAsWrite(serverID, uid);
                }

                cache.UpdatePadIntValue(serverID, uid, value);
                return true;
            } catch(PadIntNotFoundException) {
                try {
                    cache.UpdatePadIntServer(serverID, uid);
                    return Write(value);
                } catch(PadIntNotFoundException) {
                    throw;
                }
            } catch(WrongPadIntRequestException) {
                throw;
            } catch(AbortException) {
                Console.WriteLine("Abort Exception: cannot obtain the write lock.");
                PadiDstm.TxAbort();
                throw;
            } catch(SocketException) {
                try {
                    cache.UpdatePadIntServer(serverID, uid);
                    return Write(value);
                } catch(PadIntNotFoundException) {
                    throw;
                }
            }
        }
    }
}
