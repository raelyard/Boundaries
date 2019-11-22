using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Podcasting.Podcaster.Commands;

namespace Podcasting.Podcaster.Processor
{
    public class CreatePodcasterCommandHandler : IHandleMessages<CreatePodcasterCommand>
    {
        private static readonly ILog Log = LogManager.GetLogger<CreatePodcasterCommandHandler>();

        public Task Handle(CreatePodcasterCommand command, IMessageHandlerContext context)
        {
            System.Console.WriteLine("Received CreatePodcasterCommand");
            return Task.CompletedTask;
        }
    }
}
