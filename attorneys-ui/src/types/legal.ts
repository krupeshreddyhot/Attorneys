export type Court = { courtId: string; courtName: string; courtCity?: string };
export type CaseType = { caseTypeId: string; name: string };

export type CaseDetailRow = {
  id?: number;
  caseNoId: number;
  stage?: string;
  previousDate?: string;
  nextDate?: string;
  ia?: string;
  iaStage?: string;
};

export type LegalCase = {
  caseNo: string;
  courtId?: string;
  court?: Court;
  caseTypeId?: string;
  caseType?: CaseType;
  appearingFor?: string;
  clientAddress?: string;
  clientPhone?: string;
  serialNo?: string;
  dateOfFiling?: string;
  dateOfAppearance?: string;
  otherParty?: string;
  counselForOtherParty?: string;
  remarks?: string;
  details?: CaseDetailRow[];
};

export type CasePayload = {
  caseNo: string;
  courtId?: string;
  caseTypeId?: string;
  appearingFor?: string;
  clientAddress?: string;
  clientPhone?: string;
  serialNo?: string;
  dateOfFiling?: string | null;
  dateOfAppearance?: string | null;
  otherParty?: string;
  counselForOtherParty?: string;
  remarks?: string;
  details?: {
    caseNoId: number;
    stage?: string;
    previousDate?: string | null;
    nextDate?: string | null;
    ia?: string;
    iaStage?: string;
  }[];
};

export type CourtWiseRow = {
  caseNo: string;
  courtName?: string;
  caseType?: string;
  appearingFor?: string;
  serialNo?: string;
  dateOfFiling?: string;
  latestNextDate?: string;
  hearingCount: number;
};

export type ReportRow = {
  caseNo: string;
  courtName?: string;
  caseType?: string;
  appearingFor?: string;
  stage?: string;
  previousDate?: string;
  nextDate?: string;
  ia?: string;
  iaStage?: string;
  serialNo?: string;
};

export type CaseDocument = {
  fileId: number;
  caseNo: string;
  fileName: string;
  description?: string;
  fileType?: string;
  fileSizeBytes: number;
  uploadedAtUtc: string;
};
