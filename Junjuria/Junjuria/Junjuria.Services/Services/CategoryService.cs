namespace Junjuria.Services.Services
{
    using AutoMapper;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IMapper mapper;

        public CategoryService(IRepository<Category> categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public  Category GetById(int catId)
        {
            return categoryRepository.All().FirstOrDefault(x => x.Id == catId); 
        }

        public ICollection<int> GetSubcategoriesOfCagetoryId(int id)
        {
            HashSet<int> ids = new HashSet<int>();
            ids.Add(id);
            var idPool = categoryRepository.All().Where(x => x.CategoryId != null).Select(x => new
            {
                x.Id,
                x.CategoryId
            }).ToArray();

            while (true)
            {
                int collectedBefore = ids.Count();
                foreach (var idPack in idPool)
                {
                    if (ids.Contains(idPack.CategoryId.Value) && !ids.Contains(idPack.Id))
                    {
                        ids.Add(idPack.Id);
                    }
                }
                if (collectedBefore == ids.Count()) break;
            }
            return ids;
        }



    }
}
