using KanjiReader.Domain.DomainObjects.EventData;

namespace KanjiReader.Domain.GenerationRules;

public class WatanocRulesService : IGenerationRulesService<WatanocParsingData>
{
    private static readonly OrderedDictionary<string, int> WatanocCategoryPages = new()
    {
        { "japan-fun", 21 },
        { "japan-news", 8 },
        { "simplejapanese", 5 }
    };

    public WatanocParsingData GetNextState(WatanocParsingData? data)
    {
        if (data == null)
        {
            return CreateNewState();
        }
        var maxCategoryPage = WatanocCategoryPages[data.Category];
        if (data.PageNumber < maxCategoryPage)
        {
            return new WatanocParsingData
            {
                Category = data.Category,
                PageNumber = data.PageNumber + 1
            };
        }
        
        var currentIndex = WatanocCategoryPages.IndexOf(data.Category);
        var newIndex = currentIndex < WatanocCategoryPages.Count ? currentIndex + 1 : 0;

        return new WatanocParsingData
        {
            Category = WatanocCategoryPages.GetAt(newIndex).Key,
            PageNumber = 1
        };
    }

    private WatanocParsingData CreateNewState()
    {
        return new WatanocParsingData
        {
            Category = WatanocCategoryPages.First().Key,
            PageNumber = 1,
        };
    }
}