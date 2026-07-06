import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";
import type {
  RadicacionDocumentalState,
  RadicacionPendienteEstadoActivoDto,
} from "../types/radicacionDocumental.types";

export const RADICACION_ESTADO_ACTIVO_ENDPOINT =
  "/api/radicacion/pendientes/estado-activo";

const resolveData = (
  payload:
    | ApiResponse<RadicacionPendienteEstadoActivoDto | null>
    | RadicacionPendienteEstadoActivoDto
    | null
    | undefined,
): RadicacionPendienteEstadoActivoDto | null => {
  if (!payload) return null;
  if ("data" in payload) {
    return payload.data ?? null;
  }
  return payload;
};

export const mapEstadoActivoToDocumentalState = (
  dto: RadicacionPendienteEstadoActivoDto | null,
): RadicacionDocumentalState | null => {
  if (!dto?.tieneActivoEstado0) {
    return null;
  }

  return {
    idEstadoRadicado: dto.idEstadoRadicado ?? null,
    idRadicado: dto.idRadicado ?? null,
    consecutivoRadicado: dto.consecutivoRadicado ?? null,
    idTareaWorkflow: dto.idTareaWorkflow ?? null,
    estadoActual: dto.estadoActual ?? null,
    tramite: dto.tramite ?? null,
    remitente: dto.remitente ?? null,
    plantillaId: dto.plantillaId ?? null,
    tipoPlantillaId: dto.tipoPlantillaId ?? null,
    requiereGestionDocumental: dto.requiereGestionDocumental === true,
    tieneTramiteDocumentalActivoEstado0:
      dto.tieneTramiteDocumentalActivoEstado0 === true,
    destinoPostRegistro: dto.destinoPostRegistro ?? "resumen",
    contextoDocumental: dto.contextoDocumental ?? null,
  };
};

export const fetchRadicacionEstadoActivo = async () => {
  const response = await clienteApi.get<
    ApiResponse<RadicacionPendienteEstadoActivoDto | null>
  >(RADICACION_ESTADO_ACTIVO_ENDPOINT);

  return resolveData(response.data);
};
