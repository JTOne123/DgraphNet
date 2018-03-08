using DgraphNet.Client.Proto;
using Grpc.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static DgraphNet.Client.Proto.Dgraph;

namespace DgraphNet.Client.Tests
{
    [TestFixture]
    public class DgraphIntegrationTest
    {
        protected Channel _channel;
        protected DgraphNetClient _client;

        protected const string HOSTNAME = "localhost";
        protected const string PORT = "9080";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _channel = new Channel($"{HOSTNAME}:{PORT}", ChannelCredentials.Insecure);
            var stub = new DgraphClient(_channel);
            _client = new DgraphNetClient(new[] { stub });

            _client.Alter(new Operation { DropAll = true });
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _channel.ShutdownAsync();
        }
    }
}
