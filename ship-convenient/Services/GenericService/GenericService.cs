using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Core.UnitOfWork;

namespace ship_convenient.Services.GenericService
{
    public class GenericService<T>
    {
        protected readonly ILogger<T> _logger;
        protected readonly IUnitOfWork _unitOfWork;

        public GenericService(ILogger<T> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public string? VerifyPaging(int pageIndex, int pageSize) {
            if (pageIndex < 0)
            {
                return "Số trang phải lớn hơn hoặc bằng 0";
            }
            if (pageSize < 1)
            {
                return "Số phần tử của trang phải lớn hơn 0";
            }
            return null;
        }

        public bool IsExistedAccount(Guid id) {
            return _unitOfWork.Accounts.GetById(id: id) != null;
        }
        public bool IsExistedPackage(Guid id)
        {
            return _unitOfWork.Packages.GetById(id: id) != null;
        }

    }
}
