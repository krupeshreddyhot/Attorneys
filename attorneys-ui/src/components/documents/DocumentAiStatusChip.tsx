import { useEffect, useRef, useState } from "react";
import { Chip, Skeleton } from "@mui/material";
import { fetchDisplayStatus, getCachedDisplayStatus } from "../../services/documentAnalysisApi";
import {
  AI_DISPLAY_STATUS_CHIP_COLOR,
  AI_DISPLAY_STATUS_LABELS,
  type AiDisplayStatus,
} from "../../types/documentAnalysis";

type DocumentAiStatusChipProps = {
  fileId: number;
  statusOverride?: AiDisplayStatus;
};

const DocumentAiStatusChip = ({ fileId, statusOverride }: DocumentAiStatusChipProps) => {
  const cellRef = useRef<HTMLDivElement>(null);
  const [status, setStatus] = useState<AiDisplayStatus | "loading">(() => {
    if (statusOverride) return statusOverride;
    return getCachedDisplayStatus(fileId) ?? "loading";
  });
  const fetchedRef = useRef(false);

  useEffect(() => {
    if (statusOverride) {
      setStatus(statusOverride);
    }
  }, [statusOverride]);

  useEffect(() => {
    if (statusOverride) return;

    const cached = getCachedDisplayStatus(fileId);
    if (cached) {
      setStatus(cached);
      return;
    }

    const element = cellRef.current;
    if (!element) return;

    const observer = new IntersectionObserver(
      (entries) => {
        const isVisible = entries.some((entry) => entry.isIntersecting);
        if (!isVisible || fetchedRef.current) return;

        fetchedRef.current = true;
        observer.disconnect();

        void fetchDisplayStatus(fileId)
          .then((result) => setStatus(result))
          .catch(() => setStatus("notAnalyzed"));
      },
      { rootMargin: "100px" },
    );

    observer.observe(element);
    return () => observer.disconnect();
  }, [fileId, statusOverride]);

  if (status === "loading") {
    return (
      <div ref={cellRef}>
        <Skeleton variant="rounded" width={88} height={24} />
      </div>
    );
  }

  return (
    <div ref={cellRef}>
      <Chip
        label={AI_DISPLAY_STATUS_LABELS[status]}
        color={AI_DISPLAY_STATUS_CHIP_COLOR[status]}
        size="small"
        variant={status === "notAnalyzed" ? "outlined" : "filled"}
      />
    </div>
  );
};

export default DocumentAiStatusChip;
