import React from "react";
import { Paper, Typography, Box, Chip, Checkbox } from "@mui/material";
import { type ProcessingResult } from "../../ApiCalls/texts";
import {
  getRatioColor,
  getRatioLabel,
  getRelativeTime,
} from "../../Common/utils";
import "./TextItem.css";

interface TextItemProps {
  text: ProcessingResult;
  onClick: (text: ProcessingResult) => void;
  isDeleteMode: boolean;
  isSelected: boolean;
  onSelectionChange: (isSelected: boolean) => void;
}

export default function TextItem({
  text,
  onClick,
  isDeleteMode,
  isSelected,
  onSelectionChange,
}: TextItemProps) {
  const handleClick = () => {
    if (!isDeleteMode) {
      onClick(text);
    }
  };

  const handleCheckboxChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    onSelectionChange(event.target.checked);
  };

  return (
    <Paper
      className={`text-item ${isDeleteMode ? "delete-mode" : ""}`}
      elevation={1}
      onClick={handleClick}
      sx={{
        cursor: isDeleteMode ? "default" : "pointer",
        "&:hover": { elevation: isDeleteMode ? 1 : 3 },
      }}
    >
      {isDeleteMode && (
        <Box className="text-checkbox-container">
          <Checkbox
            checked={isSelected}
            onChange={handleCheckboxChange}
            color="primary"
            onClick={(e) => e.stopPropagation()}
            className="text-checkbox"
          />
        </Box>
      )}

      <Box className="text-content">
        <Typography variant="h6" className="text-title">
          {text.title}
        </Typography>

        <Box className="text-ratio-info">
          <Typography variant="body2" className="ratio-label">
            Difficulty:
          </Typography>
          <Chip
            label={`${getRatioLabel(text.ratio)} (${(text.ratio * 100).toFixed(
              1
            )}% unknown)`}
            color={getRatioColor(text.ratio) as any}
            size="small"
            className="ratio-chip"
          />
        </Box>

        {text.unknownKanji.length > 0 && (
          <Box className="text-unknown-kanji">
            <Typography variant="body2" className="unknown-kanji-preview">
              Unknown kanji: {text.unknownKanji.slice(0, 5).join(" ")}
              {text.unknownKanji.length > 5 &&
                ` +${text.unknownKanji.length - 5} more`}
            </Typography>
          </Box>
        )}

        <Typography variant="body2" className="text-content">
          {text.content.substring(0, 100)}...
        </Typography>

        <Box className="text-footer">
          <Typography variant="caption" className="text-source">
            Source: {text.sourceType}
          </Typography>

          <Typography variant="caption" className="text-create-date">
            Created: {getRelativeTime(text.createDate)}
          </Typography>
        </Box>
      </Box>
    </Paper>
  );
}
