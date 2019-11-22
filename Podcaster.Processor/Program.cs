using System;
using System.Threading.Tasks;
using Podcasting.Infrastructure.Common;

namespace Podcaster.Processor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new Host();
            Console.Title = host.EndpointName;

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => { e.Cancel = true; tcs.SetResult(null); };

            await host.Start();
            await Console.Out.WriteLineAsync("Press Ctrl+C to exit...");

            await tcs.Task;
            await host.Stop();
        }
    }
}
