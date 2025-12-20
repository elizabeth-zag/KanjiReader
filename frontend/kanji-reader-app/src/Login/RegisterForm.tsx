import React, { useState } from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Link from "@mui/material/Link";
import IconButton from "@mui/material/IconButton";
import InputAdornment from "@mui/material/InputAdornment";
import Tooltip from "@mui/material/Tooltip";
import InfoIcon from "@mui/icons-material/Info";
import { Visibility, VisibilityOff } from "@mui/icons-material";
import "./RegisterForm.css";
import { register } from "../ApiCalls/login";
import Snackbar from "../Common/Snackbar";
import { getErrorMessage } from "../Common/utils";

export type RegisterProps = {
  onRegister?: (username: string) => void;
  onSwitchToLogin?: () => void;
};

export default function Register({
  onRegister,
  onSwitchToLogin,
}: RegisterProps) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [email, setEmail] = useState("");
  const [waniKaniToken, setWaniKaniToken] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [errorSnackbar, setErrorSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({
    open: false,
    message: "",
  });
  const [validationErrors, setValidationErrors] = useState<{
    username: boolean;
    password: boolean;
    email: boolean;
  }>({
    username: false,
    password: false,
    email: false,
  });

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    
    setValidationErrors({ username: false, password: false, email: false });
    setErrorSnackbar({
      open: false,
      message: "",
    });
    
    const hasErrors = !username.trim() || !password.trim();
    
    if (hasErrors) {
      setValidationErrors({
        username: !username.trim(),
        password: !password.trim(),
        email: !email.trim(),
      });
      setErrorSnackbar({
        open: true,
        message: "Please fill in all required fields",
      });
      return;
    }
    
    setIsLoading(true);
    try {
      await register(
        username, 
        password, 
        email.trim(), 
        waniKaniToken.trim() || null
      );
      if (onRegister) onRegister(username);
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
    <Box className="register-form-container">
      <Box component="form" onSubmit={handleSubmit}>
        <Box className="register-form">
          <TextField
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Username *"
            required
            fullWidth
            variant="outlined"
            error={validationErrors.username}
            helperText={validationErrors.username ? "Username is required" : ""}
          />
          <TextField
            type={showPassword ? "text" : "password"}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Password *"
            required
            fullWidth
            variant="outlined"
            error={validationErrors.password}
            helperText={validationErrors.password ? "Password is required" : ""}
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
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Email *"
            required
            fullWidth
            variant="outlined"
            helperText={validationErrors.email ? "Email is required" : ""}
            error={validationErrors.email}
          />
          <TextField
            value={waniKaniToken}
            onChange={(e) => setWaniKaniToken(e.target.value)}
            placeholder="WaniKaniToken"
            fullWidth
            variant="outlined"
            InputProps={{
              endAdornment: (
                <Tooltip title={<>On WaniKani: Profile ─&gt; API Tokens <br />
                 You can fill it later or manually select kanji if you don't have WaniKani</>} arrow placement="top">
                  <IconButton size="small" className="field-info-icon">
                    <InfoIcon fontSize="small" />
                  </IconButton>
                </Tooltip>
              ),
            }}
          />
        </Box>
        <Box className="register-button-container">
          <Button 
            variant="contained" 
            type="submit" 
            className="register-button"
            disabled={isLoading}
          >
            {isLoading ? "Registering..." : "Register"}
          </Button>
        </Box>
        {onSwitchToLogin && (
          <Box className="register-form-switch-row">
            <Typography component="span" className="register-form-switch-text">
              Already have an account?
            </Typography>
            <Link
              component="button"
              variant="body1"
              onClick={onSwitchToLogin}
              className="register-form-switch-link"
            >
              Login
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
