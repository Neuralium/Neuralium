using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.AccountAttributeContexts;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Tools.AccountAttributeContexts {
	public class NeuraliumFreezeAttributeContext : TransferAttributeContextBase {

		public Amount Amount { get; set; } = new Amount();
		
		protected override void Rehydrate(IDataRehydrator rehydrator) {
			base.Rehydrate(rehydrator);

			this.Amount.Rehydrate(rehydrator);
		}

		protected override void Dehydrate(IDataDehydrator dehydrator) {
			base.Dehydrate(dehydrator);

			this.Amount.Dehydrate(dehydrator);
		}
	}
}