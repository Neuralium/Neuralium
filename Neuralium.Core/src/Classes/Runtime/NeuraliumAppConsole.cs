using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Widgets;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Keys;
using Neuralia.Blockchains.Common.Classes.Services;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Compression;
using Neuralia.Blockchains.Core.General;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.P2p.Connections;
using Neuralia.Blockchains.Core.Services;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Data.Arrays;
using Neuralia.Blockchains.Tools.Serialization;
using Neuralium.Core.Classes.Configuration;
using Neuralium.Core.Classes.Services;
using System.Text.Json;
using Serilog;

namespace Neuralium.Core.Classes.Runtime {
	public interface INeuraliumAppDebug : INeuraliumApp {
	}

	public class NeuraliumAppConsole : NeuraliumAppConsole<AppSettings> {

		public NeuraliumAppConsole(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime, IRpcService rpcService, IOptions<AppSettings> appSettings, NeuraliumOptions options, IBlockchainTimeService timeService, IBlockchainNetworkingService networkingService, IGlobalsService globalService) : base(serviceProvider, applicationLifetime, rpcService, appSettings, options, timeService, networkingService, globalService) {
		}
	}

	public class NeuraliumAppConsole<APP_SETTINGS> : NeuraliumApp<APP_SETTINGS>, INeuraliumAppDebug
		where APP_SETTINGS : AppSettings, new() {

		private bool lineEntered;

		public NeuraliumAppConsole(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime, IRpcService rpcService, IOptions<APP_SETTINGS> appSettings, NeuraliumOptions options, IBlockchainTimeService timeService, IBlockchainNetworkingService networkingService, IGlobalsService globalsService) : base(serviceProvider, applicationLifetime, rpcService, appSettings, options, timeService, networkingService, globalsService) {
		}

		protected override void RunLoop() {
			base.RunLoop();

			if(this.CmdOptions.DebugConsole) {
				this.CaptureCommand();
			}
		}

		protected virtual void CaptureCommand() {
			if(this.lineEntered) {
				Console.WriteLine("");
				Console.WriteLine("");

				Console.WriteLine("enter command:");
				this.lineEntered = false;
			}

			bool success = Reader.TryReadLine(out string line, 1000);

			if(success) {
				if(!string.IsNullOrWhiteSpace(line)) {
					try {
						var items = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
						this.ProcessKey(items);

					} catch(Exception ex) {
						Log.Error(ex, "capture error");
					} finally {
						this.lineEntered = true;
					}
				}
			}
		}
		
		//DEBUG
		protected virtual async void ProcessKey(string[] items) {

#if TESTNET || DEVNET
			if(items[0] == "refill") {
				this.neuraliumBlockChainInterface.RefillNeuraliums(Guid.Empty, new CorrelationContext());

				return;
			}

			if(items[0] == "refill5") {
				for(int i = 0; i < 5; i++) {
					this.neuraliumBlockChainInterface.RefillNeuraliums(Guid.Empty, new CorrelationContext());
				}

				return;
			}
#endif

			if(items[0] == "create") {
				this.neuraliumBlockChainInterface.CreateNewWallet( new CorrelationContext(), "test",false, false, false, null, false);

				return;
			}
			
			if(items[0] == "con") {
				var conns = this.neuraliumBlockChainInterface.QueryPeersList();

				Console.WriteLine($"We have {conns.Count} connections");

				foreach(string entry in conns) {
					Console.WriteLine($"{entry}");
				}

				return;
			}
			
			if(items[0] == "ips") {
				var conns = this.neuraliumBlockChainInterface.QueryIPsList();

				Console.WriteLine($"IPs in our IP pool");
				foreach(var ip in conns) {
					Console.WriteLine($"{ip}");
				}
				Console.WriteLine($"-----------------------");

				return;
			}

			if(items[0] == "conc") {
				int conns = this.neuraliumBlockChainInterface.QueryPeerCount();

				Console.WriteLine($"We have {conns} connections");

				return;
			}

			if(items[0] == "bh") {
				long conns = await this.neuraliumBlockChainInterface.QueryBlockHeight().awaitableTask;

				Console.WriteLine($"Current block height: {conns}");

				return;
			}
			
			if(items[0] == "bi") {
				var status = await this.neuraliumBlockChainInterface.QueryBlockChainInfo().awaitableTask;

				Console.WriteLine($"Current download block height: {status.DownloadBlockId}");
				Console.WriteLine($"Current disk block height: {status.DiskBlockId}");
				Console.WriteLine($"Current interpreted block height: {status.BlockId}");
				Console.WriteLine($"Public block height: {status.PublicBlockId}");
				
				return;
			}

			if(items[0] == "newwal") {

				var passprhases = new Dictionary<int, string>();
				passprhases.Add(0, "qwerty");
				passprhases.Add(1, "qwerty1");
				passprhases.Add(2, "qwerty2");
				passprhases.Add(3, "qwerty3");

				bool result = await this.neuraliumBlockChainInterface.CreateNewWallet(new CorrelationContext(), items[1], true, true, true, passprhases.ToImmutableDictionary(), false).awaitableTask;

				Console.WriteLine($"create new account, result {result}");

				return;
			}

			if(items[0] == "newacc") {

				bool result = await this.neuraliumBlockChainInterface.CreateNewAccount(new CorrelationContext(), items[1], false, false, null).awaitableTask;

				Console.WriteLine($"create new account, result {result}");

				return;
			}

			if(items[0] == "setacc") {
				bool result = await this.neuraliumBlockChainInterface.SetActiveAccount(items[1]).awaitableTask;

				Console.WriteLine($"set new account, result {result}");

				return;
			}

			if(items[0] == "send") {
				this.neuraliumBlockChainInterface.SendNeuraliums(AccountId.FromString(items[1]), double.Parse(items[2]), 0, null, new CorrelationContext());

				return;
			}

			if(items[0] == "present") {
				CorrelationContext cc = new CorrelationContext();
				cc.InitializeNew();
				this.neuraliumBlockChainInterface.PresentAccountPublicly(cc, null);

				return;
			}

			if(items[0] == "sign") {

				AccountId ss = AccountId.FromString("{SJ}");
				var l = ss.ToLongRepresentation();
				long password = 181818;
				byte[] bytes = new byte[8];
				TypeSerializer.Serialize(password, bytes.AsSpan());
				string sdas = (ByteArray.Wrap(bytes)).ToBase64();

				var signature = await this.neuraliumBlockChainInterface.SignXmssMessage(Guid.Empty, ByteArray.WrapAndOwn(bytes)).awaitableTask;

				Console.WriteLine(signature.Entry.ToBase64());

				return;
			}
			
			if(items[0] == "timeline") {
				CorrelationContext cc = new CorrelationContext();
				cc.InitializeNew();
				TimelineHeader result = await this.neuraliumBlockChainInterface.QueryNeuraliumTimelineHeader(Guid.Empty);

				var result2 = await this.neuraliumBlockChainInterface.QueryNeuraliumTimelineSection(Guid.Empty, DateTime.Parse(result.FirstDay), 0, 5);

				return;
			}

			if(items[0] == "test") {
				long para = 1;

				if(items.Length == 2) {
					para = long.Parse(items[1]);
				}
				//this.neuraliumBlockChainInterface.Test("");

				var block = await this.neuraliumBlockChainInterface.QueryBlock(para).awaitableTask;
				Log.Information(block);
				
				//var ff = this.test(gg.ConfirmedGeneralTransactions.Values.ToList()[5]);
				// GzipCompression compressor = new GzipCompression();
				//
				// 	ArrayWrapper bytes = compressor.Decompress((ByteArray) );
				// 	
				// this.neuraliumBlockChainInterface.SyncBlockchainExternal(txt);
				//
				// var result3 = await this.neuraliumBlockChainInterface.QueryBlockBinaryTransactions(para).awaitableTask;
				//
				// foreach(var trx in result3) {
				// 	Console.WriteLine($"{trx.Key}--\"{((ByteArray) trx.Value).ToBase64()}\"");
				// }

				//var datas = await this.neuraliumBlockChainInterface.QueryBlockBinaryTransactions(para).awaitableTask;
				
				Log.Information("test results: ");
			}

			if(items[0] == "query") {
				string block = await this.neuraliumBlockChainInterface.QueryBlock(int.Parse(items[1])).awaitableTask;
				Log.Information(block);

				File.WriteAllText("/home/jdb/block2.json", block);
			}

			if(items[0] == "gen") {

				IBlock block = await this.neuraliumBlockChainInterface.LoadBlock(int.Parse(items[1])).awaitableTask;
				NeuraliumSynthesizedBlockApi veee = new NeuraliumSynthesizedBlockApi();

				veee.BlockId = block.BlockId.Value;

				AccountId accountId = new AccountId("{9}");
				veee.AccountId = accountId.ToString();

				foreach(var trx in block.GetAllConfirmedTransactions()) {

					IDehydratedTransaction dehydrated = trx.Value.Dehydrate(BlockChannelUtils.BlockChannelTypes.All);
					IDataDehydrator dehydrator = DataSerializationFactory.CreateDehydrator();
					dehydrated.Dehydrate(dehydrator);
					var ff = dehydrator.ToArray().ToExactByteArrayCopy();
					veee.ConfirmedTransactions.Add(trx.Key.ToString(), ff);
				}

				foreach(RejectedTransaction reject in block.RejectedTransactions) {

					veee.RejectedTransactions.Add(reject.ToString(), reject.Reason.Value);
				}

				foreach(IFinalElectionResults result in block.FinalElectionResults) {
					NeuraliumSynthesizedBlockApi.NeuraliumSynthesizedElectionResultAPI res = new NeuraliumSynthesizedBlockApi.NeuraliumSynthesizedElectionResultAPI();
					res.Offset = result.BlockOffset;

					if(result is INeuraliumFinalElectionResults neuraliumFinalElectedResults) {

						if(neuraliumFinalElectedResults.ElectedCandidates.ContainsKey(accountId)) {
							if(neuraliumFinalElectedResults.ElectedCandidates[accountId] is INeuraliumElectedResults neuraliumElectedResults) {
								res.BountyShare = neuraliumElectedResults.BountyShare;
								res.Tips = 1;
							}
						}

					}

					veee.FinalElectionResults.Add(res);
				}

				var settingseee = JsonUtils.CreatePrettySerializerSettings();


				string resulssst = JsonSerializer.Serialize(veee, settingseee);

				File.WriteAllText("/home/jdb/genesis.json", resulssst);
			}

			if(items[0] == "total") {
				TotalAPI total = await this.neuraliumBlockChainInterface.QueryWalletTotal(Guid.Empty).awaitableTask;

				Console.WriteLine($"We have {total.Total} neuraliums in total with {total.ReservedCredit} reserved credit and {total.ReservedDebit} reserved debit. {total.Total - total.ReservedDebit} are available for spending");

				return;
			}

			if(items[0] == "q") {
				Console.WriteLine("quitting..");
				this.Shutdown();
			}

			if(items[0] == "w") {
				this.neuraliumBlockChainInterface.LoadWallet(default);
			}

			if(items[0] == "change") {
				Console.WriteLine("Creating key change transaction...");

				bool result = await this.neuraliumBlockChainInterface.ChangeKey(GlobalsService.TRANSACTION_KEY_ORDINAL_ID, null, new CorrelationContext()).awaitableTask;

				Console.WriteLine("Key change done...");
			}

			if(items[0] == "loadkey") {
				Console.WriteLine("loading key...");

				Guid acc = this.neuraliumBlockChainInterface.CentralCoordinator.ChainComponentProvider.WalletProvider.GetAccountUuid();
				IWalletKey key = this.neuraliumBlockChainInterface.CentralCoordinator.ChainComponentProvider.WalletProvider.LoadKey(acc, GlobalsService.TRANSACTION_KEY_NAME);

				Console.WriteLine("Key load done...");
			}

			if(items[0] == "loadgen") {
				Console.WriteLine("loading key...");

				IBlock results = await this.neuraliumBlockChainInterface.LoadBlock(1).awaitableTask;

				ByteArray previousHash = ByteArray.WrapAndOwn(new byte[] {1, 2, 3, 4, 5});
				SafeArrayHandle hash1 = BlockchainHashingUtils.GenerateBlockHash(results, previousHash);

				Console.WriteLine("Key load done...");
			}

			if(items[0] == "historym") {
				Console.WriteLine("showing mining history...");

				var result = this.neuraliumBlockChainInterface.QueryMiningHistory();

				foreach(MiningHistory entry in result) {
					Log.Information($"Mining entry: {entry}");
				}
			}

			if(items[0] == "historyt") {
				Console.WriteLine("showing transaction history...");

				var results = await this.neuraliumBlockChainInterface.QueryWalletTransactionHistory(Guid.Empty).awaitableTask;

				foreach(WalletTransactionHistoryHeaderAPI entry in results) {
					Log.Information($"Transaction entry: {entry}");
				}
			}

			if(items[0] == "m") {
				Console.WriteLine("Creating debug message...");

				//Console.WriteLine("Clearing {0} debug transaction Guids", this.Guids.Count);

				//this.Guids.Clear();

				this.neuraliumBlockChainInterface.SubmitDebugMessage(uid => {
					//this.Guids.Clear();
					//this.Guids.Add((uid.Item1, uid.Item2));
				});

				Console.WriteLine("Creation done...");
			}

			if(items[0] == "mine") {
				Console.WriteLine("registering for mining...");

				if(items[1] == "1") {
					this.neuraliumBlockChainInterface.EnableMining();
				}

				if(items[1] == "0") {
					this.neuraliumBlockChainInterface.DisableMining();
				}

				Console.WriteLine("mining registration done...");
			}

			if(items[0] == "l") {
				Console.WriteLine("=====================================================");
				Console.WriteLine("Connected to peers:");

				foreach(PeerConnection node in this.networkingService.ConnectionStore.AllConnectionsList) {
					Console.WriteLine($"Peer: {node.ScoppedAdjustedIp}:{node.NodeAddressInfoInfo.RealPort}");
				}

				Console.WriteLine("=====================================================");
			}

			if(items[0] == "t") {
				this.neuraliumBlockChainInterface.PrintChainDebug("prnt");
			}

			if(items[0] == "e") {
				this.neuraliumBlockChainInterface.PrintChainDebug("effectors");
			}

			if(items[0] == "x") {
				//Console.WriteLine("Clearing {0} debug transaction Guids", this.Guids.Count);
				//this.Guids.Clear();
			}

			if(items[0] == "gossip") {
				Console.WriteLine("Sending debug gossip message");

				//networkingService.SendNewGossipMessage();
			}

			if(items[0] == "c") {

				var accounts = await this.neuraliumBlockChainInterface.QueryWalletAccounts().awaitableTask;

				this.neuraliumBlockChainInterface.QueryWalletAccountPresentationTransactionId(accounts[0].AccountUuid);

				//							if(items.Length > 1) {
				//								TransactionId uuid = TransactionId.Parse(items[1]);
				//								this.neuraliumBlockChainInterface.SubmitDebugConfirm(uuid, hash: (ByteArray)new byte[0]);
				//							} else {
				//								foreach((TransactionId, ArrayWrapper) item in this.Guids) {
				//									this.neuraliumBlockChainInterface.SubmitDebugConfirm(item.Item1, hash: item.Item2);
				//								}
				//							}
			}

			if(items[0] == "load") {
				//							NeuraliumSerializationManager manager = new NeuraliumSerializationManager(null);
				//
				//							ITransaction transaction = manager.LoadArchiveTransaction(Guid.Parse("94738ec5-ef0f-4e0b-a544-0162c080d272"));
				//
				//							int i = 0;
			}

			if(items[0] == "sync") {
				Console.WriteLine("Synching chain...");
				this.neuraliumBlockChainInterface.TriggerChainSynchronization();

			}

			if(items[0] == "syncw") {
				Console.WriteLine("Synching wallet...");
				this.neuraliumBlockChainInterface.TriggerChainWalletSynchronization();

			}

			if(items[0] == "s") {
				// send some data
				//							try {
				//								AppSettingsBase.NodeAddressInfo        nodeAddressInfo      = this.appSettingsBase.nodes.First();
				//								ClientHandshakeWorkflow hashshake = ClientWorkflowFactory.CreateRequestHandshakeWorkflow(new NetworkEndPoint(IPAddress.Parse(nodeAddressInfo.ip), port: nodeAddressInfo.port, mode: IPMode.IPv4));
				//
				//								this.networkingService.WorkflowCoordinator.AddWorkflow(hashshake);
				//							} catch(Exception ex) {
				//								Log.Error(ex, message: "failed to create handshake");
				//							}
			}
		}

		//DEBUG
		private class Reader {
			private static readonly Thread inputThread;
			private static readonly AutoResetEvent getInput;
			private static readonly AutoResetEvent gotInput;
			private static string input;
			private static readonly string giberish = ";1R";

			static Reader() {
				try {
					getInput = new AutoResetEvent(false);
					gotInput = new AutoResetEvent(false);
					inputThread = new Thread(reader);
					inputThread.IsBackground = true;
					inputThread.Start();
				} catch(Exception ex) {
					Log.Error(ex, "Failed to start console input reader.");
				}
			}

			private static void reader() {

				try {
					while(true) {
						getInput.WaitOne();
						input = Console.ReadLine();

						int index = input.IndexOf(giberish, StringComparison.Ordinal);

						if(index != -1) {
							input = input.Substring(index + giberish.Length, input.Length - index - giberish.Length);
						}

						gotInput.Set();
					}
				} catch(Exception ex) {
					Log.Error(ex, "Failed to read from console input reader.");
				}
			}

			// omit the parameter to read a line without a timeout
			public static string ReadLine(int timeOutMillisecs = Timeout.Infinite) {
				getInput.Set();
				bool success = gotInput.WaitOne(timeOutMillisecs);

				if(success) {
					return input;
				}

				throw new TimeoutException("User did not provide input within the timelimit.");
			}

			public static bool TryReadLine(out string line, int timeOutMillisecs = Timeout.Infinite) {
				getInput.Set();
				bool success = gotInput.WaitOne(timeOutMillisecs);

				if(success) {
					line = input;
				} else {
					line = null;
				}

				return success;
			}
		}
	}
}