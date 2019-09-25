namespace Junjuria.Services.Services
{

    using AutoMapper;
    using Junjuria.Common.Extensions;
    using Junjuria.DataTransferObjects.Orders;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using System.Collections.Generic;
    using System.Linq;
    public class OrderService : IOrderService
    {
        private readonly IRepository<Product> productsRepository;
        private readonly IMapper mapper;

        private readonly IRepository<ProductOrder> productOrderRepository;
        private readonly IRepository<Order> orderRepository;

        public OrderService(IRepository<Product> productsRepository, IMapper mapper, IRepository<ProductOrder> productOrderRepository, IRepository<Order> orderRepository)
        {
            this.productsRepository = productsRepository;
            this.mapper = mapper;
            this.productOrderRepository = productOrderRepository;
            this.orderRepository = orderRepository;
        }

        public void Add(ICollection<PurchaseItemDto> basket, int productId, int ammount)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.ProductId == productId);
            if (purchase is null)
            {
                var productFound = productsRepository.All().FirstOrDefault(x => x.Id == productId);
                if (productFound is null) return;
                purchase = mapper.Map<PurchaseItemDto>(productFound);
                //TO DO add handling of requesting details of non existing product!
                purchase.Quantity = ammount;
                basket.Add(purchase);
                return;
            }
            purchase.Quantity += ammount;
        }

        public void Subtract(List<PurchaseItemDto> basket, int productId, int ammount)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.ProductId == productId);
            if (purchase.Quantity <= ammount)
            {
                basket.Remove(purchase);
                return;
            }
            purchase.Quantity -= ammount;
        }

        public ICollection<ProductWarranty> GetMyWarranties(string userId)
        {
            ProductWarranty[] result = productOrderRepository.All().Select(x => new
            {
                Id = x.ProductId,
                x.Product.Name,
                OwnerId = x.Order.CustomerId,
                x.Quantity,
                PurchasedOnPrice = x.CurrentPrice,
                OrderStatus = x.Order.Status,
                PurchaseDate = x.Order.DateOfCreation,
                ExpireDate = x.Product.MonthsWarranty == null ? x.Order.DateOfCreation : x.Order.DateOfCreation.AddMonths((int)x.Product.MonthsWarranty.Value)
            }).Where(x => x.OwnerId == userId && x.OrderStatus == Status.Finalised).Select(x => new ProductWarranty
            {
                Id = x.Id,
                Name = x.Name,
                Quantity = x.Quantity,
                DiscountedPrice = x.PurchasedOnPrice,
                PurchaseDate = x.PurchaseDate,
                ExpireDate = x.ExpireDate
            }).ToArray();

            return result;
        }

        public ICollection<OrderOutMinifiedDto> GetMyOrders(string userId)
        {
            var orderDtos = orderRepository.All().Where(x => x.CustomerId == userId).To<OrderOutMinifiedDto>().ToArray();
            return orderDtos;






        }
    }
}