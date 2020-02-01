using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots {

	public interface INeuraliumAccountSnapshotEntry : IAccountSnapshotEntry, INeuraliumAccountSnapshot {
	}

	public interface INeuraliumAccountSnapshotEntry<ACCOUNT_ATTRIBUTE> : IAccountSnapshotEntry<ACCOUNT_ATTRIBUTE>, INeuraliumAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumAccountSnapshotEntry
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttributeEntry {
	}

}