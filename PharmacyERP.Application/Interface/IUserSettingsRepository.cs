using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Application.Common.Interfaces.Repositories;

public interface IUserSettingsRepository : IGenericRepository<UserSettings>
{
    Task<UserSettings?> GetByUserIdAsync(int userId);
}