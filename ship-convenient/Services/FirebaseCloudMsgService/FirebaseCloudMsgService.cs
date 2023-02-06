using FcmMessage = FirebaseAdmin.Messaging.Message;
using FcmNotification = FirebaseAdmin.Messaging.Notification;
using FcmFirebaseMsg = FirebaseAdmin.Messaging.FirebaseMessaging;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.FirebaseNotificationModel;
using ship_convenient.Services.GenericService;

namespace ship_convenient.Services.FirebaseCloudMsgService
{
    public class FirebaseCloudMsgService : GenericService<FirebaseCloudMsgService>, IFirebaseCloudMsgService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly INotificationRepository _notificationRepo;
        public FirebaseCloudMsgService(ILogger<FirebaseCloudMsgService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _accountRepo = unitOfWork.Accounts;
            _notificationRepo = unitOfWork.Notifications;
        }

        public async Task<ApiResponse> SendNotification(SendNotificationModel model) 
        {
            ApiResponse response = new();
            Account? account = await _accountRepo.GetByIdAsync(model.AccountId);
            if (account == null) {
                response.ToFailedResponse("Không tìm thấy tài khoản");
            }
            FcmMessage message = new FcmMessage();
            message.Token = account?.RegistrationToken;
            message.Notification = new FcmNotification()
            {
                Title = model.Title,
                Body = model.Body
            };
            string responseFirebase = await FcmFirebaseMsg.DefaultInstance.SendAsync(message);
            Console.WriteLine($"Response firebase notification: {response}");
            if (!string.IsNullOrEmpty(responseFirebase))
            {
                Notification notification = new Notification()
                {
                    AccountId = model.AccountId,
                    Title = model.Title,
                    Content = model.Body,
                };
                await _notificationRepo.InsertAsync(notification);
                response.ToSuccessResponse($"Gửi thông báo thành công - {responseFirebase}");
            }
            else {
                response.ToFailedResponse($"Gửi thông báo thất bại");
            }
            return response;
        }

    }
}
