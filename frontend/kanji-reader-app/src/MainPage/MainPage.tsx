import { useState, useEffect } from "react";
import UpperMenuBar from "./UpperMenuBar";
import { Box, Typography } from "@mui/material";
import "./MainPage.css";
import { getUserKanji, type KanjiWithData } from "../ApiCalls/kanji";
import UserKanji from "./Kanji/UserKanji";
import KanjiSetup from "./Kanji/KanjiSetup";
import Readings from "./Readings/Readings";
import Profile from "./Profile/Profile";
import Loader from "../Common/Loader";
import Snackbar from "../Common/Snackbar";
import { useLocation, useNavigate } from "react-router-dom";

interface MainPageProps {
  userName: string;
  onLogout: () => void;
}

function MainPage({ userName, onLogout }: MainPageProps) {
  const location = useLocation();
  const navigate = useNavigate();
  const [userKanji, setUserKanji] = useState<KanjiWithData[] | null>(null);
  const [kanjiSourceType, setKanjiSourceType] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [errorSnackbar, setErrorSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({
    open: false,
    message: "",
  });

  const currentPath = location.pathname;
  const showKanji = currentPath === "/kanji";
  const showReadings = currentPath === "/readings";
  const showProfile = currentPath === "/profile";

  const handleShowKanji = async () => {
    navigate("/kanji");
    setIsLoading(true);

    try {
      setUserKanji(null);
      const result = await getUserKanji();
      setUserKanji(result.kanji);
      setKanjiSourceType(result.kanjiSourceType);
    } catch (error: any) {
      const errorMessage =
        error.response?.data?.message ||
        error.response?.data?.error ||
        error.message ||
        "Failed to load kanji. Please try again.";

      setErrorSnackbar({
        open: true,
        message: errorMessage,
      });
      setUserKanji([]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleShowReadings = async () => {
    navigate("/readings");
  };

  const handleShowProfile = () => {
    navigate("/profile");
  };

  const handleCloseErrorSnackbar = () => {
    setErrorSnackbar((prev) => ({ ...prev, open: false }));
  };

  useEffect(() => {
    if (showKanji && !userKanji) {
      handleShowKanji();
    }
  }, []);

  return (
    <>
      <Box className="mainpage-container">
        <UpperMenuBar
          onShowKanji={handleShowKanji}
          onShowReadings={handleShowReadings}
          onShowProfile={handleShowProfile}
          onLogout={onLogout}
        />
        <Box className="mainpage-centered">
          {!showKanji && !showReadings && !showProfile && (
            <Box>
              <Typography variant="h4" className="mainpage-welcome">
                Welcome to the Kanji Reader Main Page, {userName}!
              </Typography>
              <Typography variant="body1">
                Start exploring kanji or use the menu above.
              </Typography>
            </Box>
          )}
          {showKanji &&
            (userKanji === null ? (
              <Loader />
            ) : userKanji.length > 0 ? (
              <UserKanji
                userKanji={userKanji}
                kanjiSourceType={kanjiSourceType ?? ""}
                onShowKanji={handleShowKanji}
              />
            ) : (
              <KanjiSetup onShowKanji={handleShowKanji} />
            ))}
          {showReadings && (isLoading ? <Loader /> : <Readings />)}
          {showProfile && (isLoading ? <Loader /> : <Profile username={userName} onLogout={onLogout} />)}
        </Box>
      </Box>

      <Snackbar
        open={errorSnackbar.open}
        message={errorSnackbar.message}
        severity="error"
        onClose={handleCloseErrorSnackbar}
      />
    </>
  );
}

export default MainPage;
