import * as React from 'react';
import Button from '@mui/material/Button';

export default function KanjiMenu({ onShowKanji }: { onShowKanji?: () => void }) {
  return (
    <Button
      color="secondary"
      variant="outlined"
      onClick={onShowKanji}
      sx={{ 
        ml: 2,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: 'transparent',
        boxShadow: 'none',
        '&:focus': { outline: 'none' },
        '&.Mui-focusVisible': { outline: 'none' },
        '&:active': { backgroundColor: 'transparent' },
        '&:hover': { backgroundColor: 'rgba(0,0,0,0.04)' }
      }}
    >
      Kanji
    </Button>
  );
}
