import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Link from '@mui/material/Link';
import { useNavigate } from 'react-router-dom';
import './Contact.css';

export default function Contact() {
  const navigate = useNavigate();
  return (
    <Box className='contact-container'>
      <Button onClick={() => navigate('/')} variant="outlined" sx={{ mb: 2 }}>← Return to main page</Button>
      <Typography className="contact-title" color="primary" variant="h4">Contact</Typography>
      <Typography variant="body1">If you have any questions or feedback, please contact us at <strong>readerkanji@gmail.com</strong></Typography>
      <Typography variant="body1" sx={{ mt: 2 }}>View the source code on <Link href="https://github.com/elizabeth-zag/KanjiReader">GitHub</Link></Typography>
    </Box>
  );
}
