namespace Junjuria.AutomapperConfig.AutoMapperConfiguration
{
    using AutoMapper;
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.DataTransferObjects.Orders;
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using System;
    using System.Linq;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

            CreateMapToMappings(allTypes);
            CreateMapFromMappings(allTypes);

            CreateMap<Product, ProductDetailedOutDto>()
             .ForMember(d => d.ProductPictures, opt => opt.MapFrom(s => s.ProductPictures.Select(pctr => pctr.PictureURL).ToArray()));

            CreateMap<Product, PurchaseItemDto>()
                 .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.Id))
                 .ForMember(d => d.Quantity, opt => opt.Ignore());

            CreateMap<Product, ProductMinifiedOutDto>()
                .ForMember(d => d.IsAvailable, opt => opt.MapFrom(s => s.Quantity > 0))
                .ForMember(d => d.ComentsCount, opt => opt.MapFrom(s => s.ProductComments.Count))
                .ForMember(d => d.Grade, opt => opt.MapFrom(s =>
                 s.Votes.Any() ? (Grade)(int)Math.Round((double)s.Votes.Sum(x => (int)x.Grade) / s.Votes.Count()) : Grade.NotRated))
                .ForMember(d => d.OrdersCount, opt => opt.MapFrom(s => s.ProductOrders.Count));

            CreateMap<Order, OrderOutMinifiedDto>()
                 .ForMember(d => d.TotalPrice, opt => opt.MapFrom(s => s.OrderProducts.Select(x => (x.Quantity) * (x.Product.Price)).Sum()))
                 .ForMember(d => d.TotalWeight, opt => opt.MapFrom(s => s.OrderProducts.Select(x => (x.Quantity) * (x.Product.Weight)).Sum()));
        }

        private void CreateMapToMappings(System.Collections.Generic.IEnumerable<Type> allTypes)
        {
            Type[] sourseTypes = allTypes.Where(x => x.GetInterfaces()
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
                    CreateMap(sType, targetType);
                }
            }
        }
        private void CreateMapFromMappings(System.Collections.Generic.IEnumerable<Type> allTypes)
        {
            Type[] destTypes = allTypes.Where(x => x.GetInterfaces()
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
                    CreateMap(sType, dType);
                }
            }
        }
    }
}