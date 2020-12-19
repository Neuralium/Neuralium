using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Models;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Messages.Appointments;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Configuration;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Messages.Appointments {
	
	public interface INeuraliumSendAppointmentRequestMessageWorkflow : ISendAppointmentRequestMessageWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}
	
	public class NeuraliumSendAppointmentRequestMessageWorkflow : SendAppointmentRequestMessageWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumSendAppointmentRequestMessageWorkflow {

		public NeuraliumSendAppointmentRequestMessageWorkflow(int preferredRegion, INeuraliumCentralCoordinator centralCoordinator, CorrelationContext correlationContext) : base(preferredRegion, centralCoordinator, correlationContext) {
		}
	}
}