using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General;
using Neuralia.Blockchains.Components.Transactions.Identifiers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General {
	public class NeuraliumDebugKeyedTransaction : DebugKeyedTransaction {
		public NeuraliumDebugKeyedTransaction() {

		}

		public NeuraliumDebugKeyedTransaction(TransactionId trxId) {
			this.TransactionId = trxId;
		}
	}
}