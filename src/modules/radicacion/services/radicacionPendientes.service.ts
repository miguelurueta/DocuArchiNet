import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";
import type {
  ApiResponse as DynamicUiApiResponse,
  DynamicUiTableDto,
} from "../../../app/Components/UI/AppTable/types/dynamicUiTable.types";
import type {
  RadicacionDocumentalState,
  RadicacionPendienteEstadoActivoDto,
} from "../types/radicacionDocumental.types";
import type {
  TomarRadicadoPendienteRequestDto,
  TomarRadicadoPendienteResponseDto,
} from "../types/radicacionPendientes.types";

export const RADICACION_ESTADO_ACTIVO_ENDPOINT =
  "/api/radicacion/pendientes/estado-activo";
export const RADICACION_PENDIENTES_LISTADO_ENDPOINT =
  "/api/tramite/tramites/apListaRadicadosPendientes";
export const RADICACION_PENDIENTES_CONTADOR_ENDPOINT =
  "/api/radicacion/pendientes/contador";

export const buildRadicacionTomarPendienteEndpoint = (
  idEstadoRadicado: number | string,
) => `/api/radicacion/pendientes/${idEstadoRadicado}/tomar`;

export const buildRadicacionEnviarPendienteEndpoint = (
  idEstadoRadicado: number | string,
) => `/api/radicacion/pendientes/${idEstadoRadicado}/enviar-pendiente`;

export type RadicacionPendientesTableRequest = {
  SearchType: number;
  Search: string;
  SortField: string;
  SortDir: "ASC" | "DESC";
  Page: number;
  PageSize: number;
  IncludeConfig: boolean;
};

export type RadicacionPendientesContadorDto = {
  totalPendientes?: number | null;
  TotalPendientes?: number | null;
  cantidad?: number | null;
  Cantidad?: number | null;
  total?: number | null;
  Total?: number | null;
};

export type EnviarRadicadoPendienteResponseDto = {
  idEstadoRadicado?: number | null;
  estadoActual?: number | null;
  enviadoAPendiente?: boolean | null;
};

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
  if ("Data" in payload) {
    return (
      (payload as { Data?: RadicacionPendienteEstadoActivoDto | null }).Data ??
      null
    );
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

export const fetchRadicacionPendientesTable = async (
  request: RadicacionPendientesTableRequest,
) => {
  const response = await clienteApi.post<
    DynamicUiApiResponse<DynamicUiTableDto | null>
  >(RADICACION_PENDIENTES_LISTADO_ENDPOINT, request);

  return response.data;
};

export const fetchRadicacionPendientesContador = async () => {
  const response = await clienteApi.get<
    ApiResponse<RadicacionPendientesContadorDto | null>
  >(RADICACION_PENDIENTES_CONTADOR_ENDPOINT);

  return (
    response.data?.data ??
    (response.data as { Data?: RadicacionPendientesContadorDto | null })?.Data ??
    null
  );
};

export const tomarRadicacionPendiente = async (
  idEstadoRadicado: number | string,
  request?: TomarRadicadoPendienteRequestDto,
) => {
  const response = await clienteApi.post<
    ApiResponse<TomarRadicadoPendienteResponseDto | null>
  >(buildRadicacionTomarPendienteEndpoint(idEstadoRadicado), request ?? {});

  return (
    response.data?.data ??
    (response.data as { Data?: TomarRadicadoPendienteResponseDto | null })
      ?.Data ??
    null
  );
};

export const enviarRadicacionPendiente = async (
  idEstadoRadicado: number | string,
) => {
  const response = await clienteApi.post<
    ApiResponse<EnviarRadicadoPendienteResponseDto | null>
  >(buildRadicacionEnviarPendienteEndpoint(idEstadoRadicado));

  return (
    response.data?.data ??
    (response.data as { Data?: EnviarRadicadoPendienteResponseDto | null })
      ?.Data ??
    null
  );
};
