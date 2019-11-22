using System.Threading.Tasks;
using NServiceBus;
using Podcasting.Podcaster.Commands;

namespace Podcasting.Podcaster.UserInterface
{
    public class Podcaster
    {
        private readonly IEndpointInstance _bus;

        public Podcaster(IEndpointInstance bus)
        {
            _bus = bus;
        }
        
        public async Task Create(string name)
        {
            await _bus.Send<CreatePodcasterCommand>(command => {});
        }
    }
}
