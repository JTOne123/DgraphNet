using DgraphNet.Client;
using DgraphNet.Client.Proto;
using Google.Protobuf;
using Grpc.Core;
using System;
using System.Text;
using static DgraphNet.Client.Proto.Dgraph;

namespace DgraphNet.CliTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Channel channel = new Channel("127.0.0.1:9080", ChannelCredentials.Insecure);
            
            var stub = new DgraphClient(channel);
            var client = new DgraphNetClient(new[] { stub });

            using (var trans = client.NewTransaction())
            {
                var q1 = @"
                    {
                      everyone(func: anyofterms(name, ""Michael Amit"")) {
                        name
                        friend {
                          name@ru:ko:en
                          friend { expand(_all_) { expand(_all_) } }
                        }
                      }
                    }
                ";

                var r1 = trans.Query(q1);
                var json1 = r1.Json.ToStringUtf8();

                var mut1 = new Mutation();
                mut1.SetJson = ByteString.CopyFrom(@"
                [
                    {
	                    ""name"": ""CompanyABC"",
	                    ""industry"": ""Machinery""
                    },
                    {
	                    ""name"": ""The other company"",
	                    ""industry"": ""High tech""
                    }
                ]
                ", Encoding.ASCII);

                mut1.CommitNow = true;
                var mutR1 = trans.Mutate(mut1);

                var q2 = @"
                    {
                      company(func: allofterms(name, ""CompanyABC"")) {
                        expand(_all_)
                      }
                    }
                ";

                var r2 = trans.Query(q2);
                var json2 = r2.Json.ToStringUtf8();
            }

            Console.ReadKey();
        }
    }
}
