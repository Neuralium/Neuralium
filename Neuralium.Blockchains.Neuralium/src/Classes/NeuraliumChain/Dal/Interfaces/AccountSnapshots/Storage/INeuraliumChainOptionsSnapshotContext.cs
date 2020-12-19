using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Storage;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage {

	public interface INeuraliumChainOptionsSnapshotContext : IChainOptionsSnapshotContext {
	}

	public interface INeuraliumChainOptionsSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT> : IChainOptionsSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT>, INeuraliumChainOptionsSnapshotContext
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumChainOptionsSnapshotEntry, new() {
	}
}