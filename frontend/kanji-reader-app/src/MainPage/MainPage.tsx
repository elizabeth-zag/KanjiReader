import React, { useState } from 'react';
import UpperMenuBar from './UpperMenuBar';
import { Box, Typography } from '@mui/material';
import './MainPage.css';
import { getUserKanji } from '../ApiCalls/kanji';
import UserKanji from './Kanji/UserKanji.tsx';
import KanjiSetup from './Kanji/KanjiSetup.tsx';

function MainPage({ userName }: { userName: string }) {
  const [showKanji, setShowKanji] = useState(false);
  const [showReadings, setShowReadings] = useState(false);
  const [userKanji, setUserKanji] = useState<string[] | null>(null);
  const [kanjiCount, setKanjiCount] = useState<number | null>(null);
  const [kanjiSourceType, setKanjiSourceType] = useState<string | null>(null);
  const [isLoadingKanji, setIsLoadingKanji] = useState(false);

  const handleShowKanji = async () => {
    setShowKanji(true);
    setShowReadings(false);
    setIsLoadingKanji(true);
    const result = await getUserKanji();
    setUserKanji(result?.kanji ?? []);
    setKanjiCount(result?.kanjiCount ?? 0);
    setKanjiSourceType(result?.kanjiSourceType ?? "");
    setIsLoadingKanji(false);
  };

  return (
    <Box className="mainpage-container">
      <UpperMenuBar onShowKanji={handleShowKanji} onShowReadings={() => { setShowReadings(true); setShowKanji(false); }} />
      <Box className="mainpage-centered">
        {!showKanji && !showReadings && (
          <Box>
            <Typography variant="h4" className="mainpage-welcome">
              Welcome to the Kanji Reader Main Page, {userName}!
            </Typography>
            <Typography variant="body1">
              Start exploring kanji or use the menu above.
            </Typography>
          </Box>
        )}
        {showKanji && (
          isLoadingKanji ? (
            <Typography className="mainpage-loading" variant="h5">Loading kanji...</Typography>
          ) : (
            (userKanji && userKanji.length > 0)
              ? <UserKanji userKanji={userKanji} kanjiCount={kanjiCount ?? 0} kanjiSourceType={kanjiSourceType ?? ""}/> 
              : <KanjiSetup />
          )
        )}
        {showReadings && (
          <Box style={{ marginTop: 32 }}>
            <Typography variant="h5">There will be some readings content in the future here!</Typography>
          </Box>
        )}
      </Box>
    </Box>
  );
}

export default MainPage;
