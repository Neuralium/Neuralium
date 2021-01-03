using System;
using System.Collections.Generic;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AppointmentRegistry;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Models;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Messages.Appointments;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Types;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Messages.Appointments {
	public interface INeuraliumSendAppointmentVerificationResultsMessageWorkflow : ISendAppointmentVerificationResultsMessageWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public class NeuraliumSendAppointmentVerificationResultsMessageWorkflow : SendAppointmentVerificationResultsMessageWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumSendAppointmentVerificationResultsMessageWorkflow {

		public NeuraliumSendAppointmentVerificationResultsMessageWorkflow(DateTime appointment, INeuraliumCentralCoordinator centralCoordinator, CorrelationContext correlationContext) : base(appointment, centralCoordinator, correlationContext) {
		}
	}
}