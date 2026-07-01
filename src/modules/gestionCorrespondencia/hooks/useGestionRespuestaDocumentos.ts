import { useContext, useMemo } from "react";
import {
  GestionRespuestaDocumentosContext,
  type GestionRespuestaDocumentosState,
} from "../context/GestionRespuestaDocumentosContext";

type GestionRespuestaDocumentosHookState = GestionRespuestaDocumentosState & {
  available: boolean;
};

const noopSetFiles: GestionRespuestaDocumentosState["setFiles"] = () => undefined;
const noopReloadGabinete = async () => undefined;
const noopRefreshDocumentos = () => undefined;

export const useGestionRespuestaDocumentos = () => {
  const ctx = useContext(GestionRespuestaDocumentosContext);

  return useMemo<GestionRespuestaDocumentosHookState>(() => {
    if (!ctx) {
      return {
        idTareaWf: undefined,
        idRutaWf: undefined,
        radicado: undefined,
        idRespuestaRadicado: undefined,
        nombreGabinete: undefined,
        gabineteLoading: false,
        gabineteError: undefined,
        reloadGabinete: noopReloadGabinete,
        documentosRefreshKey: 0,
        refreshDocumentos: noopRefreshDocumentos,
        files: [],
        setFiles: noopSetFiles,
        available: false,
      };
    }

    return { ...ctx, available: true };
  }, [ctx]);
};
