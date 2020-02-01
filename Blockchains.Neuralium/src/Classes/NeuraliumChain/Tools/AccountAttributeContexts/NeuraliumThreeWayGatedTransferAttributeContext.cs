using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.AccountAttributeContexts;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Tools.AccountAttributeContexts {
	public class NeuraliumThreeWayGatedTransferAttributeContext :ThreeWayGatedTransferAttributeContext {
		
		public Amount Amount { get; set; } = new Amount();
		public Amount VerifierServiceFee { get; set; } = new Amount();
		
		protected override void Rehydrate(IDataRehydrator rehydrator) {
			base.Rehydrate(rehydrator);

			if(this.Role == Roles.Sender) {
				this.Amount.Rehydrate(rehydrator);
				this.VerifierServiceFee.Rehydrate(rehydrator);
			}
			else if(this.Role == Roles.Receiver) {
				this.Amount.Rehydrate(rehydrator);
				this.VerifierServiceFee.Rehydrate(rehydrator);
			}
			else if(this.Role == Roles.Verifier) {
				this.VerifierServiceFee.Rehydrate(rehydrator);
			}
		}

		protected override void Dehydrate(IDataDehydrator dehydrator) {
			base.Dehydrate(dehydrator);

			if(this.Role == Roles.Sender) {
				this.Amount.Dehydrate(dehydrator);
				this.VerifierServiceFee.Dehydrate(dehydrator);
			}
			else if(this.Role == Roles.Receiver) {
				this.Amount.Dehydrate(dehydrator);
				this.VerifierServiceFee.Dehydrate(dehydrator);
			}
			else if(this.Role == Roles.Verifier) {
				this.VerifierServiceFee.Dehydrate(dehydrator);
			}
			
		}
	}
}