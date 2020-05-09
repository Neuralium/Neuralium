using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage {

	public interface INeuraliumStandardAccountKeysSnapshotContext : INeuraliumAccountKeysSnapshotContext {
	}

	public interface INeuraliumStandardAccountKeysSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT> : INeuraliumAccountKeysSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT>, INeuraliumStandardAccountKeysSnapshotContext
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumStandardAccountKeysSnapshotEntry, new() {
	}
}