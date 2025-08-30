import React, { useState } from "react";
import { login } from "../ApiCalls/login";
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
};

export default function Login({ onLogin, onSwitchToRegister, preFilledUsername }: LoginProps) {
  const [username, setUsername] = useState(preFilledUsername || "");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [errorSnackbar, setErrorSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({
    open: false,
    message: "",
  });

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setErrorSnackbar({
      open: false,
      message: "",
    });
    
    setIsLoading(true);
    try {
      await login(username, password);
      if (onLogin) onLogin(username);
    } catch (err: any) {
      setErrorSnackbar({
        open: true,
        message: getErrorMessage(err),
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleCloseErrorSnackbar = () => {
    setErrorSnackbar((prev) => ({ ...prev, open: false }));
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
          severity="error"
          onClose={handleCloseErrorSnackbar}
        />
      </Box>
    </Box>
  );
}
