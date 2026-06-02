import { useContext, useMemo } from "react";
import type { AppUploadFile } from "../../../app/Components/UI/AppUpload/AppUpload";
import { GestionRespuestaDocumentosContext } from "../context/GestionRespuestaDocumentosContext";

type GestionRespuestaDocumentosHookState = {
  files: AppUploadFile[];
  setFiles: (files: AppUploadFile[]) => void;
  available: boolean;
};

export const useGestionRespuestaDocumentos = () => {
  const ctx = useContext(GestionRespuestaDocumentosContext);

  return useMemo<GestionRespuestaDocumentosHookState>(() => {
    if (!ctx) {
      return {
        files: [],
        setFiles: () => undefined,
        available: false,
      };
    }

    return { ...ctx, available: true };
  }, [ctx]);
};

