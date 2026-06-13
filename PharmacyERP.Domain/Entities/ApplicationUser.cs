using Microsoft.AspNetCore.Identity;

namespace PharmacyERP.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; }

        public bool IsActive { get; set; } = true;

    }
}