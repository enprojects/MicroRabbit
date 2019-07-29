using MediatR;
using MicroRabbit.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Transfer.Domain.Commands
{
    public class TestCInstruction : Command
    {

        public TestCInstruction(DateTime dt)
        {
            this.Timestamp = DateTime.Now;
        }

     
    }
}
