using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {

    /// <summary>
    /// Remote PadInt server interface
    /// </summary>
    public interface IServer {
        void createPrimaryServer(string backupAddress);
        void createBackupServer(string primaryAddress);
        bool createPadInt(int uid);
        bool confirmPadInt(int uid);
        int readPadInt(int tid, int uid);
        bool writePadInt(int tid, int uid, int value);
        bool commit(int tid, List<int> usedPadInts);
        bool abort(int tid, List<int> usedPadInts);
        bool Dump();
        bool Freeze();
        bool Fail();
        bool Recover();
    }

    /// <summary>
    /// Remote Master server interface
    /// </summary>
    public interface IMaster {
        int getNextTID();
        int registerServer(String address);
        Tuple<int, string> getPadIntServer(int uid);
        Tuple<int, string> registerPadInt(int uid);
        bool Status();
    }

    /// <summary>
    /// Remote Log server interface
    /// </summary>
    public interface ILog {
        void log(String[] logs);
    }

    /// <summary>
    /// Remote PadInt Exception interface
    /// </summary>
    [Serializable]
    public abstract class IPadiException : System.Exception {
        public abstract string getMessage();

        public IPadiException() {
        }

        public IPadiException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }
    }

    /// <summary>
    /// Logger class which redirects log messages to LogServer
    /// </summary>
    public static class Logger {

        /// <summary>
        /// The log message
        /// </summary>
        private static string message = DateTime.Now + " ";
        /// <summary>
        /// Predicate that defines if debug mode is one
        /// </summary>
        private static bool debugOn = true;
        /// <summary>
        /// Predicate that defines where the log messages are printed
        /// </summary>
        private static bool isLocal = true;

        /// <summary>
        /// Redirects the log message according to predicates
        /// </summary>
        /// <param name="args">The log message arguments</param>
        public static void log(String[] args) {
            message = "";
            if(debugOn) {
                if(isLocal) {
                    foreach(String s in args) {
                        message += s + " ";
                    }
                    Console.WriteLine(message);
                } else {
                    ILog logServer = (ILog) Activator.GetObject(typeof(ILog), "tcp://localhost:7002/LogServer");
                    logServer.log(args);
                }
            }
        }
    }
}
