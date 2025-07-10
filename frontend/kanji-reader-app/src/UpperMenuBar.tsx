import * as React from 'react';
import { AppBar, Box, Toolbar, Typography } from '@mui/material';
import ProfileMenu from './ProfileMenu';
import KanjiMenu from './KanjiMenu';
import ReadingsMenu from './ReadingsMenu';

export default function UpperMenuBar({ onShowKanji, onShowReadings }:
   { onShowKanji?: () => void, onShowReadings?: () => void }) {
  return (
    <Box sx={{ width: '100vw', position: 'relative', left: '50%', right: '50%', marginLeft: '-50vw', marginRight: '-50vw' }}>
      <AppBar position="static" sx={{ width: '100%' }}>
        <Toolbar sx={{ justifyContent: 'space-between' }}>
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            <Typography
              variant="h3"
              sx={{ ml: 2 }}
            >
              Kanji app
            </Typography>
          </Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 15, flex: 1, justifyContent: 'center' }}>
            <KanjiMenu onShowKanji={onShowKanji} />
            <ReadingsMenu onShowReadings={onShowReadings} />
          </Box>
          <ProfileMenu />
        </Toolbar>
      </AppBar>
    </Box>
  );
}
