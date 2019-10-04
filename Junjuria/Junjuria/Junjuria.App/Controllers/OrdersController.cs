namespace Junjuria.App.Controllers
{
    using Junjuria.DataTransferObjects.Orders;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class OrdersController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly UserManager<AppUser> userManager;

        public OrdersController(IOrderService orderService, UserManager<AppUser> userManager)
        {
            this.orderService = orderService;
            this.userManager = userManager;
        }

        // GET: Orders
        public ActionResult Index()
        {
            throw new System.Exception();
        }

        [HttpPost]
        public ActionResult AddInBasket(int productId, uint count = 1, string returnPath = null)
        {
            var session = HttpContext.Session;
            if (!session.Keys.Any(x => x == "Basket"))
            {
                List<PurchaseItemDto> purchaseItems = new List<PurchaseItemDto>();
                session.SetString("Basket", JsonConvert.SerializeObject(purchaseItems));
            }
            var basket = JsonConvert.DeserializeObject<PurchaseItemDto[]>(session.GetString("Basket")).ToList();
            orderService.AddProductToBasket(basket, productId, count);
            session.SetString("Basket", JsonConvert.SerializeObject(basket));
            //ToDo what if product is added from layot of another view!
            if (returnPath == null)
            {
                return RedirectToAction("Details", "Products", new { id = productId });
            }
            return Redirect(returnPath);
        }

        public ActionResult SubtractFromBasket(int productId, string returnPath, uint count = 1)
        {
            var session = HttpContext.Session;
            if (session.Keys.Any(x => x == "Basket"))
            {
                var basket = JsonConvert.DeserializeObject<PurchaseItemDto[]>(session.GetString("Basket")).ToList();
                if (basket.Any(x => x.Id == productId))
                {
                    orderService.SubtractProductFromBasket(basket, productId, count);
                }
                session.SetString("Basket", JsonConvert.SerializeObject(basket));
            }
            return Redirect(returnPath);
        }

        public async Task<IActionResult> MyWarranties()
        {
            var user = await userManager.GetUserAsync(User);
            var warranties = orderService.GetMyWarranties(user.Id);
            return View(warranties);
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await userManager.GetUserAsync(User);
            var orders = orderService.GetMyOrders(user.Id);
            return View(orders);
        }

        [Authorize]
        public IActionResult ManageCurrentOrder()
        {
            var session = HttpContext.Session;
            if (session.Keys.Any(x => x == "Basket"))
            {
                var basket = JsonConvert.DeserializeObject<PurchaseItemDto[]>(session.GetString("Basket"));
                var orderItems = orderService.GetDetailedPurchaseInfo(basket);
                session.SetString("Basket", JsonConvert.SerializeObject(basket));
                return View(orderItems);
            }
            return RedirectToRoute(HttpContext.Request.Headers["Referer"]);
        }

        [Authorize]
        public async Task<IActionResult> Details(string id)
        {
            var orderDto =await orderService.GetOrderDetailsAsync(id);

            return View(orderDto);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ModifyItemCount(uint newAmmount, int productId)
        {
            var session = HttpContext.Session;
            if (session.Keys.Any(x => x == "Basket"))
            {
                var basket = JsonConvert.DeserializeObject<PurchaseItemDto[]>(session.GetString("Basket")).ToList();
                orderService.ModifyCountOfProductInBasket(basket, productId, newAmmount);
                session.SetString("Basket", JsonConvert.SerializeObject(basket));
            }
            return RedirectToAction(nameof(ManageCurrentOrder));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitOrder(string userName)
        {
            var session = HttpContext.Session;
            var currentUser = await userManager.GetUserAsync(User);
            if (userName == this.User.Identity.Name)
            {
                var basket = JsonConvert.DeserializeObject<PurchaseItemDto[]>(session.GetString("Basket")).ToList();
                bool attempt = orderService.TryCreateOrder(basket, currentUser.Id);
                if (!attempt)
                {
                    session.SetString("Basket", JsonConvert.SerializeObject(basket));
                    return RedirectToAction(nameof(ManageCurrentOrder));
                }
                session.Clear();
            }
            return RedirectToAction("Index", "Home");
        }

        #region Scafolded
        //// GET: Orders/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Orders/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Orders/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Orders/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Orders/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Orders/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Orders/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        #endregion
    }
}