import * as React from 'react';
import { Menu, IconButton, MenuItem, Typography } from '@mui/material';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import './ProfileMenu.css';
import { logout } from '../ApiCalls/login';

interface ProfileMenuProps {
  onLogout: () => void;
}

export default function ProfileMenu({ onLogout }: ProfileMenuProps) {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const [isLoggingOut, setIsLoggingOut] = React.useState(false);
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
      console.error('Logout failed:', error);
      // todo: maybe add error feedback here (snackbar or something)
    } finally {
      setIsLoggingOut(false);
    }
  };

  return (
    <div>
      <IconButton
        size="large"
        edge="start"
        color="inherit"
        aria-label="menu"
        onClick={handleClick}
        aria-controls={open ? 'basic-menu' : undefined}
        aria-haspopup="true"
        aria-expanded={open ? 'true' : undefined}
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
            'aria-labelledby': 'basic-button',
          },
        }}
      >
        <MenuItem onClick={handleClose}>My account</MenuItem>
        <MenuItem 
          onClick={handleLogOut}
          disabled={isLoggingOut}
        >
          {isLoggingOut ? 'Logging out...' : 'Logout'}
        </MenuItem>
      </Menu>
    </div>
  );
}
