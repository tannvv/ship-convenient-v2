using ship_convenient.Constants.AccountConstant;
using ship_convenient.Entities;
using System.ComponentModel.DataAnnotations;

namespace ship_convenient.Model.UserModel
{
    public class CreateAccountModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public Account ToEntityModel() { 
            Account account = new();
            account.UserName = this.UserName;
            account.Password = this.Password;
            account.Status = AccountStatus.ACTIVE;
            account.Role = this.Role;
            if (IsCreateInfo()) {
                InfoUser info = new();
                info.FirstName = this.FirstName;
                info.LastName = this.LastName;
                info.Email = this.Email;
                info.Phone = this.Phone;
                info.PhotoUrl = this.PhotoUrl;
                info.Gender = this.Gender;
                account.InfoUser = info;
            }
            return account;
        }

        public bool IsCreateInfo() {
            if (!string.IsNullOrEmpty(this.FirstName) || !string.IsNullOrEmpty(this.PhotoUrl)
                || !string.IsNullOrEmpty(this.Gender) || !string.IsNullOrEmpty(this.LastName)
                || !string.IsNullOrEmpty(this.Email) || !string.IsNullOrEmpty(this.Phone))
            {
                return true;
            }
            return false;
        }
    }
}
