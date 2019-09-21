using Junjuria.Infrastructure.Models;
using System.Collections.Generic;

namespace Junjuria.Services.Services.Contracts
{
  public  interface ICategoryService
    {
/// <summary>
/// Returns the category and its child categories !
/// </summary>
/// <param name="categoryId"></param>
/// <returns></returns>
        ICollection<int> GetSubcategoriesOfCagetoryId(int categoryId);
        Category GetById(int catId);
    }
}