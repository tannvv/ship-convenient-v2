﻿using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using ship_convenient.Entities;

namespace ship_convenient.Core.Repository
{
    public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }
    }
}