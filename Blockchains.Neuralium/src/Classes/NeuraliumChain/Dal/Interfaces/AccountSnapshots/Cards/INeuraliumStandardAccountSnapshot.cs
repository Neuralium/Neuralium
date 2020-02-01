using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards {
	public interface INeuraliumStandardAccountSnapshot : IStandardAccountSnapshot, INeuraliumAccountSnapshot {
	}

	public interface INeuraliumStandardAccountSnapshot<ACCOUNT_ATTRIBUTE> : IStandardAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumStandardAccountSnapshot
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttribute{
	}
}