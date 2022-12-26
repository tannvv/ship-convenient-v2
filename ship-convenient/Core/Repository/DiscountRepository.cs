﻿using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using ship_convenient.Entities;

namespace ship_convenient.Core.Repository
{
    public class DiscountRepository : GenericRepository<Discount>, IDiscountRepository
    {
        public DiscountRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }
    }
}
