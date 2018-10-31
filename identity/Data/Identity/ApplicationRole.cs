namespace identity.Data.Identity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    [Table("Roles", Schema = "Security")]
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdministrator { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
        public ApplicationRole()
        {
            Name = string.Empty;
            IsActive = false;

            this.RoleClaims = new HashSet<ApplicationRoleClaim>();
            this.UserRoles = new HashSet<ApplicationUserRole>();
        }

        public ApplicationRole(bool active, string name)
        {
            Name = name;
            IsActive = active;

            this.RoleClaims = new HashSet<ApplicationRoleClaim>();
            this.UserRoles = new HashSet<ApplicationUserRole>();
        }
    }
}