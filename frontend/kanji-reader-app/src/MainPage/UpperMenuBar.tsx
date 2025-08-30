import * as React from "react";
import { AppBar, Box, Toolbar, Typography, Button } from "@mui/material";
import ProfileMenu from "./ProfileMenu";
import "./UpperMenuBar.css";
import { useLocation, useNavigate } from "react-router-dom";

interface UpperMenuBarProps {
  onShowKanji?: () => void;
  onShowReadings?: () => void;
  onShowProfile?: () => void;
  onLogout: () => void;
}

export default function UpperMenuBar({
  onShowKanji,
  onShowReadings,
  onShowProfile,
  onLogout,
}: UpperMenuBarProps) {
  const location = useLocation();
  const navigate = useNavigate();

  const handleHome = () => {
    navigate("/");
  };

  return (
    <Box className="uppermenubar-outer">
      <AppBar position="static">
        <Toolbar className="uppermenubar-toolbar">
          <Box className="uppermenubar-logo-title">
            <img
              src="/logo.png"
              alt="Kanji Reader Logo"
              className="uppermenubar-logo"
              onClick={handleHome}
              style={{ cursor: 'pointer' }}
            />
            <Box className="uppermenubar-title-container">
              <Typography 
                variant="h3" 
                className="uppermenubar-title"
                onClick={handleHome}
                style={{ cursor: 'pointer' }}
              >
                Kanji Reader
              </Typography>
            </Box>
          </Box>
          <Box className="uppermenubar-center">
            <Button
              variant="outlined"
              onClick={onShowKanji}
              className="kanjimenu-button"
            >
              Kanji
            </Button>
            <Button
              variant="outlined"
              onClick={onShowReadings}
              className="readingsmenu-button"
            >
              Readings
            </Button>
          </Box>
          <ProfileMenu onLogout={onLogout} onShowProfile={onShowProfile} />
        </Toolbar>
      </AppBar>
    </Box>
  );
}
