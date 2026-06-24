export const DocumentAnalysisStatus = {
  Pending: 1,
  Processing: 2,
  Completed: 3,
  Failed: 4,
} as const;

export type DocumentAnalysisStatus =
  (typeof DocumentAnalysisStatus)[keyof typeof DocumentAnalysisStatus];

export type AiDisplayStatus = "notAnalyzed" | "processing" | "ready" | "failed";

export type DocumentAnalysisDto = {
  fileId: number;
  status: DocumentAnalysisStatus;
  summary: string | null;
  keyPoints: string[];
  parties: string[];
  importantDates: string[];
  nextActions: string[];
  aiModel: string | null;
  promptVersion: string | null;
  createdUtc: string;
  processedUtc: string | null;
};

export type MessageResponse = {
  message: string;
};

export const mapAnalysisToDisplayStatus = (analysis: DocumentAnalysisDto | null): AiDisplayStatus => {
  if (!analysis) return "notAnalyzed";
  switch (analysis.status) {
    case DocumentAnalysisStatus.Completed:
      return "ready";
    case DocumentAnalysisStatus.Failed:
      return "failed";
    case DocumentAnalysisStatus.Pending:
    case DocumentAnalysisStatus.Processing:
      return "processing";
    default:
      return "notAnalyzed";
  }
};

export const AI_DISPLAY_STATUS_LABELS: Record<AiDisplayStatus, string> = {
  notAnalyzed: "Not Analyzed",
  processing: "Processing",
  ready: "Ready",
  failed: "Failed",
};

export const AI_DISPLAY_STATUS_CHIP_COLOR: Record<
  AiDisplayStatus,
  "default" | "warning" | "success" | "error"
> = {
  notAnalyzed: "default",
  processing: "warning",
  ready: "success",
  failed: "error",
};

const MONTHS = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

export const formatProcessedDate = (iso: string | null): string => {
  if (!iso) return "—";
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) return "—";
  const day = date.getDate().toString().padStart(2, "0");
  const month = MONTHS[date.getMonth()];
  const year = date.getFullYear();
  const time = date.toLocaleTimeString("en-US", {
    hour: "numeric",
    minute: "2-digit",
    hour12: true,
  });
  return `${day}-${month}-${year} ${time}`;
};
