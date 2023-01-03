using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.RouteModel;
using ship_convenient.Services.RouteService;
using Swashbuckle.AspNetCore.Annotations;

namespace ship_convenient.Controllers
{
    public class RouteController : BaseApiController
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService, ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        [HttpGet("{accountId}")]
        [SwaggerOperation(Summary = "Get route with accountId")]
        [ProducesResponseType(typeof(ApiResponse<ResponseRouteModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> 
            GetAccountRoute(Guid accountId)
        {
            try
            {
                ApiResponse<List<ResponseRouteModel>> response = 
                    await _routeService.GetRouteUserId(accountId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get shipper route : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost()]
        [SwaggerOperation(Summary = "Create route")]
        [ProducesResponseType(typeof(ApiResponse<ResponseRouteModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateRoute(CreateRouteModel model)
        {
            try
            {
                ApiResponse<ResponseRouteModel> response = await _routeService.Create(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Create route exception : " + ex.Message.Substring(0, 100));
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("active-route")]
        [SwaggerOperation(Summary = "Set active route")]
        [ProducesResponseType(typeof(ActionResult<ApiResponse<List<ResponseRouteModel>>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetActiveRoute(Guid id)
        {
            try
            {
                ApiResponse response = await _routeService.SetActiveRoute(id);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Set active route : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
