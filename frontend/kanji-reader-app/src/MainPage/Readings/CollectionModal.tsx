import { useState, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  FormControlLabel,
  Checkbox,
  Tooltip,
  IconButton,
} from "@mui/material";

import InfoIcon from "@mui/icons-material/Info";
import {
  type GenerationSource,
  getGenerationSources,
  startCollecting,
} from "../../ApiCalls/texts";
import "./CollectionModal.css";
import { getErrorMessage } from "../../Common/utils";

interface CollectionModalProps {
  open: boolean;
  onClose: () => void;
  onStartCollecting: () => void;
  openErrorSnackbar: (message: string) => void;
}

export default function CollectionModal({
  open,
  onClose,
  onStartCollecting,
  openErrorSnackbar,
}: CollectionModalProps) {
  const [availableSources, setAvailableSources] = useState<GenerationSource[]>(
    []
  );
  const [selectedSources, setSelectedSources] = useState<
    Record<string, boolean>
  >({});
  const [isCollecting, setIsCollecting] = useState(false);



  const loadGenerationSources = async () => {
    const response = await getGenerationSources();
    const sources = response.sources;
    setAvailableSources(sources);
    const initialSelection: Record<string, boolean> = {};
    sources.forEach((source) => {
      initialSelection[source.value] = true;
    });
    setSelectedSources(initialSelection);
  };

  const handleStartCollecting = async () => {
    setIsCollecting(true);
    try {
      const selectedSourceValues = Object.entries(selectedSources)
        .filter(([_, isSelected]) => isSelected)
        .map(([sourceValue, _]) => sourceValue);

      await startCollecting(selectedSourceValues);
      onStartCollecting();
    } catch (error) {
      openErrorSnackbar(getErrorMessage(error));
    } finally {
      setIsCollecting(false);
      onClose();
    }
  };

  const handleSourceToggle = (sourceValue: string) => {
    setSelectedSources((prev) => ({
      ...prev,
      [sourceValue]: !prev[sourceValue],
    }));
  };

  const hasSelectedSources = Object.values(selectedSources).some(Boolean);

  useEffect(() => {
    if (open) {
      try {
        loadGenerationSources();
      } catch (error) {
        openErrorSnackbar(getErrorMessage(error));
      }
    }
  }, [open]);

  return (
    <Dialog
      open={open && availableSources.length > 0}
      onClose={onClose}
      maxWidth="xs"
      fullWidth
    >
      <DialogTitle className="modal-title">Collection Settings</DialogTitle>
      <DialogContent>
        <Typography variant="body2" className="modal-description">
          Select the sources you want to use for text generation:
        </Typography>

        <Box className="modal-source-checkboxes">
          {availableSources.map((source) => (
            <FormControlLabel
              key={source.value}
              control={
                <Checkbox
                  checked={selectedSources[source.value]}
                  onChange={() => handleSourceToggle(source.value)}
                  color="primary"
                />
              }
              label={
                <Box display="flex" alignItems="center">
                  {source.name}
                  <Tooltip
                    className="source-tooltip-button"
                    title={source.description}
                    placement="top"
                    slotProps={{
                      popper: {
                        modifiers: [
                          {
                            name: "offset",
                            options: {
                              offset: [0, -14],
                            },
                          },
                        ],
                      },
                    }}
                  >
                    <IconButton size="small">
                      <InfoIcon
                        className="source-tooltip-icon"
                        fontSize="small"
                      />
                    </IconButton>
                  </Tooltip>
                </Box>
              }
            />
          ))}
        </Box>
      </DialogContent>
      <DialogActions className="modal-actions">
        <Button
          onClick={onClose}
          color="inherit"
          className="modal-cancel-button"
        >
          Cancel
        </Button>
        <Button
          onClick={handleStartCollecting}
          disabled={!hasSelectedSources || isCollecting}
          variant="contained"
          className="modal-start-button"
        >
          {isCollecting ? "Collecting..." : "Start Collecting"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
