import React, { useState } from 'react';
import LoginForm from './LoginForm';
import RegisterForm from './RegisterForm';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import './LoginPage.css';

export type LoginProps = {
  onLogin?: (username: string) => void;
};

export default function LoginPage({ onLogin }: LoginProps) {
  const [isLogin, setIsLogin] = useState(true);

  const handleSwitchToRegister = () => {
    setIsLogin(false);
  };

  const handleSwitchToLogin = () => {
    setIsLogin(true);
  };

  return (
    <Box className="login-container">
      <Box className="login-header">
        <img 
          src="/logo.png" 
          alt="Kanji Reader Logo" 
          className="login-logo"
        />
        <Typography variant="h2" className="login-title">
          Hello! Welcome to the Kanji Reader
        </Typography>
        <Typography variant="body1" className="login-subtitle">
          Practice reading Japanese with texts that include kanji you already know
        </Typography>
      </Box>
      {isLogin ? (
        <LoginForm onLogin={onLogin} onSwitchToRegister={handleSwitchToRegister} />
      ) : (
        <RegisterForm onRegister={onLogin} onSwitchToLogin={handleSwitchToLogin} />
      )}
    </Box>
  );
}
