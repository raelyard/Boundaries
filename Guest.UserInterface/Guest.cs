using System.Threading.Tasks;
using NServiceBus;
using Podcasting.Guest.Commands;

namespace Podcasting.Guest.UserInterface
{
    public class Guest
    {
        private readonly IEndpointInstance _bus;

        public Guest(IEndpointInstance bus)
        {
            _bus = bus;
        }

        public async Task Create(string name)
        {
            await _bus.Send<CreateGuestCommand>(command => { command.Name = name; });
        }
    }
}
