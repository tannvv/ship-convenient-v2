using ship_convenient.Core.CoreModel;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.ConfigModel;
using ship_convenient.Services.GenericService;

namespace ship_convenient.Services.ConfigService
{
    public class ConfigService : GenericService<ConfigService>, IConfigService
    {
        public ConfigService(ILogger<ConfigService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
        }

        public async Task<ApiResponse<List<ConfigApp>>> GetAll()
        {
            ApiResponse<List<ConfigApp>> response = new();
            response.ToSuccessResponse(await _configRepo.GetAllAsync(), "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse<ConfigApp>> Update(UpdateConfigModel model)
        {
            ApiResponse<ConfigApp> response = new();
            ConfigApp? config = await _configRepo.FirstOrDefaultAsync((c) => c.Name == model.Name, disableTracking: false);
            if (config == null) {
                response.ToFailedResponse("Không tìm thấy thông tin cấu hình");
                return response;
            }
            config.Note = model.Note;
            config.ModifiedBy = model.ModifiedBy;
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse(config, "Cập nhật thông tin thành công");
            }
            else {
                response.ToFailedResponse("Cập nhật thông tin thất bại");
            }
            return response;
        }
    }
}
