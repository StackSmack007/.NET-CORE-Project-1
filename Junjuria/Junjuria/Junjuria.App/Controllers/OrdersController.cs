namespace Junjuria.App.Controllers
{
    using Junjuria.DataTransferObjects.Orders;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OrdersController : BaseController
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }


        // GET: Orders
        public ActionResult Index()
        {
            throw new System.Exception();
        }

        [HttpPost]
        public ActionResult AddInBasket(int productId, int count = 1,string returnPath=null)
        {
            var session = HttpContext.Session;
            if (!session.Keys.Any(x => x == "Basket"))
            {
                List<PurchaseItemDto> purchaseItems = new List<PurchaseItemDto>();
                session.SetString("Basket", JsonConvert.SerializeObject(purchaseItems));
            }
            var basket = JsonConvert.DeserializeObject<PurchaseItemDto[]>(session.GetString("Basket")).ToList();
            orderService.Add(basket, productId, count);
            session.SetString("Basket", JsonConvert.SerializeObject(basket));
            //ToDo what if product is added from layot of another view!
            if (returnPath == null)
            {
                return RedirectToAction("Details", "Products", new { id = productId });
            }
            return Redirect(returnPath);
        }

        public ActionResult SubtractFromBasket(int productId, int count = 1, string returnPath = null)
        {
            var session = HttpContext.Session;
            if (session.Keys.Any(x => x == "Basket"))
            {
 var basket = JsonConvert.DeserializeObject<PurchaseItemDto[]>(session.GetString("Basket")).ToList();
            orderService.Add(basket, productId, count);
            session.SetString("Basket", JsonConvert.SerializeObject(basket));
            }
            return Redirect(returnPath);
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