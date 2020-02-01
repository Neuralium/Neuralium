using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base;
using Microsoft.EntityFrameworkCore;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Storage;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage {

	public interface INeuraliumStandardAccountSnapshotContext : IStandardAccountSnapshotContext, INeuraliumAccountSnapshotContext {
	}

	public interface INeuraliumStandardAccountSnapshotContext<ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT> : INeuraliumAccountSnapshotContext<ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT>, IStandardAccountSnapshotContext<ACCOUNT_SNAPSHOT, ACCOUNT_ATTRIBUTE_SNAPSHOT>, INeuraliumStandardAccountSnapshotContext
		where ACCOUNT_SNAPSHOT : class, INeuraliumStandardAccountSnapshotEntry<ACCOUNT_ATTRIBUTE_SNAPSHOT>, new()
		where ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new() {

	}
}