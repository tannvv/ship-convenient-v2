using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Entities;
using ship_convenient.Model.ConfigModel;
using ship_convenient.Services.ConfigService;

namespace ship_convenient.Controllers
{
    public class ConfigController : BaseApiController
    {
        private readonly IConfigService _configService;

        public ConfigController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ConfigApp>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _configService.GetAll();
            return SendResponse(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<ConfigApp>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateConfigModel model)
        {
            var result = await _configService.Update(model);
            return SendResponse(result);
        }
    }
}
