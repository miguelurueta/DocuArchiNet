import { createContext, useMemo, useState } from "react";
import type { ReactNode } from "react";
import type { AppUploadFile } from "../../../app/Components/UI/AppUpload/AppUpload";

export type GestionRespuestaDocumentosState = {
  files: AppUploadFile[];
  setFiles: (files: AppUploadFile[]) => void;
};

export const GestionRespuestaDocumentosContext =
  createContext<GestionRespuestaDocumentosState | null>(null);

export function GestionRespuestaDocumentosProvider({ children }: { children: ReactNode }) {
  const [files, setFiles] = useState<AppUploadFile[]>([]);

  const value = useMemo<GestionRespuestaDocumentosState>(
    () => ({
      files,
      setFiles,
    }),
    [files],
  );

  return (
    <GestionRespuestaDocumentosContext.Provider value={value}>
      {children}
    </GestionRespuestaDocumentosContext.Provider>
  );
}

