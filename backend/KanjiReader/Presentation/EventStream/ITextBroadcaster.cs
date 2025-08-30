using System.Threading.Channels;

namespace KanjiReader.Presentation.EventStream;

public interface ITextBroadcaster
{
    ChannelReader<string> Subscribe(CancellationToken cancellationToken);
    ValueTask Publish(string json, CancellationToken cancellationToken = default);
}