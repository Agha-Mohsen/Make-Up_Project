using System.Collections.Generic;
using _0_Framework.Application;
using ShopManagement.Application.Contracts.ProductCategory;
using ShopManagement.Domain.ProductCategoryAgg;

namespace ShopManagement.Application
{
    public class ProductCategoryApplication : IProductCategoryApplication
    {
        public IProductCategoryRepository ProductCategoryRepository;

        public ProductCategoryApplication(IProductCategoryRepository productCategoryRepository)
        {
            ProductCategoryRepository = productCategoryRepository;
        }

        public OperationResult Create(CreateProductCategory command)
        {
            var operation = new OperationResult();
            if (ProductCategoryRepository.Exists(x => x.Name == command.Name))
                return operation.Failed("امکان ثبت رکورد تکراری وجود ندارد. لطفا مجدد تلاش فرمایید.");

            var slug = command.Slug.Slugify();
            var productCategory = new ProductCategory(command.Name, command.Description
                , command.Picture, command.PictureAlt, command.PictureTitle
                , command.Keywords, command.MetaDescription, slug);

            ProductCategoryRepository.Create(productCategory);
            ProductCategoryRepository.SaveChanges();

            return operation.Succeeded();
        }

 
        public OperationResult Edit(EditProductCategory command)
        {
            var operation = new OperationResult();

            var productCategory = ProductCategoryRepository.Get(command.Id);
            if (productCategory == null)
                return operation.Failed("رکورد با اطلاعات درخواست شده یافت نشد . لطفا مجدد تلاش فرمایید.");

            if (ProductCategoryRepository.Exists(x => x.Name == command.Name && x.Id != command.Id))
                return operation.Failed("رکورد با اطلاعات درخواست شده یافت نشد . لطفا مجدد تلاش فرمایید.");

            var slug = command.Slug.Slugify();

            productCategory.Edit(command.Name, command.Description, command.Picture, command.PictureAlt
                , command.PictureTitle, command.Keywords, command.MetaDescription, slug);
            ProductCategoryRepository.SaveChanges();

            return operation.Succeeded();
        }

        public EditProductCategory GetDetails(long id)
        {
            return ProductCategoryRepository.GetDetails(id);
        }

        public List<ProductCategoryViewModel> Search(ProductCategorySearchModel searchModel)
        {
            return ProductCategoryRepository.Search(searchModel);
        }
    }
}