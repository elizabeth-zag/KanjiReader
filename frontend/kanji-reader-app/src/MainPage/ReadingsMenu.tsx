import * as React from 'react';
import Button from '@mui/material/Button';
import './ReadingsMenu.css';

export default function ReadingsMenu({ onShowReadings }: { onShowReadings?: () => void }) {
  return (
    <Button
      variant="outlined"
      onClick={onShowReadings}
      className="readingsmenu-button"
    >
      Readings
    </Button>
  );
}
