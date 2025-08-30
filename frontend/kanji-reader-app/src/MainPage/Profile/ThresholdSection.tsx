import { useState } from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  Paper,
  Divider,
  IconButton,
  Tooltip,
  InputAdornment,
} from "@mui/material";
import {
  Edit as EditIcon,
  Save as SaveIcon,
  Cancel as CancelIcon,
  Info as InfoIcon,
} from "@mui/icons-material";
import { setUserThreshold, getUserThreshold } from "../../ApiCalls/login";
import "./ThresholdSection.css";
import { getErrorMessage } from "../../Common/utils";

interface ThresholdSectionProps {
  threshold: number;
  isThresholdUserSet: boolean;
  onThresholdUpdate: (newThreshold: number, isUserSet: boolean) => void;
  isSaving: boolean;
  onError: (message: string) => void;
  onSuccess: (message: string) => void;
  setIsSaving: (saving: boolean) => void;
}

export default function ThresholdSection({
  threshold,
  isThresholdUserSet,
  onThresholdUpdate,
  isSaving,
  onError,
  onSuccess,
  setIsSaving,
}: ThresholdSectionProps) {
  const [isEditingThreshold, setIsEditingThreshold] = useState(false);
  const [newThreshold, setNewThreshold] = useState<number>(threshold);

  const decimalToPercentage = (decimal: number): number => {
    return Math.round(decimal * 100);
  };

  const percentageToDecimal = (percentage: number): number => {
    return percentage / 100;
  };

  const handleEditThreshold = () => {
    setIsEditingThreshold(true);
    setNewThreshold(threshold);
  };

  const handleSaveThreshold = async () => {
    const percentageValue = decimalToPercentage(newThreshold);

    if (percentageValue < 0 || percentageValue > 100) {
      onError("Threshold must be between 0% and 100%.");
      return;
    }

    try {
      setIsSaving(true);
      await setUserThreshold(newThreshold);
      onThresholdUpdate(newThreshold, true);
      setIsEditingThreshold(false);
      onSuccess("Threshold updated successfully!");
    } catch (error) {
      onError(getErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancelThresholdEdit = () => {
    setIsEditingThreshold(false);
    setNewThreshold(threshold);
  };

  const handleResetThreshold = async () => {
    try {
      setIsSaving(true);
      await setUserThreshold(null);
      const thresholdResult = await getUserThreshold();
      if (thresholdResult?.threshold !== undefined) {
        onThresholdUpdate(thresholdResult.threshold, thresholdResult.isUserSet);
      }
      onSuccess("Threshold reset to default successfully!");
    } catch (error) {
      onError(getErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <Paper className="profile-section" elevation={2}>
      <Box className="section-header">
        <Typography variant="h6" className="section-title">
          User Threshold
        </Typography>
        <Tooltip
          title="Threshold is a maximum percent of unknown kanji out of all kanji that's allowed in the text"
          arrow
          placement="top"
        >
          <IconButton size="small" className="info-icon">
            <InfoIcon />
          </IconButton>
        </Tooltip>
      </Box>
      <Divider className="section-divider" />

      {isEditingThreshold ? (
        <Box className="edit-section">
          <TextField
            fullWidth
            label="Threshold (%)"
            value={decimalToPercentage(newThreshold)}
            onChange={(e) =>
              setNewThreshold(percentageToDecimal(Number(e.target.value)))
            }
            type="number"
            variant="outlined"
            className="profile-input"
            inputProps={{ min: 0, max: 100 }}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">%</InputAdornment>
              ),
            }}
          />
          <Box className="edit-actions">
            <Button
              variant="contained"
              onClick={handleSaveThreshold}
              disabled={isSaving}
              startIcon={<SaveIcon />}
              className="save-button"
            >
              Save
            </Button>
            <Button
              variant="outlined"
              onClick={handleCancelThresholdEdit}
              disabled={isSaving}
              startIcon={<CancelIcon />}
              className="cancel-button"
            >
              Cancel
            </Button>
            {isThresholdUserSet && (
              <Button
                variant="outlined"
                color="warning"
                onClick={handleResetThreshold}
                disabled={isSaving}
                className="reset-button"
              >
                Reset to Default
              </Button>
            )}
          </Box>
        </Box>
      ) : (
        <Box className="display-section">
          <Box className="threshold-info">
            <Typography variant="body1" className="field-value">
              {decimalToPercentage(threshold)}%
            </Typography>
            <Typography variant="caption" className="threshold-source">
              {isThresholdUserSet ? "User-set" : "Automatic"}
            </Typography>
          </Box>
          <Box className="field-actions">
            <Button
              variant="outlined"
              onClick={handleEditThreshold}
              startIcon={<EditIcon />}
              className="edit-button"
            >
              Change Threshold
            </Button>
            {isThresholdUserSet && (
              <Button
                variant="outlined"
                color="warning"
                onClick={handleResetThreshold}
                disabled={isSaving}
                className="reset-button"
              >
                Reset to Default
              </Button>
            )}
          </Box>
        </Box>
      )}
    </Paper>
  );
}
