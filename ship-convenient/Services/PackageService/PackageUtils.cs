using Microsoft.EntityFrameworkCore;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Services.GenericService;
using unitofwork_core.Constant.ConfigConstant;
using unitofwork_core.Constant.Package;
using unitofwork_core.Model.PackageModel;
using unitofwork_core.Model.ProductModel;

namespace ship_convenient.Services.PackageService
{
    public class PackageUtils : GenericService<PackageService>
    {
        public PackageUtils(ILogger<PackageService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
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

        public async Task<int> BalanceAvaiableDeliver(Guid deliverId) {
            int result = 0;
            if (await IsNewDeliver(deliverId)) {
                int balanceDefault = _configRepo.GetDefaultBalanceNewAccount();
                result += balanceDefault;
            };
            List<string> validStatus = new() { 
                PackageStatus.DELIVER_PICKUP
            };
            List<Package> packages = await _packageRepo.GetAllAsync(
                include: source => source.Include(p => p.Products),
                predicate: p => validStatus.Contains(p.Status) && p.DeliverId == deliverId);
            int totalBalanceNotAvaiable = 0;
            foreach (var item in packages) {
                totalBalanceNotAvaiable += item.Products.Sum(pro => pro.Price);
            }
            Account? deliver = await _accountRepo.GetByIdAsync(deliverId);
            if (deliver == null) throw new ArgumentException("Tài khoản không tồn tại");
            return deliver.Balance - totalBalanceNotAvaiable;
        }

        public bool IsValidPackageBalance(int accountBalance, ResponsePackageModel package) {
            bool result = false;
            int priceOfPackage = 0;
            foreach (var item in package.Products) {
                priceOfPackage += item.Price;
            }
            if (accountBalance - priceOfPackage >= 0) result = true;
            return result;
        }

        public bool IsValidComboBalance(int accountBalance, ResponseComboPackageModel combo)
        {
            bool result = false;
            if (accountBalance - combo.ComboPrice >= 0) result = true;
            return result;
        }
    }
}
