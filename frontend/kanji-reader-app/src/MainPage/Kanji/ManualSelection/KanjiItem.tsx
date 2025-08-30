import React from "react";
import { FormControlLabel, Checkbox } from "@mui/material";
import "./KanjiItem.css";

interface KanjiItemProps {
  kanji: string;
  isChecked: boolean;
  onToggle: (kanji: string) => void;
}

const KanjiItem = React.memo(function KanjiItem({
  kanji,
  isChecked,
  onToggle,
}: KanjiItemProps) {
  return (
    <FormControlLabel
      control={
        <Checkbox
          checked={isChecked}
          onChange={() => onToggle(kanji)}
          className="individual-kanji-checkbox"
        />
      }
      label={kanji}
      className="individual-kanji-item"
    />
  );
});

export default KanjiItem;
