import './App.css'
import LoginPage from './Login/LoginPage.tsx';
import MainPage from './MainPage/MainPage.tsx';
import Faq from './Pages/Faq';
import Contact from './Pages/Contact';
import Footer from './Common/Footer';
import { useState, useEffect } from 'react';
import { getCurrentUser } from './ApiCalls/login';
import { Box } from "@mui/material";
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';

const theme = createTheme({
  typography: {
    fontFamily: "IBM Plex Sans",
    h2: {
      fontWeight: '400',
    }
  },
  palette: {
    primary: {
      light: '#ffbf60ff',
      main: '#FF9800',
      dark: '#210b0bff',
      contrastText: '#fff',
    }
  }
});

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
    <ThemeProvider theme={theme}>
      <Router>
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
          <Box sx={{ flex: 1 }}>
            {userName ? (
              <Routes>
                <Route path="/" element={<MainPage userName={userName} onLogout={handleLogout} />} />
                <Route path="/kanji" element={<MainPage userName={userName} onLogout={handleLogout} />} />
                <Route path="/readings" element={<MainPage userName={userName} onLogout={handleLogout} />} />
                <Route path="/profile" element={<MainPage userName={userName} onLogout={handleLogout} />} />
                <Route path="/faq" element={<Faq />} />
                <Route path="/contact" element={<Contact />} />
                <Route path="*" element={<Navigate to="/" replace />} />
              </Routes>
            ) : (
              <Routes>
                <Route path="/faq" element={<Faq />} />
                <Route path="/contact" element={<Contact />} />
                <Route path="*" element={<LoginPage onLogin={setUserName} />} />
              </Routes>
            )}
          </Box>
          <Footer />
        </Box>
      </Router>
    </ThemeProvider>
  );
}

export default App
