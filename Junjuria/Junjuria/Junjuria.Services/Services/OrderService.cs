namespace Junjuria.Services.Services
{
    using Abp.Net.Mail;
    using AutoMapper;
    using Junjuria.Common;
    using Junjuria.Common.Extensions;
    using Junjuria.DataTransferObjects.Email;
    using Junjuria.DataTransferObjects.Orders;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class OrderService : IOrderService
    {
        private readonly IRepository<Product> productsRepository;
        private readonly IMapper mapper;

        private readonly IRepository<ProductOrder> productOrderRepository;
        private readonly IRepository<Order> orderRepository;
        private readonly IEmailSender emailSender;
        private readonly IViewRenderService viewRenderService;
        private IRepository<AppUser> appUsersRepository;

        public OrderService(IRepository<Product> productsRepository,
            IRepository<AppUser> appUsersRepository,
            IMapper mapper,
            IRepository<ProductOrder> productOrderRepository,
            IRepository<Order> orderRepository,
            IEmailSender emailSender,
            IViewRenderService viewRenderService
            )
        {
            this.productsRepository = productsRepository;
            this.appUsersRepository = appUsersRepository;
            this.mapper = mapper;
            this.productOrderRepository = productOrderRepository;
            this.orderRepository = orderRepository;
            this.emailSender = emailSender;
            this.viewRenderService = viewRenderService;
        }

        public void AddProductToBasket(ICollection<PurchaseItemDto> basket, int productId, uint ammount)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.Id == productId);
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
            var stockAmmounts = GetProductsStockQuantities(basket.Select(x => x.Id).ToArray());

            var productsInfo = productsRepository.All().Where(x => stockAmmounts.Select(st => st.Id).Contains(x.Id)).Select(x => new
            {
                x.Id,
                x.Weight,
                x.MainPicURL,
                StockAmmount = x.Quantity
            }).ToArray();
            var result = new HashSet<PurchaseItemDetailedDto>();
            foreach (var id in stockAmmounts.Select(x => x.Id))
            {
                var newOrderItem = mapper.Map<PurchaseItemDetailedDto>(basket.FirstOrDefault(x => x.Id == id));
                var productInfo = productsInfo.FirstOrDefault(x => x.Id == id);
                newOrderItem.MainPicURL = productInfo.MainPicURL;
                newOrderItem.StockAmmount = productInfo.StockAmmount;
                newOrderItem.Weight = productInfo.Weight;
                result.Add(newOrderItem);
            }
            return result;
        }
        private ICollection<ProductQuantityDto> GetProductsStockQuantities(IEnumerable<int> ids) => productsRepository.All().To<ProductQuantityDto>().Where(x => ids.Contains(x.Id)).ToArray();

        public void SubtractProductFromBasket(List<PurchaseItemDto> basket, int productId, uint ammount)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.Id == productId);
            if (purchase.Quantity <= ammount)
            {
                basket.Remove(purchase);
                return;
            }
            purchase.Quantity -= ammount;
        }

        public void ModifyCountOfProductInBasket(List<PurchaseItemDto> basket, int productId, uint newAmmount)
        {
            PurchaseItemDto purchase = basket.FirstOrDefault(p => p.Id == productId);
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

        public bool TryCreateOrder(List<PurchaseItemDto> basket, string userId)
        {
            lock (ConcurencyMaster.LockProductsObj)
            {
                var itemsOutOfStockFound = FixQuantitiesInBasket(basket);
                if (itemsOutOfStockFound) return false;
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
                        ProductId = purchase.Id,
                        Quantity = (int)purchase.Quantity,
                        CurrentPrice = purchase.DiscountedPrice
                    });
                }
                SuccessfullOrderInfoOut orderInfo = new SuccessfullOrderInfoOut
                {
                    OrderDateTime = order.DateOfCreation,
                    Value = order.OrderProducts.Select(x => (x.Quantity) * (x.CurrentPrice)).Sum(),
                };
                orderRepository.AddAssync(order).GetAwaiter().GetResult();
                RemoveProuctsFromStore(basket);
                orderRepository.SaveChangesAsync().GetAwaiter().GetResult();
                orderInfo.OrderId = order.Id;
                SendEmail(userId, "New order created", "Emails/OrderEmail", orderInfo);
                return true;
            }
        }

        private void SendEmail(string receiverId, string subject, string viewPath, object model = null)
        {
            string userEmail = appUsersRepository.All().FirstOrDefault(x => x.Id == receiverId).Email;
            var contentHtml = viewRenderService.RenderToStringAsync(viewPath, model).GetAwaiter().GetResult();
            var mail = new MailMessage();
            mail.From = new MailAddress(GlobalConstants.JunjuriaEmail, GlobalConstants.JunjuriaEmailSenderName);
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            mail.Body = contentHtml;
            mail.To.Add(userEmail);
            emailSender.Send(mail);
        }

        private void RemoveProuctsFromStore(List<PurchaseItemDto> basket)
        {
            var products = productsRepository.All().Where(x => basket.Select(b => b.Id).Contains(x.Id)).ToArray();
            foreach (var purchase in basket.Select(x => new { x.Id, x.Quantity }))
            {
                var product = products.FirstOrDefault(x => x.Id == purchase.Id);
                if (product != null)
                {
                    product.Quantity -= purchase.Quantity;
                }
            }
        }

        private uint GetProductStockQuantity(int productId)
        {
            var product = productsRepository.All().FirstOrDefault(x => x.Id == productId);
            if (product is null || product.Quantity == 0) return 0;
            return product.Quantity;
        }

        private bool FixQuantitiesInBasket(List<PurchaseItemDto> basket)
        {
            var productQuantities = GetProductsStockQuantities(basket.Select(x => x.Id));
            bool problemFound = false;
            foreach (var item in productQuantities)
            {
                var purchaseItem = basket.FirstOrDefault(x => x.Id == item.Id);
                if (purchaseItem.Quantity > item.Quantity || item.IsDeleted)
                {
                    problemFound = true;
                    if (item.Quantity == 0 || item.IsDeleted)
                    {
                        basket.Remove(purchaseItem);
                        continue;
                    }
                    purchaseItem.Quantity = item.Quantity;
                }
            }
            return problemFound;
        }

        public async Task<OrderDetailsOutDto> GetOrderDetailsAsync(string id)
        {
            var order = await orderRepository.All()
                                             .Include(x => x.OrderProducts)
                                             .ThenInclude(x => x.Product)
                                             .FirstOrDefaultAsync(x => x.Id == id);
            if (order is null) return null;
            OrderDetailsOutDto result = mapper.Map<OrderDetailsOutDto>(order);
            return result;
        }
        public IQueryable<OrderForManaging> GetAllForManaging() => orderRepository.All().Where(x => x.Status != Status.Canceled).OrderBy(x => x.Status).ThenBy(x => x.DateOfCreation).To<OrderForManaging>();

        public async Task SetStatus(string orderId, Status status)
        {
            Order order = await orderRepository.All()
                .Include(x=>x.OrderProducts)
                .ThenInclude(x=>x.Product)
                .FirstOrDefaultAsync(x => x.Id == orderId);
            if (order is null || order.Status == status) return;
            var orderInfo = new OrderStatusChangeOut
            {
                OrderId = orderId,
                Value=order.TotalPrice,
                OrderDateTime = order.DateOfCreation,
                PreviousStatus = order.Status,
                CurrentStatus = status,
            };
            SendEmail(order.CustomerId, "Status of order Changed", "Emails/OrderStatusChange", orderInfo);
            order.Status = status;
            if (status == Status.Canceled)
            {
                lock (ConcurencyMaster.LockProductsObj)
                {
                    ProductOrder[] productOrders = productOrderRepository.All().Where(x => x.OrderId == orderId).Include(x => x.Product).ToArray();
                    foreach (var productOrder in productOrders)
                    {
                        productOrder.Product.Quantity += (uint)productOrder.Quantity;
                    }
                    productOrderRepository.SaveChangesAsync().GetAwaiter().GetResult();
                }
            }
            await orderRepository.SaveChangesAsync();
        }
    }
}