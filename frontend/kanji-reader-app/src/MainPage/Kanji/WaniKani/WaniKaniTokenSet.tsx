import React, { useState } from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  Paper,
  IconButton,
  Tooltip,
} from "@mui/material";
import InfoIcon from "@mui/icons-material/Info";
import { setWaniKaniToken } from "../../../ApiCalls/login";
import Snackbar from "../../../Common/Snackbar";
import "./WaniKaniTokenSet.css";
import { getErrorMessage } from "../../../Common/utils";

interface WaniKaniTokenSetProps {
  onTokenSave: () => {};
  initialToken?: string;
}

export default function WaniKaniTokenSet({
  onTokenSave,
  initialToken = "",
}: WaniKaniTokenSetProps) {
  const [token, setToken] = useState(initialToken);
  const [isSaving, setIsSaving] = useState(false);
  const [snackbar, setSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({
    open: false,
    message: "",
  });

  const handleTokenChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setToken(event.target.value);
  };

  const handleSave = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!token.trim()) {
      return;
    }

    setIsSaving(true);
    try {
      await setWaniKaniToken(token.trim());
      onTokenSave();
    } catch (error) {
      setSnackbar({
        open: true,
        message: getErrorMessage(error),
      });
    } finally {
      setIsSaving(false);
    }
  };

  const handleCloseSnackbar = () => {
    setSnackbar((prev) => ({ ...prev, open: false }));
  };

  return (
    <>
      <Paper className="wanikani-token-container" elevation={2}>
        <Box
          component="form"
          onSubmit={handleSave}
          className="wanikani-token-content"
        >
          <Box className="wanikani-token-header">
            <Typography variant="h6" className="wanikani-token-title">
              WaniKani API Token
            </Typography>
            <Tooltip
              title="On WaniKani: Profile -> API Tokens"
              placement="top"
              arrow
            >
              <IconButton size="small" className="wanikani-info-icon">
                <InfoIcon />
              </IconButton>
            </Tooltip>
          </Box>

          <Typography variant="body2" className="wanikani-token-description">
            Enter your WaniKani API token to sync your kanji progress
          </Typography>

          <Box className="wanikani-token-input-container">
            <TextField
              fullWidth
              variant="outlined"
              placeholder="Enter your WaniKani API token..."
              value={token}
              onChange={handleTokenChange}
              className="wanikani-token-input"
              type="password"
              size="small"
            />
          </Box>

          <Box className="wanikani-token-actions">
            <Button
              variant="contained"
              disabled={!token.trim() || isSaving}
              className="wanikani-save-button"
              size="medium"
              type="submit"
            >
              {isSaving ? "Saving..." : "Save Token"}
            </Button>
          </Box>
        </Box>
      </Paper>

      <Snackbar
        open={snackbar.open}
        message={snackbar.message}
        severity="error"
        onClose={handleCloseSnackbar}
      />
    </>
  );
}
