using Carter;
using FluentValidation;
using MediatR;
using Mapster;
using Newsletter.Api.Shared;
using Newsletter.API.Contracts;
using Newsletter.API.Database;
using Newsletter.API.Entities;
using MassTransit;
using Contracts;

namespace Newsletter.API.Features.Articles;

public static class CreateArticle
{
    public sealed record Command(string Title, string Content, List<string> Tags) : IRequest<Result<Guid>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Title).NotEmpty();
            RuleFor(c => c.Content).NotEmpty();
        }
    }

    internal sealed class Handler(
        ApplicationDbContext context, 
        IValidator<Command> validator,
        IPublishEndpoint publishEndpoint) 
        : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(new Error(
                    "CreateArticle.Validation",
                    validationResult.ToString()));
            }

            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                Tags = request.Tags,
                CreatedOnUtc = DateTime.UtcNow
            };

            context.Add(article);

            await context.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(
                new ArticleCreatedEvent(
                    article.Id,
                    article.CreatedOnUtc),
                cancellationToken);

            return article.Id;
        }
    }
}

public class CreateArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/articles", async (CreateArticleRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateArticle.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}