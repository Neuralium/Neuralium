using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General {

	public class NeuraliumDebugTransaction : DebugTransaction, INeuraliumTransaction {

		public NeuraliumDebugTransaction() {

		}
		
		public NeuraliumDebugTransaction(TransactionId trxId) {
			this.TransactionId = trxId;
		}

		protected override void RehydrateContents(ChannelsEntries<IDataRehydrator> dataChannels, ITransactionRehydrationFactory rehydrationFactory) {
			base.RehydrateContents(dataChannels, rehydrationFactory);

		}

		protected override void DehydrateContents(ChannelsEntries<IDataDehydrator> dataChannels) {
			base.DehydrateContents(dataChannels);

		}
		
		
	}
}