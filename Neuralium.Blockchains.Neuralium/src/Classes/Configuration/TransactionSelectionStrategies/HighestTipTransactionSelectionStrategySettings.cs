using Neuralia.Blockchains.Common.Classes.Configuration.TransactionSelectionStrategies;

namespace Neuralium.Blockchains.Neuralium.Classes.Configuration.TransactionSelectionStrategies {
	public class HighestTipTransactionSelectionStrategySettings : TransactionSelectionStrategySettings {

		public enum TimeSortingMethods {
			OlderToNewer,
			NewerToOlder,
			Random
		}

		public enum TipSortingMethods {
			MostToLess,
			LessToMost,
			Random
		}

		public TipSortingMethods TipSortingMethod { get; set; } = TipSortingMethods.MostToLess;
		public TimeSortingMethods TimeSortingMethod { get; set; } = TimeSortingMethods.NewerToOlder;

		public bool RandomIncludeNoTip { get; set; } = false;
	}
}