using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI.Wallet;
using Neuralia.Blockchains.Core.Services;
using Neuralium.Core.Classes.General;

//https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-2.1
namespace Neuralium.Core.Controllers {
	public interface IRpcClient : IRpcClientMethods, IRpcClientEvents {
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

		public Task StartMining(ushort chainType, string delegateAccountId) {
			return this.rpcProvider.StartMining(chainType, delegateAccountId);
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

		public override async Task OnConnectedAsync() {
			await this.Groups.AddToGroupAsync(this.Context.ConnectionId, "SignalR Users");
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception) {
			await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, "SignalR Users");
			await base.OnDisconnectedAsync(exception);
		}

	#region Global Queries

		public Task<bool> ToggleServerMessages(bool enable) {
			return this.rpcProvider.ToggleServerMessages(enable);
		}

		public Task EnterWalletPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase) {
			return this.rpcProvider.EnterWalletPassphrase(correlationId, chainType, keyCorrelationCode, passphrase);
		}

		public Task EnterKeyPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string passphrase) {
			return this.rpcProvider.EnterKeyPassphrase(correlationId, chainType, keyCorrelationCode, passphrase);
		}

		public Task WalletKeyFileCopied(int correlationId, ushort chainType, int keyCorrelationCode) {
			return this.rpcProvider.WalletKeyFileCopied(correlationId, chainType, keyCorrelationCode);
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

		public Task<bool> SyncBlockchain(ushort chainType, bool force) {
			return this.rpcProvider.SyncBlockchain(chainType, force);
		}

		public Task<bool> Shutdown() {
			return this.rpcProvider.Shutdown();
		}

		public Task<object> BackupWallet(ushort chainType) {
			return this.rpcProvider.BackupWallet(chainType);
		}

		/// <summary>
		///     ping the server
		/// </summary>
		/// <returns></returns>
		public Task Test() {
			return Task.CompletedTask;
		}

		/// <summary>
		///     ping the server
		/// </summary>
		/// <returns></returns>
		public Task<string> Ping() {
			return this.rpcProvider.Ping();
		}

		public Task<int> QueryTotalConnectedPeersCount() {
			return this.rpcProvider.QueryTotalConnectedPeersCount();
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
			return this.rpcProvider.LoadWallet(chainType);
		}

		/// <summary>
		///     Returns the current block height for the chain
		/// </summary>
		/// <param name="chainType"></param>
		/// <returns></returns>
		public Task<long> QueryBlockHeight(ushort chainType) {
			return this.rpcProvider.QueryBlockHeight(chainType);
		}

		public Task<string> QueryBlock(ushort chainType, long blockId) {
			return this.rpcProvider.QueryBlock(chainType, blockId);
		}

		public Task<byte[]> QueryCompressedBlock(ushort chainType, long blockId) {
			return this.rpcProvider.QueryCompressedBlock(chainType, blockId);
		}

		public Task<List<object>> QueryBlockBinaryTransactions(ushort chainType, long blockId) {
			return this.rpcProvider.QueryBlockBinaryTransactions(chainType, blockId);
		}

		public Task<int> CreateAccount(ushort chainType, string accountName, bool publishAccount, bool encryptKeys, bool encryptKeysIndividually, ImmutableDictionary<string, string> passphrases) {
			return this.rpcProvider.CreateAccount(chainType, accountName, publishAccount, encryptKeys, encryptKeysIndividually, passphrases);
		}

		public Task<bool> SetActiveAccount(ushort chainType, Guid accountUuid) {
			return this.rpcProvider.SetActiveAccount(chainType, accountUuid);
		}
		
		public Task<int> CreateNewWallet(ushort chainType, string accountName, bool encryptWallet, bool encryptKey, bool encryptKeysIndividualy, ImmutableDictionary<string, string> passphrases, bool publishAccount) {
			return this.rpcProvider.CreateNewWallet(chainType, accountName, encryptWallet, encryptKey, encryptKeysIndividualy, passphrases, publishAccount);
		}

		public Task<bool> SetWalletPassphrase(int correlationId, string passphrase) {
			return this.rpcProvider.SetWalletPassphrase(correlationId, passphrase);
		}

		public Task<bool> SetKeysPassphrase(int correlationId, string passphrase) {
			return this.rpcProvider.SetKeysPassphrase(correlationId, passphrase);
		}

		public Task<List<object>> QueryWalletTransactionHistory(ushort chainType, Guid accountUuid) {
			return this.rpcProvider.QueryWalletTransactionHistory(chainType, accountUuid);
		}

		public Task<object> QueryWalletTransationHistoryDetails(ushort chainType, Guid accountUuid, string transactionId) {
			return this.rpcProvider.QueryWalletTransationHistoryDetails(chainType, accountUuid, transactionId);
		}
		
		public Task<List<object>> QueryWalletAccounts(ushort chainType) {
			
			return this.rpcProvider.QueryWalletAccounts(chainType);
		}

		public Task<object> QueryWalletAccountDetails(ushort chainType, Guid accountUuid) {
			return this.rpcProvider.QueryWalletAccountDetails(chainType, accountUuid);
		}

		public Task<string> QueryWalletAccountPresentationTransactionId(ushort chainType, Guid accountUuid) {
			return this.rpcProvider.QueryWalletAccountPresentationTransactionId(chainType, accountUuid);
		}

		public Task<int> PublishAccount(ushort chainType, Guid? accountUuId) {
			return this.rpcProvider.PublishAccount(chainType, accountUuId);
		}

		public Task<List<object>> QueryMiningHistory(ushort chainType, int page, int pageSize, byte maxLevel) {
			return this.rpcProvider.QueryMiningHistory(chainType, page, pageSize, maxLevel);
		}
		
	#endregion

	#region Neuralium chain queries

		public Task<bool> CreateNextXmssKey(ushort chainType, Guid accountUuid, byte ordinal) {
			return this.rpcProvider.CreateNextXmssKey(chainType, accountUuid, ordinal);
		}

		public Task<int> SendNeuraliums(string targetAccountId, decimal amount, decimal tip, string note) {
			return this.rpcProvider.SendNeuraliums(targetAccountId, amount, tip, note);
		}

		public Task<object> QueryNeuraliumTimelineHeader(Guid accountUuid) {
			return this.rpcProvider.QueryNeuraliumTimelineHeader(accountUuid);
		}

		public Task<List<object>> QueryNeuraliumTimelineSection(Guid accountUuid, DateTime firstday, int skip, int take) {
			return this.rpcProvider.QueryNeuraliumTimelineSection(accountUuid, firstday, skip, take);
		}

		public Task<byte[]> SignXmssMessage(ushort chainType, Guid accountUuid, byte[] message) {
			return this.rpcProvider.SignXmssMessage(chainType, accountUuid, message);
		}

#if TESTNET || DEVNET
		public Task<int> RefillNeuraliums(Guid accountUuid) {
			return this.rpcProvider.RefillNeuraliums(accountUuid);
		}
#endif

		public Task<object> QueryElectionContext(ushort chainType, long blockId) {
			return this.rpcProvider.QueryElectionContext(chainType, blockId);
		}

		public Task<List<object>> QueryNeuraliumTransactionPool() {
			return this.rpcProvider.QueryNeuraliumTransactionPool();
		}

		public Task<bool> RestoreWalletBackup(string source, string dest) {
			return this.rpcProvider.RestoreWalletBackup(source, dest);
		}

		public Task<object> QueryAccountTotalNeuraliums(Guid accountId) {
			return this.rpcProvider.QueryAccountTotalNeuraliums(accountId);
		}

	#endregion

	}
}