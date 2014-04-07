﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes {
    [Serializable]
    public class AlreadyRegisteredException : IPadiException {
        int serverID;

        public AlreadyRegisteredException(int serverID) {
            this.serverID = serverID;
        }

        public int getServerID() {
            return serverID;
        }

        public AlreadyRegisteredException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }

        public int getUid() {
            throw new NotImplementedException();
        }

        public override string getMessage() {
            return "The server with id " + serverID + " is already registered on master server";
        }

    }
}
