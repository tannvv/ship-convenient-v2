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

    }
}
