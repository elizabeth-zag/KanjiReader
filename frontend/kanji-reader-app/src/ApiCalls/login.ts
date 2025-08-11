import axios from 'axios';

const API_URL = 'http://localhost:5000/api/login'; // todo: move to config

export type LoginResponse = {
  userName: string;
};

export async function register(username: string, password: string, email: string, waniKaniToken: string) {
  await axios.post(
    `${API_URL}/Register`,
    { username, password, email, waniKaniToken },
    { withCredentials: true }
  );
};

export async function login(username: string, password: string) {
  await axios.post(
    `${API_URL}/LogIn`,
    { username, password },
    { withCredentials: true }
  );
}

export async function logout() {
  const res = await axios.post(
    `${API_URL}/LogOut`,
    {},
    { withCredentials: true }
  );
  return res.data;
}

export async function getCurrentUser(): Promise<LoginResponse | null> {
  try {
    const res = await axios.get<LoginResponse>(
      `${API_URL}/GetCurrentUser`,
      { withCredentials: true }
    );
    return res.data;
  } catch {
    return null;
  }
}

export async function setWaniKaniToken(token: string)  {
  const res = await axios.post(
    `${API_URL}/SetWaniKaniToken`,
    { token },
    { withCredentials: true }
  );
  return res.data;
}
