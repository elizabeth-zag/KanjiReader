﻿namespace KanjiReader.Domain.DomainObjects;

public enum EventType
{
    Unspecified,
    StartGenerating,
    WatanocParsing,
    NhkParsing,
    SatoriReaderParsing,
    GoogleAiGeneration
}