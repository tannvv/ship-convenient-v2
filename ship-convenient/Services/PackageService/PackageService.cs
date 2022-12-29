﻿using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Constants.AccountConstant;
using ship_convenient.Constants.DatimeConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
using ship_convenient.Model.MapboxModel;
using ship_convenient.Services.GenericService;
using ship_convenient.Services.MapboxService;
using System.Linq.Expressions;
using unitofwork_core.Constant.ConfigConstant;
using unitofwork_core.Constant.Package;
using unitofwork_core.Constant.Transaction;
using unitofwork_core.Model.PackageModel;
using unitofwork_core.Model.ProductModel;
using Route = ship_convenient.Entities.Route;

namespace ship_convenient.Services.PackageService
{
    public class PackageService : GenericService<PackageService>, IPackageService
    {
        private readonly IPackageRepository _packageRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ITransactionPackageRepository _transactionPackageRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IConfigRepository _configRepo;
        private readonly IRouteRepository routeRepo;
        private readonly IMapboxService _mapboxService;


        public PackageService(ILogger<PackageService> logger, IUnitOfWork unitOfWork,
            IMapboxService mapboxService) : base(logger, unitOfWork)
        {
            _packageRepo = unitOfWork.Packages;
            _accountRepo = unitOfWork.Accounts;
            _transactionPackageRepo = unitOfWork.TransactionPackages;
            _transactionRepo = unitOfWork.Transactions;
            _configRepo = unitOfWork.Configs;
            routeRepo = unitOfWork.Routes;

            _mapboxService = mapboxService;
        }

        public async Task<ApiResponse<ResponsePackageModel>> Create(CreatePackageModel model)
        {
            ApiResponse<ResponsePackageModel> response = new();
            #region Verify params
            Account? account = await _accountRepo.GetByIdAsync(model.CreatorId);
            if (account == null)
            {
                response.ToFailedResponse("Tài khoản không tồn tại");
                return response;
            }
            #endregion

            Package package = model.ConverToEntity();
            await _packageRepo.InsertAsync(package);

            #region Create history
            TransactionPackage history = new();
            history.FromStatus = PackageStatus.NOT_EXIST;
            history.ToStatus = PackageStatus.WAITING;
            history.Description = "Đơn hàng được tạo vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion

            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Tạo đơn hàng thành công" : "Tạo đơn thất bại";
            response.Data = result > 0 ? package.ToResponseModel() : null;
            #endregion

            return response;
        }

        public async Task<ApiResponse> ApprovedPackage(Guid id)
        {
            ApiResponse response = new ApiResponse();
            Package? package = await _packageRepo.GetByIdAsync(id, disableTracking: false);

            #region Verify params
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
                return response;
            }
            if (package.Status != PackageStatus.WAITING)
            {
                response.ToFailedResponse("Gói hàng không tồn tại không ở trạng thái chờ để duyệt");
                return response;
            }
            #endregion

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.APPROVED;
            history.Description = "Đơn hàng được duyệt vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion
            package.Status = PackageStatus.APPROVED;

            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Duyệt đơn thành công" : "Duyệt đơn thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse> DeliveryFailed(Guid packageId)
        {
            ApiResponse response = new ApiResponse();
            Package? package = await _packageRepo.GetByIdAsync(packageId, disableTracking: false);

            #region Verify params
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
                return response;
            }
            if (package.Status != PackageStatus.DELIVERY)
            {
                response.ToFailedResponse("Gói hàng không ở trạng thái đang giao để chuyển sang giao thất bại");
                return response;
            }
            #endregion

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.DELIVERY_FAILED;
            history.Description = "Giao thất bại vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            package.Status = PackageStatus.DELIVERY_FAILED;
            #endregion

            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Yêu cầu thành công" : "Yêu cầu thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse<List<ResponsePackageModel>>> GetAll(Guid deliverId, Guid creatorId, string? status)
        {
            ApiResponse<List<ResponsePackageModel>> response = new ApiResponse<List<ResponsePackageModel>>();
            #region Includable
            Func<IQueryable<Package>, IIncludableQueryable<Package, object?>> include = (source) => source.Include(p => p.Products)
            .Include(p => p.Deliver).ThenInclude(d => d == null ? null : d.InfoUser)
                .Include(p => p.Creator).ThenInclude(c => c == null ? null : c.InfoUser);
            #endregion
            #region Order
            Func<IQueryable<Package>, IOrderedQueryable<Package>> orderBy = (source) => source.OrderByDescending(p => p.ModifiedAt);
            #endregion
            #region Predicates
            List<Expression<Func<Package, bool>>> predicates = new List<Expression<Func<Package, bool>>>();
            if (deliverId != Guid.Empty)
            {
                Expression<Func<Package, bool>> filterShipper = (p) => p.DeliverId == deliverId;
                predicates.Add(filterShipper);
            }
            if (creatorId != Guid.Empty)
            {
                Expression<Func<Package, bool>> filterShop = (p) => p.CreatorId == creatorId;
                predicates.Add(filterShop);
            }
            if (status != null)
            {
                Expression<Func<Package, bool>> filterStatus = (p) => p.Status == status.ToUpper();
                predicates.Add(filterStatus);
            }
            #endregion
            Expression<Func<Package, ResponsePackageModel>> selector = (package) => package.ToResponseModel();
            List<ResponsePackageModel> packages = await _packageRepo.GetAllAsync(predicates: predicates, selector: selector, orderBy: orderBy, include: include);
            response.ToSuccessResponse(packages, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse<ResponsePackageModel>> GetById(Guid id)
        {
            #region Includable
            Func<IQueryable<Package>, IIncludableQueryable<Package, object?>> include = (p) => p.Include(p => p.Products).Include(p => p.Creator).Include(p => p.Deliver);
            #endregion

            Package? package = await _packageRepo.GetByIdAsync(id, include: include);

            #region Response result
            ApiResponse<ResponsePackageModel> response = new ApiResponse<ResponsePackageModel>();
            if (package != null)
            {
                response.Message
                    = "Lấy thông tin đơn hàng thành công";
                response.Data = package.ToResponseModel();
            }
            else
            {
                response.Success = false;
                response.Message = "Id đơn hàng không tồn tại";
            }
            #endregion

            return response;
        }

        public async Task<ApiResponsePaginated<ResponsePackageModel>> GetFilter(Guid? deliverId, Guid? creatorId, string? status, int? pageIndex, int? pageSize)
        {
            ApiResponsePaginated<ResponsePackageModel> response = new ApiResponsePaginated<ResponsePackageModel>();
            #region Verify params
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex < 0 || pageSize < 1)
                {
                    response.ToFailedResponse("Thông tin phân trang không hợp lệ");
                    return response;
                }
            }
            #endregion

            #region Includable
            Func<IQueryable<Package>, IIncludableQueryable<Package, object?>> include = (source) => source.Include(p => p.Products)
                .Include(p => p.Deliver).ThenInclude(p => p != null ? p.InfoUser : null)
                .Include(p => p.Creator).ThenInclude(c => c != null ? c.InfoUser : null);
            #endregion

            #region Predicates
            List<Expression<Func<Package, bool>>> predicates = new List<Expression<Func<Package, bool>>>();
            if (deliverId != Guid.Empty && deliverId != null)
            {
                Expression<Func<Package, bool>> filterShipper = (p) => p.DeliverId == deliverId;
                predicates.Add(filterShipper);
            }
            if (creatorId != Guid.Empty && creatorId != null)
            {
                Expression<Func<Package, bool>> filterShop = (p) => p.CreatorId == creatorId;
                predicates.Add(filterShop);
            }
            if (status != null)
            {
                Expression<Func<Package, bool>> filterStatus = (p) => p.Status == status.ToUpper();
                predicates.Add(filterStatus);
            }
            #endregion

            #region Order
            Func<IQueryable<Package>, IOrderedQueryable<Package>> orderBy = (source) => source.OrderByDescending(p => p.ModifiedAt);
            #endregion

            Expression<Func<Package, ResponsePackageModel>> selector = (package) => package.ToResponseModel();
            PaginatedList<ResponsePackageModel> items;
            if (pageIndex != null && pageSize != null)
            {
                items = await _packageRepo.GetPagedListAsync(
                selector: selector, include: include, predicates: predicates,
                orderBy: orderBy, pageIndex: pageIndex ?? 0, pageSize: pageSize ?? 0);
            }
            else
            {
                int countAll = _packageRepo.Count();
                items = await _packageRepo.GetPagedListAsync(
                selector: selector, include: include, predicates: predicates,
                orderBy: orderBy, pageIndex: 0, pageSize: countAll);
            }
            _logger.LogInformation("Total count: " + items.TotalCount);
            #region Response result
            response.SetData(items);
            int countPackage = items.Count;
            if (countPackage > 0)
            {
                response.Message = "Không tìm không đơn hàng";
            }
            else
            {
                response.Message = "Lấy thông tin đơn hàng thành công";
            }
            #endregion
            return response;
        }

        public async Task<ApiResponse> RefundFailed(Guid packageId)
        {
            ApiResponse response = new ApiResponse();
            Package? package = await _packageRepo.GetByIdAsync(packageId, disableTracking: false);
            #region Verify params
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
                return response;
            }
            if (package.Status != PackageStatus.DELIVERED)
            {
                response.ToFailedResponse("Hàng không giao thất bại thì hoàn trả cái gì!!");
                return response;
            }
            #endregion
            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.REFUND_FAILED;
            history.Description = $"Người giao ({package.DeliverId}) trả hàng thất bại vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;

            await _transactionPackageRepo.InsertAsync(history);
            package.Status = PackageStatus.REFUND_FAILED;
            #endregion
            await _unitOfWork.CompleteAsync();

            return response;
        }

        public async Task<ApiResponse> RefundSuccess(Guid packageId)
        {
            ApiResponse response = new ApiResponse();
            decimal profitPercent = decimal.Parse(_configRepo.GetValueConfig(ConfigConstant.PROFIT_PERCENTAGE));
            decimal profitPercentRefund = decimal.Parse(_configRepo.GetValueConfig(ConfigConstant.PROFIT_PERCENTAGE_REFUND));

            #region Predicate
            Expression<Func<Account, bool>> predicateAdminBalance = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            #endregion
            #region Includable pakage
            Func<IQueryable<Package>, IIncludableQueryable<Package, object?>> includePackage = (source) => source.Include(p => p.Creator).Include(p => p.Deliver);
            #endregion
            Package? package = await _packageRepo.GetByIdAsync(packageId, disableTracking: false, include: includePackage);
            Account? deliver = package?.Deliver;
            Account? creator = package?.Creator;
            Account? adminBalance = await _accountRepo.FirstOrDefaultAsync(predicate: predicateAdminBalance, disableTracking: false);
            #region Verify params
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
                return response;
            }
            if (deliver == null)
            {
                response.ToFailedResponse("Đơn hàng chưa được người giao pickup");
                return response;
            }
            if (package.Status != PackageStatus.DELIVERY_FAILED)
            {
                response.ToFailedResponse("Hàng không giao thất bại thì hoàn trả cái gì!!");
                return response;
            }
            if (deliver == null || creator == null || adminBalance == null)
            {
                response.ToFailedResponse("Không tìm thấy đủ các tài khoản để tạo giao dịch");
                return response;
            }
            #endregion
            int totalPrice = 0;
            package.Products.ToList().ForEach(pr =>
            {
                totalPrice += pr.Price;
            });

            #region Create transactions
            Transaction systemTrans = new Transaction();
            systemTrans.Description = deliver.Id + " hoàn trả thành công gói hàng với id: " + package.Id;
            systemTrans.Status = TransactionStatus.ACCOMPLISHED;
            systemTrans.TransactionType = TransactionType.REFUND;
            systemTrans.CoinExchange = Convert.ToInt32(Math.Round(package.PriceShip * profitPercent - (package.PriceShip * profitPercentRefund) - totalPrice));
            systemTrans.BalanceWallet = Convert.ToInt32(Math.Round(
                adminBalance.Balance - (package.PriceShip * profitPercentRefund) - totalPrice));
            systemTrans.PackageId = package.Id;
            systemTrans.AccountId = adminBalance.Id;

            Transaction deliverTrans = new Transaction();
            deliverTrans.Description = "Hoàn trả thành công đơn hàng id : " + package.Id;
            deliverTrans.Status = TransactionStatus.ACCOMPLISHED;
            deliverTrans.TransactionType = TransactionType.DELIVERED_SUCCESS;
            deliverTrans.CoinExchange = Convert.ToInt32(Math.Round(
                totalPrice + package.PriceShip * (1 - profitPercent)));
            deliverTrans.BalanceWallet = Convert.ToInt32(
                Math.Round(deliver.Balance + totalPrice +
                package.PriceShip * (1 - profitPercent) * package.PriceShip * profitPercentRefund));
            deliverTrans.PackageId = package.Id;
            deliverTrans.AccountId = deliver.Id;

            Transaction creatorTrans = new Transaction();
            creatorTrans.Description = "Hoàn trả thành công đơn hàng id : " + package.Id;
            creatorTrans.Status = TransactionStatus.ACCOMPLISHED;
            creatorTrans.TransactionType = TransactionType.DELIVERED_SUCCESS;
            creatorTrans.CoinExchange = -package.PriceShip;
            creatorTrans.BalanceWallet = creator.Balance - package.PriceShip;
            creatorTrans.PackageId = package.Id;
            creatorTrans.AccountId = creator.Id;

            adminBalance.Balance = Convert.ToInt32(Math.Round(adminBalance.Balance - (package.PriceShip * profitPercentRefund) - totalPrice));
            deliver.Balance = Convert.ToInt32(Math.Round(deliver.Balance + totalPrice +
                package.PriceShip * (1 - profitPercent) * package.PriceShip * profitPercentRefund));
            creator.Balance = creator.Balance - package.PriceShip;

            List<Transaction> transactions = new List<Transaction> {
                    systemTrans, deliverTrans, creatorTrans
                };
            await _transactionRepo.InsertAsync(transactions);
            #endregion

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.REFUND_SUCCESS;
            history.Description = $"Người tạo đơn xác nhận người giao đã trả hàng thành công vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;

            await _transactionPackageRepo.InsertAsync(history);
            package.Status = PackageStatus.REFUND_SUCCESS;
            #endregion
            await _unitOfWork.CompleteAsync();

            return response;
        }

        public async Task<ApiResponse> RejectPackage(Guid id)
        {
            ApiResponse response = new ApiResponse();
            Package? package = await _packageRepo.GetByIdAsync(id, disableTracking: false);

            #region Verify params
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
                return response;
            }
            if (package.Status != PackageStatus.WAITING)
            {
                response.ToFailedResponse("Gói hàng không tồn tại không ở trạng thái chờ để hủy");
                return response;
            }
            #endregion

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.REJECT;
            history.Description = "Đơn hàng bị từ chối vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion
            package.Status = PackageStatus.REJECT;

            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Từ chối gói hàng thành công" : "Từ chối gói hàng thất bại thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse> DeliverCancelPackage(Guid packageId)
        {
            ApiResponse response = new ApiResponse();
            Package? package = await _packageRepo.GetByIdAsync(packageId, disableTracking: false);

            #region Verify params
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
                return response;
            }
            if (package.Status != PackageStatus.APPROVED && package.Status != PackageStatus.WAITING && package.Status != PackageStatus.DELIVER_PICKUP)
            {
                response.ToFailedResponse("Gói hàng đang ở trạng thái không thể hủy");
                return response;
            }
            #endregion
            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.DELIVER_CANCEL;
            history.Description = "Đơn hàng đã bị người tạo hủy vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion
            package.Status = history.ToStatus;

            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Hủy đơn thành công" : "Hủy đơn thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponseListError> DeliverConfirmPackages(List<Guid> packageIds, Guid deliverId)
        {
            ApiResponseListError response = new ApiResponseListError();

            #region Includable pakage
            Func<IQueryable<Package>, IIncludableQueryable<Package, object>> includePackage = (source) => source.Include(pk => pk.Products);
            #endregion
            #region Predicate system admin balance
            Expression<Func<Account, bool>> predicateAdminBalance = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            #endregion

            Account? deliver = await _accountRepo.GetByIdAsync(deliverId, disableTracking: false);
            Account? adminBalance = await _accountRepo.FirstOrDefaultAsync(
                predicate: predicateAdminBalance, disableTracking: false);

            #region Verify params
            List<Package> packages = new List<Package>();
            List<string> errors = new List<string>();

            if (deliver == null || adminBalance == null)
            {
                errors.Add("UserId không tồn tại, không tìm thấy ví hệ thống");
            }
            #region Checking packages valid
            foreach (Guid id in packageIds)
            {
                Package? package = await _packageRepo.GetByIdAsync(id, include: includePackage, disableTracking: false);
                if (package == null)
                {
                    string error = $"Có gói hàng không tồn tại id: {id}";
                    errors.Add(error);
                }
                else
                {
                    if (package.Status != PackageStatus.DELIVER_PICKUP)
                    {
                        string error = $"Có gói hàng không ở trạng thái đã chọn id: {id}-{package.Status}";
                        errors.Add(error);
                    }
                    else
                    {
                        packages.Add(package);
                    }
                }
            }
            #endregion
            #region Checking balance
            int totalPriceCombo = 0;
            packages.ForEach(p =>
            {
                p.Products.ToList().ForEach(pr =>
                {
                    totalPriceCombo += pr.Price;
                });
            });
            if (deliver == null || deliver.Balance < totalPriceCombo)
            {
                errors.Add("Số dư ví không đủ để thực hiện nhận gói hàng");
            }

            #endregion

            if (errors.Count > 0)
            {
                response.ToFailedResponse(errors);
                return response;
            }
            #endregion

            #region Create transations, history and update wallet deliver and system
            int pakageCount = packages.Count();
            for (int i = 0; i < pakageCount; i++)
            {
                Package package = packages[i];
                int packagePrice = 0;
                package.Products.ToList().ForEach(pr =>
                {
                    totalPriceCombo += pr.Price;
                    packagePrice += pr.Price;
                });
                _logger.LogInformation("Total price package: " + packagePrice);
                #region Create transactions
                Transaction systemTrans = new Transaction();
                systemTrans.Description = $"Người giao ({deliver!.Id})" + "đã nghận gói hàng với id: " + package.Id;
                systemTrans.Status = TransactionStatus.ACCOMPLISHED;
                systemTrans.TransactionType = TransactionType.PICKUP;
                systemTrans.CoinExchange = totalPriceCombo;
                systemTrans.BalanceWallet = adminBalance!.Balance + packagePrice;
                systemTrans.PackageId = package.Id;
                systemTrans.AccountId = adminBalance.Id;

                Transaction deliverTrans = new Transaction();
                deliverTrans.Description = "Đã nhận đơn hàng id : " + package.Id;
                deliverTrans.Status = TransactionStatus.ACCOMPLISHED;
                deliverTrans.TransactionType = TransactionType.PICKUP;
                deliverTrans.CoinExchange = -packagePrice;
                deliverTrans.BalanceWallet = deliver!.Balance - packagePrice;
                deliverTrans.PackageId = package.Id;
                deliverTrans.AccountId = deliver.Id;

                adminBalance.Balance = adminBalance.Balance + packagePrice;
                deliver.Balance = deliver.Balance - packagePrice;

                package.DeliverId = deliver.Id;

                List<Transaction> transactions = new List<Transaction> {
                    systemTrans, deliverTrans
                };
                await _transactionRepo.InsertAsync(transactions);
                #endregion

                #region Create history
                TransactionPackage history = new TransactionPackage();
                history.FromStatus = package.Status;
                history.ToStatus = PackageStatus.DELIVERY;
                history.Description = $"Người giao đang giao đơn hàng vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
                history.PackageId = package.Id;
                package.Status = PackageStatus.DELIVERY;
                await _transactionPackageRepo.InsertAsync(history);
                #endregion
            };
            #endregion
            int result = await _unitOfWork.CompleteAsync();
            #region Response result
            response.Success = result > 0 ? true : false;
            response.Note = result > 0 ? "Nhận hàng để giao thành công" : "Nhận hàng để giao thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse> DeliverDeliverySuccess(Guid packageId)
        {
            ApiResponse response = new ApiResponse();
            Package? package = await _packageRepo.GetByIdAsync(packageId, disableTracking: false);

            #region Verify params
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
                return response;
            }
            if (package.Status != PackageStatus.DELIVERY)
            {
                response.ToFailedResponse("Gói hàng không ở trạng thái đang giao để chuyển sang giao thành công");
                return response;
            }
            #endregion

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.DELIVERED;
            history.Description = $"Người giao ({package.DeliverId}) giao hàng thành công vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            package.Status = PackageStatus.DELIVERED;
            #endregion

            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Yêu cầu thành công" : "Yêu cầu thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse> DeliverPickupPackages(Guid deliverId, List<Guid> packageIds)
        {
            ApiResponse response = new ApiResponse();

            #region Includable account, pakage
            Func<IQueryable<Account>, IIncludableQueryable<Account, object?>> includeDeliver = (source) => source.Include(sh => sh.InfoUser);
            Func<IQueryable<Package>, IIncludableQueryable<Package, object?>> includePackage = (source) => source.Include(sh => sh.Products);
            #endregion
            #region Predicate system admin
            Expression<Func<Account, bool>> predicateSystemAdmin = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            #endregion

            Account? deliver = await _accountRepo.GetByIdAsync(deliverId, include: includeDeliver, disableTracking: false);

            List<Package> packages = new List<Package>();
            for (int i = 0; i < packageIds.Count; i++)
            {
                Package? package = await _packageRepo.GetByIdAsync(packageIds[i], disableTracking: false, include: includePackage);
                if (package == null)
                {
                    response.ToFailedResponse("Có gói hàng không tồn tại");
                    return response;
                }
                if (package.Status != PackageStatus.APPROVED)
                {
                    response.ToFailedResponse("Có gói hàng không tồn tại không ở trạng thái chờ để duyệt");
                    return response;
                }
                packages.Add(package);
            }

            #region Verify params
            decimal totalPrice = 0;
            for (int i = 0; i < packages.Count; i++)
            {
                Package package = packages[i];
                package.Products.ToList().ForEach(pr =>
                {
                    totalPrice += pr.Price;
                });
            }
            if (deliver == null || deliver.Balance < totalPrice)
            {
                response.ToFailedResponse("Số dư ví không đủ để thực hiện nhận gói hàng");
                return response;
            }
            #endregion
            #region Create history
            for (int i = 0; i < packages.Count; i++)
            {
                Package package = packages[i];
                package.DeliverId = deliverId;
                TransactionPackage history = new TransactionPackage();
                history.FromStatus = package.Status;
                history.ToStatus = PackageStatus.DELIVER_PICKUP;
                history.Description = "Đơn hàng được nhận vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
                history.PackageId = package.Id;
                await _transactionPackageRepo.InsertAsync(history);
                package.Status = history.ToStatus;
            }
            #endregion
            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Chọn đơn thành công" : "Chọn đơn thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse> CreatorCancelPackage(Guid packageId)
        {
            ApiResponse response = new ApiResponse();
            Package? package = await _packageRepo.GetByIdAsync(packageId, disableTracking: false);

            #region Verify params
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
                return response;
            }
            if (package.Status != PackageStatus.APPROVED && package.Status != PackageStatus.WAITING && package.Status != PackageStatus.DELIVER_PICKUP)
            {
                response.ToFailedResponse("Gói hàng đang ở trạng thái không thể hủy");
                return response;
            }
            #endregion
            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.CREATOR_CANCEL;
            history.Description = "Đơn hàng đã bị shop hủy vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion
            package.Status = history.ToStatus;

            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Hủy đơn thành công" : "Hủy đơn = thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse> CreatorConfirmDeliverySuccess(Guid packageId)
        {
            ApiResponse response = new ApiResponse();
            decimal profitPercent = decimal.Parse(_configRepo.GetValueConfig(ConfigConstant.PROFIT_PERCENTAGE)) / 100;


            #region Includable pakage
            Func<IQueryable<Package>, IIncludableQueryable<Package, object?>> includePackage = (source) => source.Include(p => p.Creator).Include(p => p.Deliver).Include(p => p.Products);
            #endregion
            Package? package = await _packageRepo.GetByIdAsync(packageId, disableTracking: false, include: includePackage);

            #region Predicate
            Expression<Func<Account, bool>> predicateAdminBalance = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            #endregion

            Account? deliver = package?.Deliver;
            Account? creator = package?.Creator;
            Account? adminBalance = await _accountRepo.FirstOrDefaultAsync(predicateAdminBalance, disableTracking: false);
            #region Verify params
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
                return response;
            }
            if (deliver == null)
            {
                response.ToFailedResponse("Đơn hàng chưa được người giao pickup");
                return response;
            }
            if (package.Status != PackageStatus.DELIVERED)
            {
                response.ToFailedResponse("Gói hàng chưa được giao để có thể hoàn thành");
                return response;
            }
            if (deliver == null || creator == null || adminBalance == null)
            {
                response.ToFailedResponse("Không tìm thấy đủ các ví để tạo giao dịch");
                return response;
            }
            #endregion
            int totalPrice = 0;
            package.Products.ToList().ForEach(pr =>
            {
                totalPrice += pr.Price;
            });
            _logger.LogInformation($"Profit percent: {profitPercent} ,Total price : {totalPrice}");
            
            #region Create transactions
            Transaction systemTrans = new Transaction();
            systemTrans.Description = $"({deliver.Id}) giao thành công gói hàng với id: {package.Id}";
            systemTrans.Status = TransactionStatus.ACCOMPLISHED;
            systemTrans.TransactionType = TransactionType.DELIVERED_SUCCESS;
            systemTrans.CoinExchange = ParseHelper.RoundedToInt(package.PriceShip * profitPercent - totalPrice);
            systemTrans.BalanceWallet = ParseHelper.RoundedToInt(adminBalance.Balance - totalPrice + package.PriceShip * profitPercent);
            systemTrans.PackageId = package.Id;
            systemTrans.AccountId = adminBalance.Id;
            _logger.LogInformation($"System transaction: {systemTrans.CoinExchange}, Balance: {systemTrans.BalanceWallet}");

            Transaction deliverTrans = new Transaction();
            deliverTrans.Description = "Giao thành công đơn hàng id : " + package.Id;
            deliverTrans.Status = TransactionStatus.ACCOMPLISHED;
            deliverTrans.TransactionType = TransactionType.DELIVERED_SUCCESS;
            deliverTrans.CoinExchange = ParseHelper.RoundedToInt(totalPrice + package.PriceShip * (1 - profitPercent));
            deliverTrans.BalanceWallet = ParseHelper.RoundedToInt(deliver.Balance + totalPrice + package.PriceShip * (1 - profitPercent));
            deliverTrans.PackageId = package.Id;
            deliverTrans.AccountId = deliver.Id;
            _logger.LogInformation($"Shipper transaction: {deliverTrans.CoinExchange}, Balance: {deliverTrans.BalanceWallet}");

            Transaction creatorTrans = new Transaction();
            creatorTrans.Description = "Giao thành công đơn hàng id : " + package.Id;
            creatorTrans.Status = TransactionStatus.ACCOMPLISHED;
            creatorTrans.TransactionType = TransactionType.DELIVERED_SUCCESS;
            creatorTrans.CoinExchange = -totalPrice - package.PriceShip;
            creatorTrans.BalanceWallet = creator.Balance - totalPrice - package.PriceShip;
            creatorTrans.PackageId = package.Id;
            creatorTrans.AccountId = creator.Id;
            _logger.LogInformation($"Shop transaction: {creatorTrans.CoinExchange}, Balance: {creatorTrans.BalanceWallet}");

            adminBalance.Balance = ParseHelper.RoundedToInt(adminBalance.Balance - totalPrice + package.PriceShip * profitPercent);
            deliver.Balance = ParseHelper.RoundedToInt(
                deliver.Balance + totalPrice + package.PriceShip * (1 - profitPercent));
            creator.Balance = creator.Balance - totalPrice - package.PriceShip;

            List<Transaction> transactions = new List<Transaction> {
                    systemTrans, deliverTrans, creatorTrans
                };
            await _transactionRepo.InsertAsync(transactions);
            #endregion

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.CREATOR_CONFIRM_DEIVERED;
            history.Description = $"Người tạo xác nhận đã giao hàng thành công vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;

            package.Status = history.ToStatus;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0) response.ToSuccessResponse("Yêu cầu thành công");
            return response;
        }

        public async Task<ApiResponsePaginated<ResponseComboPackageModel>> SuggestCombo(Guid deliverId, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseComboPackageModel> response = new ApiResponsePaginated<ResponseComboPackageModel>();
            #region Verify params
            Account? deliver = await _accountRepo.GetByIdAsync(deliverId
                ,include: (source) => source.Include(acc => acc.InfoUser));
            if (deliver == null)
            {
                response.ToFailedResponse("Người dùng không tồn tại");
                return response;
            }
            if (deliver.InfoUser == null)
            {
                response.ToFailedResponse("Thông tin người dùng chưa được tạo");
                return response;
            }
            Route? route = await routeRepo.FirstOrDefaultAsync(
                    predicate: (rou) => rou.InfoUserId == deliver!.InfoUser!.Id && rou.IsActive == true);
            if (route == null)
            {
                response.ToFailedResponse("Chưa chọn tuyến đường");
                return response;
            }
            if (pageIndex < 0 || pageSize < 1)
            {
                response.ToFailedResponse("Thông tin phân trang không hợp lệ");
                return response;
            }
            #endregion

            GeoCoordinate homeCoordinate = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
            GeoCoordinate destinationCoordinate = new GeoCoordinate(route.ToLatitude, route.ToLongitude);

            PolyLineModel polyLineShipper = await _mapboxService.GetPolyLineModel(homeCoordinate, destinationCoordinate);

            #region Includale package
            Func<IQueryable<Package>, IIncludableQueryable<Package, object>> include = (source) => source.Include(p => p.Products);
            #endregion
            #region Predicate package
            Expression<Func<Package, bool>> predicate = (source) => source.Status == PackageStatus.APPROVED;
            #endregion

            #region Find packages valid spacing
            List<ResponsePackageModel> packagesValid = new List<ResponsePackageModel>();
            List<Package> packages = (await _packageRepo.GetAllAsync(include: include, predicate: predicate)).ToList();
            int packageCount = packages.Count;
            for (int i = 0; i < packageCount; i++)
            {
                bool isValidOrder = MapHelper.ValidDestinationBetweenShipperAndPackage(polyLineShipper, packages[i]);
                _logger.LogInformation($"Package valid destination: {packages[i].Id}");
                if (isValidOrder) packagesValid.Add(packages[i].ToResponseModel());
            }
            #endregion

            #region Group with creator
            List<Guid> creatorIds = new List<Guid>();
            foreach (ResponsePackageModel p in packagesValid)
            {
                if (!creatorIds.Contains(p.CreatorId))
                {
                    creatorIds.Add(p.CreatorId);
                    _logger.LogInformation($"Shop have combos suggest: {p.CreatorId}");
                }
            }
            List<ResponseComboPackageModel> combos = new List<ResponseComboPackageModel>();
            foreach (Guid creatorId in creatorIds)
            {
                ResponseComboPackageModel combo = new ResponseComboPackageModel();
                combo.Creator = (await _accountRepo.GetByIdAsync(creatorId,
                    include: (source) => source.Include(acc => acc.InfoUser)))?.ToResponseModel();
                combo.Packages = packagesValid.Where(p => p.CreatorId == creatorId).ToList();
                int comboPrice = 0;
                foreach (ResponsePackageModel pac in combo.Packages)
                {
                    foreach (ResponseProductModel pr in pac.Products)
                    {
                        comboPrice += pr.Price;
                    }
                }
                combo.ComboPrice = comboPrice;
                _logger.LogInformation($"Combo[Shop: {combo.Creator?.Id},Price: {combo.ComboPrice},Package: {combo.Packages.Count}]");
                combos.Add(combo);
            }
            PaginatedList<ResponseComboPackageModel> responseList = await combos.ToPaginatedListAsync(pageIndex, pageSize);
            response.SetData(responseList);
            response.ToSuccessResponse("Lấy những đề xuất combo");
            #endregion

            return response;
        }
    }
}