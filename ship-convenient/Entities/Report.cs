namespace ship_convenient.Entities
{
    public class Report : BaseEntity
    {
        public string TypeOfReport { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        #region Relationship
        public Guid PackageId { get; set; }
        public Package? Package { get; set; }
        #endregion
    }
}
