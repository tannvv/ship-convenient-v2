﻿using ship_convenient.Core.Context;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.AuthorizeService;
using ship_convenient.Services.DatabaseService;
using ship_convenient.Services.GoongService;
using ship_convenient.Services.MapboxService;
using ship_convenient.Services.PackageService;
using ship_convenient.Services.RouteService;
using ship_convenient.Services.TransactionPackageService;
using ship_convenient.Services.TransactionService;

namespace ship_convenient.Config
{
    public static class DIServiceExtension
    {
        public static void AddDIService(this IServiceCollection services) {
            services.AddDbContext<AppDbContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IGoongService, GoongService>();
            services.AddTransient<IMapboxService, MapboxService>();
            services.AddTransient<IDatabaseService, DatabaseService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAuthorizeService, AuthorizeService>();
            services.AddTransient<IRouteService, RouteService>();
            services.AddTransient<IPackageService, PackageService>();
            services.AddTransient<ITransactionPackageService, TransactionPackageService>();
            services.AddTransient<ITransactionService, TransactionService>();
        }
    }
}
