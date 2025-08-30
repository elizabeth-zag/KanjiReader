import {
  Box,
  Typography,
  Button,
  Paper,
  Chip,
  Divider,
  Link,
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { type ProcessingResult } from "../../ApiCalls/texts";
import { getRatioColor, getRatioLabel, formatDate } from "../../Common/utils";
import "./TextDetail.css";

interface TextDetailProps {
  text: ProcessingResult;
  onBack: () => void;
}

export default function TextDetail({ text, onBack }: TextDetailProps) {
  return (
    <Box className="readings-container">
      <Box className="readings-header">
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={onBack}
          variant="outlined"
          className="back-button"
        >
          Back to List
        </Button>
      </Box>

      <Box className="text-detail-content">
        <Paper className="text-detail-paper" elevation={2}>
          <Typography variant="h4" className="text-detail-title">
            {text.title}
          </Typography>

          <Box className="text-detail-meta">
            <Box className="text-ratio-info">
              <Typography variant="body2" className="ratio-label">
                Difficulty:
              </Typography>
              <Chip
                label={`${getRatioLabel(text.ratio)} (${(
                  text.ratio * 100
                ).toFixed(1)}% unknown)`}
                color={getRatioColor(text.ratio) as any}
                size="small"
                className="ratio-chip"
              />
            </Box>
          </Box>

          {text.unknownKanji.length > 0 && (
            <Box className="unknown-kanji-section">
              <Typography variant="h6" className="unknown-kanji-title">
                Unknown Kanji ({text.unknownKanji.length})
              </Typography>
              <Box className="unknown-kanji-list">
                {text.unknownKanji.map((kanji, index) => (
                  <Chip
                    key={index}
                    label={kanji}
                    variant="outlined"
                    className="kanji-chip"
                  />
                ))}
              </Box>
            </Box>
          )}

          <Typography variant="h6" className="text-content-title">
            Full Text
          </Typography>
          <Typography variant="body1" className="text-full-content">
            {text.content}
          </Typography>

          <Divider className="text-content-divider" />

          <Box className="text-detail-footer">
            <Box className="text-detail-footer-left">
              <Typography variant="body2" className="text-source">
                Source: {text.sourceType}
              </Typography>

              {text.url && (
                <Link
                  href={text.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-detail-url-link"
                >
                  View Original Source
                </Link>
              )}
            </Box>

            <Box className="text-detail-footer-right">
              <Typography variant="body2" className="create-date-info">
                Created: {formatDate(text.createDate)}
              </Typography>
            </Box>
          </Box>
        </Paper>
      </Box>
    </Box>
  );
}
