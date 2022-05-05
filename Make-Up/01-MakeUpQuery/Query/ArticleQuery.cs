using System;
using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _01_MakeUpQuery.Contracts.Article;
using _01_MakeUpQuery.Contracts.Comment;
using BlogManagement.Infrastructure.EFCore;
using CommentManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;

namespace _01_MakeUpQuery.Query
{
    public class ArticleQuery : IArticleQuery
    {
        private readonly BlogContext _context;
        private readonly CommentContext _commentContext;

        public ArticleQuery(BlogContext context, CommentContext commentContext)
        {
            _context = context;
            _commentContext = commentContext;
        }

        public List<ArticleQueryModel> LatestArticles()
        {
            return _context.Article
                .Include(x => x.Category)
                .Where(x => x.PublishDate <= DateTime.Now)
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
                .Include(x => x.Category)
                .Where(x => x.PublishDate <= DateTime.Now)
                .Select(x => new ArticleQueryModel
                {
                    Id = x.Id,
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
                }).AsNoTracking().FirstOrDefault(x => x.Slug == slug);

            if (!string.IsNullOrWhiteSpace(article.Keywords))
                article.KeywordList = article.Keywords.Split(",").ToList();

            var comments = _commentContext.Comments
                .Where(x => x.IsConfirmed)
                .Where(x => !x.IsCanceled)
                .Where(x => x.Type == CommentType.Article)
                .Where(x => x.OwnerRecordId == article.Id)
                .Include(x => x.Parent)
                .Select(x => new CommentQueryModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Message = x.Message,
                    CreationDate = x.CreationDate.ToFarsi(),
                    ParentId = x.ParentId
                }).AsNoTracking()
                .OrderByDescending(x => x.Id).ToList();

            foreach (var comment in comments)
            {
                if (comment.ParentId > 0)
                {
                    comment.ParentName = comments.FirstOrDefault(x => x.Id == comment.ParentId)?.Name;
                }
            }

            article.Comments = comments;

            return article;
        }
    }
}