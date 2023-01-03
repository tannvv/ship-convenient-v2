using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Services.TransactionService;
using unitofwork_core.Model.TransactionModel;

namespace ship_convenient.Controllers
{
    public class TransactionController : BaseApiController
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ApiResponsePaginated<ResponseTransactionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactions(Guid accountId, DateTime? from, DateTime? to, int pageIndex = 0, int pageSize = 20)
        {
            ApiResponsePaginated<ResponseTransactionModel> response = await _transactionService.GetTransactions(accountId, from, to, pageIndex, pageSize);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
