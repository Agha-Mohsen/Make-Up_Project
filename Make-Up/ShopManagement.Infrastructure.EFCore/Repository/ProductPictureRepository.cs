using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _0_Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ShopManagement.Application.Contracts.ProductPicture;
using ShopManagement.Domain.ProductPictureAgg;

namespace ShopManagement.Infrastructure.EFCore.Repository
{
    public class ProductPictureRepository : RepositoryBase<long, ProductPicture>, IProductPictureRepository
    {
        private readonly ShopContext _context;

        public ProductPictureRepository(ShopContext _context) : base(_context)
        {
            this._context = _context;
        }

        public EditProductPicture GetDetails(long id)
        {
            return _context.ProductsPictures.Select(x => new EditProductPicture
            {
                Id = x.Id,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                ProductId = x.ProductId
            }).FirstOrDefault(x => x.Id == id);
        }

        public ProductPicture GetWithProductAndCategory(long id)
        {
            return _context.ProductsPictures
                .Include(productPicture => productPicture.Product)
                .ThenInclude(product => product.Category)
                .FirstOrDefault(productPicture => productPicture.Id == id);
        }

        public List<ProductPictureViewModel> Search(ProductPictureSearchModel searchModel)
        {
            var query = _context.ProductsPictures.Include(x => x.Product).Select(x => new ProductPictureViewModel
            {
                Id = x.Id,
                Picture = x.Picture,
                Product = x.Product.Name,
                ProductId = x.ProductId,
                CreationDate = x.CreationDate.ToFarsi(),
                IsRemoved = x.IsRemoved
            }).ToList();

            if (searchModel.ProductId != 0)
                query = query.Where(x => x.ProductId == searchModel.ProductId).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }
    }
}