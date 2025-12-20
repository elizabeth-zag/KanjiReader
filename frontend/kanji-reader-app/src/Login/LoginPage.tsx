import { useState } from "react";
import LoginForm from "./LoginForm";
import RegisterForm from "./RegisterForm";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import "./LoginPage.css";

export type LoginProps = {
  onLogin?: (username: string) => void;
};

export default function LoginPage({ onLogin }: LoginProps) {
  const [isLogin, setIsLogin] = useState(true);
  const [postRegistrationUsername, setPostRegistrationUsername] = useState<string>("");

  const handleSwitchToRegister = () => {
    setIsLogin(false);
    setPostRegistrationUsername("");
  };

  const handleSwitchToLogin = () => {
    setIsLogin(true);
  };

  const handleRegistrationSuccess = (username: string) => {
    setPostRegistrationUsername(username);
    setIsLogin(true);
  };

  return (
    <Box className="login-container">
      <Box className="login-header">
        <img src="/logo.png" alt="Kanji Reader Logo" className="login-logo" />
        <Typography variant="h2" className="login-title">
          Welcome to Kanji Reader
        </Typography>
        <Typography variant="body1" className="login-subtitle">
          Practice reading Japanese texts with kanji you already know
        </Typography>
      </Box>
      {isLogin && postRegistrationUsername && (
        <Box className="registration-success-message">
          <Typography variant="body1" className="success-text">
            Registration successful! Please log in with your new account.
          </Typography>
        </Box>
      )}
      {isLogin ? (
        <LoginForm
          onLogin={onLogin}
          onSwitchToRegister={handleSwitchToRegister}
          preFilledUsername={postRegistrationUsername}
          showEmailConfirmationInitially={!!postRegistrationUsername}
        />
      ) : (
        <RegisterForm
          onRegister={handleRegistrationSuccess}
          onSwitchToLogin={handleSwitchToLogin}
        />
      )}
    </Box>
  );
}
