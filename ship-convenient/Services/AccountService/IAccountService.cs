using ship_convenient.Core.CoreModel;
using ship_convenient.Model.UserModel;

namespace ship_convenient.Services.AccountService
{
    public interface IAccountService
    {
        Task<ApiResponse<ResponseAccountModel>> Create(CreateAccountModel model);
        Task<ApiResponse<ResponseAccountModel>> Update(UpdateAccountModel model);
        Task<ApiResponse<ResponseAccountModel>> UpdateInfo(UpdateInfoModel model);
        Task<ApiResponse<ResponseAccountModel>> GetId(Guid id);
        Task<ApiResponsePaginated<ResponseAccountModel>> GetList(string? userName, string? status, int pageIndex, int pageSize);
        
    }
}
