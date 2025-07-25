﻿using KanjiReader.Domain.DomainObjects.KanjiLists;

namespace KanjiReader.Domain.Common;

public static class KanjiListsDescription
{
    public static readonly Dictionary<KanjiListType, string> KanjiListDescriptions = new()
    {
        { KanjiListType.JlptN5, "Japanese Language Proficiency Test N5" },
        { KanjiListType.JlptN4, "Japanese Language Proficiency Test N4" },
        { KanjiListType.JlptN3, "Japanese Language Proficiency Test N3" },
        { KanjiListType.JlptN2, "Japanese Language Proficiency Test N2" },
        { KanjiListType.JlptN1, "Japanese Language Proficiency Test N1" },
        { KanjiListType.Kyouiku, "List of all Kyōiku kanji" },
        { KanjiListType.Grade1, "List of Grade 1 Kyōiku kanji" },
        { KanjiListType.Grade2, "List of Grade 2 Kyōiku kanji" },
        { KanjiListType.Grade3, "List of Grade 3 Kyōiku kanji" },
        { KanjiListType.Grade4, "List of Grade 4 Kyōiku kanji" },
        { KanjiListType.Grade5, "List of Grade 5 Kyōiku kanji" },
        { KanjiListType.Grade6, "List of Grade 6 Kyōiku kanji" },
        { KanjiListType.Grade8, "List of Jōyō kanji excluding Kyōiku kanji" },
    };
}