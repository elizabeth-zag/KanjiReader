import React, { useState, useEffect, useCallback, useMemo, useRef } from 'react';
import { Box, Typography, Button } from '@mui/material';
import { getKanjiForManualSelection, getKanjiLists } from '../../../ApiCalls/kanji';
import type { KanjiForSelectionResponse, KanjiListsResponse } from '../../../ApiCalls/kanji';
import IndividualKanjiSelection from './IndividualKanjiSelection';
import KanjiListsSelection from './KanjiListsSelection';
import './ManualKanjiSelection.css';

export default function ManualKanjiSelection() {
  const [availableKanji, setAvailableKanji] = useState<string[]>([]);
  const [kanjiLists, setKanjiLists] = useState<{ kanjiList: string; description: string; }[]>([]);
  const [selectedKanji, setSelectedKanji] = useState<string[]>([]);
  const [selectedLists, setSelectedLists] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const selectedKanjiRef = useRef(selectedKanji);

  useEffect(() => {
    selectedKanjiRef.current = selectedKanji;
  }, [selectedKanji]);

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      try {
        const [kanjiResult, listsResult] = await Promise.all([
          getKanjiForManualSelection(),
          getKanjiLists()
        ]);
        
        setAvailableKanji(kanjiResult?.kanji ?? []);
        setKanjiLists(listsResult?.kanjiLists ?? []);
      } catch (error) {
        console.error('Error fetching kanji data:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleKanjiToggle = (kanji: string) => {
    const isSelected = selectedKanjiRef.current.includes(kanji);
    setSelectedKanji(prev => {      
      const result = isSelected 
        ? prev.filter(k => k !== kanji)
        : [...prev, kanji];
      return result;
    });
  }

  const handleListToggle = (listName: string) => {
    setSelectedLists(prev => 
      prev.includes(listName) 
        ? prev.filter(l => l !== listName)
        : [...prev, listName]
    );
  };

  const handleSaveSelection = () => {
    console.log('Selected kanji:', selectedKanji);
    console.log('Selected lists:', selectedLists);
    // TODO: Implement save functionality
  };

  if (isLoading) {
    return (
      <Box className="manual-kanji-loading">
        <Typography variant="h6">Loading kanji options...</Typography>
      </Box>
    );
  }

  return (
    <Box className="manual-kanji-container">
      <Typography variant="h5" className="manual-kanji-title">
        Manual Kanji Selection
      </Typography>
      <KanjiListsSelection
        kanjiLists={kanjiLists}
        selectedLists={selectedLists}
        onListToggle={handleListToggle}
      />
      <IndividualKanjiSelection
        availableKanji={availableKanji}
        selectedKanji={selectedKanji}
        onKanjiToggle={handleKanjiToggle}
      />
      <Box className="manual-kanji-actions">
        <Button
          variant="contained"
          onClick={handleSaveSelection}
          className="manual-kanji-save-button"
        >
          Save Selection ({selectedKanji.length} kanji, {selectedLists.length} lists)
        </Button>
      </Box>
    </Box>
  );
}
