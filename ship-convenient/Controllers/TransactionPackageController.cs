using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.TransactionPackageModel;
using ship_convenient.Services.TransactionPackageService;

namespace ship_convenient.Controllers
{
    public class TransactionPackageController : BaseApiController
    {
        private readonly ITransactionPackageService _transactionPackageService;
        public TransactionPackageController(ITransactionPackageService transactionPackageService)
        {
            _transactionPackageService = transactionPackageService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ApiResponsePaginated<ResponseTransactionPackageModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistoryPackage(Guid packageId, int pageIndex = 0, int pageSize = 20)
        {
            ApiResponsePaginated<ResponseTransactionPackageModel> response = await _transactionPackageService.GetHistoryPackage(packageId, pageIndex, pageSize);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }

}
