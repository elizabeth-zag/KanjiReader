import { useState } from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  Paper,
  Divider,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  InputAdornment,
  IconButton,
} from "@mui/material";
import {
  Visibility,
  VisibilityOff,
} from "@mui/icons-material";
import { updatePassword } from "../../ApiCalls/login";
import { getErrorMessage } from "../../Common/utils";
import "./PasswordSection.css";
import "./Dialogs.css";

interface PasswordSectionProps {
  isSaving: boolean;
  onError: (message: string) => void;
  onSuccess: (message: string) => void;
  setIsSaving: (saving: boolean) => void;
}

export default function PasswordSection({
  isSaving,
  onError,
  onSuccess,
  setIsSaving,
}: PasswordSectionProps) {
  const [showPasswordDialog, setShowPasswordDialog] = useState(false);
  const [currentPassword, setCurrentPassword] = useState<string>("");
  const [newPassword, setNewPassword] = useState<string>("");
  const [confirmPassword, setConfirmPassword] = useState<string>("");
  const [showPassword, setShowPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const handleChangePassword = async () => {
    if (
      !currentPassword.trim() ||
      !newPassword.trim() ||
      !confirmPassword.trim()
    ) {
      onError("All password fields are required.");
      return;
    }

    if (newPassword !== confirmPassword) {
      onError("New passwords do not match.");
      return;
    }

    if (newPassword.length < 6) {
      onError("New password must be at least 6 characters long.");
      return;
    }

    try {
      setIsSaving(true);
      await updatePassword(currentPassword, newPassword);
      setShowPasswordDialog(false);
      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");
      onSuccess("Password changed successfully!");
    } catch (error) {
      onError(getErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleCloseDialog = () => {
    setShowPasswordDialog(false);
    setCurrentPassword("");
    setNewPassword("");
    setConfirmPassword("");
  };

  return (
    <>
      <Paper className="profile-section" elevation={2}>
        <Typography variant="h6" className="section-title">
          Password
        </Typography>
        <Divider className="section-divider" />

        <Box className="display-section">
          <Typography variant="body1" className="field-value">
            ••••••••
          </Typography>
          <Button
            variant="outlined"
            onClick={() => setShowPasswordDialog(true)}
            className="edit-button"
          >
            Change Password
          </Button>
        </Box>
      </Paper>
      
      <Dialog
        open={showPasswordDialog}
        onClose={handleCloseDialog}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Change Password</DialogTitle>
        <DialogContent>
          <Box className="dialog-content">
            <TextField
              fullWidth
              placeholder="Current Password"
              type={showPassword ? "text" : "password"}
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              variant="outlined"
              className="dialog-input"
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowPassword(!showPassword)}
                      edge="end"
                    >
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />
            <TextField
              fullWidth
              placeholder="New Password"
              type={showNewPassword ? "text" : "password"}
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              variant="outlined"
              className="dialog-input"
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowNewPassword(!showNewPassword)}
                      edge="end"
                    >
                      {showNewPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />
            <TextField
              fullWidth
              placeholder="Confirm New Password"
              type={showConfirmPassword ? "text" : "password"}
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              variant="outlined"
              className="dialog-input"
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() =>
                        setShowConfirmPassword(!showConfirmPassword)
                      }
                      edge="end"
                      className="password-visibility-icon-button"
                    >
                      {showConfirmPassword ? (
                        <VisibilityOff />
                      ) : (
                        <Visibility />
                      )}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />
          </Box>
        </DialogContent>
        <DialogActions className="change-password-buttons">
          <Button
            onClick={handleCloseDialog}
            disabled={isSaving}
            className="change-password-cancel-button"
          >
            Cancel
          </Button>
          <Button
            onClick={handleChangePassword}
            variant="contained"
            disabled={isSaving}
            className="change-password-button"
          >
            Change Password
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
