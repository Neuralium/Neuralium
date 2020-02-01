using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Types;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumAccreditationCertificateProvider : INeuraliumAccreditationCertificateProviderGenerix<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumAccreditationCertificatesSnapshotSqliteDal, NeuraliumAccreditationCertificatesSnapshotSqliteContext, NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry> {
	}

	/// <summary>
	///     Provide access to the chain state that is saved in the DB
	/// </summary>
	public class NeuraliumAccreditationCertificateProvider : NeuraliumAccreditationCertificateProviderGenerix<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumAccreditationCertificatesSnapshotSqliteDal, NeuraliumAccreditationCertificatesSnapshotSqliteContext, NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry>, INeuraliumAccreditationCertificateProvider {

		public NeuraliumAccreditationCertificateProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}

	public interface INeuraliumAccreditationCertificateProviderGenerix : IAccreditationCertificateProvider {
		Dictionary<AccountId, (decimal delegateBountyShare, decimal InfrastructureServiceFees)> GetDelegateAllocationPercentages(ImmutableList<AccountId> validDelegates, Enums.CertificateApplicationTypes applicationType);

	}

	public interface INeuraliumAccreditationCertificateProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, ACCREDITATION_CERTIFICATE_DAL, ACCREDITATION_CERTIFICATE_CONTEXT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT> : IAccreditationCertificateProvider<ACCREDITATION_CERTIFICATE_DAL, ACCREDITATION_CERTIFICATE_CONTEXT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, INeuraliumAccreditationCertificateProviderGenerix
		where CENTRAL_COORDINATOR : ICentralCoordinator<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_COMPONENT_PROVIDER : IChainComponentProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where ACCREDITATION_CERTIFICATE_DAL : class, IAccreditationCertificatesSnapshotDal<ACCREDITATION_CERTIFICATE_CONTEXT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_CONTEXT : class, IAccreditationCertificatesSnapshotContext<ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_SNAPSHOT : class, IAccreditationCertificateSnapshotEntry<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, new()
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT : class, IAccreditationCertificateSnapshotAccountEntry, new() {

	}

	public abstract class NeuraliumAccreditationCertificateProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, ACCREDITATION_CERTIFICATE_DAL, ACCREDITATION_CERTIFICATE_CONTEXT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT> : AccreditationCertificateProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, ACCREDITATION_CERTIFICATE_DAL, ACCREDITATION_CERTIFICATE_CONTEXT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, INeuraliumAccreditationCertificateProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, ACCREDITATION_CERTIFICATE_DAL, ACCREDITATION_CERTIFICATE_CONTEXT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where CENTRAL_COORDINATOR : ICentralCoordinator<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_COMPONENT_PROVIDER : IChainComponentProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where ACCREDITATION_CERTIFICATE_DAL : class, INeuraliumAccreditationCertificatesSnapshotDal<ACCREDITATION_CERTIFICATE_CONTEXT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_CONTEXT : class, INeuraliumAccreditationCertificatesSnapshotContext<ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotEntry<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, new()
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotAccountEntry, new() {

		public NeuraliumAccreditationCertificateProviderGenerix(CENTRAL_COORDINATOR centralCoordinator) : base(centralCoordinator) {
		}

		private INeuraliumAccountSnapshotsProvider NeuraliumAccountSnapshotsProvider => (INeuraliumAccountSnapshotsProvider) this.SnapshotProvider;

		/// <summary>
		///     here we determine what percentage of the bounty a delegate will get based on the accreditate certificate it has
		/// </summary>
		/// <param name="validDelegates"></param>
		/// <returns></returns>
		public Dictionary<AccountId, (decimal delegateBountyShare, decimal InfrastructureServiceFees)> GetDelegateAllocationPercentages(ImmutableList<AccountId> validDelegates, Enums.CertificateApplicationTypes applicationType) {

			//TODO: avoid requery with previous method call
			var certificateSnapshots = this.SnapshotProvider.GetAccreditationCertificates(validDelegates, new[] {AccreditationCertificateTypes.Instance.DELEGATE, AccreditationCertificateTypes.Instance.SDK_PROVIDER}, applicationType);
			var results = new Dictionary<AccountId, (decimal delegateBountyShare, decimal InfrastructureServiceFees)>();

			foreach(AccountId account in validDelegates) {

				decimal allocation = 0;
				decimal serviceFees = 0;

				ACCREDITATION_CERTIFICATE_SNAPSHOT certificate = certificateSnapshots.SingleOrDefault(c => c.AssignedAccount == account.ToLongRepresentation());

				if(certificate != null) {

					if(certificate.CertificateType == AccreditationCertificateTypes.Instance.DELEGATE.Value) {

						serviceFees = certificate.InfrastructureServiceFees;
						allocation = 1 - serviceFees; // a delegate takes the full amount less service fees
					} else if(certificate.CertificateType == AccreditationCertificateTypes.Instance.SDK_PROVIDER.Value) {
						serviceFees = certificate.InfrastructureServiceFees;
						allocation = Math.Min(Math.Abs(certificate.ProviderBountyshare), 1 - serviceFees); // here we determine the % they take
					}

					results.Add(account, (allocation, serviceFees));
				}

			}

			return results;
		}
	}
}