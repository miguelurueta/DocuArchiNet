import { createContext } from "react";
import type {
  RadicacionDocumentalContextValue,
  RadicacionDocumentalState,
} from "../types/radicacionDocumental.types";

export const RADICACION_DOCUMENTAL_INITIAL_STATE: RadicacionDocumentalState = {
  idEstadoRadicado: null,
  idRadicado: null,
  consecutivoRadicado: null,
  idTareaWorkflow: null,
  estadoActual: null,
  tramite: null,
  remitente: null,
  plantillaId: null,
  tipoPlantillaId: null,
  requiereGestionDocumental: false,
  tieneTramiteDocumentalActivoEstado0: false,
  destinoPostRegistro: "resumen",
  contextoDocumental: null,
};

export const isRadicacionDocumentalActivaEstado0 = (
  state: RadicacionDocumentalState,
) =>
  state.estadoActual === 0 &&
  state.requiereGestionDocumental === true &&
  Number(state.idEstadoRadicado ?? 0) > 0;

export const normalizeRadicacionDocumentalState = (
  value: RadicacionDocumentalState,
): RadicacionDocumentalState => {
  const normalized = {
    ...RADICACION_DOCUMENTAL_INITIAL_STATE,
    ...value,
  };

  return {
    ...normalized,
    tieneTramiteDocumentalActivoEstado0:
      normalized.tieneTramiteDocumentalActivoEstado0 === true &&
      isRadicacionDocumentalActivaEstado0(normalized),
  };
};

export const RadicacionDocumentalContext =
  createContext<RadicacionDocumentalContextValue | null>(null);
