using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Factories;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Tools.Data;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {

	public interface INeuraliumJointAccountSnapshotSqliteDal : INeuraliumJointAccountSnapshotDal<NeuraliumJointAccountSnapshotSqliteContext, NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, IJointAccountSnapshotSqliteDal<NeuraliumJointAccountSnapshotSqliteContext, NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry> {
	}

	public class NeuraliumJointAccountSnapshotSqliteDal : JointAccountSnapshotSqliteDal<NeuraliumJointAccountSnapshotSqliteContext, NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry>, INeuraliumJointAccountSnapshotSqliteDal {

		public NeuraliumJointAccountSnapshotSqliteDal(int groupSize, string folderPath, BlockchainServiceSet serviceSet, SoftwareVersion softwareVersion, INeuraliumChainDalCreationFactory chainDalCreationFactory, AppSettingsBase.SerializationTypes serializationType) : base(groupSize, folderPath, serviceSet, softwareVersion, chainDalCreationFactory, serializationType) {

		}

		public override Task InsertNewAccount(AccountId accountId, List<(byte ordinal, SafeArrayHandle key, TransactionId declarationTransactionId)> keys, long inceptionBlockId, bool Verified) {
			throw new NotImplementedException();
		}
	}
}