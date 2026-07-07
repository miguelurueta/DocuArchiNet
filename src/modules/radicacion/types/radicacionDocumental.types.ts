export type RadicacionDestinoPostRegistro = "resumen" | "documentos";

export type RadicacionDocumentalEstadoActual = 0 | 1 | null;

export interface RadicacionContextoDocumentalDetalle {
  idGabinete?: number | null;
  nombreGabinete?: string | null;
  idTipoTramite?: number | null;
  nombreTramite?: string | null;
  utilEstadoPendienteRad?: boolean;
}

export interface RadicacionDocumentalState {
  idEstadoRadicado: number | null;
  idRadicado?: number | null;
  consecutivoRadicado?: string | null;
  idTareaWorkflow?: number | null;
  estadoActual?: RadicacionDocumentalEstadoActual;
  tramite?: string | null;
  remitente?: string | null;
  plantillaId?: number | null;
  tipoPlantillaId?: number | null;
  requiereGestionDocumental: boolean;
  tieneTramiteDocumentalActivoEstado0: boolean;
  destinoPostRegistro?: RadicacionDestinoPostRegistro;
  contextoDocumental?: RadicacionContextoDocumentalDetalle | null;
  metadataOperativa?: {
    tramite?: string | null;
    remitente?: string | null;
    plantillaId?: number | null;
    workflowFueCreado?: boolean;
  } | null;
}

export interface RadicacionDocumentalContextValue
  extends RadicacionDocumentalState {
  setContextoDocumental: (value: RadicacionDocumentalState) => void;
  clearContextoDocumental: () => void;
}

export interface RadicacionPendienteEstadoActivoDto {
  tieneActivoEstado0: boolean;
  idEstadoRadicado?: number | null;
  idRadicado?: number | null;
  consecutivoRadicado?: string | null;
  idTareaWorkflow?: number | null;
  estadoActual?: 0 | null;
  tramite?: string | null;
  remitente?: string | null;
  plantillaId?: number | null;
  tipoPlantillaId?: number | null;
  requiereGestionDocumental: boolean;
  tieneTramiteDocumentalActivoEstado0: boolean;
  destinoPostRegistro: RadicacionDestinoPostRegistro;
  contextoDocumental?: RadicacionContextoDocumentalDetalle | null;
}
