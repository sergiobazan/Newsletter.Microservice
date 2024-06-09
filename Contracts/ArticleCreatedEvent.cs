namespace Contracts;

public sealed record ArticleCreatedEvent(
    Guid Id,
    DateTime CreatedOnUtc);