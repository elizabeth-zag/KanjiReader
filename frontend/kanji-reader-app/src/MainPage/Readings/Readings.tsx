import { useState, useEffect } from "react";
import { Box, Button } from "@mui/material";
import {
  getProcessedTexts,
  type ProcessingResult,
  removeTexts,
} from "../../ApiCalls/texts";
import TextList from "./TextList";
import TextDetail from "./TextDetail";
import CollectionModal from "./CollectionModal";
import Snackbar from "../../Common/Snackbar";
import { useTextsStream } from "../../ApiCalls/textsStream";
import "./Readings.css";
import { getErrorMessage } from "../../Common/utils";

interface ReadingsProps {
}

export default function Readings({}: ReadingsProps) {
  const [texts, setTexts] = useState<ProcessingResult[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [hasLoadedTexts, setHasLoadedTexts] = useState(false);

  const [isReceivingUpdates, setIsReceivingUpdates] = useState(false);
  const [errorSnackbar, setErrorSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({ open: false, message: "" });
  const [successSnackbar, setSuccessSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({ open: false, message: "" });
  const [selectedText, setSelectedText] = useState<ProcessingResult | null>(
    null
  );
  const [showTextDetail, setShowTextDetail] = useState(false);
  const [selectedSourceType, setSelectedSourceType] = useState<string>("all");

  const [sortField, setSortField] = useState<string>("date");
  const [sortDirection, setSortDirection] = useState<string>("desc");

  const [isDeleteMode, setIsDeleteMode] = useState(false);
  const [selectedTextIds, setSelectedTextIds] = useState<string[]>([]);

  const showErrorSnackbar = (message: string) => {
    setErrorSnackbar({ open: true, message });
  };

  const showSuccessSnackbar = (message: string) => {
    setSuccessSnackbar({ open: true, message });
  };

  const handleCloseErrorSnackbar = () => {
    setErrorSnackbar({ open: false, message: "" });
  };

  const handleCloseSuccessSnackbar = () => {
    setSuccessSnackbar({ open: false, message: "" });
  };

  const loadTexts = async () => {
    setIsLoading(true);
    try {
      const response = await getProcessedTexts();
      const processedTexts = response.processedTexts.map((text) => ({
        ...text,
        createDate: text.createDate || new Date().toISOString(),
      }));

      setTexts(processedTexts);
      setHasLoadedTexts(true);
    } catch (error) {
      setTexts([]);
      setHasLoadedTexts(true);
      showErrorSnackbar(getErrorMessage(error));
    } finally {
      setIsLoading(false);
    }
  };

  const handleStartCollecting = async () => {
    setIsModalOpen(false);
    setHasLoadedTexts(false);
    await loadTexts();
  };

  const handleTextStreamUpdate = (newTexts: ProcessingResult[]) => {
    setIsReceivingUpdates(true);
    setTimeout(() => setIsReceivingUpdates(false), 3000);

    setTexts((prevTexts) => {
      const existingTextsMap = new Map(
        prevTexts.map((text) => [text.id, text])
      );

      newTexts.forEach((newText) => {
        const processedText = {
          ...newText,
          createDate: newText.createDate || new Date().toISOString(),
        };

        existingTextsMap.set(newText.id, processedText);
      });

      const updatedTexts = Array.from(existingTextsMap.values()).sort(
        (a, b) => {
          const dateA = new Date(a.createDate).getTime();
          const dateB = new Date(b.createDate).getTime();
          return dateB - dateA;
        }
      );

      return updatedTexts;
    });

    if (!hasLoadedTexts) {
      setHasLoadedTexts(true);
    }
  };

  useTextsStream(handleTextStreamUpdate);

  const handleOpenModal = () => {
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
  };

  const handleTextClick = (text: ProcessingResult) => {
    setSelectedText(text);
    setShowTextDetail(true);
  };

  const handleBackToList = () => {
    setShowTextDetail(false);
    setSelectedText(null);
  };

  const handleSourceTypeFilterChange = (sourceType: string) => {
    setSelectedSourceType(sourceType);
  };

  const getUniqueSourceTypes = (): string[] => {
    const sourceTypes = texts.map((text) => text.sourceType);
    return Array.from(new Set(sourceTypes)).sort();
  };

  const getFilteredTexts = (): ProcessingResult[] => {
    let filtered = texts;

    if (selectedSourceType !== "all") {
      filtered = texts.filter((text) => text.sourceType === selectedSourceType);
    }

    filtered = [...filtered].sort((a, b) => {
      if (sortField === "date") {
        const dateA = new Date(a.createDate).getTime();
        const dateB = new Date(b.createDate).getTime();
        return sortDirection === "asc" ? dateA - dateB : dateB - dateA;
      } else if (sortField === "difficulty") {
        return sortDirection === "asc" ? a.ratio - b.ratio : b.ratio - a.ratio;
      }
      return 0;
    });

    return filtered;
  };

  const handleSortChange = (field: string, direction: string) => {
    setSortField(field);
    setSortDirection(direction);
  };

  const toggleDeleteMode = () => {
    setIsDeleteMode(!isDeleteMode);
    setSelectedTextIds([]);
  };

  const handleTextSelection = (textId: string, isSelected: boolean) => {
    if (isSelected) {
      setSelectedTextIds((prev) => [...prev, textId]);
    } else {
      setSelectedTextIds((prev) => prev.filter((id) => id !== textId));
    }
  };

  const handleSelectAll = (isSelected: boolean) => {
    if (isSelected) {
      const allIds = getFilteredTexts().map((text) => text.id);
      setSelectedTextIds(allIds);
    } else {
      setSelectedTextIds([]);
    }
  };

  const handleDeleteTexts = async () => {
    if (selectedTextIds.length === 0) return;

    try {
      await removeTexts(selectedTextIds);

      setSelectedTextIds([]);
      setIsDeleteMode(false);

      await loadTexts();

      showSuccessSnackbar(`Successfully deleted ${selectedTextIds.length} text(s)`);
    } catch (error) {
      showErrorSnackbar(getErrorMessage(error));
    }
  };

  useEffect(() => {
    loadTexts();
  }, []);

  if (showTextDetail && selectedText) {
    return <TextDetail text={selectedText} onBack={handleBackToList} />;
  }

  const uniqueSourceTypes = getUniqueSourceTypes();
  const filteredTexts = getFilteredTexts();

  return (
    <Box className="readings-container">
      <Box className="readings-start-collecting">
        <Button
          variant="contained"
          onClick={handleOpenModal}
          className="start-collecting-button"
          size="medium"
        >
          Start Collecting Texts
        </Button>
      </Box>

      <Box className="readings-main-content">
        <TextList
          texts={filteredTexts}
          allTexts={texts}
          isLoading={isLoading}
          hasLoadedTexts={hasLoadedTexts}
          onTextClick={handleTextClick}
          sourceTypes={uniqueSourceTypes}
          selectedSourceType={selectedSourceType}
          onSourceTypeChange={handleSourceTypeFilterChange}
          sortField={sortField}
          sortDirection={sortDirection}
          onSortChange={handleSortChange}
          isDeleteMode={isDeleteMode}
          selectedTextIds={selectedTextIds}
          onToggleDeleteMode={toggleDeleteMode}
          onTextSelection={handleTextSelection}
          onSelectAll={handleSelectAll}
          onDeleteTexts={handleDeleteTexts}
          isReceivingUpdates={isReceivingUpdates}
        />
      </Box>

      <CollectionModal
        open={isModalOpen}
        onClose={handleCloseModal}
        onStartCollecting={handleStartCollecting}
        openErrorSnackbar={showErrorSnackbar}
      />

      <Snackbar
        open={errorSnackbar.open}
        message={errorSnackbar.message}
        severity="error"
        onClose={handleCloseErrorSnackbar}
      />
      <Snackbar
        open={successSnackbar.open}
        message={successSnackbar.message}
        severity="success"
        onClose={handleCloseSuccessSnackbar}
      />
    </Box>
  );
}
