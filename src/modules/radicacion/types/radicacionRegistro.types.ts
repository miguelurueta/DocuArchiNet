import type {
  RadicacionContextoDocumentalDetalle,
  RadicacionDestinoPostRegistro,
} from "./radicacionDocumental.types";

export type RadicacionRegistroFormValue =
  | string
  | number
  | boolean
  | null
  | undefined
  | {
      value?: string | number | null;
      label?: unknown;
    }
  | Array<{
      value?: string | number | null;
      label?: unknown;
    }>
  | {
      format?: (pattern?: string) => string;
      toDate?: () => Date;
    };

export type RadicacionRegistroFormValues = Record<
  string,
  RadicacionRegistroFormValue
>;

export interface RegistrarRadicacionPersonaRemitenteDto {
  Nombre: string;
  id_Dest_Ext: number;
}

export interface RegistrarRadicacionPersonaDestinatarioDto {
  Destinatario: string;
  id_Remit_Dest_Int: number;
}

export interface RegistrarRadicacionTipoTramiteDto {
  Descripcion: string;
  tipo_doc_entrante: number;
}

export interface RegistrarRadicacionFlujoTrabajoDto {
  NombreFlujo: string;
  id_tipo_flujo_workflow: number;
}

export interface RegistrarRadicacionTipoRadicadoDto {
  TipoRadicacion: string;
  IdTipoRadicado: number;
}

export interface RegistrarRadicacionTipoPlantillaDto {
  TipoPlantillaRadicado: string;
  IdTipoPlantillaRdicado: number;
}

export interface RegistrarRadicacionExpedienteDto {
  Expediente: string;
  idExpediente: number;
}

export interface RegistrarRadicacionRelacionadoDto {
  consecutivoRelacionadohijo: string;
  idregistroradicadohijo: number;
  idplantillahijo: number;
}

export interface RegistrarRadicacionCampoDto {
  IdDetallePlantillaRadicado: number;
  NombreCampo: string;
  Valor: string;
}

export interface RegistrarRadicacionEntranteRequestDto {
  tipoModuloRadicacion: number;
  ASUNTO: string;
  Remitente: RegistrarRadicacionPersonaRemitenteDto;
  Destinatario: RegistrarRadicacionPersonaDestinatarioDto;
  Tipo_tramite: RegistrarRadicacionTipoTramiteDto;
  RE_flujo_trabajo: RegistrarRadicacionFlujoTrabajoDto;
  TipoRadicado: RegistrarRadicacionTipoRadicadoDto;
  TipoPlantillaRadicado: RegistrarRadicacionTipoPlantillaDto;
  expedienteRelacionado: RegistrarRadicacionExpedienteDto;
  radicadoRelacionados: RegistrarRadicacionRelacionadoDto[];
  ANEXOS_COR: string;
  FECHALIMITERESPUESTA: string;
  numeroFolios?: number | null;
  Campos: RegistrarRadicacionCampoDto[];
}

export interface ReturnRegistraRadicacionDto {
  ConsecutivoRadicado?: string | null;
  IdRadicado?: number | null;
  IdEstadoRadicado?: number | null;
  consecutivoRadicado?: string | null;
  idRadicado?: number | null;
  idEstadoRadicado?: number | null;
}

export interface RegistrarRadicacionEntranteResponseDto {
  ConsecutivoRadicado?: string | null;
  ReturnRegistraRadicacion?: ReturnRegistraRadicacionDto | null;
  EstadoAsignacion?: string | null;
  Alertas?: string[] | null;
  MetadataOperativa?: Record<string, unknown> | null;
  consecutivoRadicado?: string | null;
  returnRegistraRadicacion?: ReturnRegistraRadicacionDto | null;
  estadoAsignacion?: string | null;
  alertas?: string[] | null;
  metadataOperativa?: Record<string, unknown> | null;
}

export interface AppResponses<T> {
  success?: boolean;
  Success?: boolean;
  message?: string | null;
  Message?: string | null;
  data?: T | null;
  Data?: T | null;
  errors?: unknown[] | null;
  Errors?: unknown[] | null;
  meta?: unknown;
  Meta?: unknown;
}

export interface RadicacionPostRegistroState {
  consecutivoRadicado: string;
  idRadicado: number;
  idEstadoRadicado: number;
  estadoAsignacion: string;
  metadataOperativa: Record<string, unknown>;
  requiereGestionDocumental: boolean;
  tieneTramiteDocumentalActivoEstado0: boolean;
  destinoPostRegistro: RadicacionDestinoPostRegistro;
  contextoDocumental?: RadicacionContextoDocumentalDetalle | null;
}
