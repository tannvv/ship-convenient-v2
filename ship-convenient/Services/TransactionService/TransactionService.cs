using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Services.GenericService;
using System.Linq.Expressions;
using unitofwork_core.Model.TransactionModel;

namespace ship_convenient.Services.TransactionService
{
    public class TransactionService : GenericService<TransactionService>, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;
        public TransactionService(ILogger<TransactionService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _transactionRepo = unitOfWork.Transactions;
            _accountRepo = unitOfWork.Accounts;
        }

        public async Task<ApiResponsePaginated<ResponseTransactionModel>> GetTransactions(Guid accountId, DateTime? from, DateTime? to, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseTransactionModel> response = new ApiResponsePaginated<ResponseTransactionModel>();
            #region Verify params
            if (pageIndex < 0 || pageSize < 1)
            {
                response.ToFailedResponse("Thông tin phân trang không hợp lệ");
                return await Task.FromResult(response);
            }
            #endregion
            #region Predicates
            List<Expression<Func<Transaction, bool>>> predicates = new ();
            Expression<Func<Transaction, bool>> predicateAccount = 
                (trans) => trans.AccountId == accountId;
            predicates.Add(predicateAccount);
            if (from != null && to != null)
            {
                Expression<Func<Transaction, bool>> predicateDateTime = (transaction) => transaction.CreatedAt.CompareTo(from) >= 0
                            && transaction.CreatedAt.CompareTo(to) <= 0;
                predicates.Add(predicateDateTime);
            }
            #endregion
            #region Order
            Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy = (source) => source.OrderByDescending(tr => tr.CreatedAt);
            #endregion
            #region Selector
            Expression<Func<Transaction, ResponseTransactionModel>> selector = (tran) => tran.ToResponseModel();
            #endregion
            PaginatedList<ResponseTransactionModel> result = await _transactionRepo.GetPagedListAsync<ResponseTransactionModel>(
                predicates: predicates, orderBy: orderBy, selector: selector);
            response.SetData(result);
            response.ToSuccessResponse("Lấy thông tin thành công");
            return response;
        }
    }
}
