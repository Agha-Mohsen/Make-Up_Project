using System.Collections.Generic;
using _01_MakeUpQuery.Contracts.ArticleCategory;
using _01_MakeUpQuery.Contracts.ProductCategory;

namespace _01_MakeUpQuery
{
    public class MenuModel
    {
        public List<ArticleCategoryQueryModel> ArticleCategories { get; set; }
        public List<ProductCategoryQueryModel> ProductCategories { get; set; }
    }
}
