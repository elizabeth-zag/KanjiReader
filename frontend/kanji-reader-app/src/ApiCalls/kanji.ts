import axios from "axios";

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/kanji`;

type KanjiList = {
  kanjiList: string;
  description: string;
};

type tryUpdateKanjiSourceResponse = {
  success: boolean;
};

export type KanjiWithData = {
  character: string;
  kunReadings: string;
  onReadings: string;
  meanings: string;
};

export type UserKanjiResponse = {
  kanji: KanjiWithData[];
  kanjiSourceType: string;
};

export type KanjiListsResponse = {
  kanjiLists: KanjiList[];
};

export type KanjiForSelectionResponse = {
  kanji: string[];
};

export async function getUserKanji(): Promise<UserKanjiResponse> {
  const res = await axios.get<UserKanjiResponse>(`${API_URL}/GetUserKanji`, {
    withCredentials: true,
  });
  return res.data;
}

export async function getKanjiLists(): Promise<KanjiListsResponse | null> {
  const res = await axios.get<KanjiListsResponse>(
    `${API_URL}/GetKanjiListsForSelection`,
    { withCredentials: true }
  );
  return res.data;
}

export async function getKanjiForManualSelection(): Promise<KanjiForSelectionResponse | null> {
  const res = await axios.get<KanjiForSelectionResponse>(
    `${API_URL}/GetKanjiForManualSelection`,
    { withCredentials: true }
  );
  return res.data;
}

export async function saveSelectedKanji(kanji: string[], kanjiLists: string[]) {
  await axios.post(
    `${API_URL}/SetSelectedKanji`,
    { kanji, kanjiLists },
    { withCredentials: true }
  );
}

export async function tryUpdateKanjiSource(
  kanjiSourceType: string
): Promise<boolean | null> {
  const res = await axios.post<tryUpdateKanjiSourceResponse>(
    `${API_URL}/TryUpdateKanjiSource`,
    { kanjiSourceType },
    { withCredentials: true }
  );
  return res.data.success;
}

export async function refreshCache()
{
  console.log("Refreshing cache...");
  const res = await axios.post(
    `${API_URL}/RefreshWaniKaniCache`,
    {},
    { withCredentials: true }
  );
  return res.data.success;
}