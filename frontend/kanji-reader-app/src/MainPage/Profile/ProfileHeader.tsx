import { Box, Typography } from "@mui/material";
import "./ProfileHeader.css";

interface ProfileHeaderProps {
  username: string;
}

export default function ProfileHeader({ username }: ProfileHeaderProps) {
  return (
    <>
      <Typography variant="h4" className="profile-title">
        Profile Settings
      </Typography>
      
      <Box className="profile-name-header">
        <Typography variant="h5" className="profile-name">
          {username}
        </Typography>
      </Box>
    </>
  );
}
