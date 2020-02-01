using System;
using System.Collections.Generic;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainPool;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Blockchains.Neuralium.Classes.NeuraliumChain.Factories;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.ChainPool;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Versions;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.ChainPool {

	public interface INeuraliumChainPoolSqliteDal : INeuraliumChainPoolDal<NeuraliumChainPoolSqlitePublicTransactions> {
	}

	public class NeuraliumChainPoolSqliteDal : ChainPoolSqliteDal<NeuraliumChainPoolSqliteContext, NeuraliumChainPoolSqlitePublicTransactions>, INeuraliumChainPoolSqliteDal {

		public NeuraliumChainPoolSqliteDal(string folderPath, BlockchainServiceSet serviceSet, SoftwareVersion softwareVersion, INeuraliumChainDalCreationFactory chainDalCreationFactory, AppSettingsBase.SerializationTypes serializationType) : base(folderPath, serviceSet, softwareVersion, chainDalCreationFactory, serializationType) {

		}

		public List<(TransactionId transactionIds, decimal tip)> GetTransactionsAndTip() {
			return this.PerformOperation(db => {

				return db.PublicTransactions.Select(t => new Tuple<TransactionId, decimal>(TransactionId.FromCompactString(t.TransactionId), t.Tip)).ToList();
			}).Select(e => (e.Item1, e.Item2)).ToList();
		}

		protected override void PrepareTransactionEntry(NeuraliumChainPoolSqlitePublicTransactions entry, ITransactionEnvelope transactionEnvelope, DateTime chainInception) {
			base.PrepareTransactionEntry(entry, transactionEnvelope, chainInception);

			if(transactionEnvelope.Contents.RehydratedTransaction is ITipTransaction tipTransaction) {
				entry.Tip = tipTransaction.Tip.Value;
			}

		}
	}
}