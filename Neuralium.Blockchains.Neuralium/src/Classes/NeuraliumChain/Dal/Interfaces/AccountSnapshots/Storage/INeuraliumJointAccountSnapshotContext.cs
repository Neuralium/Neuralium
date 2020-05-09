using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Storage;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage {

	public interface INeuraliumJointAccountSnapshotContext : IJointAccountSnapshotContext, INeuraliumAccountSnapshotContext {
	}

	public interface INeuraliumJointAccountSnapshotContext<ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT, ACCOUNT_MEMBERS_SNAPSHOT> : INeuraliumAccountSnapshotContext<ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT>, IJointAccountSnapshotContext<ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT, ACCOUNT_MEMBERS_SNAPSHOT>, INeuraliumJointAccountSnapshotContext
		where ACCOUNT_SNAPSHOT : class, INeuraliumJointAccountSnapshotEntry<ACCOUNT_ATTRIBUTE_SNAPSHOT, ACCOUNT_MEMBERS_SNAPSHOT>, new()
		where ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where ACCOUNT_MEMBERS_SNAPSHOT : class, INeuraliumJointMemberAccountEntry, new() {
	}
}