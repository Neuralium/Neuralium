using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Factories;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Versions;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {

	public interface INeuraliumChainOptionsSnapshotSqliteDal : INeuraliumChainOptionsSnapshotDal<NeuraliumChainOptionsSnapshotSqliteContext, NeuraliumChainOptionsSnapshotSqliteEntry>, IChainOptionsSnapshotSqliteDal<NeuraliumChainOptionsSnapshotSqliteContext, NeuraliumChainOptionsSnapshotSqliteEntry> {
	}

	public class NeuraliumChainOptionsSnapshotSqliteDal : ChainOptionsSnapshotSqliteDal<NeuraliumChainOptionsSnapshotSqliteContext, NeuraliumChainOptionsSnapshotSqliteEntry>, INeuraliumChainOptionsSnapshotSqliteDal {

		public NeuraliumChainOptionsSnapshotSqliteDal(string folderPath, BlockchainServiceSet serviceSet, SoftwareVersion softwareVersion, INeuraliumChainDalCreationFactory chainDalCreationFactory, AppSettingsBase.SerializationTypes serializationType) : base(folderPath, serviceSet, softwareVersion, chainDalCreationFactory, serializationType) {

		}
	}
}