
namespace unitofwork_core.Model.PackageModel
{
    public class DeliverConfirmPackagesModel
    {
        public List<Guid> packageIds { get; set; } = new();
        public Guid deliverId{ get; set; }
    }
}
