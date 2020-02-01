using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots {

	public interface INeuraliumAccountSnapshotSqliteEntry : IAccountSnapshotSqliteEntry, INeuraliumAccountSnapshotEntry {
	}

	public interface INeuraliumAccountSnapshotSqliteEntry<ACCOUNT_ATTRIBUTE> : IAccountSnapshotSqliteEntry<ACCOUNT_ATTRIBUTE>, INeuraliumAccountSnapshotEntry<ACCOUNT_ATTRIBUTE>, INeuraliumAccountSnapshotSqliteEntry
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttributeSqliteEntry {
	}
}