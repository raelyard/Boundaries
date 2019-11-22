using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using Podcasting.Infrastructure.Common;
using Podcasting.Podcaster.Commands;

namespace Podcasting.UserInterface
{
    class Program
    {
        // input takes the form:
        // subdomain command args[]
        // 
        // example podcaster create "Joe Rogan"
        static async Task Main(string[] args)
        {
            var host = new Host();
            Console.Title = host.EndpointName;

            try
            {
                var bus = await host.Start(typeof(CreatePodcasterCommand).Assembly);

                if(args.Length < 3)
                {
                    throw new ArgumentException("Insufficient input - parameters must take form of\r\nsubdomain command args[]");
                }
                await TakeAction(bus, args[0], args[1], args.Skip(2).ToArray());
            }
            finally
            {
                await host.Stop();
            }
        }

        private static async Task TakeAction(IEndpointInstance bus, string subdomain, string command, string[] args)
        {
            switch(subdomain)
            {
                case "podcaster":
                    await TakeAction(new Podcaster.UserInterface.Podcaster(bus), command, args);
                    break;
                default:
                    throw new Exception($"{subdomain} is not a valid subdomain");
            }
        }

        private static async Task TakeAction(Podcaster.UserInterface.Podcaster podcaster, string command, string[] args)
        {
           await podcaster.Create(args[0]);
        }
    }
}
