import React, { useState } from 'react';
import { login } from './auth';
import Button from '@mui/material/Button';

export type LoginProps = {
  onLogin?: (username: string) => void;
};

export default function Login({ onLogin }: LoginProps ) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      await login(username, password);
      setError('');
      if (onLogin) onLogin(username);
    } catch (err: any) {
      const errorMessage =
        err.response?.data?.message ||
        err.message ||
        'An error occurred during login';
      setError(errorMessage);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input value={username} onChange={e => setUsername(e.target.value)} placeholder="Username" required />
      <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Password" required />
      <br /><br />
      <Button
        variant="contained"
        type="submit"
        sx={{ backgroundColor: '#FF9800', color: 'white' }}
      >
        Login
      </Button>
      {error && <div style={{ color: 'red' }}>{error}</div>}
    </form>
  );
}
