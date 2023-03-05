using Microsoft.EntityFrameworkCore;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
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

        public bool IsValidComboBalance(int accountBalance, ResponseComboPackageModel combo)
        {
            bool result = false;
            if (accountBalance - combo.ComboPrice >= 0) result = true;
            return result;
        }

        public async Task<bool> IsMaxCancelInDay(Guid deliverId) {
            bool result = false;
            List<Package> packages = await _packageRepo.GetAllAsync(
                predicate: p => p.DeliverId == deliverId && p.Status == PackageStatus.DELIVER_CANCEL);
            packages = packages.Where(p => Utils.IsTimeToday(p.ModifiedAt)).ToList();
            if (packages.Count > _configRepo.GetMaxCancelInDay()) result = true;
            return result;
        }
    }
}
