using System;
using System.Collections.Generic;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Blockchains.Neuralium.Classes.NeuraliumChain.Factories;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Tools.Data;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {

	public interface INeuraliumStandardAccountSnapshotSqliteDal : INeuraliumStandardAccountSnapshotDal<NeuraliumStandardAccountSnapshotSqliteContext, NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry>, IStandardAccountSnapshotSqliteDal<NeuraliumStandardAccountSnapshotSqliteContext, NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry> {
	}

	public class NeuraliumStandardAccountSnapshotSqliteDal : StandardAccountSnapshotSqliteDal<NeuraliumStandardAccountSnapshotSqliteContext, NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry>, INeuraliumStandardAccountSnapshotSqliteDal {

		public NeuraliumStandardAccountSnapshotSqliteDal(int groupSize, string folderPath, BlockchainServiceSet serviceSet, SoftwareVersion softwareVersion, INeuraliumChainDalCreationFactory chainDalCreationFactory, AppSettingsBase.SerializationTypes serializationType) : base(groupSize, folderPath, serviceSet, softwareVersion, chainDalCreationFactory, serializationType) {

		}

		public override void InsertNewAccount(AccountId accountId, List<(byte ordinal, SafeArrayHandle key, TransactionId declarationTransactionId)> keys, long inceptionBlockId, long? correlationId) {
			throw new NotImplementedException();
		}
	}
}