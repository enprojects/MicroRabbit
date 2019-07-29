using System.Collections.Generic;
using MicroRabbit.Transfer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using  MicroRabbit.Transfer.Domain.Models;

namespace MicroRabbit.Transfer.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

   
        [HttpGet]
        public ActionResult<IEnumerable<TransferLog>> GetTransfereds()
        {
            return Ok(_transferService.GetTransferLogs());
        }

    }
}
