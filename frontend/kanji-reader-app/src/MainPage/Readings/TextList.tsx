import {
  Box,
  Typography,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
  IconButton,
  Tooltip,
  Button,
  Checkbox,
  FormControlLabel,
  Fade,
} from "@mui/material";
import SortIcon from "@mui/icons-material/Sort";
import TrendingUpIcon from "@mui/icons-material/TrendingUp";
import TrendingDownIcon from "@mui/icons-material/TrendingDown";
import DeleteIcon from "@mui/icons-material/Delete";
import RadioButtonCheckedIcon from "@mui/icons-material/RadioButtonChecked";
import { type ProcessingResult } from "../../ApiCalls/texts";
import TextItem from "./TextItem";
import Loader from "../../Common/Loader";
import "./TextList.css";

interface TextListProps {
  texts: ProcessingResult[];
  allTexts: ProcessingResult[];
  isLoading: boolean;
  hasLoadedTexts: boolean;
  onTextClick: (text: ProcessingResult) => void;
  sourceTypes: string[];
  selectedSourceType: string;
  onSourceTypeChange: (sourceType: string) => void;
  sortField: string;
  sortDirection: string;
  onSortChange: (field: string, direction: string) => void;
  isDeleteMode: boolean;
  selectedTextIds: string[];
  onToggleDeleteMode: () => void;
  onTextSelection: (textId: string, isSelected: boolean) => void;
  onSelectAll: (isSelected: boolean) => void;
  onDeleteTexts: () => void;
  isReceivingUpdates: boolean;
}

export default function TextList({
  texts,
  allTexts,
  isLoading,
  hasLoadedTexts,
  onTextClick,
  sourceTypes,
  selectedSourceType,
  onSourceTypeChange,
  sortField,
  sortDirection,
  onSortChange,
  isDeleteMode,
  selectedTextIds,
  onToggleDeleteMode,
  onTextSelection,
  onSelectAll,
  onDeleteTexts,
  isReceivingUpdates,
}: TextListProps) {
  if (isLoading) {
    return <Loader />;
  }

  if (hasLoadedTexts && texts.length === 0) {
    return (
      <Box className="no-texts-message">
        <Typography variant="h5" className="no-texts-title">
          {selectedSourceType === "all"
            ? "You don't have any texts yet..."
            : `No texts found for source type: ${selectedSourceType}`}
        </Typography>
        <Typography variant="body1" className="no-texts-subtitle">
          {selectedSourceType === "all"
            ? "Start collecting by clicking the button on the left"
            : "Try selecting a different source type or start collecting new texts"}
        </Typography>
      </Box>
    );
  }

  const getSourceTypeCount = (sourceType: string): number => {
    if (sourceType === "all") {
      return allTexts.length;
    }
    return allTexts.filter(text => text.sourceType === sourceType).length;
  };

  const handleSortChange = (field: string) => {
    let newDirection = "asc";
    if (sortField === field && sortDirection === "asc") {
      newDirection = "desc";
    } else if (sortField === field && sortDirection === "desc") {
      newDirection = "asc";
    }
    onSortChange(field, newDirection);
  };

  const getSortIcon = (field: string) => {
    if (sortField !== field) {
      return <SortIcon />;
    }
    return sortDirection === "asc" ? <TrendingUpIcon /> : <TrendingDownIcon />;
  };

  const getSortTooltip = (field: string) => {
    if (sortField !== field) {
      return `Sort by ${field}`;
    }
    return sortDirection === "asc" 
      ? `${field} (ascending) - click to reverse` 
      : `${field} (descending) - click to reverse`;
  };

  return (
    <Box className="texts-content">
      <Box className="texts-header">
        <Typography variant="h5" className="texts-title">
          Available Texts ({texts.length})
        </Typography>

        <Box className="texts-controls">
          <Box className="delete-controls">
            <Button
              variant={isDeleteMode ? "contained" : "outlined"}
              color={isDeleteMode ? "error" : "primary"}
              startIcon={<DeleteIcon />}
              onClick={onToggleDeleteMode}
              size="small"
              className="delete-mode-button"
            >
              {isDeleteMode ? "Cancel Delete" : "Delete Texts"}
            </Button>
          </Box>

          <Box className="sorting-controls">
            <Tooltip title={getSortTooltip("date")}>
              <IconButton
                onClick={() => handleSortChange("date")}
                className={`sort-button ${sortField === "date" ? "active" : ""}`}
                size="small"
              >
                {getSortIcon("date")}
              </IconButton>
            </Tooltip>
            <Typography variant="caption" className="sort-label">
              Date
            </Typography>
            
            <Tooltip title={getSortTooltip("difficulty")}>
              <IconButton
                onClick={() => handleSortChange("difficulty")}
                className={`sort-button ${sortField === "difficulty" ? "active" : ""}`}
                size="small"
              >
                {getSortIcon("difficulty")}
              </IconButton>
            </Tooltip>
            <Typography variant="caption" className="sort-label">
              Difficulty
            </Typography>
          </Box>

          <Box className="texts-filter">
            <FormControl size="small" className="source-type-filter">
              <InputLabel
                className="source-type-filter-label"
                id="source-type-filter-label"
              >
                Source Type
              </InputLabel>
              <Select
                labelId="source-type-filter-label"
                value={selectedSourceType}
                label="Source Type"
                onChange={(e) => onSourceTypeChange(e.target.value)}
                MenuProps={{
                  PaperProps: {
                    className: "source-type-filter-selection",
                  },
                }}
              >
                <MenuItem value="all">
                  <Box className="filter-option">
                    <Typography variant="body2">All Sources</Typography>
                    <Chip
                      label={getSourceTypeCount("all")}
                      size="small"
                      variant="outlined"
                      className="filter-count-chip"
                    />
                  </Box>
                </MenuItem>
                {sourceTypes.map((sourceType) => (
                  <MenuItem key={sourceType} value={sourceType}>
                    <Box className="filter-option">
                      <Typography variant="body2">{sourceType}</Typography>
                      <Chip
                        label={getSourceTypeCount(sourceType)}
                        size="small"
                        variant="outlined"
                        className="filter-count-chip"
                      />
                    </Box>
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>
        </Box>
      </Box>

      {isDeleteMode && (
        <Box className="select-all-container">
          <FormControlLabel
            control={
              <Checkbox
                checked={texts.length > 0 && texts.every(text => selectedTextIds.includes(text.id))}
                indeterminate={texts.some(text => selectedTextIds.includes(text.id)) && !texts.every(text => selectedTextIds.includes(text.id))}
                onChange={(e) => onSelectAll(e.target.checked)}
                color="primary"
                className=""
              />
            }
            label="Select All"
            className="select-all-label"
          />
          {texts.length > 0 && (
            <Button
              variant="contained"
              color="error"
              startIcon={<DeleteIcon />}
              onClick={onDeleteTexts}
              size="small"
              disabled={selectedTextIds.length === 0}
              className="delete-selected-button"
            >
              Delete Selected ({selectedTextIds.length})
            </Button>
          )}
        </Box>
      )}

      <Fade in={isReceivingUpdates}>
        <Box className="realtime-indicator">
          <RadioButtonCheckedIcon className="realtime-icon" />
          <Typography variant="body2" className="realtime-text">
            Receiving new texts in real-time...
          </Typography>
        </Box>
      </Fade>

      <Box className="texts-list">
        {texts.map((text, index) => (
          <TextItem 
            key={index} 
            text={text} 
            onClick={onTextClick}
            isDeleteMode={isDeleteMode}
            isSelected={selectedTextIds.includes(text.id)}
            onSelectionChange={(isSelected) => onTextSelection(text.id, isSelected)}
          />
        ))}
      </Box>
    </Box>
  );
}
