using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using WorldRank.Application.Behaviors;
using Xunit;

namespace WorldRank.UnitTests.Behaviors;

public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_WhenHandlerSucceeds_ReturnsHandlerResponse()
    {
        // Arrange
        var logger =
            NullLogger<
                LoggingBehavior<TestRequest, string>>
                .Instance;

        var behavior =
            new LoggingBehavior<TestRequest, string>(
                logger);

        var request =
            new TestRequest("test-value");

        var handlerCallCount = 0;

        RequestHandlerDelegate<string> next =
            _ =>
            {
                handlerCallCount++;

                return Task.FromResult(
                    "handler-response");
            };

        // Act
        var result = await behavior.Handle(
            request,
            next,
            CancellationToken.None);

        // Assert
        Assert.Equal(
            "handler-response",
            result);

        Assert.Equal(1, handlerCallCount);
    }

    [Fact]
    public async Task Handle_WhenHandlerFails_RethrowsOriginalException()
    {
        // Arrange
        var logger =
            NullLogger<
                LoggingBehavior<TestRequest, string>>
                .Instance;

        var behavior =
            new LoggingBehavior<TestRequest, string>(
                logger);

        var request =
            new TestRequest("test-value");

        var expectedException =
            new InvalidOperationException(
                "Handler failed.");

        RequestHandlerDelegate<string> next =
            _ => Task.FromException<string>(
                expectedException);

        // Act
        var exception =
            await Assert.ThrowsAsync<
                InvalidOperationException>(
                () => behavior.Handle(
                    request,
                    next,
                    CancellationToken.None));

        // Assert
        Assert.Same(
            expectedException,
            exception);
    }

    [Fact]
    public async Task Handle_WhenCalled_ExecutesHandlerOnlyOnce()
    {
        // Arrange
        var logger =
            NullLogger<
                LoggingBehavior<TestRequest, int>>
                .Instance;

        var behavior =
            new LoggingBehavior<TestRequest, int>(
                logger);

        var request =
            new TestRequest("test-value");

        var handlerCallCount = 0;

        RequestHandlerDelegate<int> next =
            _ =>
            {
                handlerCallCount++;

                return Task.FromResult(42);
            };

        // Act
        var result = await behavior.Handle(
            request,
            next,
            CancellationToken.None);

        // Assert
        Assert.Equal(42, result);
        Assert.Equal(1, handlerCallCount);
    }

    public sealed record TestRequest(
        string Value);
}