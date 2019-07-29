using System;
using System.Collections.Generic;
using System.Text;
using MicroRabbit.Domain.Core.Events;

namespace MicroRabbit.Transfer.Domain.Events
{
    public class TransferCreatedEvent : Event
    {
        public string From { get; private set; }
        public string To { get; private set; }
        public decimal  Amount { get; private set; }

        public TransferCreatedEvent(string from , string to , decimal amount )

        {
            From = from;
            To = to;
            Amount = amount;
        }

    }
}

