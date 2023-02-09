using ship_convenient.Core.CoreModel;
using ship_convenient.Entities;
using ship_convenient.Model.ConfigModel;

namespace ship_convenient.Services.ConfigService
{
    public interface IConfigService
    {
        Task<ApiResponse<List<ConfigApp>>> GetAll();
        Task<ApiResponse<ConfigApp>> Update(UpdateConfigModel model);
    }
}
