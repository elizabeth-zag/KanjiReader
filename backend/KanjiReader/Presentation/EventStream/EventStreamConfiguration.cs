using Microsoft.AspNetCore.Http;

namespace KanjiReader.Presentation.EventStream;

public static class EventStreamConfiguration
{
    public static async Task StreamGet(HttpContext context, ITextBroadcaster bus, CancellationToken cancellationToken)
    {
        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers["X-Accel-Buffering"] = "no";
        context.Response.Headers.Connection = "keep-alive";
        context.Response.ContentType = "text/event-stream";

        var reader = bus.Subscribe(cancellationToken);

        async Task Heartbeat()
        {
            await context.Response.WriteAsync($": ping {DateTimeOffset.UtcNow:O}\n\n", cancellationToken);
            await context.Response.Body.FlushAsync(cancellationToken);
        }
        
        await context.Response.WriteAsync("retry: 3000\n\n", cancellationToken);

        var hbEvery = TimeSpan.FromSeconds(15);
        var nextHb = DateTime.UtcNow + hbEvery;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (await reader.WaitToReadAsync(cancellationToken))
            {
                while (reader.TryRead(out var json))
                {
                    await context.Response.WriteAsync($"data: {json}\n\n", cancellationToken);
                    await context.Response.Body.FlushAsync(cancellationToken);
                }
            }

            if (DateTime.UtcNow >= nextHb)
            {
                await Heartbeat();
                nextHb = DateTime.UtcNow + hbEvery;
            }
        }
    }
}