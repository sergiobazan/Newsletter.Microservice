using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newsletter.Reporting.Api.Database;
using Newsletter.Reporting.Api.Entities;

namespace Newsletter.Reporting.Api.Features.Articles;

public class ArticleViewed(ApplicationDbContext dbContext) : IConsumer<ArticleViewedEvent>
{
    public async Task Consume(ConsumeContext<ArticleViewedEvent> context)
    {
        var article = await dbContext
            .Articles
            .FirstOrDefaultAsync(article => article.Id == context.Message.Id);

        if (article is null)
        {
            return;
        }

        var articleEvent = new ArticleEvent
        {
            Id = Guid.NewGuid(),
            ArticleId = article.Id,
            CreatedOnUtc = context.Message.ViewedOnUtc,
            EventType = ArticleEventType.View
        };

        dbContext.Add(articleEvent);

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
