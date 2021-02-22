using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Neuralia.Blockchains.Core.Services;
using Neuralium.Api.Common;
using Neuralium.Core.Classes.General;

//https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-2.1
namespace Neuralium.Core.Controllers {
	public interface IRpcClient : IRpcClientMethods, INeuraliumApiEvents {
	}

	public interface IRpcServer : IRpcServerMethods {
	}

	[Route("/signal")]
	public class RpcHub<RCP_CLIENT> : Hub<RCP_CLIENT>, IRpcServer
		where RCP_CLIENT : class, IRpcClient {
		protected readonly IRpcProvider rpcProvider;

		public RpcHub() {
			this.rpcProvider = DIService.Instance.GetService<IRpcProvider>();
		}

		public Task StartMining(ushort chainType, string delegateAccountId, int tier = 0) {
			return this.rpcProvider.StartMining(chainType, delegateAccountId, tier);
		}

		public Task StopMining(ushort chainType) {
			return this.rpcProvider.StopMining(chainType);
		}

		public Task<bool> IsMiningEnabled(ushort chainType) {
			return this.rpcProvider.IsMiningEnabled(chainType);
		}

		public Task<bool> IsMiningAllowed(ushort chainType) {
			return this.rpcProvider.IsMiningAllowed(chainType);
		}

		public Task<bool> QueryBlockchainSynced(ushort chainType) {
			return this.rpcProvider.QueryBlockchainSynced(chainType);
		}

		public Task<bool> QueryWalletSynced(ushort chainType) {
			return this.rpcProvider.QueryWalletSynced(chainType);
		}

		public Task<string> GenerateTestPuzzle() {
			return this.rpcProvider.GenerateTestPuzzle();
		}

		public override async Task OnConnectedAsync() {
			await this.Groups.AddToGroupAsync(this.Context.ConnectionId, "SignalR Users").ConfigureAwait(false);
			await base.OnConnectedAsync().ConfigureAwait(false);
		}

		public override async Task OnDisconnectedAsync(Exception exception) {
			await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, "SignalR Users").ConfigureAwait(false);
			await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
		}

	#region Global Queries

		public Task<bool> ToggleServerMessages(bool enable) {
			return this.rpcProvider.ToggleServerMessages(enable);
		}

		public Task EnterWalletPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase, bool setKeysToo = false) {
			return this.rpcProvider.EnterWalletPassphrase(correlationId, chainType, keyCorrelationCode, passphrase);
		}

		public Task EnterKeyPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase) {
			return this.rpcProvider.EnterKeyPassphrase(correlationId, chainType, keyCorrelationCode, passphrase);
		}

		public Task WalletKeyFileCopied(int correlationId, ushort chainType, int keyCorrelationCode) {
			return this.rpcProvider.WalletKeyFileCopied(correlationId, chainType, keyCorrelationCode);
		}

		public Task<int> TestP2pPort(int testPort, bool callback) {
			return this.rpcProvider.TestP2pPort(testPort, callback);
		}

		public Task<object> QuerySystemInfo() {
			return this.rpcProvider.QuerySystemInfo();
		}

		/// <summary>
		///     Return the general information about the chains. Any chain suppored by code is returned. then enabled is if config
		///     permit it to start, and started if it is started or not.
		/// </summary>
		/// <returns>[{"Id":1,"Name":"Neuralium","Enabled":true,"Started":true}]</returns>
		public Task<List<object>> QuerySupportedChains() {
			return this.rpcProvider.QuerySupportedChains();
		}

		public Task SetUILocale(string locale) {
			return this.rpcProvider.SetUILocale(locale);
		}

		public Task<byte> GetMiningRegistrationIpMode(ushort chainType) {
			return this.rpcProvider.GetMiningRegistrationIpMode(chainType);
		}

		public Task<bool> CompleteLongRunningEvent(int correlationId, object data) {
			return this.rpcProvider.CompleteLongRunningEvent(correlationId, data);
		}

		public Task<bool> RenewLongRunningEvent(int correlationId) {
			return this.rpcProvider.RenewLongRunningEvent(correlationId);
		}

		public Task<bool> IsBlockchainSynced(ushort chainType) {
			return this.rpcProvider.IsBlockchainSynced(chainType);
		}

		public Task<bool> IsWalletSynced(ushort chainType) {
			return this.rpcProvider.IsWalletSynced(chainType);
		}

		public Task<int> GetCurrentOperatingMode(ushort chainType) {
			return this.rpcProvider.GetCurrentOperatingMode(chainType);
		}

		public Task<bool> SyncBlockchain(ushort chainType, bool force) {
			return this.rpcProvider.SyncBlockchain(chainType, force);
		}

		public Task<bool> Shutdown() {
			return this.rpcProvider.Shutdown();
		}

		public Task<object> BackupWallet(ushort chainType) {
			return this.rpcProvider.BackupWallet(chainType);
		}

		public Task<bool> RestoreWalletFromBackup(ushort chainType, string backupsPath, string passphrase, string salt, string nonce, int iterations) {
			return this.rpcProvider.RestoreWalletFromBackup(chainType, backupsPath, passphrase, salt, nonce, iterations);
		}
		
		public Task<bool> AttemptWalletRescue(ushort chainType) {
			return this.rpcProvider.AttemptWalletRescue(chainType);
		}

		/// <summary>
		///     ping the server
		/// </summary>
		/// <returns></returns>
		public Task<string> Ping() {
			return this.rpcProvider.Ping();
		}
		
		public Task<object> GetPortMappingStatus()
		{
			return this.rpcProvider.GetPortMappingStatus();
		}

		public Task<bool> ConfigurePortMappingMode(bool useUPnP, bool usePmP, int natDeviceIndex)
		{
			return this.rpcProvider.ConfigurePortMappingMode(useUPnP, usePmP, natDeviceIndex);
		}
		
		public Task<byte> GetPublicIPMode() {
			return this.rpcProvider.GetPublicIPMode();
		}

		public Task<int> QueryTotalConnectedPeersCount() {
			return this.rpcProvider.QueryTotalConnectedPeersCount();
		}

		public Task<List<object>> QueryPeerConnectionDetails()
		{
			return this.rpcProvider.QueryPeerConnectionDetails();
		}

		public Task<bool> QueryMiningPortConnectable() {
			return this.rpcProvider.QueryMiningPortConnectable();
		}

	#endregion

	#region Common Chain Queries

		public Task<object> QueryChainStatus(ushort chainType) {
			return this.rpcProvider.QueryChainStatus(chainType);
		}

		public Task<object> QueryWalletInfo(ushort chainType) {
			return this.rpcProvider.QueryWalletInfo(chainType);
		}

		public Task<byte[]> GenerateSecureHash(byte[] parameter, ushort chainType)
		{
			return this.rpcProvider.GenerateSecureHash(parameter, chainType);
		}

		public Task<object> QueryBlockChainInfo(ushort chainType) {
			return this.rpcProvider.QueryBlockChainInfo(chainType);
		}

		public Task<bool> IsWalletLoaded(ushort chainType) {
			return this.rpcProvider.IsWalletLoaded(chainType);
		}

		public Task<bool> WalletExists(ushort chainType) {
			return this.rpcProvider.WalletExists(chainType);
		}

		public Task<int> LoadWallet(ushort chainType, string passphrase = null) {
			return this.rpcProvider.LoadWallet(chainType, passphrase);
		}

		/// <summary>
		///     Returns the current block height for the chain
		/// </summary>
		/// <param name="chainType"></param>
		/// <returns></returns>
		public Task<long> QueryBlockHeight(ushort chainType) {
			return this.rpcProvider.QueryBlockHeight(chainType);
		}

		public Task<int> QueryDigestHeight(ushort chainType) {
			return this.rpcProvider.QueryDigestHeight(chainType);
		}

		public Task<bool> ResetWalletIndex(ushort chainType) {
			return this.rpcProvider.ResetWalletIndex(chainType);
		}

		public Task<long> QueryLowestAccountBlockSyncHeight(ushort chainType) {
			return this.rpcProvider.QueryLowestAccountBlockSyncHeight(chainType);
		}

		public Task<string> QueryBlock(ushort chainType, long blockId) {
			return this.rpcProvider.QueryBlock(chainType, blockId);
		}

		public Task<object> QueryDecomposedBlock(ushort chainType, long blockId) {
			return this.rpcProvider.QueryDecomposedBlock(chainType, blockId);
		}

		public Task<string> QueryDecomposedBlockJson(ushort chainType, long blockId) {
			return this.rpcProvider.QueryDecomposedBlockJson(chainType, blockId);
		}

		public Task<byte[]> QueryCompressedBlock(ushort chainType, long blockId) {
			return this.rpcProvider.QueryCompressedBlock(chainType, blockId);
		}

		public Task<byte[]> QueryBlockBytes(ushort chainType, long blockId) {
			return this.rpcProvider.QueryBlockBytes(chainType, blockId);
		}

		public Task<string> GetBlockSizeAndHash(ushort chainType, long blockId) {
			return this.rpcProvider.GetBlockSizeAndHash(chainType, blockId);
		}

		public Task<List<object>> QueryBlockBinaryTransactions(ushort chainType, long blockId) {
			return this.rpcProvider.QueryBlockBinaryTransactions(chainType, blockId);
		}

		public Task<int> CreateStandardAccount(ushort chainType, string accountName, int accountType, bool publishAccount, bool encryptKeys, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases) {
			return this.rpcProvider.CreateStandardAccount(chainType, accountName, accountType, publishAccount, encryptKeys, encryptKeysIndividually, passphrases);
		}

		public Task<bool> SetActiveAccount(ushort chainType, string accountCode) {
			return this.rpcProvider.SetActiveAccount(chainType, accountCode);
		}

		public Task<object> QueryAppointmentConfirmationResult(ushort chainType, string accountCode) {
			return this.rpcProvider.QueryAppointmentConfirmationResult(chainType, accountCode);
		}

		public Task<bool> ClearAppointment(ushort chainType, string accountCode) {
			return this.rpcProvider.ClearAppointment(chainType, accountCode);
		}

		public Task<object> CanPublishAccount(ushort chainType, string accountCode) {
			return this.rpcProvider.CanPublishAccount(chainType, accountCode);
		}

		public Task SetSMSConfirmationCode(ushort chainType, string accountCode, long confirmationCode) {
			return this.rpcProvider.SetSMSConfirmationCode(chainType, accountCode, confirmationCode);
		}

		public Task SetSMSConfirmationCodeString(ushort chainType, string accountCode, string confirmationCode) {
			return this.rpcProvider.SetSMSConfirmationCodeString(chainType, accountCode, confirmationCode);
		}

		public Task GenerateXmssKeyIndexNodeCache(ushort chainType, string accountCode, byte ordinal, long index) {
			return this.rpcProvider.GenerateXmssKeyIndexNodeCache(chainType, accountCode, ordinal, index);
		}

		public Task<int> CreateNewWallet(ushort chainType, string accountName, int accountType, bool encryptWallet, bool encryptKey, bool encryptKeysIndividualy, ImmutableDictionary<string, string> passphrases, bool publishAccount) {
			return this.rpcProvider.CreateNewWallet(chainType, accountName, accountType, encryptWallet, encryptKey, encryptKeysIndividualy, passphrases, publishAccount);
		}

		public Task<bool> SetWalletPassphrase(int correlationId, string passphrase, bool setKeysToo) {
			return this.rpcProvider.SetWalletPassphrase(correlationId, passphrase);
		}

		public Task<bool> SetKeysPassphrase(int correlationId, string passphrase) {
			return this.rpcProvider.SetKeysPassphrase(correlationId, passphrase);
		}

		public Task<List<object>> QueryWalletTransactionHistory(ushort chainType, string accountCode) {
			return this.rpcProvider.QueryWalletTransactionHistory(chainType, accountCode);
		}

		public Task<object> QueryWalletTransactionHistoryDetails(ushort chainType, string accountCode, string transactionId) {
			return this.rpcProvider.QueryWalletTransactionHistoryDetails(chainType, accountCode, transactionId);
		}

		public Task<List<object>> QueryWalletAccounts(ushort chainType) {

			return this.rpcProvider.QueryWalletAccounts(chainType);
		}

		public Task<string> QueryDefaultWalletAccountId(ushort chainType) {
			return this.rpcProvider.QueryDefaultWalletAccountId(chainType);
		}

		public Task<string> QueryDefaultWalletAccountCode(ushort chainType) {
			return this.rpcProvider.QueryDefaultWalletAccountCode(chainType);
		}

		public Task<object> QueryWalletAccountDetails(ushort chainType, string accountCode) {
			return this.rpcProvider.QueryWalletAccountDetails(chainType, accountCode);
		}

		public Task<object> QueryWalletAccountAppointmentDetails(ushort chainType, string accountCode) {
			return this.rpcProvider.QueryWalletAccountAppointmentDetails(chainType, accountCode);
		}

		public Task<string> QueryWalletAccountPresentationTransactionId(ushort chainType, string accountCode) {
			return this.rpcProvider.QueryWalletAccountPresentationTransactionId(chainType, accountCode);
		}

		public Task<string> Test(string data) {
			return this.rpcProvider.Test(data);
		}

		public Task<int> RequestAppointment(ushort chainType, string accountCode, int preferredRegion) {
			return this.rpcProvider.RequestAppointment(chainType, accountCode, preferredRegion);
		}

		public Task<int> PublishAccount(ushort chainType, string accountCode) {
			return this.rpcProvider.PublishAccount(chainType, accountCode);
		}

		public Task<List<object>> QueryMiningHistory(ushort chainType, int page, int pageSize, byte maxLevel) {
			return this.rpcProvider.QueryMiningHistory(chainType, page, pageSize, maxLevel);
		}

		public Task<object> QueryMiningStatistics(ushort chainType) {
			return this.rpcProvider.QueryMiningStatistics(chainType);
		}

		public Task<bool> ClearCachedCredentials(ushort chainType) {
			return this.rpcProvider.ClearCachedCredentials(chainType);
		}

		public Task<long> QueryCurrentDifficulty(ushort chainType) {
			return this.rpcProvider.QueryCurrentDifficulty(chainType);
		}

	#endregion

	#region Neuralium chain queries

		public Task<bool> CreateNextXmssKey(ushort chainType, string accountCode, byte ordinal) {
			return this.rpcProvider.CreateNextXmssKey(chainType, accountCode, ordinal);
		}

		public Task<int> SendNeuraliums(string targetAccountId, decimal amount, decimal tip, string note) {
			return this.rpcProvider.SendNeuraliums(targetAccountId, amount, tip, note);
		}

		public Task<object> QueryNeuraliumTimelineHeader(string accountCode) {
			return this.rpcProvider.QueryNeuraliumTimelineHeader(accountCode);
		}

		public Task<object> QueryNeuraliumTimelineSection(string accountCode, DateTime day) {
			return this.rpcProvider.QueryNeuraliumTimelineSection(accountCode, day);
		}

		public Task<byte[]> SignXmssMessage(ushort chainType, string accountCode, byte[] message) {
			return this.rpcProvider.SignXmssMessage(chainType, accountCode, message);
		}

		public Task SetPuzzleAnswers(ushort chainType, List<int> answers) {
			return this.rpcProvider.SetPuzzleAnswers(chainType, answers);
		}

#if TESTNET || DEVNET
		public Task<int> RefillNeuraliums(string accountCode) {
			return this.rpcProvider.RefillNeuraliums(accountCode);
		}
#endif
#if COLORADO_EXCLUSION
		public Task<bool> BypassAppointmentVerification(string accountCode) {
			return this.rpcProvider.BypassAppointmentVerification(accountCode);
		}
#endif

		public Task<object> QueryElectionContext(ushort chainType, long blockId) {
			return this.rpcProvider.QueryElectionContext(chainType, blockId);
		}

		public Task<List<object>> QueryNeuraliumTransactionPool() {
			return this.rpcProvider.QueryNeuraliumTransactionPool();
		}

		public Task<bool> RestoreWalletNarballBackup(string source, string dest) {
			return this.rpcProvider.RestoreWalletNarballBackup(source, dest);
		}

		public Task<object> ReadAppSetting(string name)
		{
			return this.rpcProvider.ReadAppSetting(name);
		}

		public Task<bool> WriteAppSetting(string name, string value)
		{
			return this.rpcProvider.WriteAppSetting(name, value);
		}

		public Task<object> ReadAppSettingDomain(string name)
		{
			return this.rpcProvider.ReadAppSettingDomain(name);
		}

		public Task<object> QueryAccountTotalNeuraliums(string accountCode) {
			return this.rpcProvider.QueryAccountTotalNeuraliums(accountCode);
		}

	#endregion

	}
}