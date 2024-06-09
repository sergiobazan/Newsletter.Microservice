namespace Contracts;

public sealed record ArticleViewedEvent(
    Guid Id,
    DateTime ViewedOnUtc);
