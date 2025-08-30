import axios from "axios";

const API_URL = "http://localhost:5000/api/texts"; // todo: move to config

export type ProcessingResult = {
  id: string;
  title: string;
  content: string;
  ratio: number;
  unknownKanji: string[];
  url: string;
  sourceType: string;
  createDate: string; // UTC date string from backend
};

export type GenerationSource = {
  name: string;
  description: string;
  value: string;
};

export type GenerationSourcesResponse = {
  sources: GenerationSource[];
};

export type GetProcessedTextsResponse = {
  processedTexts: ProcessingResult[];
};

export async function getProcessedTexts(): Promise<GetProcessedTextsResponse> {
  const res = await axios.get<GetProcessedTextsResponse>(
    `${API_URL}/GetProcessedTexts`,
    { withCredentials: true }
  );
  return res.data;
}

export async function getGenerationSources(): Promise<GenerationSourcesResponse> {
  const res = await axios.get<GenerationSourcesResponse>(
    `${API_URL}/GetGenerationSources`,
    { withCredentials: true }
  );
  return res.data;
}

export async function startCollecting(sources: string[]) {
  const res = await axios.post<GenerationSourcesResponse>(
    `${API_URL}/StartCollecting`,
    { sources },
    { withCredentials: true }
  );
  return res.data;
}

export async function removeTexts(textIds: string[]) {
  const res = await axios.post(
    `${API_URL}/RemoveTexts`,
    { textIds },
    { withCredentials: true }
  );
  return res.data;
}
