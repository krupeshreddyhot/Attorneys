import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { useParams } from "react-router-dom";
import { fetchFirmLanding, type FirmLanding } from "../services/publicService";

type FirmLandingContextValue = {
  firmCode?: string;
  landing?: FirmLanding;
  loading: boolean;
  notFound: boolean;
};

const FirmLandingContext = createContext<FirmLandingContextValue>({
  loading: false,
  notFound: false,
});

export const FirmLandingProvider = ({ children }: { children: ReactNode }) => {
  const { firmCode: rawCode } = useParams();
  const firmCode = rawCode?.toUpperCase();
  const [landing, setLanding] = useState<FirmLanding>();
  const [loading, setLoading] = useState(false);
  const [notFound, setNotFound] = useState(false);

  useEffect(() => {
    if (!firmCode) {
      setLanding(undefined);
      setNotFound(false);
      return;
    }

    let cancelled = false;
    setLoading(true);
    setNotFound(false);

    fetchFirmLanding(firmCode)
      .then((data) => {
        if (!cancelled) setLanding(data);
      })
      .catch(() => {
        if (!cancelled) {
          setLanding(undefined);
          setNotFound(true);
        }
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => {
      cancelled = true;
    };
  }, [firmCode]);

  const value = useMemo(
    () => ({ firmCode, landing, loading, notFound }),
    [firmCode, landing, loading, notFound],
  );

  return <FirmLandingContext.Provider value={value}>{children}</FirmLandingContext.Provider>;
};

export const useFirmLanding = () => useContext(FirmLandingContext);
