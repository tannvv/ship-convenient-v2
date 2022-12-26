namespace ship_convenient.Entities
{
    public class ConfigApp: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
