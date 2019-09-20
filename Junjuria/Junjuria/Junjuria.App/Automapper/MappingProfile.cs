namespace Junjuria.App.Automapper
{
    using AutoMapper;
    using Junjuria.App.ViewModels.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using System;
    using System.Linq;
    using System.Reflection;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMapToMappings();
            CreateMapFromMappings();

            CreateMap<Product, ProductMinorOutDto>()
                .ForMember(d => d.IsAvailable, opt => opt.MapFrom(s => s.Quantity > 0))
                .ForMember(d => d.ComentsCount, opt => opt.MapFrom(s => s.ProductComments.Count))
                .ForMember(d => d.Grade, opt => opt.MapFrom(s =>
                 s.Votes.Any() ? (Grade)((int)Math.Round((double)s.Votes.Sum(x => (int)x.Grade) / s.Votes.Count())) : Grade.NotRated))
                .ForMember(d => d.OrdersCount, opt => opt.MapFrom(s => s.ProductOrders.Count));
        }

        private void CreateMapToMappings()
        {
            Type[] sourseTypes = Assembly.GetCallingAssembly()
                                         .GetTypes()
                                         .Where(x => x.GetInterfaces()
                                         .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapTo<>)))
                                         .ToArray();
            foreach (Type sType in sourseTypes)
            {
                Type[] targetTypes = sType.GetInterfaces()
                                          .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapTo<>))
                                          .Select(x => x.GetGenericArguments().First())
                                          .ToArray();

                foreach (Type targetType in targetTypes)
                {
                    this.CreateMap(sType, targetType);
                }
            }
        }

        private void CreateMapFromMappings()
        {
            Type[] destTypes = Assembly.GetCallingAssembly()
                                       .GetTypes()
                                       .Where(x => x.GetInterfaces()
                                       .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                                       .ToArray();

            foreach (Type dType in destTypes)
            {
                Type[] sourceTypes = dType.GetInterfaces()
                                          .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                                          .Select(x => x.GetGenericArguments().First())
                                          .ToArray();

                foreach (Type sType in sourceTypes)
                {
                    this.CreateMap(sType, dType);
                }
            }
        }
    }
}