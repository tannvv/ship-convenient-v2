using ship_convenient.Core.CoreModel;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.DashboardModel;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.GenericService;
using unitofwork_core.Constant.Package;

namespace ship_convenient.Services.DashboardService
{
    public class DashboardService : GenericService<DashboardService>, IDashboardService
    {
        private readonly AccountUtils _accountUtils;
        public DashboardService(ILogger<DashboardService> logger, IUnitOfWork unitOfWork, AccountUtils accountUtils) : base(logger, unitOfWork)
        {
            _accountUtils = accountUtils;
        }

        public async Task<ApiResponse<PackageCountModel>> GetCountPackage()
        {
            ApiResponse<PackageCountModel> response = new();
            int waitings = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.WAITING)).Count;
            int approved = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.APPROVED)).Count;
            int reject = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.REJECT)).Count;
        /*    int deliverPickup = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.DELIVER_PICKUP)).Count;
            int deliverCancel = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.DELIVER_CANCEL)).Count;
            int senderCancel = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.SENDER_CANCEL)).Count;
            int delivery = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.DELIVERY)).Count;
            int delivereds = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.DELIVERED)).Count;
            int deliveryFailed = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.DELIVERY_FAILED)).Count;
            int refundSuccess = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.REFUND_SUCCESS)).Count;
            int refundFailed = (await _packageRepo.GetAllAsync(predicate: p => p.Status == PackageStatus.REFUND_FAILED)).Count;*/
            int all = (await _packageRepo.GetAllAsync()).Count;
            PackageCountModel result = new PackageCountModel { 
                All = all,
                Waiting = waitings,
                Approved = approved,
                Reject = reject,
              /*  DeliverPickup = deliverPickup,
                DeliverCancel = deliverCancel,
                SenderCancel = senderCancel,
                Delivery = delivery,
                Delivered = delivereds,
                DeliveryFailed = deliveryFailed,
                RefundSuccess = refundSuccess,
                RefundFailed = refundFailed,*/
            };
            response.ToSuccessResponse(result, "Lấy thông tin thành công");
            return response;
            
        }

        public async Task<ApiResponse<List<ResponseAccountModel>>> GetListAccountActive()
        {
            ApiResponse<List<ResponseAccountModel>> response = new();
            List<Account> activeAccounts = await _accountUtils.GetListAccountActive();
            List<ResponseAccountModel> result = activeAccounts.Select(acc => acc.ToResponseModel()).ToList();
            response.ToSuccessResponse(result, "Lấy thông tin thành công");
            return response;
        }
    }
}
