using Hangfire;
using Hangfire.Server;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.TextProcessing.Handlers;

namespace KanjiReader.Domain.Jobs;

[AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 120, 600 })]
public class TextProcessingJob(TextProcessingHandlersFactory textProcessingHandlersFactory)
{
    public async Task Execute(string userId, GenerationSourceType sourceType, PerformContext context, CancellationToken cancellationToken)
    {
        var retryCount = context.GetJobParameter<int>("RetryCount");
        var isLastRetry = retryCount == 3; // todo: config?
        var handler = textProcessingHandlersFactory.GetHandler(sourceType);
        await handler.Handle(userId, isLastRetry, cancellationToken);
    }
}