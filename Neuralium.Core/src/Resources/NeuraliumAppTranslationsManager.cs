using Neuralium.Blockchains.Neuralium.Resources;
using Neuralia.Blockchains.Common.Resources;
using Neuralia.Blockchains.Core.Tools;

namespace Neuralium.Core.Resources {

	public class NeuraliumAppTranslationsManager {

		private readonly ResourceReader resourceReader;

		static NeuraliumAppTranslationsManager() {
		}

		private NeuraliumAppTranslationsManager() {
			this.resourceReader = new ResourceReader(typeof(NeuraliumAppTranslationsManager));
		}

		public static NeuraliumAppTranslationsManager Instance { get; } = new NeuraliumAppTranslationsManager();

		public static TranslationsManager CommonTranslationsManager => TranslationsManager.Instance;
		public static NeuraliumsTranslationsManager NeuraliumTranslationsManager => NeuraliumsTranslationsManager.Instance;

	#region KEYS

		public string TOSPresentation => this.resourceReader.GetString("TOSPresentation");
		public string Bootstrap_Run_Host_terminated_unexpectedly => this.resourceReader.GetString("Bootstrap_Run_Host_terminated_unexpectedly");

	#endregion

	}
}