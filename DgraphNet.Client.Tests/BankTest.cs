using DgraphNet.Client.Proto;
using Google.Protobuf;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DgraphNet.Client.Tests
{
    [TestFixture]
    public class BankTest : DgraphIntegrationTest
    {
        List<string> _uids = new List<string>();
        int _runs = 0;
        int _aborts = 0;

        private async Task CreateAccounts()
        {
            var schema = "bal: int .";
            await _client.AlterAsync(new Operation { Schema = schema });

            var accounts = new List<Account>();

            for (var i = 0; i < 100; i++)
            {
                var ac = new Account { Bal = 100 };
                accounts.Add(ac);
            }

            var jsonStr = JsonConvert.SerializeObject(accounts);
            Console.WriteLine(jsonStr);

            var txn = _client.NewTransaction();

            var mut = new Mutation
            {
                SetJson = ByteString.CopyFromUtf8(jsonStr)
            };

            var ag = await txn.MutateAsync(mut);
            _uids.AddRange(ag.Uids.Select(x => x.Value));
        }

        private async Task RunTotal()
        {
            string q =
              " {\n"
            + "   var(func: uid(%0)) {\n"
            + "    b as bal\n"
            + "   }\n"
            + "   total() {\n"
            + "    bal: sum(val(b))\n"
            + "   }\n"
            + "  }";

            q = q.Replace("%0", string.Join(",", _uids));

            var txn = _client.NewTransaction();
            var resp = await txn.QueryAsync(q);

            Console.WriteLine($"response json: {resp.Json.ToStringUtf8()}");
            Console.WriteLine($"Runs: {_runs}, Aborts: {_aborts}");
        }

        private async Task RunTotalInLoop()
        {
            while (true)
            {
                await RunTotal();
                await Task.Delay(1000);
            }
        }

        private async Task RunTxn()
        {
            string from, to;
            var rand = new Random();

            while (true)
            {
                from = _uids[rand.Next(_uids.Count)];
                to = _uids[rand.Next(_uids.Count)];

                if (from != to)
                {
                    break;
                }
            }

            var txn = _client.NewTransaction();

            try
            {
                string fq = "{both(func: uid(%0, %1)) { uid, bal }}"
                    .Replace("%0", from)
                    .Replace("%1", to);

                var resp = await txn.QueryAsync(fq);
                var accounts = JsonConvert.DeserializeObject<Accounts>(resp.Json.ToStringUtf8());

                if (accounts.Both.Count != 2)
                {
                    throw new InvalidOperationException("Unable to find both accounts");
                }

                accounts.Both[0].Bal += 5;
                accounts.Both[1].Bal -= 5;

                var mut = new Mutation
                {
                    SetJson = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(accounts))
                };

                await txn.MutateAsync(mut);
                await txn.CommitAsync();
            }
            finally
            {
                await txn.DiscardAsync();
            }
        }

        private async Task TxnLoop()
        {
            while (true)
            {
                try
                {
                    await RunTxn();

                    var r = Interlocked.Increment(ref _runs);

                    if (r > 1000)
                    {
                        return;
                    }
                }
                catch (TxnConflictException e)
                {
                    Interlocked.Increment(ref _aborts);
                }
            }
        }

        [Test]
        public async Task test_bank()
        {
            await CreateAccounts();

            var totalTask = Task.Run(() => RunTotalInLoop());

            var txnTask = Task.WhenAll(Enumerable.Range(0, 10)
                .Select(_ => Task.Run(() => TxnLoop())));

            if(!txnTask.Wait(1000 * 60 * 5))
            {
                Console.WriteLine("Timeout elapsed");
            }

            totalTask.Wait(1000 * 5);
        }

        class Account
        {
            public string Uid { get; set; }

            public int Bal { get; set; }
        }

        class Accounts
        {
            public List<Account> Both { get; set; }
        }
    }
}
