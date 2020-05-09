using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Storage;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage {

	public interface INeuraliumJointAccountSnapshotDal : IJointAccountSnapshotDal, INeuraliumAccountSnapshotDal {
	}

	public interface INeuraliumJointAccountSnapshotDal<ACCOUNT_SNAPSHOT_CONTEXT, ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT, ACCOUNT_MEMBERS_SNAPSHOT> : INeuraliumAccountSnapshotDal<ACCOUNT_SNAPSHOT_CONTEXT, ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT>, IJointAccountSnapshotDal<ACCOUNT_SNAPSHOT_CONTEXT, ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT, ACCOUNT_MEMBERS_SNAPSHOT>, INeuraliumJointAccountSnapshotDal
		where ACCOUNT_SNAPSHOT_CONTEXT : INeuraliumJointAccountSnapshotContext<ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT, ACCOUNT_MEMBERS_SNAPSHOT>
		where ACCOUNT_SNAPSHOT : class, INeuraliumJointAccountSnapshotEntry<ACCOUNT_ATTRIBUTE_SNAPSHOT, ACCOUNT_MEMBERS_SNAPSHOT>, new()
		where ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where ACCOUNT_MEMBERS_SNAPSHOT : class, INeuraliumJointMemberAccountEntry, new() {
	}
}