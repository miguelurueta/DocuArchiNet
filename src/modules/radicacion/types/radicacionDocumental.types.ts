export type RadicacionDestinoPostRegistro = "resumen" | "documentos";

export type RadicacionDocumentalEstadoActual = 0 | 1 | null;

export interface RadicacionDocumentalState {
  idEstadoRadicado: number | null;
  idRadicado?: number | null;
  consecutivoRadicado?: string | null;
  idTareaWorkflow?: number | null;
  estadoActual?: RadicacionDocumentalEstadoActual;
  requiereGestionDocumental: boolean;
  tieneTramiteDocumentalActivoEstado0: boolean;
  destinoPostRegistro?: RadicacionDestinoPostRegistro;
}

export interface RadicacionDocumentalContextValue
  extends RadicacionDocumentalState {
  setContextoDocumental: (value: RadicacionDocumentalState) => void;
  clearContextoDocumental: () => void;
}
