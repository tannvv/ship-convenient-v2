﻿using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
using ship_convenient.Model.FirebaseNotificationModel;
using ship_convenient.Model.MapboxModel;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.FirebaseCloudMsgService;
using ship_convenient.Services.GenericService;
using ship_convenient.Services.MapboxService;
using unitofwork_core.Constant.ConfigConstant;
using unitofwork_core.Constant.Package;
using unitofwork_core.Model.PackageModel;
using unitofwork_core.Model.ProductModel;
using RouteEntity = ship_convenient.Entities.Route;

namespace ship_convenient.Services.PackageService
{
    public class PackageUtils : GenericService<PackageService>
    {
        private readonly AccountUtils _accountUtils;
        protected readonly IFirebaseCloudMsgService _fcmService;
        private readonly IMapboxService _mapboxService;

        public PackageUtils(ILogger<PackageService> logger, IUnitOfWork unitOfWork, AccountUtils accountUtils, IMapboxService mapboxService
            , IFirebaseCloudMsgService fcmService) : base(logger, unitOfWork)
        {
            _accountUtils = accountUtils;
            _fcmService = fcmService;
            _mapboxService = mapboxService;
        }

        public bool IsValidComboBalance(int accountBalance, ResponseComboPackageModel combo)
        {
            bool result = false;
            if (accountBalance - combo.ComboPrice >= 0) result = true;
            return result;
        }

        public async Task<bool> IsMaxCancelInDay(Guid deliverId)
        {
            bool result = false;
            List<Package> packages = await _packageRepo.GetAllAsync(
                predicate: p => p.DeliverId == deliverId && p.Status == PackageStatus.DELIVER_CANCEL);
            packages = packages.Where(p => Utils.IsTimeToday(p.ModifiedAt)).ToList();
            if (packages.Count > _configRepo.GetMaxCancelInDay()) result = true;
            return result;
        }


        public async Task NotificationValidUserWithPackage(Package package)
        {
            List<Account> activeAccounts = await _accountUtils.GetListAccountActive();
            int pricePackage = package.GetPricePackage();
            List<Account> validBalanceAccounts = activeAccounts.Where(ac => (_accountUtils.AvailableBalance(ac.Id) - pricePackage) > 0).ToList();
            int count = validBalanceAccounts.Count;
            for (int i = 0; i < count; i++)
            {
                Account account = validBalanceAccounts[i];
                int spacingValid = _configUserRepo.GetPackageDistance(account.InfoUser.Id);
                string directionSuggest = _configUserRepo.GetDirectionSuggest(account.InfoUser.Id);
                RouteEntity? activeRoute = await _routeRepo.FirstOrDefaultAsync(predicate: route => 
                        route.InfoUserId == account.InfoUser.Id && route.IsActive == true);
                if (activeRoute != null)
                {
                    List<RoutePoint> routePoints = await _routePointRepo.GetAllAsync(predicate:
                            (routePoint) => activeRoute == null ? false : routePoint.RouteId == activeRoute.Id);
                    if (directionSuggest == DirectionTypeConstant.FORWARD)
                    {
                        routePoints = routePoints.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                                .OrderBy(source => source.Index).ToList();
                    }
                    else if (directionSuggest == DirectionTypeConstant.BACKWARD)
                    {
                        routePoints = routePoints.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                                .OrderBy(source => source.Index).ToList();
                    }
                    bool isValidOrder = MapHelper.ValidDestinationBetweenDeliverAndPackage(routePoints, package, spacingValid);
                    if (isValidOrder && account.RegistrationToken != "")
                    {

                        SendNotificationModel model = new SendNotificationModel() { 
                            AccountId = account.Id,
                            Title = "Có gói hàng phù hợp",
                            Body = "Có gói hàng phù hợp với lộ trình của bạn, nhanh tay vào và nhận thôi!!!"
                        };
                        try
                        {
                            await _fcmService.SendNotification(model);
                        }
                        catch (Exception ex) {
                            _logger.LogError("FCM exception" + ex.Message);
                        }
                    }
                }

              
            }
        }

        public async Task<List<RoutePoint>> GetRouteVirtual(List<GeoCoordinate> orderPoints, Guid routeId) {
            List<ResponsePolyLineModel> polylines = await _mapboxService.GetPolyLine(DirectionApiModel.FromListGeoCoordinate(orderPoints));
            List<RoutePoint> routePoints = new List<RoutePoint>();
            List<CoordinateApp>? points= polylines[0].PolyPoints;
            if (polylines[0].PolyPoints == null) new ArgumentNullException("polyline is null");
            int count = points!.Count;
            for (int i = 0; i < count; i++)
            {
                CoordinateApp point = points[i];
                RoutePoint routePoint = new RoutePoint()
                {
                    RouteId = routeId,
                    Index = i,
                    Latitude = point.Latitude,
                    Longitude = point.Longitude,
                    DirectionType = "VIRTUAL",
                    IsVitual = true
                };
                routePoints.Add(routePoint);
            }
            

            return routePoints;
        }

        public async Task RemoveRouteVirtual(Guid routeId) {
            
            List<RoutePoint> routePoints = await _routePointRepo.GetAllAsync(predicate: routePoint => routePoint.RouteId == routeId && routePoint.IsVitual == true);
            _routePointRepo.DeleteRange(routePoints);
        }
      
    }
}
