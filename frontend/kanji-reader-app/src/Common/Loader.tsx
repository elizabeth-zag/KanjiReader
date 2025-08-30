import { Box } from "@mui/material";
import "./Loader.css";

export default function Loader() {
  return (
    <Box className="loading-container">
      <Box className="loading-spinner"></Box>
    </Box>
  );
}
