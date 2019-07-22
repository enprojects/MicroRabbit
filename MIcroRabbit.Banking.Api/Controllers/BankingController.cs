using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Domain.Models;
using MIcroRabbit.Banking.Api.DtoModels;
using Microsoft.AspNetCore.Mvc;

namespace MIcroRabbit.Banking.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BankingController : ControllerBase
    {
        private readonly IAccountService _accountService;

        // GET api/values

        public BankingController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        

        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetBankingAccounts()
        {
            return Ok(_accountService.GetAccounts());
        }
        [HttpPost]
        public IActionResult TransfetToAccount([FromBody] AccountTransferRequest accountTransferRequest)
        {

            var accountTransfer = new AccountTransfer
            {
                FromAccount = accountTransferRequest.FromAccount,
                ToAccount = accountTransferRequest.ToAccount,
                TransferAmount = accountTransferRequest.TransferAmount
            };

            _accountService.Transfer(accountTransfer);
            return Ok(accountTransfer);
        }



    }
 
}
