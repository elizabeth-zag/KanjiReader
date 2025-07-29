using Hangfire;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Domain.TextProcessing.Handlers;

namespace KanjiReader.Domain.Jobs;

[AutomaticRetry(Attempts = 3)]
public class TextProcessingJob
{
    private readonly TextProcessingHandlersFactory _textProcessingHandlersFactory;

    public TextProcessingJob(TextProcessingHandlersFactory textProcessingHandlersFactory)
    {
        _textProcessingHandlersFactory = textProcessingHandlersFactory;
    }

    public async Task Execute(string userId, GenerationSourceType sourceType, CancellationToken cancellationToken)
    {
        var handler = _textProcessingHandlersFactory.GetHandler(sourceType);
        await handler.Handle(userId, cancellationToken);
    }
}