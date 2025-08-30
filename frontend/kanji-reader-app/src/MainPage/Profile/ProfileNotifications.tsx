import Snackbar from "../../Common/Snackbar";

interface ProfileNotificationsProps {
  errorSnackbar: {
    open: boolean;
    message: string;
  };
  successSnackbar: {
    open: boolean;
    message: string;
  };
  onCloseError: () => void;
  onCloseSuccess: () => void;
}

export default function ProfileNotifications({
  errorSnackbar,
  successSnackbar,
  onCloseError,
  onCloseSuccess,
}: ProfileNotificationsProps) {
  return (
    <>
      <Snackbar
        open={errorSnackbar.open}
        message={errorSnackbar.message}
        severity="error"
        onClose={onCloseError}
      />
      <Snackbar
        open={successSnackbar.open}
        message={successSnackbar.message}
        severity="success"
        onClose={onCloseSuccess}
      />
    </>
  );
}
