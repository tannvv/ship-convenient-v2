﻿using Bogus;
using ship_convenient.Constants.AccountConstant;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using unitofwork_core.Constant.ConfigConstant;
using unitofwork_core.Constant.Package;
using Route = ship_convenient.Entities.Route;

namespace ship_convenient.Services.DatabaseService
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ILogger<DatabaseService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _productRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IInfoUserRepository _infoUserRepo;
        private readonly IPackageRepository _packageRepo;
        private readonly IConfigRepository _configRepo;
        public DatabaseService(ILogger<DatabaseService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _productRepo = unitOfWork.Products;
            _packageRepo = unitOfWork.Packages;
            _accountRepo = unitOfWork.Accounts;
            _configRepo = unitOfWork.Configs;
            _infoUserRepo = unitOfWork.InfoUsers;
        }
        public void RemoveData()
        {
            _unitOfWork.Configs.DeleteRange(_configRepo.GetAll());
            _unitOfWork.Products.DeleteRange(_productRepo.GetAll());
            _unitOfWork.Packages.DeleteRange(_packageRepo.GetAll());
            _unitOfWork.InfoUsers.DeleteRange(_infoUserRepo.GetAll());
            _unitOfWork.Accounts.DeleteRange(_accountRepo.GetAll());
            _unitOfWork.Complete();
        }

        public async void GenerateData()
        {
            double minLongitude = 106.60934755879953;
            double maxLongitude = 106.82648934410292;
            double minLatitude = 10.77371671523056;
            double maxLatitude = 10.843294269787952;

            List<string> avatarsLink = new List<string>();
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/4333/4333609.png");
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/2202/2202112.png");
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/4140/4140047.png");
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/236/236832.png");
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/3006/3006876.png");
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/4333/4333609.png");
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/4140/4140048.png");
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/924/924874.png");
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/4202/4202831.png");
            avatarsLink.Add("https://cdn-icons-png.flaticon.com/512/921/921071.png");
            Faker<Account> FakerAccount = new Faker<Account>()
                .RuleFor(u => u.UserName, faker => faker.Person.UserName)
                .RuleFor(u => u.Password, faker => faker.Person.FirstName.ToLower())
                .RuleFor(u => u.Role, faker => RoleName.SENDER)
                .RuleFor(u => u.Status, faker => "ACTIVE");
            Faker<InfoUser> FakerInfoUser = new Faker<InfoUser>()
                 .RuleFor(u => u.Email, faker => faker.Person.Email)
                .RuleFor(u => u.FirstName, faker => faker.Person.FirstName)
                .RuleFor(u => u.LastName, faker => faker.Person.LastName)
                .RuleFor(u => u.PhotoUrl, faker => faker.PickRandom(avatarsLink))
                .RuleFor(u => u.Gender, faker => faker.PickRandom(AccountGender.GetAll()))
                .RuleFor(u => u.Phone, faker => faker.Person.Phone);
            Faker<Route> FakerRoute = new Faker<Route>()
              .RuleFor(r => r.IsActive, faker => true)
              .RuleFor(r => r.FromName, faker => faker.Person.Address.Street)
              .RuleFor(u => u.FromLongitude, faker => faker.Random.Double(min: minLongitude, max: maxLongitude))
              .RuleFor(u => u.FromLatitude, faker => faker.Random.Double(min: minLatitude, max: maxLatitude))
              .RuleFor(r => r.ToName, faker => faker.Person.Address.Street)
              .RuleFor(u => u.ToLongitude, faker => faker.Random.Double(min: minLongitude, max: maxLongitude))
              .RuleFor(u => u.ToLatitude, faker => faker.Random.Double(min: minLatitude, max: maxLatitude));

            List<Account> accounts = FakerAccount.Generate(10);
            for (int i = 0; i < accounts.Count; i++)
            {
                InfoUser infoUser = FakerInfoUser.Generate();
                infoUser.Routes.Add(FakerRoute.Generate());
                accounts[i].InfoUser = infoUser;
            }
            Account admin = new Account
            {
                UserName = "admin",
                Password = "admin",
                Role = RoleName.ADMIN,
                Status = AccountStatus.ACTIVE,
                Balance = 0
            };

            Account adminBalance = new Account
            {
                UserName = "admin_balance",
                Password = "admin_balance",
                Role = RoleName.ADMIN_BALANCE,
                Status = AccountStatus.ACTIVE,
                Balance = 0
            };

            await _accountRepo.InsertAsync(admin);
            await _accountRepo.InsertAsync(adminBalance);
            await _accountRepo.InsertAsync(accounts);



            ConfigApp configProfit = new ConfigApp
            {
                Name = ConfigConstant.PROFIT_PERCENTAGE,
                Note = "20",
                ModifiedBy = admin.Id
            };
            ConfigApp configProfitRefund = new ConfigApp
            {
                Name = ConfigConstant.PROFIT_PERCENTAGE_REFUND,
                Note = "50",
                ModifiedBy = admin.Id
            };
            ConfigApp configMinimumDistance = new ConfigApp
            {
                Name = ConfigConstant.MINIMUM_DISTANCE,
                Note = "1000",
                ModifiedBy = admin.Id
            };
            ConfigApp configBalanceDefault = new ConfigApp
            {
                Name = ConfigConstant.DEFAULT_BALANCE_NEW_ACCOUNT,
                Note = "100000",
                ModifiedBy = admin.Id
            };
            ConfigApp configMaxSuggestCombo = new ConfigApp
            {
                Name = ConfigConstant.MAX_SUGGEST_COMBO,
                Note = "4",
                ModifiedBy = admin.Id
            };
            ConfigApp configMaxPickupSameTime = new ConfigApp
            {
                Name = ConfigConstant.MAX_PICKUP_SAME_TIME,
                Note = "50",
                ModifiedBy = admin.Id
            };
            ConfigApp configMaxCreateRoute = new ConfigApp
            {
                Name = ConfigConstant.MAX_ROUTE_CREATE,
                Note = "3",
                ModifiedBy = admin.Id
            };
            ConfigApp configMaxCancelInDay = new ConfigApp
            {
                Name = ConfigConstant.MAX_CANCEL_IN_DAY,
                Note = "2",
                ModifiedBy = admin.Id
            };
            List<ConfigApp> configApps = new List<ConfigApp> {
                configProfit, configProfitRefund, configMinimumDistance, configMaxPickupSameTime, configMaxCreateRoute, configBalanceDefault, configMaxSuggestCombo, configMaxCancelInDay
            };
            await _configRepo.InsertAsync(configApps);

            /*Faker<Package> FakerPackage = new Faker<Package>()
                .RuleFor(o => o.StartAddress, faker => faker.Address.FullAddress())
                .RuleFor(o => o.StartLongitude, faker => faker.Random.Double(min: minLongitude, max: maxLongitude))
                .RuleFor(o => o.StartLatitude, faker => faker.Random.Double(min: minLatitude, max: maxLatitude))
                .RuleFor(o => o.DestinationAddress, faker => faker.Address.FullAddress())
                .RuleFor(o => o.DestinationLongitude, faker => faker.Random.Double(min: minLongitude, max: maxLongitude))
                .RuleFor(o => o.DestinationLatitude, faker => faker.Random.Double(min: minLatitude, max: maxLatitude))
                .RuleFor(o => o.Distance, faker => faker.Random.Double(min: 2.5, max: 20))
                .RuleFor(o => o.PickupName, faker => faker.Person.FullName)
                .RuleFor(o => o.PickupPhone, faker => faker.Person.Phone)
                .RuleFor(o => o.ReceiverName, faker => faker.Person.FullName)
                .RuleFor(o => o.ReceiverPhone, faker => faker.Person.Phone)
                .RuleFor(o => o.Height, faker => faker.Random.Double(min: 0.2, max: 0.8))
                .RuleFor(o => o.Width, faker => faker.Random.Double(min: 0.2, max: 0.8))
                .RuleFor(o => o.Length, faker => faker.Random.Double(min: 0.2, max: 0.8))
                .RuleFor(o => o.Weight, faker => faker.Random.Int(5, 20))
                .RuleFor(o => o.PhotoUrl, faker => faker.Image.ToString())
                .RuleFor(o => o.Note, faker => faker.Lorem.Sentence(6))
                .RuleFor(o => o.PriceShip, faker => faker.Random.Int(min: 10, max: 40) * 1000)
                .RuleFor(o => o.Status, faker => PackageStatus.APPROVED)
                .RuleFor(o => o.Sender, faker => faker.PickRandom(accounts));
            List<Package> packages2 = new();
            List<Package> packages = FakerPackage.Generate(300);
            for (int i = 0; i < packages.Count; i++)
            {
                Faker faker = new Faker();
                Package packageNew = new Package();
                packageNew.StartAddress = packages[i].StartAddress;
                packageNew.StartLongitude = packages[i].StartLongitude;
                packageNew.StartLatitude = packages[i].StartLatitude;
                packageNew.DestinationAddress = faker.Person.Address.Street;
                packageNew.DestinationLatitude = faker.Random.Double(min: minLatitude, max: maxLatitude);
                packageNew.DestinationLongitude = faker.Random.Double(min: minLongitude, max: maxLongitude);
                packageNew.Distance = faker.Random.Double(min: 2.5, max: 20);
                packageNew.ReceiverName = faker.Person.FullName;
                packageNew.ReceiverPhone = faker.Person.Phone;
                packageNew.Width = faker.Random.Double(min: 0.2, max: 0.8);
                packageNew.Length = faker.Random.Double(min: 0.2, max: 0.8);
                packageNew.Height = faker.Random.Double(min: 0.2, max: 0.8);
                packageNew.Weight = faker.Random.Int(5, 20);
                packageNew.PhotoUrl = packages[i].PhotoUrl;
                packageNew.Note = faker.Lorem.Sentence(6);
                packageNew.PriceShip = faker.Random.Int(min: 10, max: 40) * 1000;
                packageNew.Status = PackageStatus.APPROVED;
                packageNew.Sender = packages[i].Sender;

                Faker<Product> FakerProduct = new Faker<Product>()
                       .RuleFor(p => p.Name, faker => string.Join(" ", faker.Lorem.Words()))
                       .RuleFor(p => p.Price, faker => faker.Random.Int(20, 150) * 1000)
                       .RuleFor(p => p.Description, faker => faker.Lorem.Sentence(1));
                List<Product> products = FakerProduct.Generate(2);
                List<Product> products2 = FakerProduct.Generate(2);
                packageNew.Products = products2;
                packages2.Add(packageNew);
                packages[i].Products = products;
            }

            _logger.LogInformation("Insert orders");
            await _packageRepo.InsertAsync(packages);
            await _packageRepo.InsertAsync(packages2);*/

            _unitOfWork.Complete();
        }
    }
}
