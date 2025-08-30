import { useState } from "react";
import { Box } from "@mui/material";
import ProfileHeader from "./ProfileHeader";
import ProfileDataManager from "./ProfileDataManager";
import EmailSection from "./EmailSection";
import PasswordSection from "./PasswordSection";
import ThresholdSection from "./ThresholdSection";
import AccountDeletionSection from "./AccountDeletionSection";
import ProfileNotifications from "./ProfileNotifications";
import "./Profile.css";

interface ProfileProps {
  username: string;
  onLogout: () => void;
}

export default function Profile({ username, onLogout }: ProfileProps) {
  const [isSaving, setIsSaving] = useState(false);
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

  const handleError = (message: string) => {
    setErrorSnackbar({
      open: true,
      message,
    });
  };

  const handleSuccess = (message: string) => {
    setSuccessSnackbar({
      open: true,
      message,
    });
  };

  const handleCloseErrorSnackbar = () => {
    setErrorSnackbar((prev) => ({ ...prev, open: false }));
  };

  const handleCloseSuccessSnackbar = () => {
    setSuccessSnackbar((prev) => ({ ...prev, open: false }));
  };

  return (
    <Box className="profile-container">
      <ProfileHeader username={username} />

      <ProfileDataManager>
        {({
          email,
          threshold,
          isThresholdUserSet,
          onEmailUpdate,
          onThresholdUpdate,
        }) => (
          <>
            <EmailSection
              email={email}
              onEmailUpdate={onEmailUpdate}
              isSaving={isSaving}
              onError={handleError}
              onSuccess={handleSuccess}
              setIsSaving={setIsSaving}
            />

            <PasswordSection
              isSaving={isSaving}
              onError={handleError}
              onSuccess={handleSuccess}
              setIsSaving={setIsSaving}
            />

            <ThresholdSection
              threshold={threshold}
              isThresholdUserSet={isThresholdUserSet}
              onThresholdUpdate={onThresholdUpdate}
              isSaving={isSaving}
              onError={handleError}
              onSuccess={handleSuccess}
              setIsSaving={setIsSaving}
            />

            <AccountDeletionSection
              isSaving={isSaving}
              onError={handleError}
              onSuccess={handleSuccess}
              setIsSaving={setIsSaving}
              onLogout={onLogout}
            />
          </>
        )}
      </ProfileDataManager>

      <ProfileNotifications
        errorSnackbar={errorSnackbar}
        successSnackbar={successSnackbar}
        onCloseError={handleCloseErrorSnackbar}
        onCloseSuccess={handleCloseSuccessSnackbar}
      />
    </Box>
  );
}
