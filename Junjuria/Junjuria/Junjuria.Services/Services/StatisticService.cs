namespace Junjuria.Services.Services
{
    using Junjuria.DataTransferObjects.Statistical;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Identity;
    using System.Linq;
    public class StatisticService : IStatisticService
    {
        private readonly IRepository<Product> productsRepository;
        private readonly IRepository<Order> ordersRepository;
        private readonly IRepository<Manufacturer> manufacturersRepository;
        private readonly IRepository<IdentityRole> rolesRepository;

        public StatisticService(IRepository<Product> productsRepository, IRepository<Order> ordersRepository, IRepository<Manufacturer> manufacturersRepository, IRepository<IdentityRole> rolesRepository)
        {
            this.productsRepository = productsRepository;
            this.ordersRepository = ordersRepository;
            this.manufacturersRepository = manufacturersRepository;
            this.rolesRepository = rolesRepository;
        }

        public OveralStatistic GetStatistics()
        {
            var result = new OveralStatistic();
            result.TotalProductsCount = productsRepository.All().Count(x => !x.IsDeleted);
            result.TotalServicePersonal = rolesRepository.All().Where(x => x.Name.ToLower() == "admin" || x.Name.ToLower() == "assistance").Count();
            result.TotalUsersCount = rolesRepository.All().Where(x => x.Name.ToLower() == "user").Count();
            result.TotalManufacturersCount = manufacturersRepository.All().Count(x => !x.IsDeleted);
            result.TotalOrdersCount = ordersRepository.All().Count(x => x.Status == Status.Finalised && !x.IsDeleted);
            return result;
        }
    }
}