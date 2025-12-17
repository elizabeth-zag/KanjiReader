import React, { useState } from "react";
import {
  Box,
  Typography,
  ToggleButtonGroup,
  ToggleButton,
  Paper,
  Button,
  Link,
  Tooltip,
} from "@mui/material";
import { tryUpdateKanjiSource, type KanjiWithData, refreshCache } from "../../ApiCalls/kanji";
import WaniKaniTokenSet from "./WaniKani/WaniKaniTokenSet";
import ManualKanjiSelection from "./ManualSelection/ManualKanjiSelection";
import Snackbar from "../../Common/Snackbar";
import Loader from "../../Common/Loader";
import WaniKaniStagesSet from "./WaniKani/WaniKaniStagesSet";
import "./UserKanji.css";

interface UserKanjiProps {
  userKanji: KanjiWithData[];
  kanjiSourceType: string;
  onShowKanji?: () => void;
}

export default function UserKanji({
  userKanji,
  kanjiSourceType,
  onShowKanji,
}: UserKanjiProps) {
  const [sourceType, setSourceType] = useState(kanjiSourceType.toLowerCase());
  const [showTokenSet, setShowTokenSet] = useState(false);
  const [showManualSelection, setShowManualSelection] = useState(false);
  const [showWaniKaniStages, setShowWaniKaniStages] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [errorSnackbar, setErrorSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({
    open: false,
    message: "",
  });
  const [successSnackbar, setSuccessSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({
    open: false,
    message: "",
  });

  const handleChange = async (
    _: React.MouseEvent<HTMLElement>,
    newSourceType: string
  ) => {
    if (newSourceType !== null) {
      setIsLoading(true);
      try {
        var result = await tryUpdateKanjiSource(newSourceType);
        if (result && onShowKanji) {
          onShowKanji();
          setSourceType(newSourceType.toLowerCase());
        }
        if (result === false && newSourceType === "wanikani") {
          setShowTokenSet(true);
        }
        if (result === false && newSourceType === "manualselection") {
          setShowManualSelection(true);
        }
      } catch (error) {
        setErrorSnackbar({
          open: true,
          message: "Failed to switch kanji source. Please try again.",
        });
      } finally {
        setIsLoading(false);
      }
    }
  };

  const handleTokenSave = async () => {
    setShowTokenSet(false);
    if (onShowKanji) {
      onShowKanji();
    }
  };

  const handleManualSave = async () => {
    setShowManualSelection(false);
    if (onShowKanji) {
      onShowKanji();
    }
  };

  const handleShowTokenSet = () => {
    setShowTokenSet(true);
  };

  const handleBackFromManualSelection = () => {
    setShowManualSelection(false);
  };

  const handleManualSelectionLoadingError = () => {
    setShowManualSelection(false);
    setErrorSnackbar({
      open: true,
      message: "Failed to load kanji options. Please try again.",
    });
  };

  const handleManualSelectionLoadingComplete = () => {
    setIsLoading(false);
  };

  const handleShowWaniKaniStages = async () => {
    try {
      setShowWaniKaniStages(true);
    } catch (error) {
      setErrorSnackbar({
        open: true,
        message: "Failed to load WaniKani stages. Please try again.",
      });
    }
  };

  const handleRefreshCache = async () => {
    setIsLoading(true);
    try {
      await refreshCache();
      setSuccessSnackbar({
        open: true,
        message: "Cache refreshed successfully.",
      });
      if (onShowKanji) {
        onShowKanji();
      }
    } catch (error) {
      setErrorSnackbar({
        open: true,
        message: "Failed to refresh cache. Please try again.",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleCloseErrorSnackbar = () => {
    setErrorSnackbar((prev) => ({ ...prev, open: false }));
  };

  const handleCloseSuccessSnackbar = () => {
    setSuccessSnackbar((prev) => ({ ...prev, open: false }));
  };

  if (showTokenSet) {
    return (
      <Box className="userkanji-container-settoken">
        <Box className="userkanji-upper">
          <Button
            variant="outlined"
            onClick={() => setShowTokenSet(false)}
            className="back-button"
          >
            ← Back to kanji
          </Button>
        </Box>
        <WaniKaniTokenSet onTokenSave={handleTokenSave} />
      </Box>
    );
  }

  if (showManualSelection) {
    return (
      <Box className="userkanji-container-manualselection">
        <Box className="userkanji-upper">
          <Button
            variant="outlined"
            onClick={handleBackFromManualSelection}
            className="back-button"
          >
            ← Back to kanji
          </Button>
        </Box>
        <ManualKanjiSelection
          onSelectionSave={handleManualSave}
          onLoadingError={handleManualSelectionLoadingError}
          existingUserKanji={userKanji.map((k) => k.character)}
          onLoadingComplete={handleManualSelectionLoadingComplete}
        />
      </Box>
    );
  }

  return (
    <Box className="userkanji-container">
      <Box className="userkanji-upper">
        <Box className="userkanji-title-refresh-container">
          <Box className="userkanji-title-container">
            <Typography className="userkanji-yourkanji" variant="h3">
              Your kanji
            </Typography>
            <Typography className="userkanji-count" variant="h5">
              ({userKanji.length})
            </Typography>
          </Box>
          {sourceType === "wanikani" && (
            <Button
              size="small"
              variant="outlined"
              onClick={handleRefreshCache}
              disabled={isLoading}
              className="refresh-cache-button"
            >
              Refresh cache
            </Button>
          )}
        </Box>

        <Paper
          className="userkanji-kanjisource-container"
          variant="outlined"
          elevation={14}
        >
          <Typography variant="button" className="userkanji-kanjisource-text">
            Kanji source
          </Typography>
          <ToggleButtonGroup
            value={sourceType}
            exclusive
            onChange={handleChange}
            aria-label="Platform"
            className="userkanji-kanjisource-buttons"
            size="medium"
          >
            <ToggleButton value="wanikani" disabled={sourceType === "wanikani"}>
              WaniKani
            </ToggleButton>
            <ToggleButton
              value="manualselection"
              disabled={sourceType === "manualselection"}
            >
              Manual selection
            </ToggleButton>
          </ToggleButtonGroup>
          {sourceType === "wanikani" && (
            <Box className="wanikani-token-link-container">
              <Link
                variant="body2"
                onClick={handleShowWaniKaniStages}
                className="wanikani-stages-link"
              >
                Set WaniKani Stages
              </Link>
              <Link
                component="button"
                variant="body2"
                onClick={handleShowTokenSet}
                className="change-token-link"
              >
                Change WaniKani token
              </Link>
            </Box>
          )}
          {sourceType === "manualselection" && (
            <Box className="wanikani-token-link-container">
              <Link
                component="button"
                variant="body2"
                onClick={() => setShowManualSelection(true)}
                className="change-token-link"
              >
                Change selected kanji
              </Link>
            </Box>
          )}
        </Paper>
      </Box>

      {isLoading ? (
        <Loader />
      ) : (
        <Box className="userkanji-grid">
          {userKanji.map((kanji, idx) => (
            <Tooltip
              key={idx}
              title={
                <Box className="kanji-tooltip-content">
                  <Typography variant="body2" className="tooltip-title">
                    {kanji.character}
                  </Typography>
                  <Typography variant="body2" className="tooltip-reading">
                    <strong>On Reading:</strong> {kanji.onReadings}
                  </Typography>
                  <Typography variant="body2" className="tooltip-reading">
                    <strong>Kun Reading:</strong> {kanji.kunReadings}
                  </Typography>
                  <Typography variant="body2" className="tooltip-meaning">
                    <strong>Meaning:</strong> {kanji.meanings}
                  </Typography>
                </Box>
              }
              arrow
              placement="top"
              PopperProps={{
                className: "kanji-tooltip-popper",
              }}
            >
              <Typography variant="h4" className="kanji-character">
                {kanji.character}
              </Typography>
            </Tooltip>
          ))}
        </Box>
      )}

      <WaniKaniStagesSet
        onDialogClose={() => setShowWaniKaniStages(false)}
        showWaniKaniStages={showWaniKaniStages}
        onShowKanji={onShowKanji}
        openErrorSnackbar={(message: string) =>
          setErrorSnackbar({
            open: true,
            message: message,
          })
        }
        openSuccessSnackbar={(message: string) =>
          setSuccessSnackbar({
            open: true,
            message: message,
          })
        }
      />

      <Snackbar
        open={errorSnackbar.open}
        message={errorSnackbar.message}
        onClose={handleCloseErrorSnackbar}
      />
      <Snackbar
        open={successSnackbar.open}
        message={successSnackbar.message}
        severity="success"
        onClose={handleCloseSuccessSnackbar}
      />
    </Box>
  );
}
