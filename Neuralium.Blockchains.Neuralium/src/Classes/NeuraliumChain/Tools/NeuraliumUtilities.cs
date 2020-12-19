using System;
using Neuralia.Blockchains.Core.General.Types.Dynamic;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools {
	public static class NeuraliumUtilities {

		public const int NEURALIUM_PRECISION = 9;
		public static readonly decimal MaxValue = AdaptiveDecimal.MaxValue;
		public static readonly decimal MinValue = AdaptiveDecimal.MinValue;

		public static decimal RoundNeuraliumsPrecision(decimal value) {
			return Math.Round(value, NEURALIUM_PRECISION, MidpointRounding.ToEven);
		}

		public static Amount CapAndRound(Amount value) {
			return RoundNeuraliumsPrecision(Cap(value.Value));
		}

		public static Amount RoundNeuraliumsPrecision(Amount value) {
			return RoundNeuraliumsPrecision(value.Value);
		}

		public static decimal Cap(decimal value) {
			decimal adjusted = Math.Max(value, MinValue);
			adjusted = Math.Min(adjusted, MaxValue);

			return adjusted;
		}

		public static Amount Cap(Amount value) {
			return Cap(value.Value);
		}
	}
}