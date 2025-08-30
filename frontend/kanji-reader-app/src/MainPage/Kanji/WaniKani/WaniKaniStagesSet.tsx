import { useEffect, useState } from "react";
import {
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  FormControl,
  FormControlLabel,
  Checkbox,
} from "@mui/material";
import { setWaniKaniStages, getWaniKaniStages } from "../../../ApiCalls/login";
import "./WaniKaniStagesSet.css";

interface WaniKaniStagesProps {
  showWaniKaniStages: boolean;
  onShowKanji?: () => void;
  openErrorSnackbar: (message: string) => void;
  openSuccessSnackbar: (message: string) => void;
  onDialogClose: () => void;
}

export default function WaniKaniStagesSet({
  showWaniKaniStages,
  onShowKanji,
  openErrorSnackbar,
  openSuccessSnackbar,
  onDialogClose,
}: WaniKaniStagesProps) {
  const [isSavingStages, setIsSavingStages] = useState(false);
  const [selectedStages, setSelectedStages] = useState<string[]>([]);
  const [selectedStagesLoaded, setSelectedStagesLoaded] = useState<boolean>(false);

  const handleStageToggle = (stage: string) => {
    setSelectedStages((prev) =>
      prev.includes(stage) ? prev.filter((s) => s !== stage) : [...prev, stage]
    );
  };

  const handleSaveStages = async () => {
    if (selectedStages.length === 0) {
      openErrorSnackbar("Please select at least one stage.");
      return;
    }

    setIsSavingStages(true);
    try {
      await setWaniKaniStages(selectedStages);
      onDialogClose();
      openSuccessSnackbar("WaniKani stages updated successfully!");

      if (onShowKanji) {
        onShowKanji();
      }
    } catch (error) {
      openErrorSnackbar("Failed to update WaniKani stages. Please try again.");
    } finally {
      setIsSavingStages(false);
    }
  };

  const handleCloseWaniKaniStages = () => {
    onDialogClose();
    setSelectedStages([]);
    setSelectedStagesLoaded(false);
  };

  const loadWaniKaniStages = async () => {
    const existingStages = await getWaniKaniStages();
    setSelectedStages(existingStages.stages);
    setSelectedStagesLoaded(true);
  };

  useEffect(() => {
    if (showWaniKaniStages) {
      try {
        loadWaniKaniStages();
      } catch (error) {
        openErrorSnackbar("Failed to load WaniKani stages. Please try again.");
      }
    }
  }, [showWaniKaniStages]);

  return (
    <Dialog
      open={showWaniKaniStages && selectedStagesLoaded}
      onClose={handleCloseWaniKaniStages}
      maxWidth="sm"
      fullWidth
      className="wanikani-stages-dialog"
    >
      <DialogTitle>Select WaniKani Stages</DialogTitle>
      <DialogContent>
        <Typography variant="body2" sx={{ mb: 2 }}>
          Choose which WaniKani stages you want to include in your kanji
          collection (you can select multiple):
        </Typography>
        <FormControl component="fieldset">
          {["Apprentice", "Guru", "Master", "Enlightened", "Burned"].map(
            (stage) => (
              <FormControlLabel
                key={stage}
                control={
                  <Checkbox
                    id={`stage-${stage.toLowerCase()}`}
                    name={`stage-${stage.toLowerCase()}`}
                    checked={selectedStages.includes(stage)}
                    onChange={() => handleStageToggle(stage)}
                    className="wanikani-stages-checkbox"
                  />
                }
                label={stage}
              />
            )
          )}
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button
          className="save-stages-cancel-button"
          onClick={handleCloseWaniKaniStages}
          disabled={isSavingStages}
        >
          Cancel
        </Button>
        <Button
          onClick={handleSaveStages}
          variant="contained"
          className="save-stages-button"
          disabled={isSavingStages || selectedStages.length === 0}
        >
          {isSavingStages ? "Saving..." : "Save Stages"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
