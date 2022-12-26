namespace ship_convenient.Entities
{
    public class TransactionPackage : BaseEntity
    {
        public string FromStatus { get; set; } = string.Empty;
        public string ToStatus { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        #region Relationship
        public Guid PackageId { get; set; }
        public Package? Package { get; set; }
        #endregion

        /*public ResponseHistoryPackageModel ToResponseModel()
        {
            ResponseHistoryPackageModel model = new ResponseHistoryPackageModel();
            model.FromStatus = this.FromStatus;
            model.ToStatus = this.ToStatus;
            model.Description = this.Description;
            model.PackageId = this.PackageId;
            model.CreatedAt = this.CreatedAt;
            return model;
        }*/
    }
}
