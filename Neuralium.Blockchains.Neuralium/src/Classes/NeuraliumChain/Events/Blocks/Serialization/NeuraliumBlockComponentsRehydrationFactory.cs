using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Contexts;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results;
using Neuralia.Blockchains.Core.Compression;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Serialization {
	public interface INeuraliumBlockComponentsRehydrationFactory : IBlockComponentsRehydrationFactory {
	}

	public class NeuraliumBlockComponentsRehydrationFactory : BlockComponentsRehydrationFactory, INeuraliumBlockComponentsRehydrationFactory {
		public override IElectionContext CreateElectionContext(SafeArrayHandle compressedContext) {

			BrotliCompression compressor = new BrotliCompression();

			using(SafeArrayHandle decompressedContext = compressor.Decompress(compressedContext)) {

				IDataRehydrator electionContextRehydrator = DataSerializationFactory.CreateRehydrator(decompressedContext);

				ComponentVersion<ElectionContextType> version = electionContextRehydrator.RehydrateRewind<ComponentVersion<ElectionContextType>>();

				IElectionContext context = null;

				if(version.Type == ElectionContextTypes.Instance.Active) {
					if(version == (1, 0)) {
						context = new NeuraliumActiveElectionContext();
					}
				}

				if(version.Type == ElectionContextTypes.Instance.Passive) {
					if(version == (1, 0)) {
						context = new NeuraliumPassiveElectionContext();
					}
				}

				if(context == null) {
					throw new ApplicationException("Unrecognized election context version.");
				}

				context.Rehydrate(electionContextRehydrator, this);

				return context;
			}
		}

		public override ITransactionSelectionMethodFactory CreateTransactionSelectionMethodFactory() {
			return new NeuraliumTransactionSelectionMethodFactory();
		}

		public override IElectionResultsRehydrator CreateElectionResultsRehydrator() {
			return new NeuraliumElectionResultsRehydrator();
		}
	}
}