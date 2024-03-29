﻿using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Transfer.Domain.EventHandles
{
    public class TransferEventHandler : IEventHandler<TransferCreatedEvent>
    {
        public string EventName => nameof(TransferCreatedEvent);

        private readonly ITransferRepository _transferReposiotory;
        public TransferEventHandler(ITransferRepository transferRepository)
        {
            _transferReposiotory = transferRepository;
        }
        public Task Handle(TransferCreatedEvent @event)
        {
            _transferReposiotory.AddTransferLog(new TransferLog
            {
                FromAccount = @event.From,
                ToAccount = @event.To,
                TransferAmount = @event.Amount
            });
            return Task.CompletedTask;
        }


        public void Test() {

        }
    }
}
