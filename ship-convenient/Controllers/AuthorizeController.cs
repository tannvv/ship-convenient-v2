using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.AuthorizeModel;
using ship_convenient.Services.AuthorizeService;
using Swashbuckle.AspNetCore.Annotations;

namespace ship_convenient.Controllers
{
    public class AuthorizeController : BaseApiController
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly ILogger<AuthorizeController> _logger;

        public AuthorizeController(IAuthorizeService authorizeService, ILogger<AuthorizeController> logger)
        {
            this._authorizeService = authorizeService;
            this._logger = logger;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Login with username and password")]
        public async Task<ActionResult<ApiResponse<ResponseLoginModel>>> Login(LoginModel model)
        {
            try
            {
                ApiResponse<ResponseLoginModel> loginResponse = await _authorizeService.Login(model);
                if (loginResponse.Success == false)
                {
                    return BadRequest(loginResponse);
                }
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("Login exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("firebase")]
        [SwaggerOperation(Summary = "Login token firebase")]
        public async Task<ActionResult<ApiResponse<ResponseLoginModel>>> Login(LoginFirebaseModel model)
        {
            try
            {
                ApiResponse<ResponseLoginModel> loginResponse = await _authorizeService.Login(model);
                if (loginResponse.Success == false)
                {
                    return BadRequest(loginResponse);
                }
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("Login firebase exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
