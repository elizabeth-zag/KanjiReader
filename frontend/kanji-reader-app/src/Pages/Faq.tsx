import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import Button from '@mui/material/Button';
import { useNavigate } from 'react-router-dom';
import './Faq.css';

export default function Faq() {
    const navigate = useNavigate();
    return (
        <Box className='faq-container'>
            <Button onClick={() => navigate('/')} variant="outlined" sx={{ mb: 3 }}>← Return to main page</Button>
            <Typography className="faq-title" color="primary" variant="h4">FAQ</Typography>
            <Divider sx={{ my: 2 }} />
            <Typography color="primary" fontWeight='fontWeightMedium' variant="h6">What is Kanji Reader?</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">If you're learning Japanese and want to improve your reading skills, probably one of the biggest obstacles is encountering too many unknown kanji in real texts. Kanji Reader is designed to solve this problem!
                It selects reading materials based on the kanji you already know, so you can read Japanese texts with as few unknown kanji as possible.</Typography>
            <Divider sx={{ my: 2 }} />
            <Typography color="primary" fontWeight='fontWeightMedium' variant="h6">How do I create a list of learned kanji?</Typography>
            Click the <strong>KANJI</strong> button in the top menu. You can create your kanji list in two ways:
            <Divider sx={{ my: 1 }} />
            <Typography color="primary.light" fontWeight='fontWeightMedium' variant="body1">WaniKani</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">If you have a WaniKani account, you can import your kanji directly:</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">Go to WaniKani. Click your user avatar → API Tokens → Generate a new token. Copy the token and paste it into Kanji Reader</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">By default, kanji from the Master, Enlightened, and Burned stages are imported.
                You can change this by clicking <strong>Set WaniKani Stages.</strong></Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">If you've learned new kanji but don't see them in Kanji Reader, click <strong>Refresh</strong> to load the latest data.</Typography>
            <Divider sx={{ my: 1 }} />
            <Typography color="primary.light" fontWeight='fontWeightMedium' variant="body1">Manual selection</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">You can manually select kanji from 3000 most common kanji or popular kanji lists (JLPT levels or Kyōiku grades). Modify your selection at any time by clicking <strong>Change selected kanji</strong>.</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">You can switch between WaniKani and Manual selection at any time using the <strong>KANJI SOURCE</strong> toggle.</Typography>
            <Divider sx={{ my: 2 }} />
            <Typography color="primary" fontWeight='fontWeightMedium' variant="h6">How do I generate texts?</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">Click the <strong>READINGS</strong> button in the top menu, then click <strong>COLLECT TEXTS</strong>. You can choose one or more of the following sources:</Typography>
            <Divider sx={{ my: 1 }} />
            <Typography color="primary.light" fontWeight='fontWeightMedium' variant="body1">Watanoc</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">A free online magazine with simple Japanese texts. Ideal for beginners.</Typography>
            <Divider sx={{ my: 1 }} />
            <Typography color="primary.light" fontWeight='fontWeightMedium' variant="body2">NHK News Easy</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">A simplified news site by Japan's national broadcaster NHK. Articles vary in difficulty and are suitable for learners at different levels.</Typography>
            <Divider sx={{ my: 1 }} />
            <Typography color="primary.light" fontWeight='fontWeightMedium' variant="body2">Satori Reader</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">A subscription-based reading platform for Japanese learners. Each story has two free episodes. Kanji Reader only uses texts from free episodes.
                (If you enjoy the stories, please consider subscribing.)</Typography>
            <Divider sx={{ my: 1 }} />
            <Typography color="primary.light" fontWeight='fontWeightMedium' variant="body2">AI Generation</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">Claude AI generates short texts based on your known kanji.
                There is a limit on how many AI-generated texts you can request within a certain time period so if you don't get new texts, please try again after a few hours.</Typography>
            <Divider sx={{ my: 1 }} />
            <Typography fontWeight='fontWeightMedium' variant="body2">Text collection stops after 50 texts. To collect more, delete some existing texts first.</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">If you select multiple sources, Kanji Reader prioritizes them based on collection speed.</Typography>
            <Divider sx={{ my: 2 }} />
            <Typography color="primary" fontWeight='fontWeightMedium' variant="h6">How do I use generated texts?</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">Click a text card to view the full text, list of unknown kanji and a link to the original source (if available). Kanji Reader currently does not provide furigana or translations, but these features are planned for future updates. You can open the original source to access these features for now.</Typography>
            <Divider sx={{ my: 2 }} />
            <Typography color="primary" fontWeight='fontWeightMedium' variant="h6">How are texts selected?</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">Kanji Reader selects texts using a threshold system. A text is selected only if the ratio of unknown kanji to total kanji does not exceed the threshold. The threshold is calculated automatically to provide an optimal difficulty level: the more kanji you know, the lower is threshold, meaning that you'll get texts with fewer unknown kanji.</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">You can set a custom threshold in Profile → My account → User Threshold.</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">⚠️ Note:</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">- If you set the threshold too low, especially with a small known-kanji set, fewer texts may be available.</Typography>
            <Typography fontWeight='fontWeightMedium' variant="body2">- AI-generated texts do not use the threshold system.</Typography>
            <Divider sx={{ my: 2 }} />
            <Typography color="primary" fontWeight='fontWeightMedium' variant="body2">The app is developing, new features are planned for future updates. Thank you for using Kanji Reader!</Typography>
        </Box>
    );
}
