using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Model.RouteModel;
using Route = ship_convenient.Entities.Route;
using ship_convenient.Services.GenericService;
using ship_convenient.Entities;
using Microsoft.EntityFrameworkCore;
using ship_convenient.Constants.AccountConstant;

namespace ship_convenient.Services.RouteService
{
    public class RouteService : GenericService<RouteService>, IRouteService
    {
        private readonly IRouteRepository _routeRepo;
        private readonly IInfoUserRepository _infoUserRepo;
        private readonly IAccountRepository _accountRepo;
        public RouteService(ILogger<RouteService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _routeRepo = unitOfWork.Routes;
            _infoUserRepo = unitOfWork.InfoUsers;
            _accountRepo = unitOfWork.Accounts;
        }

        public async Task<ApiResponse<ResponseRouteModel>> Create(CreateRouteModel model)
        {
            ApiResponse<ResponseRouteModel> response = new ApiResponse<ResponseRouteModel>();
            Account? account = await _accountRepo.FirstOrDefaultAsync(
                predicate: (i) => i.Id == model.AccountId, disableTracking: false, include: (en) => en.Include(info => info.InfoUser));

            #region verify params
            if (account == null)
            {
                response.ToFailedResponse("Thông tin người dùng không tồn tại");
                return response;
            }
            #endregion
            Route route = model.ConvertToEntity(account.InfoUser!.Id);
            if (account.Status == AccountStatus.NO_ROUTE)
            {
                route.IsActive = true;
                await _routeRepo.InsertAsync(route);
                account.Status = AccountStatus.ACTIVE;
            }
            else
            {
                route.IsActive = false;
                await _routeRepo.InsertAsync(route);
            }
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse(route.ToResponseModel(), "Tạo tuyến đường thành công");
            }
            else
            {
                response.ToFailedResponse("Tạo tuyến đường thất bại");
            }
            return response;

        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            ApiResponse reponse = new();
            Route? route = await _routeRepo.GetByIdAsync(id);
            if (route != null)
            {
                await _routeRepo.DeleteAsync(id);
                int result = await _unitOfWork.CompleteAsync();
                if (result > 0)
                {
                    reponse.ToSuccessResponse("Xóa tuyến đường thành công");
                    return reponse;
                }
            }
            reponse.ToFailedResponse("Xóa tuyến đường thất bại");
            return reponse;
        }

        public async Task<ApiResponse<List<ResponseRouteModel>>> GetRouteUserId(Guid accountId)
        {
            ApiResponse<List<ResponseRouteModel>> response = new();
            List<ResponseRouteModel> routes = _routeRepo.GetAll(predicate: (route) =>
                route.InfoUser != null ? route.InfoUser.AccountId == accountId : false,
                selector: route => route.ToResponseModel(),
                orderBy: source => source.OrderByDescending(route => route.IsActive)).ToList();
            if (routes.Count > 0)
            {
                response.ToSuccessResponse(routes, "Lấy thông tin thành công");
            }
            else
            {
                response.ToFailedResponse("Người giao không tồn tại");
            }
            return await Task.FromResult(response);

        }

        public async Task<ApiResponse> SetActiveRoute(Guid routeId)
        {
            ApiResponse response = new ApiResponse();
            Route? route = await _routeRepo.GetByIdAsync(routeId);
            if (route != null)
            {
                List<Route> routes = await _routeRepo.GetAllAsync(predicate:
                    routeFilter => route.InfoUserId == routeFilter.InfoUserId, disableTracking: false);
                for (int i = 0; i < routes.Count; i++)
                {
                    if (routes[i].Id == route.Id)
                    {
                        routes[i].IsActive = true;
                    }
                    else
                    {
                        routes[i].IsActive = false;
                    }
                }
                int result = await _unitOfWork.CompleteAsync();
                if (result > 0)
                {
                    response.ToSuccessResponse("Yêu cầu thành công");
                    return response;

                }
            }
            response.ToFailedResponse("Yêu cầu không thành công");
            return response;
        }
    }
}
