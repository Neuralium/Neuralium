using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Serialization;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Digests;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Envelopes;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.General;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Gated;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.SAFU;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.Security;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Appointments;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Messages.Specialization.General.Elections;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization.Exceptions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Contents;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Operations;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Tools.Serialization;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.General.Appointments;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Messages.Specialization.Moderation;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Serialization {

	public interface INeuraliumEnvelopeRehydrationFactory : IEnvelopeRehydrationFactory {
	}

	public interface INeuraliumBlockRehydrationFactory : IBlockRehydrationFactory {
	}

	public interface INeuraliumMessageRehydrationFactory : IMessageRehydrationFactory {
	}

	public interface INeuraliumTransactionRehydrationFactory : ITransactionRehydrationFactory {
	}

	public interface INeuraliumBlockchainEventsRehydrationFactory : IBlockchainEventsRehydrationFactory, INeuraliumBlockRehydrationFactory, INeuraliumMessageRehydrationFactory, INeuraliumTransactionRehydrationFactory, INeuraliumEnvelopeRehydrationFactory {
	}

	public class NeuraliumBlockchainEventsRehydrationFactory : BlockchainEventsRehydrationFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumBlockchainEventsRehydrationFactory {

		public NeuraliumBlockchainEventsRehydrationFactory(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		public override BlockChannelUtils.BlockChannelTypes ActiveBlockchainChannels => NeuraliumConstants.ActiveBlockchainChannels;
		public override BlockChannelUtils.BlockChannelTypes CompressedBlockchainChannels => NeuraliumConstants.CompressedBlockchainChannels;

		public override ITransaction CreateTransaction(IDehydratedTransaction dehydratedTransaction) {

			using(IDataRehydrator rehydrator = DataSerializationFactory.CreateRehydrator(dehydratedTransaction.Header)) {

				ComponentVersion<TransactionType> version = new ComponentVersion<TransactionType>();
				version.Rehydrate(rehydrator);

				rehydrator.Rewind2Start();

				return this.CreateTransaction(version);
			}
		}

		public override IIndexedTransaction CreateIndexedTransaction(IDehydratedTransaction dehydratedTransaction) {

			ITransaction transaction = this.CreateTransaction(dehydratedTransaction);

			if(transaction is IIndexedTransaction indexedTransaction) {
				return indexedTransaction;
			}

			throw new ApplicationException("Created transaction is not a indexed transaction.");
		}

		//		public override TransactionSerializationMap CreateTransactionDehydrationMap(byte type, byte version, ByteArray keyLengths) {
		//			TransactionSerializationMap map = null;
		//
		//			switch(type) {
		//
		//				case TransactionTypes.GENESIS:
		//
		//					switch(version) {
		//						case 1:
		//							map = new NeuraliumGenesisPrimusTransactionSerializationMap(keyLengths);
		//
		//							break;
		//					}
		//
		//					break;
		//				case TransactionTypes.PRESENTATION:
		//
		//					switch(version) {
		//						case 1:
		//							map = new NeuraliumPresentationTransactionSerializationMap(keyLengths);
		//
		//							break;
		//					}
		//
		//					break;
		//				case TransactionTypes.KEY_CHANGE:
		//
		//					switch(version) {
		//						case 1:
		//							map = new NeuraliumKeyChangeTransactionSerializationMap(keyLengths);
		//
		//							break;
		//					}
		//
		//					break;
		//				case TransactionTypes.GENERIC:
		//
		//					switch(version) {
		//						case 1:
		//							map = new NeuraliumTransactionSerializationMap();
		//
		//							break;
		//					}
		//
		//					break;
		//
		//				//TODO: make sure this is added once moderation happens
		////				case TransactionTypes.MODERATION:
		////					switch (version) {
		////						case 1:
		////							return new NeuraliumModeratorTransactionSerializationMap();
		////					}
		////
		////					break;
		//				default:
		//
		//					throw new ApplicationException("Invalid transaction type");
		//
		//			}
		//
		//			// lets generate it
		//			map.GenerateMap();
		//
		//			return map;
		//		}

		public override TransactionContent CreateTransactionContent(IDataRehydrator rehydrator) {
			//start by peeking and reading both the transactiontype and version
			//			var version = rehydrator.RehydrateRewind<ComponentVersion<TransactionContentType>>();
			//
			//			if(version.Type == TransactionContentTypes.Instance.GENESIS) {
			//				if(version == (1, 0)) {
			//					return null; //new  BlockContent();
			//				}
			//			}

			return null;
		}

		public override IOperation CreateTransactionOperation(IDataRehydrator rehydrator) {
			//start by peeking and reading both the transactiontype and version
			//			var version = rehydrator.RehydrateRewind<ComponentVersion<TransactionOperationType>>();
			//
			//			if(version.Type == TransactionContentTypes.Instance.GENESIS) {
			//				if(version == (1, 0)) {
			//					return null; //new  BlockContent();
			//				}
			//			}

			return null;
		}

		public override IBlockchainDigest CreateDigest(IDehydratedBlockchainDigest dehydratedDigest) {

			IDataRehydrator rehydrator = DataSerializationFactory.CreateRehydrator(dehydratedDigest.Contents);

			return this.CreateDigest(rehydrator);
		}

		public override IBlockchainDigestChannelFactory CreateDigestChannelfactory() {
			return new NeuraliumBlockchainDigestChannelFactory();
		}

		public override IBlock CreateBlock(IDataRehydrator bodyRehydrator) {
			//start by peeking and reading both the transactiontype and version
			ComponentVersion<BlockType> version = bodyRehydrator.RehydrateRewind<ComponentVersion<BlockType>>();

			bodyRehydrator.Rewind2Start();

			IBlock block = null;

			if(version.Type == BlockTypes.Instance.Genesis) {
				if(version == (1, 0)) {
					block = new NeuraliumGenesisBlock();
				}

			} else if(version.Type == BlockTypes.Instance.Simple) {
				if(version == (1, 0)) {
					block = new NeuraliumSimpleBlock();
				}

			} else if(version.Type == BlockTypes.Instance.Election) {
				if(version == (1, 0)) {
					block = new NeuraliumElectionBlock();
				}

			} else {
				throw new UnrecognizedBlockException(this.centralCoordinator.ChainId, this.centralCoordinator.ChainName);
			}

			return block;
		}

		public override IBlock CreateBlock(IDehydratedBlock dehydratedBlock) {

			IDataRehydrator bodyRehydrator = DataSerializationFactory.CreateRehydrator(dehydratedBlock.HighHeader);

			return this.CreateBlock(bodyRehydrator);
		}

		public override IBlockComponentsRehydrationFactory CreateBlockComponentsRehydrationFactory() {
			return new NeuraliumBlockComponentsRehydrationFactory();
		}

		public override IBlockchainMessage CreateMessage(IDehydratedBlockchainMessage dehydratedMessage) {

			using(IDataRehydrator bodyRehydrator = DataSerializationFactory.CreateRehydrator(dehydratedMessage.Contents)) {

				//start by peeking and reading both the transactiontype and version
				ComponentVersion<BlockchainMessageType> version = bodyRehydrator.RehydrateRewind<ComponentVersion<BlockchainMessageType>>();

				bodyRehydrator.Rewind2Start();

				if(version.Type == BlockchainMessageTypes.Instance.DEBUG) {
					if(version == (1, 0)) {
						return new NeuraliumDebugMessage();
					}

				} else if(version.Type == BlockchainMessageTypes.Instance.ELECTIONS_REGISTRATION) {
					if(version == (1, 0)) {
						return new NeuraliumElectionsRegistrationMessage();
					}

				} else if(version.Type == BlockchainMessageTypes.Instance.ACTIVE_ELECTION_CANDIDACY) {
					if(version == (1, 0)) {
						// a very rare case where it is not scoped for the chain
						return new ActiveElectionCandidacyMessage();
					}

				} else if(version.Type == BlockchainMessageTypes.Instance.PASSIVE_ELECTION_CANDIDACY) {
					if(version == (1, 0)) {
						// a very rare case where it is not scoped for the chain
						return new PassiveElectionCandidacyMessage();
					}
				} else if(version.Type == BlockchainMessageTypes.Instance.INITIATION_APPOINTMENT_REQUESTED) {
					if(version == (1, 0)) {
						// a very rare case where it is not scoped for the chain
						return new NeuraliumInitiationAppointmentRequestMessage();
					}
				} else if(version.Type == BlockchainMessageTypes.Instance.APPOINTMENT_REQUESTED) {
					if(version == (1, 0)) {
						// a very rare case where it is not scoped for the chain
						return new NeuraliumAppointmentRequestMessage();
					}
				} else if(version.Type == BlockchainMessageTypes.Instance.APPOINTMENT_REQUEST_CONFIRMATION) {
					if(version == (1, 0)) {
						// a very rare case where it is not scoped for the chain
						return new NeuraliumAppointmentRequestConfirmationMessage();
					}
				} else if(version.Type == BlockchainMessageTypes.Instance.APPOINTMENT_CONTEXT) {
					if(version == (1, 0)) {
						// a very rare case where it is not scoped for the chain
						return new NeuraliumAppointmentContextMessage();
					}
				} else if(version.Type == BlockchainMessageTypes.Instance.APPOINTMENT_TRIGGER) {
					if(version == (1, 0)) {
						// a very rare case where it is not scoped for the chain
						return new NeuraliumAppointmentTriggerMessage();
					}
				} else if(version.Type == BlockchainMessageTypes.Instance.APPOINTMENT_VERIFICATION_RESULTS) {
					if(version == (1, 0)) {
						// a very rare case where it is not scoped for the chain
						return new NeuraliumAppointmentVerificationResultsMessage();
					}
				} else if(version.Type == BlockchainMessageTypes.Instance.APPOINTMENT_VERIFICATION_CONFIRMATION) {
					if(version == (1, 0)) {
						// a very rare case where it is not scoped for the chain
						return new NeuraliumAppointmentVerificationConfirmationMessage();
					}
				} else {
					throw new UnrecognizedMessageException(this.centralCoordinator.ChainId, this.centralCoordinator.ChainName);
				}

				return null;
			}
		}

		public override IBlockchainDigest CreateDigest(IDataRehydrator rehydrator) {
			//start by peeking and reading both the transactiontype and version
			ComponentVersion<BlockchainDigestsType> version = rehydrator.RehydrateRewind<ComponentVersion<BlockchainDigestsType>>();

			rehydrator.Rewind2Start();

			if(version.Type == BlockchainDigestsTypes.Instance.Basic) {
				if(version == (1, 0)) {
					return new NeuraliumBlockchainDigest();
				}

			} else {
				throw new UnrecognizedDigestException(this.centralCoordinator.ChainId, this.centralCoordinator.ChainName);
			}

			return null;
		}

		private ITransaction CreateTransaction(ComponentVersion<TransactionType> version) {
			ITransaction transaction = null;

			if(transaction == null) {

				if(version.Type == TransactionTypes.Instance.GENESIS) {
					if(version == (1, 0)) {
						transaction = new NeuraliumGenesisModeratorAccountPresentationTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.MODERATION_PRESENTATION) {
					if(version == (1, 0)) {
						transaction = new NeuraliumGenesisAccountPresentationTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.MODERATION_ELECTION_POOL_PRESENTATION) {
					if(version == (1, 0)) {
						transaction = new NeuraliumGenesisElectionPoolPresentationTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.STANDARD_PRESENTATION) {
					if(version == (1, 0)) {
						transaction = new NeuraliumStandardPresentationTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.JOINT_PRESENTATION) {
					if(version == (1, 0)) {
						transaction = new NeuraliumJointPresentationTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.KEY_CHANGE) {
					if(version == (1, 0)) {
						transaction = new NeuraliumStandardAccountKeyChangeTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.SET_ACCOUNT_RECOVERY) {
					if(version == (1, 0)) {
						//transaction = new NeuraliumSetAccountRecoveryTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.MODERATION_OPERATING_RULES) {
					if(version == (1, 0)) {
						transaction = new NeuraliumChainOperatingRulesTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.ACCREDITATION_CERTIFICATE) {
					if(version == (1, 0)) {
						transaction = new NeuraliumChainAccreditationCertificateTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.MODERATION_KEY_CHANGE) {
					if(version == (1, 0)) {
						transaction = new NeuraliumModeratorKeyChangeTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.MODERATION_RECLAIM_ACCOUNTS) {
					if(version == (1, 0)) {
						//transaction = new NeuraliumReclaimAccountsTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.MODERATION_ASSIGN_ACCOUNT_CORRELATIONS) {
					if(version == (1, 0)) {
						transaction = new NeuraliumAssignAccountCorrelationsTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.MODERATION_ACCOUNT_RESET_WARNING) {
					if(version == (1, 0)) {
						transaction = new NeuraliumAccountResetWarningTransaction();
					}

				} else if(version.Type == TransactionTypes.Instance.MODERATION_ACCOUNT_RESET) {
					if(version == (1, 0)) {
						transaction = new NeuraliumAccountResetTransaction();
					}
				} else if(version.Type == TransactionTypes.Instance.GATED_JUDGEMENT_TRANSACTION) {
					if(version == (1, 0)) {
						transaction = new NeuraliumGatedJudgementTransaction();
					}
				}

				// neuralium only transactions

				else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_TRANSFER) {
					if(version == (1, 0)) {
						transaction = new NeuraliumTransferTransaction();
					}
				} else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_MULTI_TRANSFER) {
					if(version == (1, 0)) {
						transaction = new NeuraliumMultiTransferTransaction();
					}

				} else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_MODERATOR_DESTROY_TOKENS) {
					if(version == (1, 0)) {
						transaction = new DestroyNeuraliumsTransaction();
					}
				} else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_FREEZE_SUSPICIOUSACCOUNTS) {
					if(version == (1, 0)) {
						transaction = new NeuraliumFreezeSuspiciousFundsTransaction();
					}
				} else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_UNWIND_STOLEN_SUSPICIOUSACCOUNTS) {
					if(version == (1, 0)) {
						transaction = new NeuraliumUnwindStolenFundsTreeTransaction();
					}
				} else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_UNFREEZE_SUSPICIOUSACCOUNTS) {
					if(version == (1, 0)) {
						transaction = new NeuraliumUnfreezeClearedFundsTransaction();
					}
				} else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_SAFU_TRANSFER) {
					if(version == (1, 0)) {
						transaction = new NeuraliumSAFUTransferTransaction();
					}
				} else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_SAFU_UPDATE_RATIO) {
					if(version == (1, 0)) {
						transaction = new NeuraliumDailySAFURatioTransaction();
					}
				} else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_SAFU_CONTRIBUTIONS) {
					if(version == (1, 0)) {
						transaction = new NeuraliumSAFUContributionTransaction();
					}
				} else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_THREE_WAY_GATED_TRANSFER_TRANSACTION) {
					if(version == (1, 0)) {
						transaction = new NeuraliumThreeWayGatedTransferTransaction();
					}
				}
#if TESTNET || DEVNET
				else if(version.Type == NeuraliumTransactionTypes.Instance.NEURALIUM_REFILL_NEURLIUMS) {
					if(version == (1, 0)) {
						transaction = new NeuraliumRefillNeuraliumsTransaction();
					}
				}
#endif

			}

			if(transaction == null) {
				throw new UnrecognizedTransactionException(this.centralCoordinator.ChainId, this.centralCoordinator.ChainName);
			}

			return transaction;
		}

		public override IEnvelope CreateNewEnvelope(ComponentVersion<EnvelopeType> version) {
			if(version == EnvelopeTypes.Instance.SignedMessage) {
				if(version == (1, 0)) {
					return new NeuraliumSignedMessageEnvelope();
				}
			} else if(version == EnvelopeTypes.Instance.Block) {
				if(version == (1, 0)) {
					return new NeuraliumBlockEnvelope();
				}
			} else if(version == EnvelopeTypes.Instance.SignedTransaction) {
				if(version == (1, 0)) {
					return new NeuraliumTransactionEnvelope();
				}
			}
			else if(version == EnvelopeTypes.Instance.PresentationTransaction) {
				if(version == (1, 0)) {
					return new NeuraliumPresentationTransactionEnvelope();
				}
			}
			else if(version == EnvelopeTypes.Instance.InitiationAppointment) {
				if(version == (1, 0)) {
					return new NeuraliumInitiationAppointmentMessageEnvelope();
				}
			}  
			else if(version == EnvelopeTypes.Instance.ModeratorSignedMessage) {
				if(version == (1, 0)) {
					return new NeuraliumModeratorSignedMessageEnvelope();
				}
			}
			else {
				throw new UnrecognizedEnvelopeException(this.centralCoordinator.ChainId, this.centralCoordinator.ChainName);
			}

			return null;
		}
	}
}