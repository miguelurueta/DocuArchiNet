import { useContext, useMemo } from "react";
import { GestionRespuestaDocumentosContext } from "../context/GestionRespuestaDocumentosContext";

export const useGestionRespuestaDocumentos = () => {
  const ctx = useContext(GestionRespuestaDocumentosContext);

  return useMemo(() => {
    if (!ctx) {
      return {
        files: [],
        setFiles: () => undefined,
        available: false,
      } as const;
    }

    return { ...ctx, available: true } as const;
  }, [ctx]);
};

