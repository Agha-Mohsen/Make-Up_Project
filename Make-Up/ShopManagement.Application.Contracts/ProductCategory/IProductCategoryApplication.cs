using System.Collections.Generic;
using _0_Framework.Application;

namespace ShopManagement.Application.Contracts.ProductCategory
{
    public interface IProductCategoryApplication
    {
        OperationResult Create(CreateProductCategory command);
        EditProductCategory GetDetails(long id);
        OperationResult Edit(EditProductCategory command);
        List<ProductCategoryViewModel> Search(ProductCategorySearchModel searchModel);
    }
}
