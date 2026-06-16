using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces.Repositories;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Infrastructure.Persistence;

namespace PharmacyERP.Infrastructure.Repositories;

public class UserSettingsRepository
    : GenericRepository<UserSettings>,
      IUserSettingsRepository
{
    private readonly ApplicationDbContext _context;

    public UserSettingsRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<UserSettings?> GetByUserIdAsync(int userId)
    {
        return await _context.UserSettings
            .FirstOrDefaultAsync(x =>
                x.UserId == userId);
    }
}