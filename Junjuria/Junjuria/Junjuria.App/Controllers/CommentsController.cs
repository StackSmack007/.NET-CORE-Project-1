namespace Junjuria.App.Controllers
{
    using Junjuria.DataTransferObjects.Products;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;
    public class CommentsController : BaseController
    {
        private readonly ICommentService commentService;
        private readonly UserManager<AppUser> userManager;

        public CommentsController(ICommentService commentService, UserManager<AppUser> userManager)
        {
            this.commentService = commentService;
            this.userManager = userManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("All","Products");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(CommentCreateInDto dto)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(this.User);
                string lastCommentorId = commentService.GetLastCommentorId(dto.ProductId);
                if (user.Id == lastCommentorId)
                {
                    TempData["OldComment"] = dto.Comment;
                    TempData["CommentAddErrors"] = new string[] { "Not allowed To post comments in a sequence!" };
                    return RedirectToAction("Details", "Products", new { id = dto.ProductId });
                }
                var comment = commentService.CreateComment(dto, user);
                await commentService.SaveCommentAsync(comment);
                return RedirectToAction("GoToProductDetails", new { productId = dto.ProductId });
            }
            TempData["OldComment"] = dto.Comment;
            TempData["CommentAddErrors"] = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToArray();
            return RedirectToAction("Details", "Products", new { id = dto.ProductId });
        }

        public IActionResult GoToProductDetails(int productId)
        {
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Vote(Attitude Vote, int CommentId)
        {
            var user = await userManager.GetUserAsync(this.User);
            await commentService.SetUserAttitudeAsync(Vote, CommentId, user);
            int? productId =await commentService.GetProduct(CommentId);
            return RedirectToAction("GoToProductDetails", new { productId = productId.Value });
        }

    }
}