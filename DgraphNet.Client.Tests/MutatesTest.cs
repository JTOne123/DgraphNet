using DgraphNet.Client.Proto;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DgraphNet.Client.Tests
{
    [TestFixture]
    public class MutatesTest : DgraphIntegrationTest
    {
        private string[] _data = new String[] { "200", "300", "400" };
        private Dictionary<string, string> _uidsMap;

        [Test]
        public void test_insert_3_quads()
        {
            var op = new Operation
            {
                Schema = "name: string @index(fulltext) ."
            };

            _client.Alter(op);

            var txn = _client.NewTransaction();
            _uidsMap = new Dictionary<string, string>();

            foreach (var d in _data)
            {
                var quad = new NQuad
                {
                    Subject = $"_:{d}",
                    Predicate = "name",
                    ObjectValue = new Value { StrVal = $"ok {d}" }
                };

                var mut = new Mutation();
                mut.Set.Add(quad);

                var ag = txn.Mutate(mut);
                _uidsMap.Add(d, ag.Uids[d]);
            }

            txn.Commit();
            Console.WriteLine("Commit Ok");
        }

        [Test]
        public void test_query_3_quads()
        {
            var txn = _client.NewTransaction();
            var uids = _data.Select(x => _uidsMap[x]);
            var query = "{ me(func: uid(%0)) { name } }"
                    .Replace("%0", string.Join(",", uids));

            Console.WriteLine($"Query: {query}");

            var response = txn.Query(query);
            var res = response.Json.ToStringUtf8();
            Console.WriteLine($"Responsive JSON: {res}");

            var exp = "{\"me\":[{\"name\":\"ok 200\"},{\"name\":\"ok 300\"},{\"name\":\"ok 400\"}]}";

            Assert.AreEqual(res, exp);
            Assert.IsTrue(response.Txn.StartTs > 0);

            txn.Commit();
        }
    }
}
