using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumAppointmentsProvider : IAppointmentsProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumAppointmentsProvider : AppointmentsProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumAppointmentsProvider {

		public NeuraliumAppointmentsProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}
}