using Microsoft.AspNetCore.Identity;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _uow;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IUnitOfWork uow)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _uow = uow;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            throw new Exception("User not found");

        if (!user.IsActive)
            throw new Exception("User disabled");

        var check = await _userManager.CheckPasswordAsync(user, password);

        if (!check)
            throw new Exception("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);

        return _jwtService.GenerateToken(user, roles);
    }

    public async Task<ApplicationUser> RegisterAsync(
        string email,
        string password,
        string fullName,
        string phone,
        string role)
    {
        var allowedRoles = new[]
        {
            "Admin",
            "Pharmacist",
            "Cashier",
            "Customer",
            "PharmacyCompany"
        };

        if (!allowedRoles.Contains(role))
            throw new Exception("Invalid role");

        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            FullName = fullName,
            PhoneNumber = phone,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(",", result.Errors.Select(x => x.Description)));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            throw new Exception(
                string.Join(",", roleResult.Errors.Select(x => x.Description)));
        }

        if (role == "PharmacyCompany")
        {
            var company = new PharmacyCompany
            {
                Name = fullName,
                Email = email,
                Phone = phone,
                Address = string.Empty,
                IsActive = true,
                UserId = user.Id
            };

            await _uow.Repository<PharmacyCompany>().AddAsync(company);
        }

        // Create default user settings
        var settings = new UserSettings
        {
            UserId = user.Id,
            Language = "en",
            Theme = "light",
            ProfileImageUrl = null
        };

        await _uow.UserSettings.AddAsync(settings);

        await _uow.SaveChangesAsync();

        return user;
    }

    public async Task<bool> IsUserInRoleAsync(ApplicationUser user, string role)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains(role);
    }
}