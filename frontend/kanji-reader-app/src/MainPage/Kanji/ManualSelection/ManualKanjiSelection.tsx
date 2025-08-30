import { useState, useEffect, useRef } from "react";
import { Box, Typography, Button, CircularProgress } from "@mui/material";
import {
  getKanjiForManualSelection,
  getKanjiLists,
  saveSelectedKanji,
} from "../../../ApiCalls/kanji";
import IndividualKanjiSelection from "./IndividualKanjiSelection";
import KanjiListsSelection from "./KanjiListsSelection";
import Snackbar from "../../../Common/Snackbar";
import "./ManualKanjiSelection.css";
import Loader from "../../../Common/Loader";

interface ManualKanjiSelectionProps {
  onSelectionSave: () => {};
  onLoadingError?: () => void;
  existingUserKanji?: string[];
  onLoadingComplete?: () => void;
}

export default function ManualKanjiSelection({
  onSelectionSave,
  onLoadingError,
  existingUserKanji,
  onLoadingComplete,
}: ManualKanjiSelectionProps) {
  const [availableKanji, setAvailableKanji] = useState<string[]>([]);
  const [kanjiLists, setKanjiLists] = useState<
    { kanjiList: string; description: string }[]
  >([]);
  const [selectedKanji, setSelectedKanji] = useState<string[]>(
    existingUserKanji || []
  );
  const [selectedLists, setSelectedLists] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [errorSnackbar, setErrorSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({
    open: false,
    message: "",
  });
  const selectedKanjiRef = useRef(selectedKanji);

  useEffect(() => {
    selectedKanjiRef.current = selectedKanji;
  }, [selectedKanji]);

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      try {
        const [kanjiResult, listsResult] = await Promise.all([
          getKanjiForManualSelection(),
          getKanjiLists(),
        ]);

        setAvailableKanji(kanjiResult?.kanji ?? []);
        setKanjiLists(listsResult?.kanjiLists ?? []);
      } catch (error) {
        onLoadingError && onLoadingError();
      } finally {
        setIsLoading(false);
        onLoadingComplete && onLoadingComplete();
      }
    };

    fetchData();
  }, [onLoadingComplete]);

  const handleKanjiToggle = (kanji: string) => {
    const isSelected = selectedKanjiRef.current.includes(kanji);
    setSelectedKanji((prev) => {
      const result = isSelected
        ? prev.filter((k) => k !== kanji)
        : [...prev, kanji];
      return result;
    });
  };

  const handleListToggle = (listName: string) => {
    setSelectedLists((prev) =>
      prev.includes(listName)
        ? prev.filter((l) => l !== listName)
        : [...prev, listName]
    );
  };

  const handleSaveSelection = async () => {
    setIsSaving(true);
    try {
      await saveSelectedKanji(selectedKanjiRef.current, selectedLists);
      onSelectionSave && onSelectionSave();
    } catch (error) {
      setErrorSnackbar({
        open: true,
        message: "Failed to save kanji selection.",
      });
    } finally {
      setIsSaving(false);
    }
  };

  const handleCloseErrorSnackbar = () => {
    setErrorSnackbar((prev) => ({ ...prev, open: false }));
  };

  const handleCheckAll = () => {
    setSelectedKanji(availableKanji);
  };

  const handleClearAll = () => {
    setSelectedKanji([]);
  };

  return (
    <>
      {isLoading ? (
        <Loader />
      ) : (
        <Box className="manual-kanji-container">
          <Typography variant="h5" className="manual-kanji-title">
            Manual Kanji Selection
          </Typography>
          <KanjiListsSelection
            kanjiLists={kanjiLists}
            selectedLists={selectedLists}
            onListToggle={handleListToggle}
          />

          <IndividualKanjiSelection
            availableKanji={availableKanji}
            selectedKanji={selectedKanji}
            onKanjiToggle={handleKanjiToggle}
            onCheckAll={handleCheckAll}
            onClearAll={handleClearAll}
          />

          <Box className="manual-kanji-actions">
            <Button
              variant="contained"
              onClick={handleSaveSelection}
              className="manual-kanji-save-button"
              disabled={isSaving}
            >
              {isSaving ? (
                <CircularProgress size={24} color="inherit" />
              ) : (
                `Save Selection (${selectedKanji.length} kanji, ${selectedLists.length} lists)`
              )}
            </Button>
          </Box>
        </Box>
      )}
      <Snackbar
        open={errorSnackbar.open}
        message={errorSnackbar.message}
        onClose={handleCloseErrorSnackbar}
      />
    </>
  );
}
