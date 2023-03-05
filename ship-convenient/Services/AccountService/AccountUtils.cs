using Microsoft.EntityFrameworkCore;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Services.GenericService;
using unitofwork_core.Constant.Package;

namespace ship_convenient.Services.AccountService
{
    public class AccountUtils : GenericService<AccountUtils>
    {
        public AccountUtils(ILogger<AccountUtils> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
        }

        public async Task<int> AvailableBalance(Guid accountId)
        {
            int result = 0;
            if (await IsNewDeliver(accountId))
            {
                int balanceDefault = _configRepo.GetDefaultBalanceNewAccount();
                result += balanceDefault;
                return result;
            };
            List<string> validStatus = new() {
                PackageStatus.DELIVER_PICKUP
            };
            List<Package> packages = await _packageRepo.GetAllAsync(
                include: source => source.Include(p => p.Products),
                predicate: p => validStatus.Contains(p.Status) && p.DeliverId == accountId);
            int totalBalanceNotAvaiable = 0;
            foreach (var item in packages)
            {
                totalBalanceNotAvaiable += item.Products.Sum(pro => pro.Price);
            }
            Account? deliver = await _accountRepo.GetByIdAsync(accountId);
            if (deliver == null) throw new ArgumentException("Tài khoản không tồn tại");
            return deliver.Balance - totalBalanceNotAvaiable;
        }

        public async Task<bool> IsNewDeliver(Guid deliverId)
        {
            bool result = false;
            List<Package> packages = await _packageRepo.GetAllAsync(
                predicate: p => p.DeliverId == deliverId);
            Account? deliver = await _accountRepo.GetByIdAsync(deliverId);
            if (packages.Count == 0 || deliver?.Balance > 0) result = true;
            return result;
        }
    }
}
