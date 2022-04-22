using System;
using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _01_MakeUpQuery.Contracts.Product;
using _01_MakeUpQuery.Contracts.ProductCategory;
using DiscountManagement.Infrastructure.EFCore;
using InventoryManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using ShopManagement.Domain.ProductAgg;
using ShopManagement.Infrastructure.EFCore;

namespace _01_MakeUpQuery.Query
{
    public class ProductCategoryQuery : IProductCategoryQuery
    {
        private readonly ShopContext _context;
        private readonly InventoryContext _inventoryContext;
        private readonly DiscountContext _discountContext;

        public ProductCategoryQuery(ShopContext context, InventoryContext inventoryContext,
            DiscountContext discountContext)
        {
            _context = context;
            _inventoryContext = inventoryContext;
            _discountContext = discountContext;
        }
        public List<ProductCategoryQueryModel> GetProductCategories()
        {
            return _context.ProductCategories.Select(productCategory => new ProductCategoryQueryModel
            {
                Id = productCategory.Id,
                Name = productCategory.Name,
                Picture = productCategory.Picture,
                PictureAlt = productCategory.PictureAlt,
                PictureTitle = productCategory.PictureTitle,
                Slug = productCategory.Slug
            }).ToList();
        }

        public List<ProductCategoryQueryModel> GetProductCategoriesWithProducts()
        {
            var inventory = _inventoryContext.Inventory.Select(inventory => new 
                {inventory.ProductId, inventory.UnitPrice}).ToList();

            var discounts = _discountContext.CustomerDiscounts
                .Where(customerDiscount => customerDiscount.StartDate < DateTime.Now && customerDiscount.EndDate > DateTime.Now)
                .Select(customerDiscount => new {customerDiscount.ProductId, customerDiscount.DiscountRate}).ToList();

            var categories = _context.ProductCategories
                .Include(productCategory => productCategory.Products)
                .ThenInclude(product => product.Category)
                .Select(productCategory => new ProductCategoryQueryModel
                {
                    Id = productCategory.Id,
                    Name = productCategory.Name,
                    Products = MapProducts(productCategory.Products)
                }).ToList();

            foreach (var category in categories)
            {
                foreach (var product in category.Products)
                {
                    var productInventory = inventory.FirstOrDefault(x => x.ProductId == product.Id);
                    if (productInventory != null)
                    {
                        var price = productInventory.UnitPrice;
                        product.Price = price.ToMoney();
                        var discount = discounts.FirstOrDefault(x => x.ProductId == product.Id);
                        if (discount != null)
                        {
                            int discountRate = discount.DiscountRate;
                            product.DiscountRate = discountRate;
                            product.HasDiscount = discountRate > 0;
                            var discountAmount = Math.Round((price * discountRate) / 100);
                            product.PriceWithDiscount = (price - discountAmount).ToMoney();
                        }
                    }
                }
            }

            return categories;
        }

        private static List<ProductQueryModel> MapProducts(List<Product> products)
        {
            return products.Select(product => new ProductQueryModel
            {
                Id = product.Id,
                Category = product.Category.Name,
                Name = product.Name,
                Picture = product.Picture,
                PictureAlt = product.PictureAlt,
                PictureTitle = product.PictureTitle,
                Slug = product.Slug
            }).ToList();
        }

        public ProductCategoryQueryModel GetProductCategoryWithProductsBy(string slug)
        {
            var inventory = _inventoryContext.Inventory.Select(inventory => new { inventory.ProductId, inventory.UnitPrice }).ToList();

            var discounts = _discountContext.CustomerDiscounts
                .Where(customerDiscount => customerDiscount.StartDate < DateTime.Now && customerDiscount.EndDate > DateTime.Now)
                .Select(customerDiscount => new { customerDiscount.ProductId, customerDiscount.DiscountRate , customerDiscount.EndDate }).ToList();

            var category = _context.ProductCategories
                .Include(productCategory => productCategory.Products)
                .ThenInclude(product => product.Category)
                .Select(productCategory => new ProductCategoryQueryModel
                {
                    Id = productCategory.Id,
                    Name = productCategory.Name,
                    Description = productCategory.Description,
                    MetaDescription = productCategory.MetaDescription,
                    Slug = productCategory.Slug,
                    Keywords = productCategory.Keywords,
                    Products = MapProducts(productCategory.Products)
                }).FirstOrDefault(x => x.Slug == slug);

            foreach (var product in category.Products)
            {
                var productInventory = inventory.FirstOrDefault(x => x.ProductId == product.Id);
                if (productInventory != null)
                {
                    var price = productInventory.UnitPrice;
                    product.Price = price.ToMoney();
                    var discount = discounts.FirstOrDefault(x => x.ProductId == product.Id);
                    if (discount != null)
                    {
                        int discountRate = discount.DiscountRate;
                        product.DiscountRate = discountRate;
                        product.DiscountExpireDate = discount.EndDate.ToDiscountFormat();
                        product.HasDiscount = discountRate > 0;
                        var discountAmount = Math.Round((price * discountRate) / 100);
                        product.PriceWithDiscount = (price - discountAmount).ToMoney();
                    }
                }
            }
            return category;
        }

    }
}