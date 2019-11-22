using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Podcasting.Podcaster.Commands;
using Podcasting.Public.Events.Podcaster;

namespace Podcasting.Podcaster.Processor
{
    public class PodcasterLifecycle : Saga<PodcasterLifecycle.State>, IAmStartedByMessages<CreatePodcasterCommand>
    {
        private static readonly ILog Log = LogManager.GetLogger<PodcasterLifecycle>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<State> mapper)
        {
            mapper.ConfigureMapping<CreatePodcasterCommand>(command => command.Username).ToSaga(lifecycle => lifecycle.Username);
        }

        public async Task Handle(CreatePodcasterCommand command, IMessageHandlerContext context)
        {
            System.Console.WriteLine($"Received CreatePodcasterCommand with name {command.Name}");
            await context.Publish<PodcasterCreatedEvent>(theEvent => { theEvent.Name = command.Name; });
        }

        public class State : ContainSagaData
        {
            public string Username { get; set; }
            public string Name { get; set; }
        }
    }
}
