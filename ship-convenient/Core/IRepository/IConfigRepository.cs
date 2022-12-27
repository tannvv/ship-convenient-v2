﻿using ship_convenient.Entities;

namespace ship_convenient.Core.IRepository
{
    public interface IConfigRepository : IGenericRepository<ConfigApp>
    {
        string GetValueConfig(string configName);
    }
}
