import axios from 'axios';

const API_URL = 'http://localhost:5000/api/kanji'; // todo: move to config

export type UserKanjiResponse = {
  kanji: string[];
  kanjiCount: number,
  kanjiSourceType: string
};

type KanjiList = {
  kanjiList: string;
  description: string;
};

export type KanjiListsResponse = {
  kanjiLists: KanjiList[];
};

export type KanjiForSelectionResponse = {
  kanji: string[];
};

export async function getUserKanji(): Promise<UserKanjiResponse | null> {
  try {
    const res = await axios.get<UserKanjiResponse>(
      `${API_URL}/GetUserKanji`,
      { withCredentials: true }
    );
    return res.data;
  } catch {
    return null;
  }
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
