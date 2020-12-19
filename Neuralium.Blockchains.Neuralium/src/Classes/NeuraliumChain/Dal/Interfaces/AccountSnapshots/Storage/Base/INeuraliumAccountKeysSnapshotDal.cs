using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Storage.Bases;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base {

	public interface INeuraliumAccountKeysSnapshotDal : IAccountKeysSnapshotDal {
	}

	public interface INeuraliumAccountKeysSnapshotDal<ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_SNAPSHOT> : IAccountKeysSnapshotDal<ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_SNAPSHOT>, INeuraliumAccountKeysSnapshotDal
		where ACCOUNT_SNAPSHOT_CONTEXT : INeuraliumAccountKeysSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT>
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumAccountKeysSnapshotEntry, new() {
	}
}