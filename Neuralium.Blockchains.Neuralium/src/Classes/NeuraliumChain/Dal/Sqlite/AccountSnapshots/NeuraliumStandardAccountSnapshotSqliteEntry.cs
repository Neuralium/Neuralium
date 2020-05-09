using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots {

	public interface INeuraliumStandardAccountSnapshotSqliteEntry : INeuraliumStandardAccountSnapshotEntry<NeuraliumStandardAccountAttributeSqliteEntry>, IStandardAccountSnapshotSqliteEntry<NeuraliumStandardAccountAttributeSqliteEntry>, INeuraliumAccountSnapshotSqliteEntry<NeuraliumStandardAccountAttributeSqliteEntry> {
	}

	/// <summary>
	///     Here we store various metadata state about our chain
	/// </summary>
	public class NeuraliumStandardAccountSnapshotSqliteEntry : StandardAccountSnapshotSqliteEntry<NeuraliumStandardAccountAttributeSqliteEntry>, INeuraliumStandardAccountSnapshotSqliteEntry {

		public decimal Balance { get; set; } = new Amount();
	}
}