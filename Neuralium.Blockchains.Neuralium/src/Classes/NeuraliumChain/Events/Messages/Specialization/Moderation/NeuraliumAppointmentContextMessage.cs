using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Appointments;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.Moderation.Appointments;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.Moderation {
	
	public interface INeuraliumAppointmentContextMessage : IAppointmentContextMessage {

	}


	public class NeuraliumAppointmentContextMessage : AppointmentContextMessage, INeuraliumAppointmentContextMessage {
		
	}
}