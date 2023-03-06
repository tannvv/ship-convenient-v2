using ship_convenient.Core.CoreModel;
using ship_convenient.Entities;
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
        Task<ApiResponse> DeliverPickupPackages(Guid deliverId, List<Guid> packageIds);
        Task<ApiResponse> SenderCancelPackage(Guid packageId, string? reason);
        Task<ApiResponse> DeliverCancelPackage(Guid packageId, string? reason);
        Task<ApiResponse> ConfirmPackages(Guid packageId);
        Task<ApiResponse> DeliverDeliverySuccess(Guid packageId);
        Task<ApiResponse> DeliveryFailed(Guid packageId);
   /*     Task<ApiResponse> SenderConfirmDeliverySuccess(Guid packageId);
        Task<ApiResponse> SenderConfirmDeliveryFailed(Guid packageId);*/
        Task<ApiResponse> RefundSuccess(Guid packageId);
        Task<ApiResponse> RefundFailed(Guid packageId);
    /*    Task<ApiResponse<List<ResponseComboPackageModel>>> SuggestCombo(Guid shipperId, int pageIndex, int pageSize);*/
        Task<ApiResponse<List<ResponseComboPackageModel>>> SuggestCombo(Guid shipperId);
        Task<List<Package>> GetPackagesNearTimePickup();
        Task<List<Package>> GetPackagesNearTimeDelivery();
    }
}
