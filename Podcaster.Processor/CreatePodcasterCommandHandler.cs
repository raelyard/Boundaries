using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Podcasting.Podcaster.Commands;
using Podcasting.Public.Events.Podcaster;

namespace Podcasting.Podcaster.Processor
{
    public class CreatePodcasterCommandHandler : IHandleMessages<CreatePodcasterCommand>
    {
        private static readonly ILog Log = LogManager.GetLogger<CreatePodcasterCommandHandler>();

        public async Task Handle(CreatePodcasterCommand command, IMessageHandlerContext context)
        {
            System.Console.WriteLine($"Received CreatePodcasterCommand with name {command.Name}");
            await context.Publish<PodcasterCreatedEvent>(theEvent => { theEvent.Name = command.Name; });
        }
    }
}
