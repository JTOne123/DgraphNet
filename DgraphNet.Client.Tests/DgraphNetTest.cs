using DgraphNet.Client.Proto;
using Google.Protobuf;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using static DgraphNet.Client.Proto.Dgraph;

namespace DgraphNet.Client.Tests
{
    [TestFixture]
    public class DgraphNetTest : DgraphIntegrationTest
    {
        [SetUp]
        public void BeforeEach()
        {
            _client.Alter(new Operation { DropAll = true });
        }

        [Test]
        public void test_merge_context()
        {
            var dst = new LinRead();
            dst.Ids.Add(new Dictionary<uint, ulong>
            {
                { 1, 10L },
                { 2, 15L },
                { 3, 10L }
            });

            var src = new LinRead();
            src.Ids.Add(new Dictionary<uint, ulong>
            {
                { 2, 10L },
                { 3, 15L },
                { 4, 10L }
            });

            var result = DgraphNet.MergeLinReads(dst, src);

            Assert.AreEqual(4, result.Ids.Count);

            Assert.AreEqual(10L, result.Ids[1]);
            Assert.AreEqual(15L, result.Ids[2]);
            Assert.AreEqual(15L, result.Ids[3]);
            Assert.AreEqual(10L, result.Ids[4]);
        }

        [Test]
        public void test_txn_query_variables()
        {
            // Set schema
            var op = new Operation { Schema = "name: string @index(exact) ." };
            _client.Alter(op);

            // Add data
            var json = new JObject();
            json.Add("name", "Alice");

            var mut = new Mutation
            {
                CommitNow = true,
                SetJson = ByteString.CopyFromUtf8(json.ToString())
            };

            _client.NewTransaction().Mutate(mut);

            // Query
            string query = "query me($a: string) { me(func: eq(name, $a)) { name }}";
            var vars = new Dictionary<string, string>
            {
                { "$a", "Alice" }
            };

            var res = _client.NewTransaction().QueryWithVars(query, vars);

            // Verify data as expected
            json = JObject.Parse(res.Json.ToStringUtf8());
            Assert.IsTrue(json.ContainsKey("me"));

            var arr = json.GetValue("me") as JArray;
            var obj = arr[0] as JObject;
            var name = obj.Property("name").Value.ToString();

            Assert.AreEqual("Alice", name);
        }

        [Test]
        public void test_delete()
        {
            using (var txn = _client.NewTransaction())
            {
                var mutation = new Mutation
                {
                    SetNquads = ByteString.CopyFromUtf8("<_:bob> <name> \"Bob\" .")
                };

                var ag = txn.Mutate(mutation);
                string bob = ag.Uids["bob"];

                string query = "{ find_bob(func: uid(%0)) { name } }"
                    .Replace("%0", bob);

                var resp = txn.Query(query);
                var json = JObject.Parse(resp.Json.ToStringUtf8());

                var arr = json.GetValue("find_bob") as JArray;
                Assert.IsTrue(arr.Count > 0);

                mutation = new Mutation
                {
                    DelNquads = ByteString.CopyFromUtf8("<%0> * * .".Replace("%0", bob))
                };

                txn.Mutate(mutation);

                resp = txn.Query(query);
                json = JObject.Parse(resp.Json.ToStringUtf8());

                arr = json.GetValue("find_bob") as JArray;
                Assert.IsTrue(arr.Count == 0);

                txn.Commit();
            }
        }

        [Test]
        public void test_commit_after_CommitNow()
        {
            Assert.Throws<TxnFinishedException>(() =>
            {
                using (var txn = _client.NewTransaction())
                {
                    var mut = new Mutation
                    {
                        SetNquads = ByteString.CopyFromUtf8("<_:bob> <name> \"Bob\" ."),
                        CommitNow = true
                    };

                    txn.Mutate(mut);
                    txn.Commit();
                }
            });
        }

        [Test]
        public void test_discard_abort()
        {
            using (var txn = _client.NewTransaction())
            {
                var mut = new Mutation
                {
                    SetNquads = ByteString.CopyFromUtf8("<_:bob> <name> \"Bob\" ."),
                    CommitNow = true
                };

                txn.Mutate(mut);
            }
        }

        [Test]
        public void test_client_with_deadline()
        {
            var channel = new Channel($"{HOSTNAME}:{PORT}", ChannelCredentials.Insecure);
            var stub = new DgraphClient(channel);
            var client = new DgraphNet(new[] { stub }, 1);

            var op = new Operation { Schema = "name: string @index(exact) ." };

            // Alters schema without exceeding the given deadline.
            client.Alter(op);

            // Creates a blocking stub directly, in order to force a deadline to be exceeded.
            var method = typeof(DgraphNet).GetMethod("AnyClient", BindingFlags.NonPublic | BindingFlags.Instance);
            var (stub2, callOptions) = (ValueTuple<DgraphClient, CallOptions>)method.Invoke(client, Array.Empty<object>());

            Thread.Sleep(1001);

            Assert.Throws<RpcException>(() => stub.Alter(op, callOptions), "Deadline should have been exceeded");
        }
    }
}
