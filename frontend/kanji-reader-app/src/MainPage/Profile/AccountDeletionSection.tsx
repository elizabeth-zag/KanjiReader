import { useState } from "react";
import {
  Typography,
  TextField,
  Button,
  Paper,
  Divider,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Alert,
  InputAdornment,
  IconButton,
} from "@mui/material";
import {
  Visibility,
  VisibilityOff,
} from "@mui/icons-material";
import { deleteUserAccount } from "../../ApiCalls/login";
import { getErrorMessage } from "../../Common/utils";
import "./AccountDeletionSection.css";
import "./Dialogs.css";

interface AccountDeletionSectionProps {
  isSaving: boolean;
  onError: (message: string) => void;
  onSuccess: (message: string) => void;
  setIsSaving: (saving: boolean) => void;
  onLogout: () => void;
}

export default function AccountDeletionSection({
  isSaving,
  onError,
  onSuccess,
  setIsSaving,
  onLogout,
}: AccountDeletionSectionProps) {
  const [showDeleteAccountDialog, setShowDeleteAccountDialog] = useState(false);
  const [deleteAccountPassword, setDeleteAccountPassword] = useState<string>("");
  const [showDeleteAccountPassword, setShowDeleteAccountPassword] = useState(false);

  const handleDeleteAccount = async () => {
    if (!deleteAccountPassword.trim()) {
      onError("Please enter your password to confirm account deletion.");
      return;
    }

    try {
      setIsSaving(true);
      await deleteUserAccount(deleteAccountPassword);
      setShowDeleteAccountDialog(false);
      setDeleteAccountPassword("");
      onSuccess("Account deleted successfully!");
      
      setTimeout(() => {
        onLogout();
      }, 1500);
    } catch (error) {
      onError(getErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleCloseDialog = () => {
    setShowDeleteAccountDialog(false);
    setDeleteAccountPassword("");
  };

  return (
    <>
      <Paper className="profile-section danger-section" elevation={2}>
        <Typography variant="h6" className="section-title danger-title">
          Delete Account
        </Typography>
        <Divider className="section-divider" />

        <Typography variant="body2" className="danger-description">
          This action cannot be undone. All your data will be permanently
          deleted.
        </Typography>

        <Button
          variant="contained"
          color="error"
          onClick={() => setShowDeleteAccountDialog(true)}
          className="delete-account-button"
        >
          Delete My Account
        </Button>
      </Paper>

      <Dialog
        open={showDeleteAccountDialog}
        onClose={handleCloseDialog}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Delete Account</DialogTitle>
        <DialogContent>
          <Alert severity="warning" className="delete-warning">
            <Typography variant="body1" className="warning-text">
              This action will permanently delete your account and all
              associated data. This cannot be undone.
            </Typography>
          </Alert>
          <TextField
            fullWidth
            placeholder="Enter your password to confirm"
            type={showDeleteAccountPassword ? "text" : "password"}
            value={deleteAccountPassword}
            onChange={(e) => setDeleteAccountPassword(e.target.value)}
            variant="outlined"
            className="dialog-input"
            margin="normal"
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() =>
                      setShowDeleteAccountPassword(!showDeleteAccountPassword)
                    }
                    edge="end"
                  >
                    {showDeleteAccountPassword ? (
                      <VisibilityOff />
                    ) : (
                      <Visibility />
                    )}
                  </IconButton>
                </InputAdornment>
              ),
            }}
          />
        </DialogContent>
        <DialogActions>
          <Button
            onClick={handleCloseDialog}
            disabled={isSaving}
          >
            Cancel
          </Button>
          <Button
            onClick={handleDeleteAccount}
            color="error"
            variant="contained"
            disabled={isSaving}
          >
            Delete Account
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
