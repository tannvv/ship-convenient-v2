using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.PackageModel;
using ship_convenient.Services.PackageService;
using Swashbuckle.AspNetCore.Annotations;
using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Controllers
{
    public class PackageController : BaseApiController
    {
        private readonly ILogger<PackageController> _logger;
        private readonly IPackageService _packageService;
        public PackageController(ILogger<PackageController> logger, IPackageService packageService)
        {
            _logger = logger;
            _packageService = packageService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get list package")]
        [ProducesResponseType(typeof(ApiResponsePaginated<ResponsePackageModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList(Guid? deliverId, Guid? senderId, string? status, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                ApiResponsePaginated<ResponsePackageModel> response = await _packageService.GetFilter(deliverId, senderId, status, pageIndex, pageSize);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get list package exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get package by id")]
        [ProducesResponseType(typeof(ApiResponse<ResponsePackageModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetId(Guid id)
        {
            try
            {
                ApiResponse<ResponsePackageModel> response = await _packageService.GetById(id);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get list package exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create pakage")]
        [ProducesResponseType(typeof(ApiResponse<ResponsePackageModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreatePackageModel model)
        {
            try
            {
                ApiResponse<ResponsePackageModel> response = await _packageService.Create(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Create package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("combos")]
        [SwaggerOperation(Summary = "Get suggest combos")]
        [ProducesResponseType(typeof(ApiResponse<List<ResponseComboPackageModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SuggestCombo(Guid deliverId)
        {
            try
            {
                /*ApiResponsePaginated<ResponseComboPackageModel> response = await _packageService.SuggestCombo(deliverId, pageIndex, pageSize);*/
                ApiResponse<List<ResponseComboPackageModel>> response = await _packageService.SuggestCombo(deliverId);
                return SendResponse(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Api mapbox has exception : " + ex.Message);
                return StatusCode(500, "Api mapbox has exception : " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get suggest combo has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("approve")]
        [SwaggerOperation(Summary = "Approved pakage from status waiting")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApprovedPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.ApprovedPackage(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Approve package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("reject")]
        [SwaggerOperation(Summary = "Reject pakage from status waiting")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.RejectPackage(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Reject package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("deliver-pickup")]
        [SwaggerOperation(Summary = "Shipper pickup a package")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> PickupPackage(ShipperPickUpModel model)
        {
            try
            {
                ApiResponse response = await _packageService.DeliverPickupPackages(model.deliverId,
                    model.packageIds);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shipper pickup package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("sender-cancel")]
        [SwaggerOperation(Summary = "Sender cancel package")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SenderCancelPackage(CancelPackageModel model)
        {
            try
            {
                ApiResponse response = await _packageService.SenderCancelPackage(model.PackageId, model.Reason);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shop cancel package exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("deliver-cancel")]
        [SwaggerOperation(Summary = "Sender cancel package")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeliverCancelPackage(CancelPackageModel model)
        {
            try
            {
                ApiResponse response = await _packageService.DeliverCancelPackage(model.PackageId, model.Reason);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver cancel package exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("confirm-packages")]
        [SwaggerOperation(Summary = "Deliver confirms packages and then delivery them")]
        [ProducesResponseType(typeof(ActionResult<ApiResponseListError>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeliverConfirmPackage(Guid packageId)
        {
            try
            {
                ApiResponseListError response = await _packageService.ConfirmPackages(
                    packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver confrims packages has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("delivery-failed")]
        [SwaggerOperation(Summary = "Shipper delivery failed package")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeliverDeliveryFailedPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.DeliveryFailed(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shipper delivery failed package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("delivery-success")]
        [SwaggerOperation(Summary = "Shipper delivery success pakage")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeliverDeliverySuccessPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.DeliverDeliverySuccess(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shipper delivery success pakage has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /*[HttpPut("sender-confirm-delivery-success")]
        [SwaggerOperation(Summary = "Sender confirm delivery success")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SenderConfirmDeliverySuccessPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.SenderConfirmDeliverySuccess(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shop confirm delivery success has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpPut("sender-confirm-delivery-failed")]
        [SwaggerOperation(Summary = "Sender confirm delivery failed")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SenderConfirmDeliveryFailed(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.SenderConfirmDeliveryFailed(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shop confirm delivery success has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }*/

        [HttpPut("refund-success")]
        [SwaggerOperation(Summary = "Deliver refund package for sender success")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefundSuccessPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.RefundSuccess(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver refund package for sender success has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("refund-failed")]
        [SwaggerOperation(Summary = "Deliver refund package for sender failed")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefundFailedPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.RefundFailed(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver refund package for sender failed : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
