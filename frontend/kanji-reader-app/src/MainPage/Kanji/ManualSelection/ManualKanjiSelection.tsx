import React, { useState, useEffect, useRef } from 'react';
import { Box, Typography, Button } from '@mui/material';
import { getKanjiForManualSelection, getKanjiLists, saveSelectedKanji } from '../../../ApiCalls/kanji';
import IndividualKanjiSelection from './IndividualKanjiSelection';
import KanjiListsSelection from './KanjiListsSelection';
import './ManualKanjiSelection.css';

interface ManualKanjiSelectionProps {
  onSelectionSave: () => {},
  existingUserKanji?: string[],
  onLoadingComplete?: () => void,
}

export default function ManualKanjiSelection( { 
  onSelectionSave, 
  existingUserKanji,
  onLoadingComplete
}: ManualKanjiSelectionProps) {
  const [availableKanji, setAvailableKanji] = useState<string[]>([]);
  const [kanjiLists, setKanjiLists] = useState<{ kanjiList: string; description: string; }[]>([]);
  const [selectedKanji, setSelectedKanji] = useState<string[]>(existingUserKanji || []);
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
        onLoadingComplete && onLoadingComplete();
      }
    };

    fetchData();
  }, [onLoadingComplete]);

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

  const handleSaveSelection = async () => {
    await saveSelectedKanji(selectedKanjiRef.current, selectedLists); // todo: handle errors
    onSelectionSave && onSelectionSave()
  };

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
