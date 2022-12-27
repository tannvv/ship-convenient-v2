using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using ship_convenient.Entities;

namespace ship_convenient.Core.Repository
{
    public class ConfigRepository : GenericRepository<ConfigApp>, IConfigRepository
    {
        public ConfigRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public string GetValueConfig(string configName)
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(configName));
            if (configApp != null)
            {
                return configApp.Note;
            }
            return "";
        }
    }
}
