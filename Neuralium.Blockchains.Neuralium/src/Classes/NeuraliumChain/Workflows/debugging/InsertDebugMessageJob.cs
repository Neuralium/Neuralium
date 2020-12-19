using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Base;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Workflows.Bases;
using Neuralia.Blockchains.Core.Workflows.Tasks.Routing;
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.debugging {
	public interface IInsertDebugMessageWorkflow : INeuraliumChainWorkflow {
	}

	public class InsertDebugMessageWorkflow : NeuraliumChainWorkflow, IInsertDebugMessageWorkflow {

		public InsertDebugMessageWorkflow(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {

		}

		protected override Task PerformWork(IChainWorkflow workflow, TaskRoutingContext taskRoutingContext, LockContext lockContext) {

			// IMessageEnvelope messageEnvelope =  this.centralCoordinator.ChainComponentProvider.AssemblyProvider.GenerateDebugMessage();
			//
			//
			// var validationTask = new NeuraliumValidationTask<ValidationResult>();
			//
			// validationTask.SetAction((validationService, taskRoutingContext) => {
			// 	IRoutedTask validateBlockMessageTask = validationService.ValidateBlockchainMessage(messageEnvelope, result => {
			// 		validationTask.Results = result;
			// 		//TODO: remove this
			// 		validationTask.Results = new ValidationResult(ValidationResult.ValidationResults.Valid);
			// 	});
			//
			// 	taskRoutingContext.AddChild(validateBlockMessageTask);
			// }, (results, taskRoutingContext) => {
			// 	if(results.Success) {
			// 		if(validationTask.Results.Invalid) {
			// 			throw new ApplicationException("Failed to validate message");
			// 		}
			//
			// 		var transactionInsertTask = new NeuraliumBlockchainTask<int>();
			//
			// 		transactionInsertTask.SetAction((transactionchainService, taskRoutingContext2) => {
			// 			transactionchainService.DispatchNewMessage(messageEnvelope);
			//
			// 		});
			//
			// 		taskRoutingContext.AddChild(transactionInsertTask);
			// 	} else {
			// 		//TODO: what do we do here?
			// 		this.CentralCoordinator.LogError(results.Exception, "Failed to validate message");
			//
			// 		throw new NotImplementedException();
			//
			// 		//this.CentralCoordinator.WalletProvider.GetActiveWalletIdentity().FindWalletKey(genesisBlockMessage.GenesisBlock.SignaturePublicKey).Status = WalletKey.Statuses.Published;
			// 		//this.CentralCoordinator.WalletProvider.SaveWallet();
			//
			// 		//return null;
			// 	}
			//
			// });
			//
			// this.DispatchTaskSync(validationTask);
			return Task.CompletedTask;
		}
	}
}