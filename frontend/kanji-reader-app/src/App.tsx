import './App.css'
import Login from './Login.tsx';
import MainPage from './MainPage';
import { useState, useEffect } from 'react';
import { getCurrentUser } from './auth';
import { ThemeProvider, createTheme } from "@mui/material";
import { lightBlue, deepPurple } from '@mui/material/colors';

const theme = createTheme({
  palette: {
    primary: deepPurple,
    secondary: lightBlue,
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

  if (loading) {
    return null;
  }

  return (
    <ThemeProvider theme={theme}>
      {userName ? <MainPage userName={userName} /> : <Login onLogin={setUserName} />}
    </ThemeProvider>
  );
}

export default App
