using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.FirebaseNotificationModel;
using ship_convenient.Services.FirebaseCloudMsgService;

namespace ship_convenient.Controllers
{
    public class NotificationController : BaseApiController
    {
        private readonly IFirebaseCloudMsgService _firebaseCloudMsgService;

        public NotificationController(IFirebaseCloudMsgService firebaseCloudMsgService)
        {
            _firebaseCloudMsgService = firebaseCloudMsgService;
        }

        [HttpPost]
        [Route("send-notification")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationModel model)
        {
            await _firebaseCloudMsgService.SendNotification(model);
            return Ok();
        }
    }
}
