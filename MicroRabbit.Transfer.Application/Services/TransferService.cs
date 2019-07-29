
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Domain.Commands;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroRabbit.Transfer.Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly ITransferRepository _trnsferRepository;
        private readonly IEventBus _bus;

        public TransferService(ITransferRepository trnsferRepository, IEventBus bus)
        {
            _trnsferRepository = trnsferRepository;
            _bus = bus;
        }
  

        public IEnumerable<TransferLog> GetTransferLogs()
        {
            _bus.SendCommand(new TestCInstruction(DateTime.Now));

            return _trnsferRepository.GetTransfers();

        }

        
    }
}
