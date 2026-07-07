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
  EnviarRadicadoPendienteRequestDto,
  EnviarRadicadoPendienteResponseDto,
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

type LooseDto = Record<string, unknown>;

const pickValue = (
  source: LooseDto,
  keys: ReadonlyArray<string>,
): unknown => {
  for (const key of keys) {
    if (key in source) {
      return source[key];
    }
  }

  return undefined;
};

const toNullableNumber = (value: unknown): number | null => {
  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "string" && value.trim().length > 0) {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : null;
  }

  return null;
};

const toNullableString = (value: unknown): string | null => {
  if (typeof value === "string" && value.trim().length > 0) {
    return value.trim();
  }

  if (typeof value === "number" && Number.isFinite(value)) {
    return String(value);
  }

  return null;
};

const toBoolean = (value: unknown): boolean | null => {
  if (typeof value === "boolean") {
    return value;
  }

  if (typeof value === "number") {
    if (value === 1) return true;
    if (value === 0) return false;
  }

  if (typeof value === "string") {
    const normalized = value.trim().toLowerCase();
    if (["true", "1", "si", "sí"].includes(normalized)) return true;
    if (["false", "0", "no"].includes(normalized)) return false;
  }

  return null;
};

const toDestinoPostRegistro = (
  value: unknown,
): RadicacionDocumentalState["destinoPostRegistro"] =>
  value === "resumen" || value === "documentos" ? value : undefined;

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
  if (!dto) {
    return null;
  }

  const source = dto as unknown as LooseDto;
  const idEstadoRadicado = toNullableNumber(
    pickValue(source, [
      "idEstadoRadicado",
      "IdEstadoRadicado",
      "id_estado_radicado",
      "ID_ESTADO_RADICADO",
      "id",
      "Id",
    ]),
  );
  const estadoActual = toNullableNumber(
    pickValue(source, ["estadoActual", "EstadoActual", "estado_actual"]),
  );
  const tieneActivoEstado0 = toBoolean(
    pickValue(source, [
      "tieneActivoEstado0",
      "TieneActivoEstado0",
      "tiene_activo_estado0",
    ]),
  );
  const tieneTramiteDocumentalActivoEstado0 = toBoolean(
    pickValue(source, [
      "tieneTramiteDocumentalActivoEstado0",
      "TieneTramiteDocumentalActivoEstado0",
      "tiene_tramite_documental_activo_estado0",
    ]),
  );
  const hasActiveEstado0 =
    tieneActivoEstado0 === true ||
    tieneTramiteDocumentalActivoEstado0 === true ||
    estadoActual === 0;

  if (!hasActiveEstado0 || !idEstadoRadicado || idEstadoRadicado <= 0) {
    return null;
  }

  const requiereGestionDocumental = toBoolean(
    pickValue(source, [
      "requiereGestionDocumental",
      "RequiereGestionDocumental",
      "requiere_gestion_documental",
    ]),
  );

  return {
    idEstadoRadicado,
    idRadicado: toNullableNumber(
      pickValue(source, ["idRadicado", "IdRadicado", "id_radicado"]),
    ),
    consecutivoRadicado: toNullableString(
      pickValue(source, [
        "consecutivoRadicado",
        "ConsecutivoRadicado",
        "consecutivo_radicado",
      ]),
    ),
    idTareaWorkflow: toNullableNumber(
      pickValue(source, [
        "idTareaWorkflow",
        "IdTareaWorkflow",
        "id_tarea_workflow",
      ]),
    ),
    estadoActual: estadoActual === 1 ? 1 : 0,
    tramite: toNullableString(pickValue(source, ["tramite", "Tramite"])),
    remitente: toNullableString(pickValue(source, ["remitente", "Remitente"])),
    plantillaId: toNullableNumber(
      pickValue(source, ["plantillaId", "PlantillaId", "plantilla_id"]),
    ),
    tipoPlantillaId: toNullableNumber(
      pickValue(source, [
        "tipoPlantillaId",
        "TipoPlantillaId",
        "tipo_plantilla_id",
      ]),
    ),
    requiereGestionDocumental: requiereGestionDocumental !== false,
    tieneTramiteDocumentalActivoEstado0: true,
    destinoPostRegistro:
      toDestinoPostRegistro(
        pickValue(source, [
          "destinoPostRegistro",
          "DestinoPostRegistro",
          "destino_post_registro",
        ]),
      ) ?? "documentos",
    contextoDocumental:
      (pickValue(source, [
        "contextoDocumental",
        "ContextoDocumental",
        "contexto_documental",
      ]) as RadicacionDocumentalState["contextoDocumental"]) ?? null,
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
  request?: EnviarRadicadoPendienteRequestDto,
) => {
  const response = await clienteApi.post<
    ApiResponse<EnviarRadicadoPendienteResponseDto | null>
  >(buildRadicacionEnviarPendienteEndpoint(idEstadoRadicado), request ?? {});

  return (
    response.data?.data ??
    (response.data as { Data?: EnviarRadicadoPendienteResponseDto | null })
      ?.Data ??
    null
  );
};
