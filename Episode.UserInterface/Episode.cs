using System.Threading.Tasks;
using NServiceBus;
using Podcasting.Episode.Commands;

namespace Podcasting.Episode.UserInterface
{
    public class Episode
    {
        private readonly IEndpointInstance _bus;

        public Episode(IEndpointInstance bus)
        {
            _bus = bus;
        }

        public async Task Create(string title)
        {
            await _bus.Send<CreateEpisodeCommand>(command => { command.Title = title; });
        }
    }
}
