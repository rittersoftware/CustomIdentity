namespace identity.Data.Identity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    [Table("Users", Schema = "Security")]
    public class ApplicationUser : IdentityUser
    {
        public string Description { get; set; }
        public string FullName { get; set; }
        public string PreferredName { get; set; }
        public bool IsPermittedToLogon { get; set; }
        public string LogonName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsExternalLogonProvider { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsSalesPerson { get; set; }
        public string UserPreferences { get; set; }
        public int Age { get; set; }
        public byte[] Photo { get; set; }
        public string FaxNumber { get; set; }
        public string CustomFields { get; set; }
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public ApplicationUser()
        {
            this.Claims = new HashSet<ApplicationUserClaim>();
            this.Logins = new HashSet<ApplicationUserLogin>();
            this.Tokens = new HashSet<ApplicationUserToken>();
            this.UserRoles = new HashSet<ApplicationUserRole>();
        }
    }
}