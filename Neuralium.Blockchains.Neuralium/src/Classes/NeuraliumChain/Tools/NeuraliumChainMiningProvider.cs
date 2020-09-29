using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools {
	public class NeuraliumAppointmentValidatorDelegate : AppointmentValidatorDelegate<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {

		public NeuraliumAppointmentValidatorDelegate(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}
}