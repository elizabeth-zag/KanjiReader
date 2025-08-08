import React from 'react';
import { Box, Typography, Checkbox, FormControlLabel } from '@mui/material';
import './KanjiListsSelection.css';

interface KanjiList {
  kanjiList: string;
  description: string;
}

interface KanjiListsSelectionProps {
  kanjiLists: KanjiList[];
  selectedLists: string[];
  onListToggle: (listName: string) => void;
}

export default function KanjiListsSelection({ 
  kanjiLists, 
  selectedLists, 
  onListToggle 
}: KanjiListsSelectionProps) {
  return (
    <Box className="kanji-lists-section">
      <Typography variant="h6" className="kanji-lists-section-title">
        Select Kanji Lists ({kanjiLists.length} available)
      </Typography>
      <Box className="kanji-lists">
        {kanjiLists.map((list) => (
          <FormControlLabel
            key={list.kanjiList}
            control={
              <Checkbox
                checked={selectedLists.includes(list.kanjiList)}
                onChange={() => onListToggle(list.kanjiList)}
                className="kanji-lists-checkbox"
              />
            }
            label={
              <Box>
                <Typography variant="body1" className="kanji-lists-name">
                  {list.kanjiList}
                </Typography>
                <Typography variant="body2" className="kanji-lists-description">
                  {list.description}
                </Typography>
              </Box>
            }
            className="kanji-lists-item"
          />
        ))}
      </Box>
    </Box>
  );
} 