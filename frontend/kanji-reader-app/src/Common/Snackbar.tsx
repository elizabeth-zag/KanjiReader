import { Snackbar as MuiSnackbar, Alert } from '@mui/material';

interface SnackbarProps {
  open: boolean;
  message: string;
  severity?: 'error' | 'warning' | 'info' | 'success';
  autoHideDuration?: number;
  onClose: () => void;
  anchorOrigin?: {
    vertical: 'top' | 'bottom';
    horizontal: 'left' | 'center' | 'right';
  };
}

export default function Snackbar({
  open,
  message,
  severity = 'error',
  autoHideDuration = 6000,
  onClose,
  anchorOrigin = { vertical: 'bottom', horizontal: 'center' }
}: SnackbarProps) {
  return (
    <MuiSnackbar
      open={open}
      autoHideDuration={autoHideDuration}
      onClose={onClose}
      anchorOrigin={anchorOrigin}
    >
      <Alert
        onClose={onClose}
        severity={severity}
        sx={{ width: '100%' }}
      >
        {message}
      </Alert>
    </MuiSnackbar>
  );
}
