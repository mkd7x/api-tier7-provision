using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.HealthChecks;

namespace ApiTier7Provision.Application.Tests;

public sealed class LogHealthCheckCommandHandlerTests
{
    [Fact]
    public async Task Handle_PersistsCurrentUtcTime_AndReturnsInsertedId()
    {
        var now = new DateTime(2026, 04, 30, 12, 0, 0, DateTimeKind.Utc);
        var repository = new FakeHealthCheckLogRepository();
        var handler = new LogHealthCheckCommandHandler(repository, new FixedDateTimeProvider(now));

        var result = await handler.Handle(new LogHealthCheckCommand(), CancellationToken.None);

        Assert.Equal(100, result.Id);
        Assert.Equal(now, result.LoggedAtUtc);
        Assert.Equal(now, repository.LastLoggedAtUtc);
    }

    private sealed class FakeHealthCheckLogRepository : IHealthCheckLogRepository
    {
        public DateTime? LastLoggedAtUtc { get; private set; }

        public Task<int> AddAsync(DateTime loggedAtUtc, CancellationToken cancellationToken)
        {
            LastLoggedAtUtc = loggedAtUtc;
            return Task.FromResult(100);
        }
    }

    private sealed class FixedDateTimeProvider(DateTime utcNow) : IDateTimeProvider
    {
        public DateTime UtcNow => utcNow;
    }
}
