﻿using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using ship_convenient.Entities;

namespace ship_convenient.Core.Repository
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }
    }
}
