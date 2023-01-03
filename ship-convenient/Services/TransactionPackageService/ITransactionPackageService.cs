using ship_convenient.Core.CoreModel;
using ship_convenient.Model.TransactionPackageModel;

namespace ship_convenient.Services.TransactionPackageService
{
    public interface ITransactionPackageService
    {
        Task<ApiResponsePaginated<ResponseTransactionPackageModel>> GetHistoryPackage(Guid packageId, int pageIndex, int pageSize);
        
    }
}
