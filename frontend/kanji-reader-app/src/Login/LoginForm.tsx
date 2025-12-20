import React, { useState } from "react";
import { login, sendConfirmationCode } from "../ApiCalls/login";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Link from "@mui/material/Link";
import IconButton from "@mui/material/IconButton";
import InputAdornment from "@mui/material/InputAdornment";
import { Visibility, VisibilityOff } from "@mui/icons-material";
import "./LoginForm.css";
import Snackbar from "../Common/Snackbar";
import { getErrorMessage } from "../Common/utils";

export type LoginProps = {
  onLogin?: (username: string) => void;
  onSwitchToRegister?: () => void;
  preFilledUsername?: string;
  showEmailConfirmationInitially?: boolean;
};

export default function Login({ onLogin, onSwitchToRegister, preFilledUsername, showEmailConfirmationInitially }: LoginProps) {
  const [username, setUsername] = useState(preFilledUsername || "");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [showEmailConfirmation, setShowEmailConfirmation] = useState<boolean>(!!showEmailConfirmationInitially);
  const [confirmationCode, setConfirmationCode] = useState<string>("");
  const [isLoading, setIsLoading] = useState(false);
  const [isResending, setIsResending] = useState(false);
  const [errorSnackbar, setErrorSnackbar] = useState<{
    open: boolean;
    message: string;
    severity?: 'error' | 'warning' | 'info' | 'success';
  }>({
    open: false,
    message: "",
    severity: 'error',
  });

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setErrorSnackbar({
      open: false,
      message: "",
    });
    
    setIsLoading(true);
    try {
      const res = await login(username, password, confirmationCode);
      if (res.needEmailConfirmation) {
        setShowEmailConfirmation(true);
        setErrorSnackbar({ open: true, message: "Email confirmation required. Enter the code sent to your email.", severity: 'info' });
      } else if (!res.errorMessage) {
        if (onLogin) onLogin(username);
      } else {
        setErrorSnackbar({ open: true, message: res.errorMessage, severity: 'error' });
      }
    } catch (err: any) {
      setErrorSnackbar({
        open: true,
        message: getErrorMessage(err),
        severity: 'error'
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleCloseErrorSnackbar = () => {
    setErrorSnackbar((prev) => ({ ...prev, open: false }));
  };

  const handleResendCode = async () => {
    if (!username) {
      setErrorSnackbar({ open: true, message: 'Enter username to resend code', severity: 'warning' });
      return;
    }
    setIsResending(true);
    try {
      await sendConfirmationCode(username);
      setErrorSnackbar({ open: true, message: 'Confirmation code sent', severity: 'success' });
    } catch (err: any) {
      setErrorSnackbar({ open: true, message: getErrorMessage(err), severity: 'error' });
    } finally {
      setIsResending(false);
    }
  };

  return (
    <Box className="login-form-container">
      <Box component="form" onSubmit={handleSubmit}>
        <Box className="login-form">
          <TextField
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Username"
            required
            fullWidth
            variant="outlined"
          />
          <TextField
            type={showPassword ? "text" : "password"}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Password"
            required
            fullWidth
            variant="outlined"
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

          {showEmailConfirmation && (
            <Box sx={{ display: 'flex', gap: 1, alignItems: 'center', width: '100%' }}>
              <TextField
                value={confirmationCode}
                onChange={(e) => setConfirmationCode(e.target.value)}
                placeholder="Confirmation code"
                required
                fullWidth
                variant="outlined"
              />
              <Button
                onClick={handleResendCode}
                disabled={isResending}
                variant="outlined"
                size="small"
                className="send-code-button"
              >
                {isResending ? 'Sending ...' : 'Resend code'}
              </Button>
            </Box>
          )}
        </Box>
        <Box className="login-button-container">
          <Button 
            variant="contained" 
            type="submit" 
            className="login-button"
            disabled={isLoading}
          >
            {isLoading ? "Logging in..." : "Login"}
          </Button>
        </Box>
        {onSwitchToRegister && (
          <Box className="login-form-switch-row">
            <Typography component="span" className="login-form-switch-text">
              Don't have an account?
            </Typography>
            <Link
              component="button"
              variant="body1"
              onClick={onSwitchToRegister}
              className="login-form-switch-link"
            >
              Register
            </Link>
          </Box>
        )}
        <Snackbar
          open={errorSnackbar.open}
          message={errorSnackbar.message}
          severity={errorSnackbar.severity}
          onClose={handleCloseErrorSnackbar}
        />
      </Box>
    </Box>
  );
}
