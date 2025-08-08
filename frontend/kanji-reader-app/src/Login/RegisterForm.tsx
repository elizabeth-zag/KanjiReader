import React, { useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import './RegisterForm.css';
import { register } from '../ApiCalls/login';

export type RegisterProps = {
  onRegister?: (username: string) => void;
  onSwitchToLogin?: () => void;
};

export default function Register({ onRegister, onSwitchToLogin }: RegisterProps ) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [email, setEmail] = useState('');
  const [waniKaniToken, setWaniKaniToken] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      await register(username, password, email, waniKaniToken);
      setError('');
      if (onRegister) onRegister(username);
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message ||
        err.message ||
        'An error occurred during registration';
      setError(errorMessage);
    }
  };

  return (
    <Box className="register-form-container">
        <Box component="form" onSubmit={handleSubmit}>
          <Box className="register-form">
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
            <TextField
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Email"
              required
              fullWidth
              variant="outlined"
            />
            <TextField
              value={waniKaniToken}
              onChange={(e) => setWaniKaniToken(e.target.value)}
              placeholder="WaniKaniToken"
              fullWidth
              variant="outlined"
            />
          </Box>
          <Box className="register-button-container">
            <Button
              variant="contained"
              type="submit"
              className="register-button"
            >
              Register
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
          {error && (
            <Typography className="register-error">
              {error}
            </Typography>
          )}
        </Box>
      </Box>
  );
} 