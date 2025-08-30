import * as React from "react";
import { Menu, IconButton, MenuItem, Typography } from "@mui/material";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import { logout } from "../ApiCalls/login";
import Snackbar from "../Common/Snackbar";
import "./ProfileMenu.css";

interface ProfileMenuProps {
  onLogout: () => void;
  onShowProfile?: () => void;
}

export default function ProfileMenu({ onLogout, onShowProfile }: ProfileMenuProps) {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const [isLoggingOut, setIsLoggingOut] = React.useState(false);
  const [errorSnackbar, setErrorSnackbar] = React.useState<{
    open: boolean;
    message: string;
  }>({
    open: false,
    message: "",
  });
  const open = Boolean(anchorEl);

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleLogOut = async () => {
    if (isLoggingOut) return;

    setIsLoggingOut(true);
    try {
      await logout();
      handleClose();
      onLogout();
    } catch (error) {
      setErrorSnackbar({
        open: true,
        message: "Logout failed. Please try again.",
      });
    } finally {
      setIsLoggingOut(false);
    }
  };

  const handleCloseErrorSnackbar = () => {
    setErrorSnackbar((prev) => ({ ...prev, open: false }));
  };

  return (
    <div>
      <IconButton
        size="large"
        edge="start"
        color="inherit"
        aria-label="menu"
        onClick={handleClick}
        aria-controls={open ? "basic-menu" : undefined}
        aria-haspopup="true"
        aria-expanded={open ? "true" : undefined}
        className="profilemenu-iconbutton"
      >
        <AccountCircleIcon />
        <Typography variant="button" className="profilemenu-label">
          Profile
        </Typography>
      </IconButton>
      <Menu
        id="basic-menu"
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        slotProps={{
          list: {
            "aria-labelledby": "basic-button",
          },
        }}
      >
        {onShowProfile && (
          <MenuItem 
            onClick={() => {
              handleClose();
              onShowProfile();
            }}
          >
            My account
          </MenuItem>
        )}
        <MenuItem onClick={handleLogOut} disabled={isLoggingOut}>
          {isLoggingOut ? "Logging out..." : "Logout"}
        </MenuItem>
      </Menu>
      <Snackbar
        open={errorSnackbar.open}
        message={errorSnackbar.message}
        onClose={handleCloseErrorSnackbar}
      />
    </div>
  );
}
