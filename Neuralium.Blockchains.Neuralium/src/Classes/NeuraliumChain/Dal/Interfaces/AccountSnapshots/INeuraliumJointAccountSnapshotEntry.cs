using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots {
	public interface INeuraliumJointAccountSnapshotEntry<ACCOUNT_ATTRIBUTE, JOINT_MEMBER_FEATURE> : INeuraliumJointAccountSnapshot<ACCOUNT_ATTRIBUTE, JOINT_MEMBER_FEATURE>, INeuraliumAccountSnapshotEntry<ACCOUNT_ATTRIBUTE>, IJointAccountSnapshotEntry<ACCOUNT_ATTRIBUTE, JOINT_MEMBER_FEATURE>
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttributeEntry
		where JOINT_MEMBER_FEATURE : IJointMemberAccountEntry {
	}

}