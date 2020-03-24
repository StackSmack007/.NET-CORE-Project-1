namespace Junjuria.App.Areas.Common.Pages.Recomendations
{
    using AutoMapper;
    using Junjuria.Common.Extensions;
    using Junjuria.DataTransferObjects.RecomendationsPage;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class IndexModel : PageModel
    {
        private readonly IRepository<Recomendation> recomendationsRepository;
        private readonly IMapper mapper;

        public IndexModel(IRepository<Recomendation> recomendationsRepository, IMapper mapper)
        {
            this.recomendationsRepository = recomendationsRepository;
            this.mapper = mapper;
            Recomendations = new HashSet<RecomendationOutDto>();
        }

        [BindProperty]
        public RecomendationInDto RecomendationNew { get; set; }

        public ICollection<RecomendationOutDto> Recomendations { get; set; }

        public void OnGet()
        {
            RecomendationNew = new RecomendationInDto();
            if (User.Identity.IsAuthenticated)
            {
                RecomendationNew.Author = User.Identity.Name;
                if (User.IsInRole("Admin"))
                {
                    Recomendations = recomendationsRepository.All().To<RecomendationOutDto>().OrderByDescending(x => x.DateOfCreation).ToArray();
                    return;
                }
            }
            Recomendations = recomendationsRepository.All().To<RecomendationOutDto>().Where(x => !x.IsDeleted).OrderByDescending(x => x.DateOfCreation).ToArray();
        }
        public async Task<IActionResult> OnPostAddRecomendationAsync()
        {
            if (ModelState.IsValid)
            {
                if (RecomendationNew.Author == User.Identity.Name)
                {
                    RecomendationNew.IsVerified = true;
                }
                await recomendationsRepository.AddAssync(mapper.Map<Recomendation>(this.RecomendationNew));
                await recomendationsRepository.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostModifyRecomendationStatusAsync(int recomendationId)
        {
            var recomendation = await recomendationsRepository.All().FirstOrDefaultAsync(x => x.Id == recomendationId);
            if (recomendation != null)
            {
                recomendation.IsDeleted = !recomendation.IsDeleted;
                await recomendationsRepository.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}