using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using WorldRank.Application.Commands.CurrencyRates;
using WorldRank.Application.ExchangeRates;

namespace WorldRank.Application.Jobs;

[DisallowConcurrentExecution]
public sealed class CurrencyRatesJob : IJob
{
    private readonly IEcbHttpClient _ecbHttpClient;
    private readonly ISender _sender;
    private readonly ILogger<CurrencyRatesJob> _logger;

    public CurrencyRatesJob(
        IEcbHttpClient ecbHttpClient,
        ISender sender,
        ILogger<CurrencyRatesJob> logger)
    {
        _ecbHttpClient = ecbHttpClient;
        _sender = sender;
        _logger = logger;
    }

    public async Task Execute(
        IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation(
                "CurrencyRatesJob started.");

            var rates =
                await _ecbHttpClient.GetLatestRatesAsync(
                    context.CancellationToken);

            await _sender.Send(
                new UpsertCurrencyRatesCommand(rates),
                context.CancellationToken);

            _logger.LogInformation(
                "CurrencyRatesJob completed. " +
                "Retrieved and persisted {Count} rates.",
                rates.Count);
        }
        catch (OperationCanceledException)
            when (context.CancellationToken
                .IsCancellationRequested)
        {
            _logger.LogInformation(
                "CurrencyRatesJob was cancelled.");

            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "CurrencyRatesJob failed.");

            var jobException =
                new JobExecutionException(exception)
                {
                    RefireImmediately = false
                };

            throw jobException;
        }
    }
}