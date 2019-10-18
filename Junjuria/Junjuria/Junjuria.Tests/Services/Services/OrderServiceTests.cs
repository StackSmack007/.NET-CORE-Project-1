using Junjuria.Common;
using Junjuria.DataTransferObjects.Orders;
using Junjuria.Infrastructure.Models;
using Junjuria.Services.Services.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Junjuria.Services.Services
{
    public class OrderServiceTests
    {
        private readonly IOrderService orderService;
        private readonly IRepository<Order> orderssRepository;
        private readonly IRepository<Product> productsRepository;
        private ICollection<PurchaseItemDto> basket;
        public OrderServiceTests()
        {
            orderService = DIContainer.GetService<IOrderService>();
            orderssRepository = DIContainer.GetService<IRepository<Order>>();
            productsRepository = DIContainer.GetService<IRepository<Product>>();
            SeedData();
            basket = new List<PurchaseItemDto>();
        }

        [Fact]
        public void AddProductToBasket_AddsProduct_When_Correct_Id_AndAmmountGiven()
        {
            int id = 1;
            uint ammount = 3;
            var expectedBasketCount = basket.Count() + 1;
            orderService.AddProductToBasket(basket, id, ammount);
            var actualBasketCount = basket.Count();
            Assert.Equal(expectedBasketCount, actualBasketCount);
            Assert.Equal("Product 1", basket.Last().Name);
            Assert.Equal(ammount, basket.Last().Quantity);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(100)]
        public void AddProductToBasket_AddsProductTo_MaxStockQuantityLimit_When_Correct_Id_And_EvenOrGreater_Ammount_Provided(uint ammount)
        {
            uint expectedAddedAmmount = 10;
            int id = 1;
            var expectedBasketCount = basket.Count() + 1;
            orderService.AddProductToBasket(basket, id, ammount);
            var actualBasketCount = basket.Count();
            Assert.Equal(expectedBasketCount, actualBasketCount);
            Assert.Equal("Product 1", basket.Last().Name);
            Assert.Equal(expectedAddedAmmount, basket.Last().Quantity);
        }

        [Fact]
        public void AddProductToBasket_DoNotAddProduct_When_Invalid_Product_Id_Given()
        {
            uint quantity = 1;
            int invalidId = 177;
            var expectedBasketCount = basket.Count();
            orderService.AddProductToBasket(basket, invalidId, quantity);
            var actualBasketCount = basket.Count();
            Assert.Equal(expectedBasketCount, actualBasketCount);
        }

        [Fact]
        public async Task AddProductToBasket_DoNotAddProduct_When_No_Available_Quantity()
        {
            int productId = 1;
            var product = productsRepository.All().FirstOrDefault(x => x.Id == productId);
            product.Quantity = 0;
            await productsRepository.SaveChangesAsync();
            uint quantityRequired = 2;
            var expectedBasketCount = basket.Count();
            orderService.AddProductToBasket(basket, productId, quantityRequired);
            var actualBasketCount = basket.Count();
            Assert.Equal(expectedBasketCount, actualBasketCount);
            ClearProductsToBeRepopulated();
        }

        [Fact]
        public void SubtractProductFromBasket_SubtractsAmmount_When_Ammount_Is_Less_ThanBasketQuantity()
        {
            this.basket = new List<PurchaseItemDto>();
            int id = 1;
            uint ammount = 5;
            var expectedBasketCount = basket.Count() + 1;
            orderService.AddProductToBasket(basket, id, ammount);
            uint withdrawAmmount = 3;
            uint expectedProductsInBasketRemaining = 2;
            orderService.SubtractProductFromBasket((List<PurchaseItemDto>)basket, id, withdrawAmmount);
            uint actualProductsInBasketRemaining = basket.FirstOrDefault(x => x.Id == id).Quantity;
            Assert.Equal(expectedProductsInBasketRemaining, actualProductsInBasketRemaining);
            ClearProductsToBeRepopulated();
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        public void SubtractProductFromBasket_RemovesPurchase_When_Ammount_Is_More_ThanBasket_Quantity(uint withdrawAmmount)
        {
            this.basket = new List<PurchaseItemDto>();
            int id = 1;
            uint ammount = 5;
            orderService.AddProductToBasket(basket, id, ammount);
            int expectedBasketProducts = basket.Count() - 1;
            orderService.SubtractProductFromBasket((List<PurchaseItemDto>)basket, id, withdrawAmmount);
            int actualBasketProducts = basket.Count();
            Assert.Equal(expectedBasketProducts, actualBasketProducts);
            ClearProductsToBeRepopulated();
        }

        [Fact]
        public void TryCreateOrder_CreatesOrder_When_Ammount_Is_Available()
        {

            this.basket = new List<PurchaseItemDto>();
            int id = 2;
            uint ammount = 5;
            orderService.AddProductToBasket(basket, id, ammount);
            Assert.True(basket.Count() == 1);
            var product = productsRepository.All().FirstOrDefault(x => x.Id == id);
            uint expectedProductStockAmmountAfterOrder = product.Quantity - ammount;
            int expectedOrdersCount = orderssRepository.All().Count() + 1;

            bool submitSuccessfully = orderService.TryCreateOrder((List<PurchaseItemDto>)basket, DIContainer.TestUserId);
            Assert.True(submitSuccessfully);
            uint actualProductStockAmmountAfterOrder = product.Quantity;
            int actualOrdersCount = orderssRepository.All().Count();
            Assert.True(basket.Any());
            Assert.Equal(expectedProductStockAmmountAfterOrder, actualProductStockAmmountAfterOrder);
            Assert.Equal(expectedOrdersCount, actualOrdersCount);
            Assert.Equal("New order created", DIContainer.EmailSent.Mail.Subject);
        }



        // bool TryCreateOrder(List<PurchaseItemDto> basket, string userId);


        //public void SubtractProductFromBasket(List<PurchaseItemDto> basket, int productId, uint ammount)
        //{
        //    PurchaseItemDto purchase = basket.FirstOrDefault(p => p.Id == productId);
        //    if (purchase.Quantity <= ammount)
        //    {
        //        basket.Remove(purchase);
        //        return;
        //    }
        //    purchase.Quantity -= ammount;
        //}







        // void SubtractProductFromBasket(List<PurchaseItemDto> basket, int productId, uint ammount);

        private void ClearProductsToBeRepopulated()
        {
            var productsLeft = productsRepository.All().ToArray();
            productsRepository.RemoveRange(productsLeft);
            productsRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        private void SeedData()
        {
            if (productsRepository.All().Any()) return;
            uint initialCount = 10;
            var setOfProducts = new List<Product>();
            for (int i = 1; i < 6; i++)
            {
                setOfProducts.Add(new Product
                {
                    Id = i,
                    Name = "Product " + i,
                    Quantity = initialCount,
                    MonthsWarranty = 9
                });
            }
            productsRepository.AddRangeAssync(setOfProducts).GetAwaiter().GetResult();
            orderssRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
