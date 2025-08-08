import * as React from 'react';
import Button from '@mui/material/Button';
import './KanjiMenu.css';
import { getUserKanji } from '../../ApiCalls/kanji';

export default function KanjiMenu({ onShowKanji }: { onShowKanji?: () => void }) {
  return (
    <Button
      variant="outlined"
      onClick={onShowKanji}
      className="kanjimenu-button"
    >
      Kanji
    </Button>
  );
}
