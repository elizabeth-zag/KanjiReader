import React, { useMemo, useCallback, useState } from 'react';
import { Box, Typography, Checkbox, FormControlLabel, TextField, InputAdornment, Button, Pagination } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import './IndividualKanjiSelection.css';
import KanjiItem from './KanjiItem';

interface IndividualKanjiSelectionProps {
  availableKanji: string[];
  selectedKanji: string[];
  onKanjiToggle: (kanji: string) => void;
}

export default function IndividualKanjiSelection({ 
  availableKanji, 
  selectedKanji, 
  onKanjiToggle 
}: IndividualKanjiSelectionProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 200;
  
  const selectedKanjiSet = useMemo(() => new Set(selectedKanji), [selectedKanji]);
  const isChecked = useCallback((kanji: string) => {
    return selectedKanjiSet.has(kanji);
  }, [selectedKanjiSet]);

  const filteredKanji = useMemo(() => {
    if (!searchTerm.trim()) {
      return availableKanji;
    }
    return availableKanji.filter(kanji => 
      kanji.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [availableKanji, searchTerm]);

  const totalPages = Math.ceil(filteredKanji.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;

  const displayedKanji = useMemo(() => {
    return filteredKanji.slice(startIndex, endIndex);
  }, [filteredKanji, startIndex, endIndex]);

  const handleToggle = useCallback((kanji: string) => {
    onKanjiToggle(kanji);
  }, []);

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(event.target.value);
    setCurrentPage(1);
  };

  const handlePageChange = (_: React.ChangeEvent<unknown>, page: number) => {
    setCurrentPage(page);
  };

  return (
    <Box className="individual-kanji-section">
      <Typography variant="h6" className="individual-kanji-section-title">
        Select Individual Kanji (Page {currentPage} of {totalPages}, {displayedKanji.length} of {filteredKanji.length} shown, {availableKanji.length} total)
      </Typography>
    
      <Box className="search-container">
        <TextField
          fullWidth
          variant="outlined"
          placeholder="Search kanji..."
          value={searchTerm}
          onChange={handleSearchChange}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
          className="search-input"
        />
      </Box>

      <Box className="individual-kanji-grid">
        {displayedKanji.map((kanji) => (
          <KanjiItem 
            key={kanji}
            kanji={kanji} 
            isChecked={isChecked(kanji)} 
            onToggle={handleToggle}
          />
        ))}
      </Box>
      
      {totalPages > 1 && (
        <Box className="pagination-container">
          <Pagination
            count={totalPages}
            page={currentPage}
            onChange={handlePageChange}
            color="primary"
            size="large"
            showFirstButton
            showLastButton
            className="pagination-controls"
          />
        </Box>
      )}
      
      {filteredKanji.length === 0 && searchTerm.trim() && (
        <Box className="no-results">
          <Typography variant="body2" color="textSecondary">
            No kanji found matching "{searchTerm}"
          </Typography>
        </Box>
      )}
    </Box>
  );
};