using System;

namespace CommonTypes {

	/**
	 * Exception that is thrown whenever the LoadBalancer isn't able to deploy a new server. 
	 */
	[Serializable]
	public class ServerCouldNotBeDeployed : IPadiException {
		String server;

		public ServerCouldNotBeDeployed(String server) {
			this.server = server;
		}

		public ServerCouldNotBeDeployed(System.Runtime.Serialization.SerializationInfo info,
		                                   System.Runtime.Serialization.StreamingContext context)
			: base(info, context) {
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
			base.GetObjectData(info, context);
		}

		public override String getMessage() {
			return "The server " + server + " couldn't be deployed.";
		}
	}
}

