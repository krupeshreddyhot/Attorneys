import axios from "axios";
import api from "./api";
import {
  mapAnalysisToDisplayStatus,
  type AiDisplayStatus,
  type DocumentAnalysisDto,
  type MessageResponse,
} from "../types/documentAnalysis";

const displayStatusCache = new Map<number, AiDisplayStatus>();

export const getCachedDisplayStatus = (fileId: number): AiDisplayStatus | undefined =>
  displayStatusCache.get(fileId);

export const setCachedDisplayStatus = (fileId: number, status: AiDisplayStatus): void => {
  displayStatusCache.set(fileId, status);
};

export const clearDisplayStatusCache = (): void => {
  displayStatusCache.clear();
};

export const getAnalysis = async (fileId: number): Promise<DocumentAnalysisDto | null> => {
  try {
    const response = await api.get<DocumentAnalysisDto>(`/documents/${fileId}/analysis`);
    return response.data;
  } catch (err) {
    if (axios.isAxiosError(err) && err.response?.status === 404) {
      return null;
    }
    throw err;
  }
};

export const fetchDisplayStatus = async (fileId: number): Promise<AiDisplayStatus> => {
  const analysis = await getAnalysis(fileId);
  const status = mapAnalysisToDisplayStatus(analysis);
  setCachedDisplayStatus(fileId, status);
  return status;
};

export const analyzeDocument = (fileId: number) =>
  api.post<MessageResponse>(`/documents/${fileId}/analyze`).then((r) => r.data);

export const retryAnalysis = (fileId: number) =>
  api.post<MessageResponse>(`/documents/${fileId}/retry-analysis`).then((r) => r.data);
