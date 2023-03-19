using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.AuthorizeModel;
using ship_convenient.Services.AuthorizeService;
using ship_convenient.Services.SendSmsService;
using Swashbuckle.AspNetCore.Annotations;

namespace ship_convenient.Controllers
{
    public class AuthorizeController : BaseApiController
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly ISendSMSService _smsService;
        private readonly ILogger<AuthorizeController> _logger;

        public AuthorizeController(IAuthorizeService authorizeService, ILogger<AuthorizeController> logger, ISendSMSService smsService)
        {
            this._authorizeService = authorizeService;
            this._logger = logger;
            this._smsService = smsService;
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


        [HttpGet("otp")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOTP()
        {
            try
            {
                var message = _smsService.SendSmsOtp();
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Update account exception : " + ex.Message.Substring(0, 300));
                return BadRequest(ex);
            }
        }

        [HttpPost("logout")]
        [SwaggerOperation(Summary = "Logout")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout(LogOutModel model)
        {
            try
            {
                ApiResponse logoutResponse = await _authorizeService.LogOut(model.AccountId);
                return SendResponse(logoutResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("Logout exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
