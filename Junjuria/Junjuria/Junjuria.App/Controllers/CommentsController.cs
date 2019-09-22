namespace Junjuria.App.Controllers
{
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    public class CommentsController : BaseController
    {
        private readonly ICommentService commentService;
        private readonly UserManager<AppUser> userManager;

        public CommentController(ICommentService commentService, UserManager<AppUser> userManager)
        {
            this.commentService = commentService;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(CommentCreateInDto dto)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(this.User);
                try
                {
                    await commentService.AddCommentAsync(dto, user);
                }
                catch (InvalidOperationException ex)
                {
                    TempData["OldComment"] = dto.Comment;
                    TempData["CommentAddErrors"] = new { ex.Message };
                }
                return RedirectToAction("RedirectToAction", new { productId = dto.ProductId });
            }
            TempData["OldComment"] = dto.Comment;
            TempData["CommentAddErrors"] = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToArray();
            return RedirectToAction("RedirectToAction", new { productId = dto.ProductId });
        }

        public IActionResult GoToProductDetails(int productId)
        {
            return RedirectToAction("Details", "Products", new { id = productId });
        }
    }
}