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
                case "guest":
                    await TakeAction(new Guest.UserInterface.Guest(bus), command, args);
                    break;
                case "episode":
                    await TakeAction(new Episode.UserInterface.Episode(bus), command, args);
                    break;
                default:
                    throw new Exception($"{subdomain} is not a valid subdomain");
            }
        }

        private static async Task TakeAction(Podcaster.UserInterface.Podcaster podcaster, string command, string[] args)
        {
            switch(command)
            {
                case "create":
                    await podcaster.Create(args[0]);
                    break;
                default:
                    throw new Exception($"{command} is not a valid command for subdomain podcaster");
            }
        }

        private static async Task TakeAction(Guest.UserInterface.Guest guest, string command, string[] args)
        {
            switch(command)
            {
                case "create":
                    await guest.Create(args[0]);
                    break;
                default:
                    throw new Exception($"{command} is not a valid command for subdomain guest1");
            }
        }

        private static async Task TakeAction(Episode.UserInterface.Episode episode, string command, string[] args)
        {
            switch(command)
            {
                case "create":
                    await episode.Create(args[0]);
                    break;
                default:
                    throw new Exception($"{command} is not a valid command for subdomain episode");
            }
        }
    }
}
