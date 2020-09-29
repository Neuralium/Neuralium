using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Appointments;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.General.Appointments {
	
	public interface INeuraliumAppointmentRequestMessage : IAppointmentRequestMessage {

	}


	public class NeuraliumAppointmentRequestMessage : AppointmentRequestMessage, INeuraliumAppointmentRequestMessage {
		
	}
}