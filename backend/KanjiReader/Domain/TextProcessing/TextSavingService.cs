using KanjiReader.Domain.DomainObjects;
using KanjiReader.ExternalServices.JapaneseTextSources.Nhk;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Repositories;

namespace KanjiReader.Domain.TextProcessing;

public class TextSavingService(
    ITextRepository textRepository, 
    NhkClient nhkClient)
{
    public async Task SaveNhkTexts(string authCookie, CancellationToken cancellationToken)
    {
        var existingTexts = await textRepository.GetAllBySourceType(GenerationSourceType.Nhk, cancellationToken);
        var articleUrlsByDate = await nhkClient.GetArticleUrls(authCookie, cancellationToken);
        articleUrlsByDate = articleUrlsByDate.Where(url => existingTexts.All(t => t.Url != url)).ToArray();
        
        foreach (var url in articleUrlsByDate)
        {
            var (title, content) = await nhkClient.ParseHtml(url, authCookie, cancellationToken);
            var text = new Text(GenerationSourceType.Nhk, title, content, url);
            await textRepository.Insert(text, cancellationToken);
        }
    }
}