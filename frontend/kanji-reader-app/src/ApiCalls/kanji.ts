import axios from 'axios';

const API_URL = 'http://localhost:5000/api/kanji'; // todo: move to config

type KanjiList = {
  kanjiList: string;
  description: string;
};

type tryUpdateKanjiSourceResponse = {
  success: boolean;
};

export type UserKanjiResponse = {
  kanji: string[];
  kanjiSourceType: string
};

export type KanjiListsResponse = {
  kanjiLists: KanjiList[];
};

export type KanjiForSelectionResponse = {
  kanji: string[];
};

export async function getUserKanji(): Promise<UserKanjiResponse> {
  const res = await axios.get<UserKanjiResponse>(
    `${API_URL}/GetUserKanji`,
    { withCredentials: true }
  );
  return res.data;
}

export async function getKanjiLists(): Promise<KanjiListsResponse | null> {
  try {
    const res = await axios.get<KanjiListsResponse>(
      `${API_URL}/GetKanjiListsForSelection`,
      { withCredentials: true }
    );
    return res.data;
  } catch {
    return null;
  }
}

export async function getKanjiForManualSelection(): Promise<KanjiForSelectionResponse | null> {
  try {
    const res = await axios.get<KanjiForSelectionResponse>(
      `${API_URL}/GetKanjiForManualSelection`,
      { withCredentials: true }
    );
    return res.data;
  } catch {
    return null;
  }
}

export async function saveSelectedKanji(kanji: string[], kanjiLists: string[]) {
  await axios.post(
    `${API_URL}/SetSelectedKanji`,
    { kanji, kanjiLists },
    { withCredentials: true }
  );
}

export async function tryUpdateKanjiSource(kanjiSourceType: string) : Promise<boolean | null> {
  const res = await axios.post<tryUpdateKanjiSourceResponse>(
    `${API_URL}/TryUpdateKanjiSource`,
    { kanjiSourceType },
    { withCredentials: true }
  );
  return res.data.success;
}