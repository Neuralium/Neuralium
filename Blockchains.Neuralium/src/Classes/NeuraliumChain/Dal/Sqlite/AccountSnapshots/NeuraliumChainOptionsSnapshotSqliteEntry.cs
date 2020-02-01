using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots {

	public interface INeuraliumChainOptionsSnapshotSqliteEntry : INeuraliumChainOptionsSnapshotEntry, IChainOptionsSnapshotSqliteEntry {
	}

	/// <summary>
	///     Here we store various metadata state about our chain
	/// </summary>
	public class NeuraliumChainOptionsSnapshotSqliteEntry : ChainOptionsSnapshotSqliteEntry, INeuraliumChainOptionsSnapshotSqliteEntry {
		public decimal SAFUDailyRatio { get; set; }
		public decimal MinimumSAFUQuantity { get; set; }
		public int MaximumAmountDays { get; set; }
	}
}