namespace DummyWebApp.DAL
{
    using System.Threading.Tasks;
    using Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;

    public class DbSeeder
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly UserManager<User> _userManager;

        public DbSeeder(ApplicationDbContext applicationDbContext, IHostEnvironment hostEnvironment, UserManager<User> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            await _applicationDbContext.Database.MigrateAsync();

            if (_hostEnvironment.IsDevelopment() && !await _applicationDbContext.Users.AnyAsync())
            {
                await _userManager.CreateAsync(
                    new User
                {
                    Email = "admin@gmail.com",
                    UserName = "admin"
                }, "SuperAdmin");
            }
        }
    }
}