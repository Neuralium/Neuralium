using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Factories;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage.Base;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Versions;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {

	public interface INeuraliumStandardAccountKeysSnapshotSqliteDal : INeuraliumStandardAccountKeysSnapshotDal<NeuraliumStandardAccountKeysSnapshotSqliteContext, NeuraliumStandardAccountKeysSnapshotSqliteEntry>, IAccountKeysSnapshotSqliteDal<NeuraliumStandardAccountKeysSnapshotSqliteContext, NeuraliumStandardAccountKeysSnapshotSqliteEntry> {
	}

	public class NeuraliumStandardAccountKeysSnapshotSqliteDal : StandardAccountKeysSnapshotSqliteDal<NeuraliumStandardAccountKeysSnapshotSqliteContext, NeuraliumStandardAccountKeysSnapshotSqliteEntry>, INeuraliumStandardAccountKeysSnapshotSqliteDal {

		public NeuraliumStandardAccountKeysSnapshotSqliteDal(int groupSize, string folderPath, BlockchainServiceSet serviceSet, SoftwareVersion softwareVersion, INeuraliumChainDalCreationFactory chainDalCreationFactory, AppSettingsBase.SerializationTypes serializationType) : base(groupSize, folderPath, serviceSet, softwareVersion, chainDalCreationFactory, serializationType) {

		}

		protected override ICardUtils GetCardUtils() {
			return this.GetNeuraliumCardUtils();
		}

		protected INeuraliumCardsUtils GetNeuraliumCardUtils() {
			return NeuraliumCardsUtils.Instance;
		}
	}
}