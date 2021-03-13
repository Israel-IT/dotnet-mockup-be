namespace DummyWebApp.DAL
{
    using System.Threading.Tasks;

    public class DbSeeder
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DbSeeder(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public Task SeedAsync()
            => _applicationDbContext.Database.EnsureCreatedAsync();
    }
}