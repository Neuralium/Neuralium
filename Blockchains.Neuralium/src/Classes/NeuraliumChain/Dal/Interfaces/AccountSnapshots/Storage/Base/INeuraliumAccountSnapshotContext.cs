using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Storage.Bases;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base {

	public interface INeuraliumAccountSnapshotContext : IAccountSnapshotContext {
	}

	public interface INeuraliumAccountSnapshotContext<ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT> : INeuraliumAccountSnapshotContext
		where ACCOUNT_SNAPSHOT : class, INeuraliumAccountSnapshotEntry<ACCOUNT_ATTRIBUTE_SNAPSHOT>, new()
		where ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new() {
	}

}