using PharmacyERP.Domain.Entities;
namespace PharmacyERP.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}