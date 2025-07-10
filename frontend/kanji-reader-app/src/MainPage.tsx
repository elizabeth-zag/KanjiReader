import React, { useState } from 'react';
import UpperMenuBar from './UpperMenuBar';
import { Box, Typography } from '@mui/material';

function MainPage({ userName }: { userName: string }) {
  const [showKanji, setShowKanji] = useState(false);
  const [showReadings, setShowReadings] = useState(false);

  return (
    <Box>
      <UpperMenuBar onShowKanji={() => setShowKanji(true)} onShowReadings={() => setShowReadings(true)} />
      <Box
        sx={{
          minHeight: 'calc(100vh - 64px)', // 64px is default AppBar height
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          alignItems: 'center',
          width: '100%',
        }}
      >
        {!showKanji && !showReadings && (
          <Box>
            <Typography variant="h4" sx={{ mb: 2 }}>
              Welcome to the Kanji Reader Main Page, {userName}!
            </Typography>
            <Typography variant="body1">
              Start exploring kanji or use the menu above.
            </Typography>
          </Box>
        )}
        {showKanji && (
          <Box sx={{ mt: 4 }}>
            <Typography variant="h5">There will be some kanji content in the future here!</Typography>
          </Box>
        )}
        {showReadings && (
          <Box sx={{ mt: 4 }}>
            <Typography variant="h5">There will be some readings content in the future here!</Typography>
          </Box>
        )}
      </Box>
    </Box>
  );
}

export default MainPage;
