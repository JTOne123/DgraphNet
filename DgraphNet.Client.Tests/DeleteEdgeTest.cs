using DgraphNet.Client.Proto;
using Google.Protobuf;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DgraphNet.Client.Tests
{
    [TestFixture]
    public class DeleteEdgeTest : DgraphIntegrationTest
    {
        [Test]
        public void test_delete_edges()
        {
            Person alice = new Person
            {
                Name = "Alice",
                Age = 26,
                Married = true,
                Location = "Riley Street"
            };

            School school = new School
            {
                Name = "Crown Public School"
            };

            alice.Schools.Add(school);

            Person bob = new Person
            {
                Name = "Bob",
                Age = 24
            };

            alice.Friends.Add(bob);

            Person charlie = new Person
            {
                Name = "Charlie",
                Age = 29
            };

            alice.Friends.Add(charlie);

            var op = new Operation
            {
                Schema = "age: int .\nmarried: bool ."
            };
            _client.Alter(op);

            var mut = new Mutation
            {
                CommitNow = true,
                SetJson = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(alice))
            };

            var ag = _client.NewTransaction().Mutate(mut);
            var uid = ag.Uids["blank-0"];

            string q =
                "{\n"
                    + "  me(func: uid(%0)) {\n"
                    + "   uid\n"
                    + "   name\n"
                    + "   age\n"
                    + "   loc\n"
                    + "   married\n"
                    + "   friends {\n"
                    + "    uid\n"
                    + "    name\n"
                    + "    age\n"
                    + "   }\n"
                    + "   schools {\n"
                    + "    uid\n"
                    + "    name@en\n"
                    + "   }\n"
                    + "  }\n"
                    + " }";

            q = q.Replace("%0", uid);

            var resp = _client.NewTransaction().Query(q);
            Console.WriteLine(resp.Json.ToStringUtf8());

            mut = DgraphNetClient.DeleteEdges(
                new Mutation { CommitNow = true },
                uid,
                "friends", "loc"
            );

            _client.NewTransaction().Mutate(mut);

            resp = _client.NewTransaction().Query(q);
            Console.WriteLine(resp.Json.ToStringUtf8());

            var root = JsonConvert.DeserializeObject<Root>(resp.Json.ToStringUtf8());
            Assert.IsTrue(root.Me[0].Friends.Count == 0);
        }

        class School
        {
            public string Uid { get; set; }
            public string Name { get; set; }
        }

        class Person
        {
            public Person()
            {
                Friends = new List<Person>();
                Schools = new List<School>();
            }

            public string Uid { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Married { get; set; }
            public string Location { get; set; }
            public List<Person> Friends { get; set; }
            public List<School> Schools { get; set; }
        }

        class Root
        {
            public List<Person> Me { get; set; }
        }
    }
}
