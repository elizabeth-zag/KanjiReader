import axios from "axios";

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/login`;

export type GetCurrentUserResponse = {
  userName: string;
};

export type GetWaniKaniStagesResponse = {
  stages: string[];
};

export type GetEmailResponse = {
  email: string;
};

export type LoginResponse = {
  needEmailConfirmation: boolean;
  errorMessage: string | null;
};

export type GetUserThresholdResponse = {
  threshold: number;
  isUserSet: boolean;
};

export async function register(
  username: string,
  password: string,
  email: string,
  waniKaniToken: string | null
) {
  await axios.post(
    `${API_URL}/Register`,
    { username, password, email, waniKaniToken },
    { withCredentials: true }
  );
}

export async function login(username: string, password: string, confirmationCode?: string): Promise<LoginResponse>
{
  const body: any = { username, password };
  if (confirmationCode) body.confirmationCode = confirmationCode;
  const res = await axios.post<LoginResponse>(
    `${API_URL}/LogIn`,
    body,
    { withCredentials: true }
  );
  return res.data;
}

export async function logout() {
  const res = await axios.post(
    `${API_URL}/LogOut`,
    {},
    { withCredentials: true }
  );
  return res.data;
}

export async function sendConfirmationCode(username: string) {
  const res = await axios.post(
    `${API_URL}/SendConfirmationCode`,
    { userName: username },
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

export async function updateEmail(email: string | null) {
  const res = await axios.post(
    `${API_URL}/UpdateEmail`,
    { email },
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

export async function getCurrentUser(): Promise<GetCurrentUserResponse | null> {
  try {
    const res = await axios.get<GetCurrentUserResponse>(`${API_URL}/GetCurrentUser`, {
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
