using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Application.Common.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(string email, string password);

    Task<ApplicationUser> RegisterAsync(
        string email,
        string password,
        string fullName,
        string phone, 
        string role);

    Task<bool> IsUserInRoleAsync(ApplicationUser user, string role);
}