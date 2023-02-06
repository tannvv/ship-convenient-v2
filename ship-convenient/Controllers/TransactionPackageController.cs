﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.PackageModel;
using ship_convenient.Model.TransactionPackageModel;
using ship_convenient.Services.TransactionPackageService;

namespace ship_convenient.Controllers
{
    public class TransactionPackageController : BaseApiController
    {
        private readonly ILogger<TransactionPackageController> _logger;
        private readonly ITransactionPackageService _transactionPackageService;
        public TransactionPackageController(ITransactionPackageService transactionPackageService, ILogger<TransactionPackageController> logger)
        {
            _transactionPackageService = transactionPackageService;
            _logger = logger;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ApiResponsePaginated<ResponseTransactionPackageModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistoryPackage(Guid packageId, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                ApiResponsePaginated<ResponseTransactionPackageModel> response = await _transactionPackageService.GetHistoryPackage(packageId, pageIndex, pageSize);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get transactions package: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("deliver-cancel")]
        [ProducesResponseType(typeof(ApiResponsePaginated<ResponseCancelPackageModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeliverCancelPackage(Guid deliverId, int pageIndex = 0, int pageSize = 20)
        {
      
            try
            {
                ApiResponsePaginated<ResponseCancelPackageModel> response = await _transactionPackageService.GetDeliverCancelPackage(deliverId, pageIndex, pageSize);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get deliver cancel : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }

}
