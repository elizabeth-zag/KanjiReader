import { useState } from "react";
import { Box, Typography, Button } from "@mui/material";
import "./KanjiSetup.css";
import ManualKanjiSelection from "./ManualSelection/ManualKanjiSelection";
import WaniKaniTokenSet from "./WaniKani/WaniKaniTokenSet";

export default function KanjiSetup({ onShowKanji }: { onShowKanji: () => {} }) {
  const [showTokenInput, setShowTokenInput] = useState(false);
  const [showManualSelection, setShowManualSelection] = useState(false);

  const handleSetWaniKaniToken = () => {
    setShowTokenInput(true);
    setShowManualSelection(false);
  };

  const handleManualSelection = () => {
    setShowManualSelection(true);
    setShowTokenInput(false);
  };

  return (
    <Box className="kanjisetup-container">
      <Box className="kanjisetup-buttons">
        <Button
          variant="contained"
          onClick={handleSetWaniKaniToken}
          className="kanjisetup-button"
        >
          Set WaniKani token
        </Button>
        <Typography className="kanjisetup-or">or</Typography>
        <Button
          variant="contained"
          onClick={handleManualSelection}
          className="kanjisetup-button"
        >
          Manually select kanji
        </Button>
      </Box>
      {showTokenInput && <WaniKaniTokenSet onTokenSave={onShowKanji} />}
      {showManualSelection && (
        <ManualKanjiSelection onSelectionSave={onShowKanji} />
      )}
    </Box>
  );
}
