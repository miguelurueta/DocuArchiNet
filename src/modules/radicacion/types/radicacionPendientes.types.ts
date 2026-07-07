import type { AppTableRow } from "../../../app/Components/UI/AppTable/AppTable.types";
import type {
  RadicacionContextoDocumentalDetalle,
  RadicacionDestinoPostRegistro,
} from "./radicacionDocumental.types";

export const RADICACION_PENDIENTE_ACTION_ID = "asignacion-tarea";

export type TomarRadicadoPendienteRequestDto = {
  idTareaWorkflow?: number | null;
};

export type TomarRadicadoPendienteResponseDto = {
  tieneActivoEstado0?: boolean;
  idEstadoRadicado?: number | null;
  idRadicado?: number | null;
  consecutivoRadicado?: string | null;
  idTareaWorkflow?: number | null;
  estadoAnterior?: 1 | number | null;
  estadoActual?: 0 | number | null;
  tramite?: string | null;
  remitente?: string | null;
  plantillaId?: number | null;
  tipoPlantillaId?: number | null;
  requiereGestionDocumental?: boolean;
  tieneTramiteDocumentalActivoEstado0?: boolean;
  destinoPostRegistro?: RadicacionDestinoPostRegistro;
  contextoDocumental?: RadicacionContextoDocumentalDetalle | null;
  metadataOperativa?: {
    tramite?: string | null;
    remitente?: string | null;
    plantillaId?: number | null;
    workflowFueCreado?: boolean;
  } | null;
};

export type EnviarRadicadoPendienteRequestDto = {
  motivo?: string;
};

export type EnviarRadicadoPendienteResponseDto = {
  idEstadoRadicado?: number | null;
  consecutivoRadicado?: string | null;
  estadoAnterior?: 0 | number | null;
  estadoActual?: 1 | number | null;
  tieneTramiteDocumentalActivoEstado0?: boolean;
  destinoPostRegistro?: RadicacionDestinoPostRegistro;
  mensaje?: string | null;
};

export type RadicacionPendienteActionPayload = {
  idEstadoRadicado: number;
  idTareaWorkflow: number | null;
  consecutivoRadicado: string | null;
};

const pickValue = (
  row: AppTableRow,
  keys: ReadonlyArray<string>,
): unknown => {
  for (const key of keys) {
    if (key in row) {
      return row[key];
    }
  }

  return undefined;
};

export const toNullableNumber = (value: unknown): number | null => {
  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "string" && value.trim().length > 0) {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : null;
  }

  return null;
};

export const toNullableString = (value: unknown): string | null => {
  if (typeof value === "string" && value.trim().length > 0) {
    return value.trim();
  }

  if (typeof value === "number" && Number.isFinite(value)) {
    return String(value);
  }

  return null;
};

export const extractRadicacionPendienteActionPayload = (
  row: AppTableRow,
): RadicacionPendienteActionPayload | null => {
  const idEstadoRadicado = toNullableNumber(
    pickValue(row, ["id_estado_radicado", "idEstadoRadicado", "IdEstadoRadicado", "id"]),
  );

  if (idEstadoRadicado === null) {
    return null;
  }

  return {
    idEstadoRadicado,
    idTareaWorkflow: toNullableNumber(
      pickValue(row, ["id_tarea_workflow", "idTareaWorkflow", "IdTareaWorkflow"]),
    ),
    consecutivoRadicado: toNullableString(
      pickValue(row, [
        "consecutivo_radicado",
        "consecutivoRadicado",
        "ConsecutivoRadicado",
        "RADICADO",
      ]),
    ),
  };
};
