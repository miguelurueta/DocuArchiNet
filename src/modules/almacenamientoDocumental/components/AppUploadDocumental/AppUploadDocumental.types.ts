import type {
  AlmacenarDocumentoResponse,
  CampoIndexacionStorage,
} from "../../types/almacenamientoDocumental.types";

export type UploadDocumentalProcessKey = string;

export type UploadDocumentalContext = {
  nombreGabinete: string;
  idExpediente?: number;
  idTipoExpediente?: number;
  idUnidadConservacion?: number;
  idClaseDocumento?: number;
  idTareaWorkflow?: number;
  idRutaWorkflow?: number;
  idRespuesta?: number;
  tipoAdjunta?: number;
  estadoAdjunto?: number;
  estadoRelacionado?: number;
  numeroDocumentoRelacionado?: number;
  idImagen?: number;
  nameModulo?: string;
  camposIndexacion?: Array<{
    nombreCampo: string;
    valor?: string;
    esObligatorio?: boolean;
  }>;
};

export type UploadDocumentalConfig = {
  accept: string;
  allowedExtensions: string[];
  maxSizeBytes: number;
  multiple: boolean;
  requiereTipologia: boolean;
  requiereFechaCarga: boolean;
  fechaCargaObligatoria?: boolean;
  validationMode?: "reject" | "queue-with-error";
  preferredChunkSizeBytes?: number;
};

export type TipoDocumentalOption = {
  idTipoDocumento: number;
  nombreTipoDocumento: string;
};

export type UploadDocumentalFileMetadata = {
  idTipoDocumento?: number;
  nombreTipoDocumento?: string;
  numeroPaginas?: number;
  fechaCarga?: string;
  error?: string;
  warning?: string;
  suggestionConfidence?: number;
  tipologiaManual?: boolean;
};

export type UploadDocumentalInterfaceRegistration =
  | {
      kind: "production-document-row";
      idRegistro: number;
      idImagen?: number;
      nombreArchivo: string;
      fecha?: string;
      tipoDocumental?: string;
      nombreGabinete?: string;
      alias?: string;
      estadoFirmaDigital?: string;
      iconName?: string;
    }
  | {
      kind: "related-document-row" | "workflow-document-row";
      nombreGabinete?: string;
      idImagen?: number;
      radicado?: string;
      tipoDocumental?: string;
      nombreTipoDocumental?: string;
      idTareaWorkflow?: number;
      estadoFirmaDigital?: string;
      iconName?: string;
    }
  | {
      kind: "migration-preview";
      url: string;
      idRegistro?: number;
    }
  | {
      kind: "page-counter";
      contadorPaginas: number;
    }
  | {
      kind: "traffic-light";
      urlImagenSemaforo: string;
    }
  | {
      kind: "dropdown-option";
      text: string;
      value: string | number;
      target?: "respuesta" | "pqrs" | "anexo";
    }
  | {
      kind: "document-version-row";
      idImagen?: number;
      idVersionDocumento?: number;
      idRegistroVersion?: number;
      tipoDocumento?: string;
      estadoFirmaDigital?: string;
      iconName?: string;
      dbt?: number;
      fechaRegistroVersion?: string;
    }
  | {
      kind: "table-import-result";
      rowTable: unknown;
      fieldTable: unknown;
      source: "rue-sii" | "virtual-sii";
    }
  | {
      kind: "raw";
      raw: unknown;
    };

export type AlmacenarDocumentoStoredResult = AlmacenarDocumentoResponse & {
  fileUid: string;
  fileName: string;
  metadata: UploadDocumentalFileMetadata;
  interfaceRegistration?: UploadDocumentalInterfaceRegistration[];
  rawBackendResult?: unknown;
};

export type UploadDocumentalBatchSummary = {
  total: number;
  stored: number;
  failed: number;
  skipped: number;
  cancelled: number;
  results: AlmacenarDocumentoStoredResult[];
};

export type AppUploadDocumentalModoDocumento =
  | "default"
  | "adjunto-radicado"
  | "relacionado-radicado"
  | "formato-respuesta"
  | "documento-libre-respuesta";

export type AppUploadDocumentalProps = {
  proceso: UploadDocumentalProcessKey;
  context: UploadDocumentalContext;
  title?: string;
  open?: boolean;
  embedded?: boolean;
  tipologiaObligatoria?: boolean;
  autoSuggestTipologia?: boolean;
  requiereFechaCarga?: boolean;
  fechaCargaObligatoria?: boolean;
  allowSingleFileStore?: boolean;
  validationMode?: "reject" | "queue-with-error";
  modoDocumento?: AppUploadDocumentalModoDocumento;
  loadConfig: (input: {
    proceso: UploadDocumentalProcessKey;
    context: UploadDocumentalContext;
    modoDocumento?: AppUploadDocumentalModoDocumento;
  }) => Promise<UploadDocumentalConfig>;
  loadTiposDocumentales: (input: {
    proceso: UploadDocumentalProcessKey;
    context: UploadDocumentalContext;
  }) => Promise<TipoDocumentalOption[]>;
  onStored?: (result: AlmacenarDocumentoStoredResult) => void;
  onInterfaceRegistration?: (events: UploadDocumentalInterfaceRegistration[]) => void;
  onBatchComplete?: (summary: UploadDocumentalBatchSummary) => void;
  onError?: (error: unknown) => void;
  onClose?: () => void;
};

export type UploadDocumentalRuntimeFileState =
  | "queued"
  | "validating"
  | "ready"
  | "uploading"
  | "completing"
  | "storing"
  | "done"
  | "warning"
  | "error"
  | "cancelled"
  | "removed";

export type BuildAlmacenarDocumentoRequestInput = {
  context: UploadDocumentalContext;
  metadata: UploadDocumentalFileMetadata;
  fileName: string;
  requestId: string;
  camposIndexacion?: CampoIndexacionStorage[] | null;
};
