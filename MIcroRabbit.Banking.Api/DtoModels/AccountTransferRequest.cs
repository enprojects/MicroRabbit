using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIcroRabbit.Banking.Api.DtoModels
{
    public class AccountTransferRequest
    {
        public int FromAccount { get; set; }
        public int ToAccount { get; set; }
        public decimal TransferAmount { get; set; }
    }
}
