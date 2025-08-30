using System.Threading.Channels;

namespace KanjiReader.Presentation.EventStream;
public class TextBroadcaster : ITextBroadcaster
{
    private readonly Channel<string> _channel =
        Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = false, SingleWriter = false });

    public ChannelReader<string> Subscribe(CancellationToken cancellationToken) => _channel.Reader;

    public ValueTask Publish(string json, CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(json, cancellationToken);
}
