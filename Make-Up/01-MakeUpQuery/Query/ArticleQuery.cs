using System;
using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _01_MakeUpQuery.Contracts.Article;
using BlogManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;

namespace _01_MakeUpQuery.Query
{
    public class ArticleQuery : IArticleQuery
    {
        private readonly BlogContext _context;

        public ArticleQuery(BlogContext context)
        {
            _context = context;
        }

        public List<ArticleQueryModel> LatestArticles()
        {
            return _context.Article
                .Include(x=>x.Category)
                .Where(x=> x.PublishDate <= DateTime.Now)
                .Select(x => new ArticleQueryModel
            {
                Title = x.Title,
                Slug = x.Slug,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                Picture = x.Picture,
                PublishDate = x.PublishDate.ToFarsi(),
                ShortDescription = x.ShortDescription
            }).AsNoTracking().Take(8).ToList();
        }

        public ArticleQueryModel GetArticleDetails(string slug)
        {
            var article = _context.Article
                .Include(x=> x.Category)
                .Where(x => x.PublishDate <= DateTime.Now)
                .Select(x => new ArticleQueryModel
            {
                Title = x.Title,
                Slug = x.Slug,
                Description = x.Description,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                Picture = x.Picture,
                CanonicalAddress = x.CanonicalAddress,
                Keywords = x.Keywords,
                MetaDescription = x.MetaDescription,
                PublishDate = x.PublishDate.ToFarsi(),
                ShortDescription = x.ShortDescription,
                CategoryName = x.Category.Name,
                CategorySlug = x.Category.Slug
            }).AsNoTracking().FirstOrDefault(x=>x.Slug == slug);

            if (!string.IsNullOrWhiteSpace(article.Keywords))
                article.KeywordList = article.Keywords.Split(",").ToList();

            return article;
        }
    }
}
