using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Storage;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage {

	public interface INeuraliumChainOptionsSnapshotDal : IChainOptionsSnapshotDal {
	}

	public interface INeuraliumChainOptionsSnapshotDal<ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_SNAPSHOT> : IChainOptionsSnapshotDal<ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_SNAPSHOT>, INeuraliumChainOptionsSnapshotDal
		where ACCOUNT_SNAPSHOT_CONTEXT : INeuraliumChainOptionsSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT>
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumChainOptionsSnapshotEntry, new() {
	}
}