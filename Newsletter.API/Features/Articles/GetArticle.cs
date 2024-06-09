using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newsletter.Api.Shared;
using Newsletter.API.Contracts;
using Newsletter.API.Database;

namespace Newsletter.API.Features.Articles;

public static class GetArticle
{
    public sealed record Query(Guid Id) : IRequest<Result<ArticleResponse>>;

    internal sealed class Handler(ApplicationDbContext context) : IRequestHandler<Query, Result<ArticleResponse>>
    {
        public async Task<Result<ArticleResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var article = await context
                .Articles
                .AsNoTracking()
                .Where(article => article.Id == request.Id)
                .Select(article => new ArticleResponse
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    Tags = article.Tags,
                    CreatedOnUtc = article.CreatedOnUtc,
                    PublishedOnUtc = article.PublishedOnUtc
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (article is null)
            {
                return Result.Failure<ArticleResponse>(new Error(
                    "GetArticle.Null",
                    "The article with the given Id was not found"));
            }

            return article;
        }
    }
}

public class GetArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/articles/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetArticle.Query(id);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result);
        });
    }
}