using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Storage.Bases;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base {

	public interface INeuraliumAccountKeysSnapshotContext : IAccountKeysSnapshotContext {
	}

	public interface INeuraliumAccountKeysSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT> : IAccountKeysSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT>, INeuraliumAccountKeysSnapshotContext
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumAccountKeysSnapshotEntry, new() {
	}
}