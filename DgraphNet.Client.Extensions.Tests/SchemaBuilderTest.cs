using DgraphNet.Client.Extensions.Builders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DgraphNet.Client.Extensions.Tests
{
    [TestFixture]
    public class SchemaBuilderTest
    {
        [Test]
        public void test_build_string_predicate()
        {
            var schema = Schema.Predicate("first")
                .String()
                .Index(StringIndexType.Hash|StringIndexType.Term|StringIndexType.FullText)
                .List()
                .Count()
                .Upsert()
                .Build();

            string exp = "first: [string] @index(hash, term, fulltext) @count @upsert .";

            Assert.AreEqual(exp, schema);
        }

        [Test]
        public void test_build_basic_predicate()
        {
            var schema = Schema.Predicate("first")
                .Int()
                .Index()
                .List()
                .Count()
                .Upsert()
                .Build();

            string exp = "first: [int] @index @count @upsert .";

            Assert.AreEqual(exp, schema);
        }

        [Test]
        public void test_build_datetime_predicate()
        {
            var schema = Schema.Predicate("first")
                .DateTime()
                .Index(DateTimeIndexType.Month)
                .List()
                .Count()
                .Upsert()
                .Build();

            string exp = "first: [datetime] @index(month) @count @upsert .";

            Assert.AreEqual(exp, schema);
        }
    }
}
