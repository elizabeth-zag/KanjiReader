import * as React from 'react';
import { AppBar, Box, Toolbar, Typography, Button } from '@mui/material';
import ProfileMenu from './ProfileMenu';
import './UpperMenuBar.css';

interface UpperMenuBarProps {
  onShowKanji?: () => void;
  onShowReadings?: () => void;
  onLogout: () => void;
}

export default function UpperMenuBar({ onShowKanji, onShowReadings, onLogout }: UpperMenuBarProps) {
  return (
    <Box className="uppermenubar-outer">
      <AppBar position="static">
        <Toolbar className="uppermenubar-toolbar">
          <img
              src="/logo.png" 
              alt="Kanji Reader Logo" 
              className="uppermenubar-logo"
            />
          <Box className="uppermenubar-title-container">
            <Typography
              variant="h3"
              className="uppermenubar-title"
            >
              Kanji Reader
            </Typography>
          </Box>
          <Box className="uppermenubar-center">
            <Button
              variant="outlined"
              onClick={onShowKanji}
              className="kanjimenu-button"
            >
              Kanji
            </Button>
            <Button
              variant="outlined"
              onClick={onShowReadings}
              className="readingsmenu-button"
            >
              Readings
            </Button>
          </Box>
          <ProfileMenu onLogout={onLogout} />
        </Toolbar>
      </AppBar>
    </Box>
  );
}
