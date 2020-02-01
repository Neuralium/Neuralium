using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots {

	public interface INeuraliumStandardAccountSnapshotEntry : INeuraliumStandardAccountSnapshot, INeuraliumAccountSnapshotEntry, IStandardAccountSnapshotEntry {
	}

	public interface INeuraliumStandardAccountSnapshotEntry<ACCOUNT_ATTRIBUTE> : INeuraliumStandardAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumAccountSnapshotEntry<ACCOUNT_ATTRIBUTE>, IStandardAccountSnapshotEntry<ACCOUNT_ATTRIBUTE>, INeuraliumStandardAccountSnapshotEntry
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttributeEntry {
	}

}