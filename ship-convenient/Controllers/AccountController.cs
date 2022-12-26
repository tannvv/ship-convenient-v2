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

        [HttpGet("{id}")]
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

    }
}
