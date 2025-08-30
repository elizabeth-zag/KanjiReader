import axios from "axios";

const API_URL = "http://localhost:5000/api/login"; // todo: move to config

export type LoginResponse = {
  userName: string;
};

export type GetWaniKaniStagesResponse = {
  stages: string[];
};

export type GetEmailResponse = {
  email: string;
};

export type GetUserThresholdResponse = {
  threshold: number;
  isUserSet: boolean;
};

export async function register(
  username: string,
  password: string,
  email: string | null,
  waniKaniToken: string | null
) {
  await axios.post(
    `${API_URL}/Register`,
    { username, password, email, waniKaniToken },
    { withCredentials: true }
  );
}

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

export async function getEmail(): Promise<GetEmailResponse> {
  const res = await axios.get<GetEmailResponse>(`${API_URL}/GetEmail`, {
    withCredentials: true,
  });
  return res.data;
}

export async function updateEmail(email: string | null, needDelete: boolean) {
  const res = await axios.post(
    `${API_URL}/UpdateEmail`,
    { email, needDelete },
    { withCredentials: true }
  );
  return res.data;
}

export async function updatePassword(oldPassword: string, newPassword: string) {
  const res = await axios.post(
    `${API_URL}/UpdatePassword`,
    { oldPassword, newPassword },
    { withCredentials: true }
  );
  return res.data;
}

export async function getCurrentUser(): Promise<LoginResponse | null> {
  try {
    const res = await axios.get<LoginResponse>(`${API_URL}/GetCurrentUser`, {
      withCredentials: true,
    });
    return res.data;
  } catch {
    return null;
  }
}

export async function setWaniKaniToken(token: string) {
  const res = await axios.post(
    `${API_URL}/SetWaniKaniToken`,
    { token },
    { withCredentials: true }
  );
  return res.data;
}

export async function setWaniKaniStages(stages: string[]) {
  const res = await axios.post(
    `${API_URL}/SetWaniKaniStages`,
    { stages },
    { withCredentials: true }
  );
  return res.data;
}

export async function getWaniKaniStages(): Promise<GetWaniKaniStagesResponse> {
  const res = await axios.get<GetWaniKaniStagesResponse>(
    `${API_URL}/GetWaniKaniStages`,
    { withCredentials: true }
  );
  return res.data;
}

export async function getUserThreshold(): Promise<GetUserThresholdResponse> {
  const res = await axios.get<GetUserThresholdResponse>(
    `${API_URL}/GetUserThreshold`,
    { withCredentials: true }
  );
  return res.data;
}

export async function setUserThreshold(threshold: number | null) {
  const res = await axios.post(
    `${API_URL}/SetUserThreshold`,
    { threshold },
    { withCredentials: true }
  );
  return res.data;
}

export async function deleteUserAccount(password: string) {
  const res = await axios.post(
    `${API_URL}/DeleteUserAccount`,
    { password },
    { withCredentials: true }
  );
  return res.data;
}
