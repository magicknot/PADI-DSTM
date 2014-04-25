﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace PadIntServer {
    class FailServer : ServerState {

        internal FailServer(Server server)
            : base(server) {
            // Nothing to do here
        }

        internal override bool createPadInt(int uid) {
            Logger.log(new String[] { "FailServer", Server.ID.ToString(), "createPadInt", "uid ", uid.ToString() });

            //TODO
            throw new NotImplementedException();
        }

        internal override bool confirmPadInt(int uid) {
            Logger.log(new String[] { "FailServer", Server.ID.ToString(), "confirmPadInt ", "uid", uid.ToString() });

            //TODO
            throw new NotImplementedException();
        }

        /* Returns the value of the PadInt when the transaction
         *  has the read/write lock.
         * Throw an exception if PadInt not found. 
         */
        internal override int readPadInt(int tid, int uid) {
            Logger.log(new String[] { "FailServer", Server.ID.ToString(), "readPadInt ", "tid", tid.ToString(), "uid", uid.ToString() });

            //TODO
            throw new NotImplementedException();
        }

        internal override bool writePadInt(int tid, int uid, int value) {
            Logger.log(new String[] { "Server ", Server.ID.ToString(), " writePadInt ", "tid", tid.ToString(), "uid", uid.ToString(), "value", value.ToString() });

            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Commits a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool commit(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "FailServer", Server.ID.ToString(), "commit", "tid", tid.ToString() });
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */

            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Aborts a transaction on this server
        /// </summary>
        /// <param name="tid">transaction identifier</param>
        /// <param name="usedPadInts">Identifiers of PadInts involved</param>
        /// <returns>A predicate confirming the sucess of the operations</returns>
        internal override bool abort(int tid, List<int> usedPadInts) {
            Logger.log(new String[] { "FailServer", Server.ID.ToString(), "abort", "tid", tid.ToString() });
            /* TODO !!!!!
             * 
             * se por acaso usarmos o tab no cliente para guardar valores para
             *  evitar andar a fazer varias chamadas remotas se calhar mete-se
             *  no cliente que ao chamar o commit (do cliente) esse metodo chama
             *  primeiro os writes para todos os PadInt que escreveu para assim
             *  actualizar no server.
             */
            //TODO
            throw new NotImplementedException();
        }
    }
}
