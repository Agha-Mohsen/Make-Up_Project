using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _0_Framework.Infrastructure;
using BlogManagement.Application.Contracts.ArticleCategory;
using BlogManagement.Domain.ArticleCategoryAgg;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Infrastructure.EFCore.Repository
{
    public class ArticleCategoryRepository : RepositoryBase<long, ArticleCategory>, IArticleCategoryRepository
    {
        private readonly BlogContext _context;

        public ArticleCategoryRepository(BlogContext context) : base(context)
        {
            _context = context;
        }

        public string GetSlugById(long id)
        {
            return _context.ArticleCategories.Select(x => new {x.Id, x.Slug}).AsNoTracking()
                .FirstOrDefault(x => x.Id == id).Slug;
        }

        public EditArticleCategory GetDetails(long id)
        {
            return _context.ArticleCategories.Select(x => new EditArticleCategory
            {
                Id = x.Id,
                Name = x.Name,
                ShowOrder = x.ShowOrder,
                Description = x.Description,
                Slug = x.Slug,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                Keywords = x.Keywords,
                MetaDescription = x.MetaDescription,
                CanonicalAddress = x.CanonicalAddress
            }).AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public List<ArticleCategoryViewModel> GetArticleCategories()
        {
            return _context.ArticleCategories.Select(x => new ArticleCategoryViewModel
            {
                Id = x.Id ,Name = x.Name
            }).ToList();
        }

        public List<ArticleCategoryViewModel> Search(ArticleCategorySearchModel categorySearchModel)
        {
            var query = _context.ArticleCategories
                .Include(x=>x.Articles)
                .Select(x => new ArticleCategoryViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Picture = x.Picture,
                ShowOrder = x.ShowOrder,
                Description = x.Description,
                CreationDate = x.CreationDate.ToFarsi(),
                ArticlesCount = x.Articles.Count
            }).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(categorySearchModel.Name))
                query = query.Where(x => x.Name.Contains(categorySearchModel.Name));

            return query.OrderByDescending(x => x.ShowOrder).ToList();
        }
    }
}