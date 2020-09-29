using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Types;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator.V1;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1 {
	public interface INeuraliumChainAccreditationCertificateTransaction : IChainAccreditationCertificateTransaction, INeuraliumModerationTransaction {
		Amount ProviderBountyshare { get; set; }
		Amount InfrastructureServiceFees { get; set; }
	}

	public class NeuraliumChainAccreditationCertificateTransaction : ChainAccreditationCertificateTransaction, INeuraliumChainAccreditationCertificateTransaction {

		/// <summary>
		///     For an SDK host, what percentage goes to the host. (and the rest to the SDK user). between 0 to 1
		/// </summary>
		public Amount ProviderBountyshare { get; set; } = new Amount();

		public Amount InfrastructureServiceFees { get; set; } = new Amount();

		public override HashNodeList GetStructuresArray(Enums.MutableStructureTypes types) {
			HashNodeList nodeList = base.GetStructuresArray(types);

			if(this.CertificateType == AccreditationCertificateTypes.Instance.SDK_PROVIDER) {
				nodeList.Add(this.ProviderBountyshare);
				nodeList.Add(this.InfrastructureServiceFees);
			}

			if(this.CertificateType == AccreditationCertificateTypes.Instance.DELEGATE) {
				nodeList.Add(this.InfrastructureServiceFees);
			}

			return nodeList;
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			if(this.CertificateType == AccreditationCertificateTypes.Instance.SDK_PROVIDER) {
				jsonDeserializer.SetProperty("HostBountyShare", this.ProviderBountyshare.Value);
				jsonDeserializer.SetProperty("InfrastructureServiceFees", this.InfrastructureServiceFees.Value);
			}

			if(this.CertificateType == AccreditationCertificateTypes.Instance.DELEGATE) {
				jsonDeserializer.SetProperty("InfrastructureServiceFees", this.InfrastructureServiceFees.Value);
			}
		}

		protected override void RehydrateContents(ChannelsEntries<IDataRehydrator> dataChannels, ITransactionRehydrationFactory rehydrationFactory) {
			base.RehydrateContents(dataChannels, rehydrationFactory);

			if(this.CertificateType == AccreditationCertificateTypes.Instance.SDK_PROVIDER) {
				this.ProviderBountyshare.Rehydrate(dataChannels.ContentsData);
				this.InfrastructureServiceFees.Rehydrate(dataChannels.ContentsData);
			}

			if(this.CertificateType == AccreditationCertificateTypes.Instance.DELEGATE) {
				this.InfrastructureServiceFees.Rehydrate(dataChannels.ContentsData);
			}
		}

		protected override void DehydrateContents(ChannelsEntries<IDataDehydrator> dataChannels) {
			base.DehydrateContents(dataChannels);

			if(this.CertificateType == AccreditationCertificateTypes.Instance.SDK_PROVIDER) {
				this.ProviderBountyshare.Dehydrate(dataChannels.ContentsData);
				this.InfrastructureServiceFees.Dehydrate(dataChannels.ContentsData);
			}

			if(this.CertificateType == AccreditationCertificateTypes.Instance.DELEGATE) {
				this.InfrastructureServiceFees.Dehydrate(dataChannels.ContentsData);
			}
		}
	}
}