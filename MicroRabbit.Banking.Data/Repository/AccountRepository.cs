using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Banking.Data.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingDbCocntext _ctx;
        public AccountRepository MyProperty { get; set; }

        public AccountRepository(BankingDbCocntext ctx)
        {
            _ctx = ctx;
        }
        public IEnumerable<Account> GetAccountes()
        {
            return _ctx.Accounts;
        }
    }
}
