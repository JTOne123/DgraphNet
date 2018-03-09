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
        protected Channel _channel1;
        protected Channel _channel2;
        protected Channel _channel3;
        protected DgraphNet _client;

        protected const string HOSTNAME = "localhost";
        protected const string PORT = "9080";

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _channel1 = new Channel($"{HOSTNAME}:{PORT}", ChannelCredentials.Insecure);
            _channel2 = new Channel($"{HOSTNAME}:{PORT}", ChannelCredentials.Insecure);
            _channel3 = new Channel($"{HOSTNAME}:{PORT}", ChannelCredentials.Insecure);

            var stub1 = new DgraphClient(_channel1);
            var stub2 = new DgraphClient(_channel2);
            var stub3 = new DgraphClient(_channel3);

            _client = new DgraphNet(new[] { stub1, stub2, stub3 });

            await _client.AlterAsync(new Operation { DropAll = true });
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _channel1.ShutdownAsync();
        }
    }
}
