import * as React from 'react';
import { AppBar, Box, Toolbar, Typography } from '@mui/material';
import ProfileMenu from './ProfileMenu';
import KanjiMenu from './Kanji/KanjiMenu';
import ReadingsMenu from './ReadingsMenu';
import './UpperMenuBar.css';

export default function UpperMenuBar({ onShowKanji, onShowReadings }:
   { onShowKanji?: () => void, onShowReadings?: () => void }) {
  return (
    <Box className="uppermenubar-outer">
      <AppBar position="static">
        <Toolbar style={{ justifyContent: 'space-between' }}>
          <Box style={{ display: 'flex', alignItems: 'center' }}>
            <Typography
              variant="h3"
              className="uppermenubar-title"
            >
              Kanji Reader
            </Typography>
          </Box>
          <Box className="uppermenubar-center">
            <KanjiMenu onShowKanji={onShowKanji} />
            <ReadingsMenu onShowReadings={onShowReadings} />
          </Box>
          <ProfileMenu />
        </Toolbar>
      </AppBar>
    </Box>
  );
}
