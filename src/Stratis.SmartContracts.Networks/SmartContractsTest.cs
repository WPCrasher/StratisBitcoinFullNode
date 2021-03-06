﻿using System;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using Stratis.Bitcoin.Features.SmartContracts.PoW;
using Stratis.Bitcoin.Networks.Deployments;

namespace Stratis.Bitcoin.Features.SmartContracts.Networks
{
    public sealed class SmartContractsTest : Network
    {
        public SmartContractsTest()
        {
            this.Name = "SmartContractsTestNet";
            this.RootFolderName = SmartContractNetwork.StratisRootFolderName;
            this.DefaultConfigFilename = SmartContractNetwork.StratisDefaultConfigFilename;
            this.Magic = 0x0709110E; // Incremented 19/06
            this.DefaultPort = 18333;
            this.RPCPort = 18332;
            this.MaxTipAge = SmartContractNetwork.BitcoinDefaultMaxTipAgeInSeconds;
            this.MinTxFee = 1000;
            this.FallbackFee = 20000;
            this.MinRelayTxFee = 1000;
            this.MaxTimeOffsetSeconds = 25 * 60;

            var consensusFactory = new SmartContractPowConsensusFactory();

            Block genesisBlock = SmartContractNetwork.CreateGenesis(consensusFactory, 1296688602, 414098458, 0x1d00ffff, 1, Money.Coins(50m));
            ((SmartContractBlockHeader)genesisBlock.Header).HashStateRoot = new uint256("21B463E3B52F6201C0AD6C991BE0485B6EF8C092E64583FFA655CC1B171FE856");
            genesisBlock.Header.Nonce = 3; // Incremented 19/06

            this.Genesis = genesisBlock;

            // Taken from StratisX.
            var consensusOptions = new PosConsensusOptions(
                maxBlockBaseSize: 1_000_000,
                maxStandardVersion: 2,
                maxStandardTxWeight: 100_000,
                maxBlockSigopsCost: 20_000,
                maxStandardTxSigopsCost: 20_000 / 5,
                provenHeadersActivationHeight: 10_000_000 // TODO: Set it to the real value once it is known.
            );

            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 21111,
                [BuriedDeployments.BIP65] = 581885,
                [BuriedDeployments.BIP66] = 330776
            };

            var bip9Deployments = new NoBIP9Deployments();

            this.Consensus = new NBitcoin.Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: consensusOptions,
                coinType: 1,
                hashGenesisBlock: genesisBlock.Header.GetHash(),
                subsidyHalvingInterval: 210000,
                majorityEnforceBlockUpgrade: 51,
                majorityRejectBlockOutdated: 75,
                majorityWindow: 100,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: new uint256("0x0000000023b3a96d3484e5abb3755c413e7d41500f8e2a5c3f0dd01299cd8ef8"),
                ruleChangeActivationThreshold: 1512, // 75% for testchains
                minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
                maxReorgLength: 500,
                defaultAssumeValid: new uint256("0x000000003ccfe92231efee04df6621e7bb3f7f513588054e19f78d626b951f59"), // 1235126
                maxMoney: long.MaxValue,
                coinbaseMaturity: 5,
                premineHeight: 2,
                premineReward: Money.Coins(1000000),
                proofOfWorkReward: Money.Coins(50),
                powTargetTimespan: TimeSpan.FromSeconds(14 * 24 * 60 * 60), // two weeks
                powTargetSpacing: TimeSpan.FromSeconds(20), // 20 second block time while on testnet
                powAllowMinDifficultyBlocks: true,
                powNoRetargeting: false,
                powLimit: new Target(new uint256("7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")), // Set extremely low difficulty for now.
                minimumChainWork: uint256.Zero,
                isProofOfStake: default(bool),
                lastPowBlock: default(int),
                proofOfStakeLimit: null,
                proofOfStakeLimitV2: null,
                proofOfStakeReward: Money.Zero
            );

            this.Base58Prefixes = new byte[12][];
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (111) };
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (196) };
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (239) };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x35), (0x87), (0xCF) };
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x35), (0x83), (0x94) };
            this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
            this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
            this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = new byte[] { 0x2b };
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 115 };
            this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = new byte[] { 0x13 };

            Bech32Encoder encoder = Encoders.Bech32("tb");
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

            this.Checkpoints = new Dictionary<int, CheckpointInfo>();

            this.DNSSeeds = new List<DNSSeedData>();
            this.SeedNodes = new List<NetworkAddress>();
        }
    }
}