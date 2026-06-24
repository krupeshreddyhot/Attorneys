import api from "./api";
import type { CasePayload, CaseType, Court, LegalCase, ReportRow, CaseDocument, CourtWiseRow } from "../types/legal";

export const fetchCourts = () => api.get<Court[]>("/courts").then((r) => r.data);
export const fetchCaseTypes = () => api.get<CaseType[]>("/casetypes").then((r) => r.data);
export const fetchCase = (caseNo: string) => api.get<LegalCase>(`/cases/${encodeURIComponent(caseNo)}`).then((r) => r.data);
export const createCase = (payload: CasePayload) => api.post("/cases", payload);
export const updateCase = (caseNo: string, payload: CasePayload) =>
  api.put(`/cases/${encodeURIComponent(caseNo)}`, payload);

export const fetchDiaryReport = (date?: string) =>
  api.get<{ date: string; rows: ReportRow[] }>("/reports/diary", { params: date ? { date } : {} }).then((r) => r.data);

export const fetchCourtWiseReport = (courtId: string) =>
  api.get<CourtWiseRow[]>("/reports/court-wise", { params: { courtId } }).then((r) => r.data);

export const fetchPendingReport = () => api.get<ReportRow[]>("/reports/pending").then((r) => r.data);

export const fetchDocuments = (caseNo?: string) =>
  api.get<CaseDocument[]>("/documents", { params: caseNo ? { caseNo } : {} }).then((r) => r.data);

export const uploadDocument = (formData: FormData) =>
  api.post("/documents/upload", formData, { headers: { "Content-Type": "multipart/form-data" } });

export const deleteDocument = (fileId: number) => api.delete(`/documents/${fileId}`);
