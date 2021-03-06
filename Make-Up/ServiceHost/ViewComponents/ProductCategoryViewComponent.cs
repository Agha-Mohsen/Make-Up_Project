using _01_MakeUpQuery.Contracts.ProductCategory;
using Microsoft.AspNetCore.Mvc;

namespace ServiceHost.ViewComponents
{
    public class ProductCategoryViewComponent : ViewComponent
    {
        private readonly IProductCategoryQuery _productCategory;

        public ProductCategoryViewComponent(IProductCategoryQuery productCategory)
        {
            _productCategory = productCategory;
        }

        public IViewComponentResult Invoke()
        {
            var productCategory = _productCategory.GetProductCategories();
            return View(productCategory);
        }
    }
}