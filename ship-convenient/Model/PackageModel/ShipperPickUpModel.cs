
namespace unitofwork_core.Model.PackageModel
{
    public class ShipperPickUpModel { 
        public Guid deliverId { get; set; }
        public List<Guid> packageIds { get; set; } = new List<Guid>();
    }
}
