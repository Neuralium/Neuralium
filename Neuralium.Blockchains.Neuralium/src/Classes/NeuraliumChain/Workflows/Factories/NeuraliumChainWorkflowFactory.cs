using System;
using System.Collections.Generic;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AppointmentRegistry;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Messages.Elections;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Transactions;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.debugging;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Models;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Chain;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Messages.Appointments;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Messages.Elections;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Creation.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Factories;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Data;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Creation.Messages.Appointments;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Factories {
	public interface INeuraliumChainWorkflowFactory : IChainWorkflowFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {

		IInsertDebugConfirmWorkflow CreateDebugConfirmChainWorkflow(TransactionId guid, SafeArrayHandle hash, byte expiration = 0);

		IInsertDebugMessageWorkflow CreateDebugMessageWorkflow();

		ICreateNeuraliumTransferTransactionWorkflow CreateSendNeuraliumsWorkflow(string accountCode, AccountId targetAccountId, Amount amount, Amount tip, string note, CorrelationContext correlationContext, byte expiration = 0);
#if TESTNET || DEVNET
		ICreateNeuraliumRefillTransactionWorkflow CreateRefillNeuraliumsWorkflow(string accountCode, CorrelationContext correlationContext, byte expiration = 0);
#endif
	}

	public class NeuraliumChainWorkflowFactory : ChainWorkflowFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainWorkflowFactory {
		public NeuraliumChainWorkflowFactory(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		public override ICreatePresentationTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreatePresentationTransactionChainWorkflow(CorrelationContext correlationContext, string accountCode, byte expiration = 0) {
			return new NeuraliumCreatePresentationTransactionWorkflow(this.centralCoordinator, expiration, correlationContext, accountCode);
		}

		public override ICreateChangeKeyTransactionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreateChangeKeyTransactionWorkflow(byte changingKeyOrdinal, string note, CorrelationContext correlationContext, byte expiration = 0) {
			return new NeuraliumCreateChangeKeyTransactionWorkflow(this.centralCoordinator, expiration, note, changingKeyOrdinal, correlationContext);
		}

		public virtual IInsertDebugConfirmWorkflow CreateDebugConfirmChainWorkflow(TransactionId guid, SafeArrayHandle hash, byte expiration = 0) {
			return new InsertDebugConfirmWorkflow(guid, hash, this.centralCoordinator);
		}

		public virtual ICreateNeuraliumTransferTransactionWorkflow CreateSendNeuraliumsWorkflow(string accountCode, AccountId targetAccountId, Amount amount, Amount tip, string note, CorrelationContext correlationContext, byte expiration = 0) {
			return new CreateNeuraliumTransferTransactionWorkflow(accountCode, targetAccountId, amount, tip, expiration, note, this.centralCoordinator, correlationContext);
		}

		public IInsertDebugMessageWorkflow CreateDebugMessageWorkflow() {
			return new InsertDebugMessageWorkflow(this.centralCoordinator);
		}

		public override ISendElectionsRegistrationMessageWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreateSendElectionsCandidateRegistrationMessageWorkflow(AccountId candidateAccountId, Enums.MiningTiers miningTier, ElectionsCandidateRegistrationInfo electionsCandidateRegistrationInfo, AppSettingsBase.ContactMethods registrationMethod, CorrelationContext correlationContext) {
			return new NeuraliumSendElectionsRegistrationMessageWorkflow(candidateAccountId, miningTier, electionsCandidateRegistrationInfo, registrationMethod, this.centralCoordinator, correlationContext);
		}

		public override ILoadWalletWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreateLoadWalletWorkflow(CorrelationContext correlationContext, string passphrase = null) {
			return new LoadWalletWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>(this.centralCoordinator, correlationContext, passphrase);
		}

		public override IGenerateXmssKeyIndexNodeCacheWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreateGenerateXmssKeyIndexNodeCacheWorkflow(string accountCode, byte ordinal, long index, CorrelationContext correlationContext) {
			return new GenerateXmssKeyIndexNodeCacheWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>(accountCode, ordinal, index, this.centralCoordinator, correlationContext);
		}

		public override ISendAppointmentRequestMessageWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreateSendAppointmentRequestMessageWorkflow(int preferredRegion, CorrelationContext correlationContext) {
			return new NeuraliumSendAppointmentRequestMessageWorkflow(preferredRegion, this.centralCoordinator, correlationContext);
		}

		public override ISendInitiationAppointmentRequestMessageWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreateSendInitiationAppointmentRequestMessageWorkflow(int preferredRegion, CorrelationContext correlationContext) {
			return new NeuraliumSendInitiationAppointmentRequestMessageWorkflow(preferredRegion, this.centralCoordinator, correlationContext);
		}

		public override IPuzzleExecutionWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreateAppointmentPuzzleExecutionWorkflow(CorrelationContext correlationContext) {
			return new NeuraliumPuzzleExecutionWorkflow(this.centralCoordinator, correlationContext);
		}

		public override ISendAppointmentVerificationResultsMessageWorkflow<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> CreateSendAppointmentVerificationResultsMessageWorkflow(List<IAppointmentRequesterResult> entries, Dictionary<long, bool> verificationResults, CorrelationContext correlationContext) {
			return new NeuraliumSendAppointmentVerificationResultsMessageWorkflow(entries, verificationResults, this.centralCoordinator, correlationContext);
		}

#if TESTNET || DEVNET
		public virtual ICreateNeuraliumRefillTransactionWorkflow CreateRefillNeuraliumsWorkflow(string accountCode, CorrelationContext correlationContext, byte expiration = 0) {
			return new CreateNeuraliumRefillTransactionWorkflow(accountCode, expiration, null, this.centralCoordinator, correlationContext);
		}
#endif
	}
}