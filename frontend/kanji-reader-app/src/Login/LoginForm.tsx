import React, { useState } from 'react';
import { login } from '../ApiCalls/login';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import './LoginForm.css';

export type LoginProps = {
  onLogin?: (username: string) => void;
  onSwitchToRegister?: () => void;
};

export default function Login({ onLogin, onSwitchToRegister }: LoginProps ) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      await login(username, password);
      setError('');
      if (onLogin) onLogin(username);
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message ||
        err.message ||
        'An error occurred during login';
      setError(errorMessage);
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
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Password"
              required
              fullWidth
              variant="outlined"
            />
          </Box>
          <Box className="login-button-container">
            <Button
              variant="contained"
              type="submit"
              className="login-button"
            >
              Login
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
          {error && (
            <Typography className="login-error">
              {error}
            </Typography>
          )}
        </Box>
      </Box>
  );
}
