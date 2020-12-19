using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1.Implementations {
	public class NeuraliumTipingTransactionImplementation : ISerializableCombo, ITip {

		public void Rehydrate(IDataRehydrator rehydrator) {

			this.Tip.Rehydrate(rehydrator);
		}

		public void Dehydrate(IDataDehydrator dehydrator) {

			this.Tip.Dehydrate(dehydrator);
		}

		public HashNodeList GetStructuresArray() {
			HashNodeList nodeList = new HashNodeList();

			nodeList.Add(this.Tip);

			return nodeList;
		}

		public void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			//
			jsonDeserializer.SetProperty("Tip", this.Tip?.Value);
		}

		public Amount Tip { get; set; } = new Amount();

		public virtual void Sanitize() {
			this.Tip = NeuraliumUtilities.CapAndRound(this.Tip);
		}
	}
}