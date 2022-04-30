using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _01_MakeUpQuery.Contracts.Article;
using _01_MakeUpQuery.Contracts.ArticleCategory;
using BlogManagement.Domain.ArticleAgg;
using BlogManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;

namespace _01_MakeUpQuery.Query
{
    public class ArticleCategoryQuery : IArticleCategoryQuery
    {
        private readonly BlogContext _context;

        public ArticleCategoryQuery(BlogContext context)
        {
            _context = context;
        }

        public ArticleCategoryQueryModel GetArticleCategory(string slug)
        {
            var articleCategory =  _context.ArticleCategories
                .Include(x=>x.Articles)
                .Select(x => new ArticleCategoryQueryModel
            {
                Slug = x.Slug,    
                Name = x.Name,
                Picture = x.Picture,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                Description = x.Description,
                Keywords = x.Keywords,
                MetaDescription = x.MetaDescription,
                CanonicalAddress = x.CanonicalAddress,
                ArticlesCount = x.Articles.Count,
                Articles = MapArticles(x.Articles)
            }).FirstOrDefault(x=>x.Slug == slug);

            if(!string.IsNullOrWhiteSpace(articleCategory.Keywords))
                articleCategory.KeywordList = articleCategory.Keywords.Split(",").ToList();

            return articleCategory;
        }

        private static List<ArticleQueryModel> MapArticles(List<Article> articles)
        {
            return articles.Select(x => new ArticleQueryModel
            {
                Slug = x.Slug,
                Title = x.Title,
                Picture = x.Picture,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                ShortDescription = x.ShortDescription,
                PublishDate = x.PublishDate.ToFarsi()
                
            }).ToList();
        }

        public List<ArticleCategoryQueryModel> GetArticleCategories()
        {
            return _context.ArticleCategories
                .Include(x => x.Articles)
                .Select(x=> new ArticleCategoryQueryModel
            {
                Name = x.Name,
                Slug = x.Slug,
                Picture = x.Picture,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                ArticlesCount = x.Articles.Count,
            }).ToList();
        }
    }
}
