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
  IconButton,
  Tooltip,
} from "@mui/material";
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  Save as SaveIcon,
  Cancel as CancelIcon,
  Info as InfoIcon,
} from "@mui/icons-material";
import { updateEmail } from "../../ApiCalls/login";
import "./EmailSection.css";
import "./Dialogs.css";
import { getErrorMessage } from "../../Common/utils";

interface EmailSectionProps {
  email: string;
  onEmailUpdate: (newEmail: string) => void;
  isSaving: boolean;
  onError: (message: string) => void;
  onSuccess: (message: string) => void;
  setIsSaving: (saving: boolean) => void;
}

export default function EmailSection({
  email,
  onEmailUpdate,
  isSaving,
  onError,
  onSuccess,
  setIsSaving,
}: EmailSectionProps) {
  const [isEditingEmail, setIsEditingEmail] = useState(false);
  const [newEmail, setNewEmail] = useState<string>(email);
  const [showDeleteEmailDialog, setShowDeleteEmailDialog] = useState(false);

  const handleEditEmail = () => {
    setIsEditingEmail(true);
    setNewEmail(email);
  };

  const handleSaveEmail = async () => {
    if (!newEmail.trim()) {
      onError("Email cannot be empty.");
      return;
    }

    try {
      setIsSaving(true);
      await updateEmail(newEmail, false);
      onEmailUpdate(newEmail);
      setIsEditingEmail(false);
      onSuccess("Email updated successfully!");
    } catch (error) {
      onError(getErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancelEmailEdit = () => {
    setIsEditingEmail(false);
    setNewEmail(email);
  };

  const handleDeleteEmail = async () => {
    try {
      setIsSaving(true);
      await updateEmail(null, true);
      onEmailUpdate("");
      setShowDeleteEmailDialog(false);
      onSuccess("Email deleted successfully!");
    } catch (error) {
      onError(getErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <>
      <Paper className="profile-section" elevation={2}>
        <Box className="section-header">
          <Typography variant="h6" className="section-title">
            Email Address
          </Typography>
          <Tooltip
            title={
              <>
                Add email if you want to get notifications when your texts
                are ready
                <br /> If you don't want to get notifications, just delete
                email
              </>
            }
            arrow
            placement="top"
          >
            <IconButton size="small" className="info-icon">
              <InfoIcon />
            </IconButton>
          </Tooltip>
        </Box>
        <Divider className="section-divider" />

        {isEditingEmail ? (
          <Box className="edit-section">
            <TextField
              fullWidth
              label="Email"
              value={newEmail}
              onChange={(e) => setNewEmail(e.target.value)}
              type="email"
              variant="outlined"
              className="profile-input"
            />
            <Box className="edit-actions">
              <Button
                variant="contained"
                onClick={handleSaveEmail}
                disabled={isSaving}
                startIcon={<SaveIcon />}
                className="save-button"
              >
                Save
              </Button>
              <Button
                variant="outlined"
                onClick={handleCancelEmailEdit}
                disabled={isSaving}
                startIcon={<CancelIcon />}
                className="cancel-button"
              >
                Cancel
              </Button>
            </Box>
          </Box>
        ) : (
          <Box className="display-section">
            <Typography variant="body1" className="field-value">
              {email || "No email set"}
            </Typography>
            <Box className="field-actions">
              <Button
                variant="outlined"
                onClick={handleEditEmail}
                startIcon={<EditIcon />}
                className="edit-button"
              >
                {email ? "Change Email" : "Add Email"}
              </Button>
              {email && (
                <Button
                  variant="outlined"
                  color="error"
                  onClick={() => setShowDeleteEmailDialog(true)}
                  startIcon={<DeleteIcon />}
                  className="delete-button"
                >
                  Delete Email
                </Button>
              )}
            </Box>
          </Box>
        )}
      </Paper>

      <Dialog
        open={showDeleteEmailDialog}
        onClose={() => setShowDeleteEmailDialog(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Delete Email</DialogTitle>
        <DialogContent>
          <Typography variant="body1">
            Are you sure you want to delete your email address? This action
            cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setShowDeleteEmailDialog(false)}
            disabled={isSaving}
          >
            Cancel
          </Button>
          <Button
            onClick={handleDeleteEmail}
            color="error"
            variant="contained"
            disabled={isSaving}
          >
            Delete Email
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
