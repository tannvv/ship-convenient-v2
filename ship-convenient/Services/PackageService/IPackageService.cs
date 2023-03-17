using ship_convenient.Core.CoreModel;
using ship_convenient.Entities;
using ship_convenient.Model.PackageModel;
using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Services.PackageService
{
    public interface IPackageService
    {
        Task<ApiResponse<ResponsePackageModel>> Create(CreatePackageModel model);
        Task<ApiResponse<ResponsePackageModel>> GetById(Guid id);
        Task<ApiResponsePaginated<ResponsePackageModel>> GetFilter(Guid? deliverId, Guid? senderId, string? status, int pageIndex, int pageSize);
        Task<ApiResponse<List<ResponsePackageModel>>> GetAll(Guid deliverId, Guid senderId, string? status);
        Task<ApiResponse> ApprovedPackage(Guid id);
        Task<ApiResponse> RejectPackage(Guid id);
        Task<ApiResponse> DeliverSelectedPackages(Guid deliverId, List<Guid> packageIds);
        Task<ApiResponse> SenderCancelPackage(Guid packageId, string? reason);
        Task<ApiResponse> DeliverCancelPackage(Guid packageId, string? reason);
        Task<ApiResponse> PickupPackageFailed(PickupPackageFailedModel model);
        Task<ApiResponse> PickupPackageSuccess(Guid packageId);
        Task<ApiResponse> DeliveredSuccess(Guid packageId);
        Task<ApiResponse> DeliveredFailed(DeliveredFailedModel packageId);
        Task<ApiResponse> RefundSuccess(Guid packageId);
        Task<ApiResponse> RefundFailed(Guid packageId);
        Task<ApiResponse<List<ResponseComboPackageModel>>> SuggestCombo(Guid deliverId);
        Task<ApiResponse<List<ResponseComboPackageModel>>> SuggestComboV2(Guid deliverId);
        Task<List<Package>> GetPackagesNearTimePickup();
        Task<List<Package>> GetPackagesNearTimeDelivery();
    }
}
