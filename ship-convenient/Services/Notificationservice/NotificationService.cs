using ship_convenient.Core.CoreModel;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.NotificationModel;
using ship_convenient.Services.GenericService;

namespace ship_convenient.Services.Notificationservice
{
    public class NotificationService :GenericService<NotificationService>, INotificationService
    {
        public NotificationService(ILogger<NotificationService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
        }

        public async Task<ApiResponsePaginated<ResponseNotificationModel>> GetList(Guid accountId, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseNotificationModel> response = new();
            string? errorPaging = VerifyPaging(pageIndex, pageSize);
            if (errorPaging != null) {
                response.ToFailedResponse(errorPaging);
            }
            bool isExistAccount = IsExistedAccount(accountId);
            if (!isExistAccount) {
                response.ToFailedResponse("Không tìm thấy tài khoản");
            }
            PaginatedList<ResponseNotificationModel> result = await _notificationRepo.GetPagedListAsync(
                predicate: (source) => source.AccountId == accountId,
                selector: (source) => source.ToResponseModel(),
                orderBy: (source) => source.OrderByDescending(n => n.CreatedAt),
                pageIndex: pageIndex, pageSize: pageSize);
            response.SetData(result, "Lấy thông tin thành công");
            return response;
        }
    }
}
