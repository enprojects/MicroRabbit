using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Domain.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace MicroRabbit.Transfer.Domain.CommandHandlers
{
    public class MyComInvok : IRequestHandler<TestCInstruction, bool>
    {
        private readonly IEventBus _bus;

        public MyComInvok(IEventBus bus)
        {
            _bus = bus;
        }

        public Task<bool> Handle(TestCInstruction request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
