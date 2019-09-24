namespace Junjuria.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Junjuria.DataTransferObjects.Orders;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    public class OrderService : IOrderService
    {
        private readonly IRepository<Product> productsRepository;
        private readonly IMapper mapper;

        public OrderService(IRepository<Product> productsRepository,IMapper mapper)
        {
            this.productsRepository = productsRepository;
            this.mapper = mapper;
        }

        public void Add(ICollection<PurchaseItemDto> basket, int productId, int quantity)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.ProductId == productId);
            if (purchase is null)
            {
                var productFound = productsRepository.All().FirstOrDefault(x=>x.Id==productId);
                if (productFound is null) return;
                purchase = mapper.Map<PurchaseItemDto>(productFound);
                //TO DO add handling of requesting details of non existing product!
                purchase.Quantity = quantity;
                basket.Add(purchase);
                return;
            }
            purchase.Quantity += quantity;
        }





    }
}