using Neuralia.Blockchains.Common.Resources;
using Neuralia.Blockchains.Core.Tools;

namespace Neuralium.Blockchains.Neuralium.Resources {

	public class NeuraliumsTranslationsManager {

		private readonly ResourceReader resourceReader;

		static NeuraliumsTranslationsManager() {
		}

		private NeuraliumsTranslationsManager() {
			this.resourceReader = new ResourceReader(typeof(NeuraliumsTranslationsManager));
		}

		public static NeuraliumsTranslationsManager Instance { get; } = new NeuraliumsTranslationsManager();

		public static TranslationsManager CommonTranslationsManager => TranslationsManager.Instance;

	#region KEYS

	#endregion

	}
}