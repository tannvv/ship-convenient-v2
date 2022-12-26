using ship_convenient.Model.UserModel;

namespace ship_convenient.Entities
{
    public class InfoUser : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        #region
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }

        public List<Route> Routes { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        #endregion
        public InfoUser()
        {
            Routes = new List<Route>();
            Vehicles = new List<Vehicle>();
        }

        public ResponseInfoUserModel ToResponseModel() {
            ResponseInfoUserModel model = new();
            model.Id = Id;
            model.FirstName = this.FirstName;
            model.LastName = this.LastName;
            model.Email = this.Email;
            model.Phone = this.Phone;
            if (Routes.Count > 0) {
                Routes.ForEach(route => model.Routes.Add(route.ToResponseModel()));
            }
            return model;
        }
    }
}
