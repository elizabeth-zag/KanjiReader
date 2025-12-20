import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { Link as RouterLink } from 'react-router-dom';
import "./Footer.css";

export default function Footer() {
  return (
    <Box className="footer">
      <Button className="footer-button" component={RouterLink} to="/faq" variant="text">FAQ</Button>
      <Button className="footer-button" component={RouterLink} to="/contact" variant="text">Contact</Button>
    </Box>
  );
}
