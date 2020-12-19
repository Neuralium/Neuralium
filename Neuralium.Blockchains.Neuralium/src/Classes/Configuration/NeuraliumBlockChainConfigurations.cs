using Neuralium.Blockchains.Neuralium.Classes.Configuration.TransactionSelectionStrategies;
using Neuralia.Blockchains.Common.Classes.Configuration;

namespace Neuralium.Blockchains.Neuralium.Classes.Configuration {
	public class NeuraliumBlockChainConfigurations : BlockChainConfigurations {

		public enum NeuraliumTransactionSelectionStrategies {
			Automatic = TransactionSelectionStrategies.Automatic,
			CreationTime = TransactionSelectionStrategies.CreationTime,
			TransactionType = TransactionSelectionStrategies.TransactionType,
			Size = TransactionSelectionStrategies.Size,
			Random = TransactionSelectionStrategies.Random,
			Tips = 6
		}

		public new NeuraliumTransactionSelectionStrategies TransactionSelectionStrategy { get; set; } = NeuraliumTransactionSelectionStrategies.Automatic;

		public HighestTipTransactionSelectionStrategySettings HighestTipTransactionSelectionStrategySettings { get; } = new HighestTipTransactionSelectionStrategySettings();
	}
}