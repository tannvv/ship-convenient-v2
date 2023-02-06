using ship_convenient.Core.CoreModel;
using ship_convenient.Model.PackageModel;
using ship_convenient.Model.TransactionPackageModel;

namespace ship_convenient.Services.TransactionPackageService
{
    public interface ITransactionPackageService
    {
        Task<ApiResponsePaginated<ResponseTransactionPackageModel>> GetHistoryPackage(Guid packageId, int pageIndex, int pageSize);
        Task<ApiResponsePaginated<ResponseCancelPackageModel>> GetDeliverCancelPackage(Guid deliverId, int pageIndex, int pageSize);
        
    }
}
