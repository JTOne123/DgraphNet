using System;
using NUnitLite;

namespace DgraphNet.Client.Tests.ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            new AutoRun(typeof(Tests.DgraphIntegrationTest).Assembly).Execute(args);
            new AutoRun(typeof(Extensions.Tests.DgraphIntegrationTest).Assembly).Execute(args);
        }
    }
}
