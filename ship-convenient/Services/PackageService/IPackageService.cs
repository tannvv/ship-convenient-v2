using ship_convenient.Core.CoreModel;
using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Services.PackageService
{
    public interface IPackageService
    {
        Task<ApiResponse<ResponsePackageModel>> Create(CreatePackageModel model);
        Task<ApiResponse<ResponsePackageModel>> GetById(Guid id);
        Task<ApiResponsePaginated<ResponsePackageModel>> GetFilter(Guid? deliverId, Guid? creatorId, string? status, int? pageIndex, int? pageSize);
        Task<ApiResponse<List<ResponsePackageModel>>> GetAll(Guid deliverId, Guid creatorId, string? status);
        Task<ApiResponse> ApprovedPackage(Guid id);
        Task<ApiResponse> RejectPackage(Guid id);
        Task<ApiResponse> DeliverPickupPackages(Guid deliverId, List<Guid> packageIds);
        Task<ApiResponse> CreatorCancelPackage(Guid packageId);
        Task<ApiResponse> DeliverCancelPackage(Guid packageId);
        Task<ApiResponseListError> DeliverConfirmPackages(List<Guid> packageIds, Guid deliverId);
        Task<ApiResponse> DeliverDeliverySuccess(Guid packageId);
        Task<ApiResponse> DeliveryFailed(Guid packageId);
        Task<ApiResponse> CreatorConfirmDeliverySuccess(Guid packageId);
        Task<ApiResponse> RefundSuccess(Guid packageId);
        Task<ApiResponse> RefundFailed(Guid packageId);
        Task<ApiResponsePaginated<ResponseComboPackageModel>> SuggestCombo(Guid shipperId, int pageIndex, int pageSize);
    }
}
