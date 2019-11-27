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
        private readonly IRepository<IdentityUserRole<string>> userRoleMappingService;

        public StatisticService(IRepository<Product> productsRepository, IRepository<Order> ordersRepository, IRepository<Manufacturer> manufacturersRepository, IRepository<IdentityRole> rolesRepository, IRepository<IdentityUserRole<string>> userRoleMappingService)
        {
            this.productsRepository = productsRepository;
            this.ordersRepository = ordersRepository;
            this.manufacturersRepository = manufacturersRepository;
            this.rolesRepository = rolesRepository;
            this.userRoleMappingService = userRoleMappingService;
        }

        public OveralStatistic GetStatistics()
        {
            var result = new OveralStatistic();
            result.TotalProductsCount = productsRepository.All().Count(x => !x.IsDeleted);
            var staffRolesIds = rolesRepository.All().Where(x => x.Name.ToLower() == "admin" || x.Name.ToLower() == "assistance").Select(x=>x.Id).ToArray();
            result.TotalServicePersonal = userRoleMappingService.All().Where(x => staffRolesIds.Contains(x.RoleId)).Count();
            var userRoleId=rolesRepository.All().SingleOrDefault(x => x.Name.ToLower() == "user").Id;
            result.TotalUsersCount = userRoleMappingService.All().Where(x => x.RoleId == userRoleId).Count();
            result.TotalManufacturersCount = manufacturersRepository.All().Count(x => !x.IsDeleted);
            result.TotalOrdersCount = ordersRepository.All().Count(x => x.Status == Status.Finalised && !x.IsDeleted);
            return result;
        }
    }
}