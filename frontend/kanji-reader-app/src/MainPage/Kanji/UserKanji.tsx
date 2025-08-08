import React, { useState } from 'react';
import { Box, Typography, ToggleButtonGroup, ToggleButton, Paper } from '@mui/material';
import { updateKanjiSourceType } from '../../ApiCalls/login.ts';
import './UserKanji.css';

export default function UserKanji({ userKanji, kanjiCount, kanjiSourceType }: 
  { userKanji: string[], kanjiCount: number, kanjiSourceType: string }) {
    const [sourceType, setSourceType] = useState(kanjiSourceType.toLowerCase());

    const handleChange = async (event: React.MouseEvent<HTMLElement>, newSourceType: string) => {
      if (newSourceType !== null) {
        setSourceType(newSourceType.toLowerCase());
        await updateKanjiSourceType(newSourceType);
      }
    };

    return (
      <Box className="userkanji-container">
        <Box className="userkanji-upper">
          <Typography className="userkanji-yourkanji" variant="h3">Your kanji</Typography>
          <Paper className="userkanji-kanjisource-container" variant="outlined" elevation={14}>
            <Typography variant="button" className="userkanji-kanjisource-text">Kanji source</Typography>
            <ToggleButtonGroup
              value={sourceType}
              exclusive
              onChange={handleChange}
              aria-label="Platform"
              className="userkanji-kanjisource-buttons"
              size="medium"
              >
              <ToggleButton value="wanikani" disabled={sourceType === "wanikani"}>WaniKani</ToggleButton>
              <ToggleButton value="manual" disabled={sourceType === "manual"}>Manual selection</ToggleButton>
            </ToggleButtonGroup>
          </Paper>
        </Box>
        <Box className="userkanji-grid">
          {userKanji.map((kanji, idx) => (
            <Typography key={idx} variant="h4">{kanji}</Typography>
            ))}
        </Box>
    </Box>
  );
}
