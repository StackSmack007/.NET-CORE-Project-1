namespace Junjuria.Services.Services
{
    using AutoMapper;
    using Junjuria.Common;
    using Junjuria.Common.Extensions;
    using Junjuria.DataTransferObjects.Orders;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

        public void AddProductToBasket(ICollection<PurchaseItemDto> basket, int productId, uint ammount)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.ProductId == productId);
            if (purchase is null)
            {
                var productFound = productsRepository.All().FirstOrDefault(x => x.Id == productId);
                if (productFound is null || productFound.Quantity == 0) return;
                purchase = mapper.Map<PurchaseItemDto>(productFound);
                //TO DO add handling of requesting details of non existing product!
                basket.Add(purchase);
            }
            uint maxAmmountPossible = GetProductStockQuantity(productId);
            if (purchase.Quantity + ammount > maxAmmountPossible)
            {
                purchase.Quantity = maxAmmountPossible;
                return;
            }
            purchase.Quantity += ammount;
        }

        public ICollection<PurchaseItemDetailedDto> GetDetailedPurchaseInfo(ICollection<PurchaseItemDto> basket)
        {
            var stockAmmounts = GetProductsStockQuantities(basket.Select(x => x.ProductId).ToArray());
            //This will refresh available quantities in the session if need be!
            //  FixQuantitiesInBasket(basket.ToList());

            var productsInfo = productsRepository.All().Where(x => stockAmmounts.Any(st => st.Id == x.Id)).Select(x => new
            {
                x.Id,
                x.Weight,
                x.MainPicURL,
                StockAmmount = x.Quantity
            }).ToArray();
            var result = new HashSet<PurchaseItemDetailedDto>();
            foreach (var id in stockAmmounts.Select(x => x.Id))
            {
                var newOrderItem = mapper.Map<PurchaseItemDetailedDto>(basket.FirstOrDefault(x => x.ProductId == id));
                var productInfo = productsInfo.FirstOrDefault(x => x.Id == id);
                newOrderItem.MainPicURL = productInfo.MainPicURL;
                newOrderItem.StockAmmount = productInfo.StockAmmount;
                newOrderItem.Weight = productInfo.Weight;
                result.Add(newOrderItem);
            }
            return result;
        }

        public void SubtractProductFromBasket(List<PurchaseItemDto> basket, int productId, uint ammount)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.ProductId == productId);
            if (purchase.Quantity <= ammount)
            {
                basket.Remove(purchase);
                return;
            }
            purchase.Quantity -= ammount;
        }

        public void ModifyCountOfProductInBasket(List<PurchaseItemDto> basket, int productId, uint newAmmount)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.ProductId == productId);
            if (newAmmount == 0)
            {
                basket.Remove(purchase);
                return;
            }
            uint stockAmmount = GetProductStockQuantity(productId);
            if (newAmmount > stockAmmount)
            {
                purchase.Quantity = stockAmmount;
                return;
            }
            purchase.Quantity = newAmmount;
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

        public async Task<bool> TryCreateOrder(List<PurchaseItemDto> basket, string userId)
        {
            var itemsOutOfStock = FixQuantitiesInBasket(basket);
            if (itemsOutOfStock) return false;
            var order = new Order
            {
                CustomerId = userId,
                Status = Status.AwaitingConfirmation
            };
            var detailedPurchaseInfo = GetDetailedPurchaseInfo(basket);
            order.DeliveryFee = GlobalConstants.DeliveryFee(detailedPurchaseInfo.Select(x => x.Quantity * x.Weight).Sum());
            foreach (var purchase in basket)
            {
                order.OrderProducts.Add(new ProductOrder
                {
                    ProductId = purchase.ProductId,
                    Quantity = (int)purchase.Quantity,
                    CurrentPrice = purchase.DiscountedPrice
                });
            }
           await orderRepository.AddAssync(order);
            await orderRepository.SaveChangesAsync();
            return true;
        }
        private ICollection<ProductQuantity> GetProductsStockQuantities(IEnumerable<int> ids) => productsRepository.All().To<ProductQuantity>().Where(x => ids.Contains(x.Id)).ToArray();

        private uint GetProductStockQuantity(int productId)
        {
            var product = productsRepository.All().FirstOrDefault(x => x.Id == productId);
            if (product is null || product.Quantity == 0) return 0;
            return product.Quantity;
        }


        private bool FixQuantitiesInBasket(List<PurchaseItemDto> basket)
        {
            var productQuantities = GetProductsStockQuantities(basket.Select(x => x.ProductId));
            bool problemFound = false;
            foreach (var item in productQuantities)
            {
                var purchaseItem = basket.FirstOrDefault(x => x.ProductId == item.Id);
                if (purchaseItem.Quantity > item.Quantity)
                {
                    problemFound = true;
                    if (item.Quantity == 0)
                    {
                        basket.Remove(purchaseItem);
                        continue;
                    }
                    purchaseItem.Quantity = item.Quantity;
                }
            }
            return problemFound;
        }
    }
}