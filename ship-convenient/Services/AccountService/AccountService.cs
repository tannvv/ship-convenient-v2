using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Constants.AccountConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.GenericService;

namespace ship_convenient.Services.AccountService
{
    public class AccountService : GenericService<AccountService> ,IAccountService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IInfoUserRepository _infoUserRepo;
        public AccountService(ILogger<AccountService> logger, IUnitOfWork unitOfWork) 
            : base(logger, unitOfWork)
        {
            _accountRepo = unitOfWork.Accounts;
            _infoUserRepo = unitOfWork.InfoUsers;
        }

        public async Task<ApiResponse<ResponseAccountModel>> Create(CreateAccountModel model)
        {
            ApiResponse<ResponseAccountModel> response = new();
            #region verify params
            if (model.IsCreateInfo()) {
                InfoUser? _checkEmail = await _infoUserRepo.FirstOrDefaultAsync(
                    predicate: (ac) => ac.Email == model.Email);
                if (_checkEmail != null)
                {
                    response.ToFailedResponse("Email đã tồn tại, không thể đăng kí");
                    return response;
                }
                InfoUser? _checkPhone = await _infoUserRepo.FirstOrDefaultAsync(
                    predicate: (ac) => ac.Phone == model.Phone);
                if (_checkPhone != null)
                {
                    response.ToFailedResponse("Số điện thoại đã tồn tại, không thể đăng kí");
                    return response;
                }
            }
            Account? _checkUserName = await _accountRepo.FirstOrDefaultAsync(
                    predicate: (ac) => ac.UserName == model.UserName);
            if (_checkUserName != null)
            {
                response.ToFailedResponse("Tên đăng nhập đã tồn tại, không thể đăng kí");
                return response;
            }
            #endregion
            Account account = model.ToEntityModel();
            account.Status = AccountStatus.NO_ROUTE;
            await _accountRepo.InsertAsync(account);
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0) {
                response.ToSuccessResponse(account.ToResponseModel(), "Tạo tài khoản thành công");
            }
            else
            {
                response.ToFailedResponse("Tạo tài khoản thất bại");
            }
            return response;
        }

        public async Task<ApiResponse<ResponseAccountModel>> GetId(Guid id)
        {
            ApiResponse<ResponseAccountModel> response = new();
            #region Includable
            Func<IQueryable<Account>, IIncludableQueryable<Account, object?>> include = (acc) => acc.Include(a => a.InfoUser);
            #endregion
            Account? account = await _accountRepo.GetByIdAsync(id: id,
                include: include);
            if (account == null)
            {
                response.ToFailedResponse("Không tìm thấy tài khoản");
            }
            else
            {
                response.ToSuccessResponse(account.ToResponseModel(), " Lấy thông tin thành công");
            }
            return response;
        }

        public Task<PaginatedList<ResponseAccountModel>> GetList(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
