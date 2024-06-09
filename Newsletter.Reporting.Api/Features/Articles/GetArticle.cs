using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newsletter.Api.Shared;
using Newsletter.Reporting.Api.Database;
using Newsletter.Reporting.Api.Entities;

namespace Newsletter.Reporting.Api.Features.Articles;

public static class GetArticle
{
    public sealed record Query(Guid Id) : IRequest<Result<ArticleResponse>>;

    public sealed class Handler(ApplicationDbContext context) : IRequestHandler<Query, Result<ArticleResponse>>
    {
        public async Task<Result<ArticleResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var articleResponse = await context
                .Articles
                .AsNoTracking()
                .Where(article => article.Id == request.Id)
                .Select(article => new ArticleResponse
                (
                    article.Id,
                    article.CreatedOnUtc,
                    article.PublishedOnUtc,
                    context
                        .ArticleEvents
                        .Where(articleEvent => articleEvent.ArticleId == article.Id)
                        .Select(articleEvent => new ArticleEventResponse
                        {
                            Id = articleEvent.Id,
                            EventType = articleEvent.EventType,
                            CreatedOnUtc = articleEvent.CreatedOnUtc
                        })
                        .ToList()
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (articleResponse is null)
            {
                return Result.Failure<ArticleResponse>(new Error(
                    "GetArticle.Null",
                    "The article with the specified ID was not found"));
            }

            return articleResponse;
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

            return Results.Ok(result.Value);
        });
    }
}

public sealed record ArticleResponse(
    Guid Id,
    DateTime CreatedOnUtc,
    DateTime? PublishedOnUtc,
    List<ArticleEventResponse> Events);

public class ArticleEventResponse
{
    public Guid Id { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public ArticleEventType EventType { get; set; }
}