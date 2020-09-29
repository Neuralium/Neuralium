using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Core.Cryptography.Trees;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes {
	public interface INeuraliumInitiationAppointmentMessageEnvelope : IInitiationAppointmentMessageEnvelope , INeuraliumMessageEnvelope {
	}
	
	public class NeuraliumInitiationAppointmentMessageEnvelope : InitiationAppointmentMessageEnvelope, INeuraliumInitiationAppointmentMessageEnvelope{

		
	}
}