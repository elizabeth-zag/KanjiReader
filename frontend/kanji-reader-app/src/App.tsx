import './App.css'
import LoginPage from './Login/LoginPage.tsx';
import MainPage from './MainPage/MainPage.tsx';
import { useState, useEffect } from 'react';
import { getCurrentUser } from './ApiCalls/login';
import { Box, createTheme } from "@mui/material";

function App() {
  const [userName, setUserName] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getCurrentUser().then(response => {
      if (response && response.userName) {
        setUserName(response.userName);
      }
      setLoading(false);
    });
  }, []);

  const handleLogout = () => {
    setUserName(null);
  };

  if (loading) {
    return null;
  }

  return (
    <Box>
      {userName ? (
        <MainPage userName={userName} onLogout={handleLogout} />
      ) : (
        <LoginPage onLogin={setUserName} />
      )}
    </Box>
  );
}

export default App
