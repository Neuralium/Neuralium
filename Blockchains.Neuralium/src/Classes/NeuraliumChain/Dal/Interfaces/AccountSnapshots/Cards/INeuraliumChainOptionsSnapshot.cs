using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards {
	public interface INeuraliumChainOptionsSnapshot : INeuraliumSnapshot, IChainOptionsSnapshot {
		decimal SAFUDailyRatio { get; set; }
		
		decimal MinimumSAFUQuantity { get; set; }
		
		int MaximumAmountDays { get; set; }
	}
}