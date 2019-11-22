using System.Threading.Tasks;
using NServiceBus;
using Podcasting.Podcaster.Commands;

namespace Podcasting.Podcaster.UserInterface
{
    public class Podcaster
    {
        public async Task SendCreatePodcasterCommand(IEndpointInstance bus)
        {
            await bus.Send<CreatePodcasterCommand>(command => {});
        }
    }
}