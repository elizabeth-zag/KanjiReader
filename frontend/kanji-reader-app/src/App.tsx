import './App.css'
import LoginPage from './Login/LoginPage.tsx';
import MainPage from './MainPage/MainPage.tsx';
import { useState, useEffect } from 'react';
import { getCurrentUser } from './ApiCalls/login';
import { Box, createTheme } from "@mui/material";
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';

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
    <Router>
      <Box>
        {userName ? (
          <Routes>
            <Route path="/" element={<MainPage userName={userName} onLogout={handleLogout} />} />
            <Route path="/kanji" element={<MainPage userName={userName} onLogout={handleLogout} />} />
            <Route path="/readings" element={<MainPage userName={userName} onLogout={handleLogout} />} />
            <Route path="/profile" element={<MainPage userName={userName} onLogout={handleLogout} />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        ) : (
          <Routes>
            <Route path="*" element={<LoginPage onLogin={setUserName} />} />
          </Routes>
        )}
      </Box>
    </Router>
  );
}

export default App
