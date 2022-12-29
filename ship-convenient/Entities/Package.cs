using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Entities
{
    public class Package : BaseEntity
    {
        public string StartAddress { get; set; } = string.Empty;
        public double StartLongitude { get; set; }
        public double StartLatitude { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public double DestinationLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double Distance { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public double Volume { get; set; }
        public double Weight { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PriceShip { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        #region Relationship
        public Guid SenderId { get; set; }
        public Account? Sender { get; set; }
        public Guid? DeliverId { get; set; }
        public Account? Deliver { get; set; }
        public Guid? DiscountId { get; set; }
        public Discount? Discount { get; set; }

        public List<Transaction> Transactions { get; set; }
        public List<TransactionPackage> TransactionPackages { get; set; }
        public List<Product> Products { get; set; }
        #endregion

        public Package()
        {
            Transactions = new List<Transaction>();
            TransactionPackages = new List<TransactionPackage>();
            Products = new List<Product>();
        }

        public ResponsePackageModel ToResponseModel()
        {
            ResponsePackageModel model = new ResponsePackageModel();
            model.Id = this.Id;
            model.StartAddress = this.StartAddress;
            model.StartLongitude = this.StartLongitude;
            model.StartLatitude = this.StartLatitude;
            model.DestinationAddress = this.DestinationAddress;
            model.DestinationLongitude = this.DestinationLongitude;
            model.DestinationLatitude = this.DestinationLatitude;
            model.ReceiverName = this.ReceiverName;
            model.ReceiverPhone = this.ReceiverPhone;
            model.Distance = this.Distance;
            model.Volume = this.Volume;
            model.Weight = this.Weight;
            model.Status = this.Status;
            model.PriceShip = this.PriceShip;
            model.PhotoUrl = this.PhotoUrl;
            model.Note = this.Note;
            model.CreatedAt = this.CreatedAt;
            model.ModifiedAt = this.ModifiedAt;
            model.SenderId = this.SenderId;
            model.DeliverId = this.DeliverId;
            model.Sender = this.Sender != null ? this.Sender.ToResponseModel() : null;
            model.Deliver = this.Deliver != null ? this.Deliver.ToResponseModel() : null;

            int countProduct = this.Products.Count;
            for (int i = 0; i < countProduct; i++)
            {
                model.Products.Add(this.Products[i].ToResponseModel());
            }
            return model;
        }
    }
}
