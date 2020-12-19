using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage {

	public interface INeuraliumStandardAccountKeysSnapshotDal : INeuraliumAccountKeysSnapshotDal {
	}

	public interface INeuraliumStandardAccountKeysSnapshotDal<ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_SNAPSHOT> : INeuraliumAccountKeysSnapshotDal<ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_SNAPSHOT>, INeuraliumStandardAccountKeysSnapshotDal
		where ACCOUNT_SNAPSHOT_CONTEXT : INeuraliumStandardAccountKeysSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT>
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumStandardAccountKeysSnapshotEntry, new() {
	}
}