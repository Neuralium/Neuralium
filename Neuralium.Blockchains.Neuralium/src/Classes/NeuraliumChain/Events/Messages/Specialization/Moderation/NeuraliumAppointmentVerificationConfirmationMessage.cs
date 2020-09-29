using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Appointments;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.Moderation.Appointments;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.Moderation {
	
	public interface INeuraliumAppointmentVerificationConfirmationMessage : IAppointmentVerificationConfirmationMessage {

	}


	public class NeuraliumAppointmentVerificationConfirmationMessage : AppointmentVerificationConfirmationMessage, INeuraliumAppointmentVerificationConfirmationMessage {

		public class NeuraliumAppointmentRequester : AppointmentRequester {
			
		}
		
		protected override AppointmentRequester CreateAppointmentRequester() {
			return new NeuraliumAppointmentRequester();
		}
	}
}