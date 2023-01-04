using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.AccountService;

namespace ship_convenient.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetList(string? userName, string? status, int pageIndex =0, int pageSize = 20)
        {
            try
            {
                ApiResponsePaginated<ResponseAccountModel> response = await _accountService.GetList(userName, status, pageIndex, pageSize);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ResponseAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetId(Guid id)
        {
            try
            {
                ApiResponse<ResponseAccountModel> response = await _accountService.GetId(id);
                if (response.Success == false)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get account exception : " + ex.Message.Substring(0,300));
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ResponseAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateAccountModel model)
        {
            try
            {
                ApiResponse<ResponseAccountModel> response = await _accountService.Create(model);
                if (response.Success == false) {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError("Create account exception : " + ex.Message.Substring(0, 300));
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<ResponseAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(UpdateAccountModel model)
        {
            try
            {
                ApiResponse<ResponseAccountModel> response = await _accountService.Update(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Update account exception : " + ex.Message.Substring(0, 300));
                return BadRequest(ex);
            }
        }

        [HttpPut("info")]
        [ProducesResponseType(typeof(ApiResponse<ResponseAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateInfo(UpdateInfoModel model)
        {
            try
            {
                ApiResponse<ResponseAccountModel> response = await _accountService.UpdateInfo(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Update account info exception : " + ex.Message.Substring(0, 300));
                return BadRequest(ex);
            }
        }
    }
}
