using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;

namespace ship_convenient.Controllers
{
    [Route("api/v1.0/[controller]s")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult SendResponse(ApiResponse response)
        {
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
