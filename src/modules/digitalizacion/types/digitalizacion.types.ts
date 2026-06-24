import type { DigitalizacionScannerClient } from "../infrastructure/dynamsoft";
import type { DigitalizacionApiClient, DigitalizacionApiError } from "./digitalizacionApi.types";

export type DigitalizacionModo = "crear" | "adjuntar";

export type DigitalizacionContext = {
  modo: DigitalizacionModo;
  nombreGabinete: string;
  radicado?: string;
  idTramite?: number;
  tipoTramite?: string;
  idTareaWorkflow?: number;
  idRutaWorkflow?: number;
  idDocumentoDestino?: number;
  requiereMetadata?: boolean;
  titulo?: string;
  sourceModule?: string;
};

export type DigitalizacionTrdMetadata = {
  idArea?: number;
  idSerie?: number;
  idSubSerie?: number;
  idTipoDocumento?: number;
  nombreArea?: string;
  nombreSerie?: string;
  nombreSubSerie?: string;
  nombreTipoDocumento?: string;
};

export type DigitalizacionResult =
  | {
      accion: "documento-creado";
      idDocumento: number;
      nombreGabinete: string;
      numeroPaginas?: number;
      trd?: DigitalizacionTrdMetadata;
    }
  | {
      accion: "documento-adjuntado";
      idDocumento: number;
      nombreGabinete: string;
      numeroPaginas?: number;
    }
  | {
      accion: "cancelado";
    };

export type DigitalizacionFunctionalErrorCode =
  | "CONTEXT_REQUIRED"
  | "INVALID_MODE"
  | "NOMBRE_GABINETE_REQUIRED"
  | "ID_DOCUMENTO_DESTINO_REQUIRED"
  | "OPERATION_NOT_READY"
  | "PDF_REQUIRED"
  | "PAGES_REQUIRED"
  | "METADATA_REQUIRED"
  | "SUBMIT_ALREADY_IN_PROGRESS"
  | "ADJUNTAR_NOT_ALLOWED";

export type DigitalizacionFunctionalError = {
  code: DigitalizacionFunctionalErrorCode;
  message: string;
  field?: keyof DigitalizacionContext;
};

export type DigitalizacionDocumentalError = DigitalizacionFunctionalError | DigitalizacionApiError;

export type DigitalizacionDocumentalProps = {
  open: boolean;
  context: DigitalizacionContext | null;
  scannerClient?: DigitalizacionScannerClient;
  apiClient?: DigitalizacionApiClient;
  onClose: () => void;
  onCompleted: (result: DigitalizacionResult) => void;
  onError?: (error: DigitalizacionDocumentalError) => void;
};

export type DigitalizacionDocumentalWorkspaceProps = {
  active?: boolean;
  context: DigitalizacionContext | null;
  scannerClient?: DigitalizacionScannerClient;
  apiClient?: DigitalizacionApiClient;
  onCancel?: () => void;
  onCompleted: (result: DigitalizacionResult) => void;
  onError?: (error: DigitalizacionDocumentalError) => void;
  showLegacyFooter?: boolean;
  showSummary?: boolean;
  showStateBadge?: boolean;
};

export type DigitalizacionScannerState = {
  selectedScannerId: string | null;
  runtimeAvailable: boolean;
  pages: DigitalizacionScannedPage[];
  generatedPdf: DigitalizacionGeneratedPdf | null;
};

export type DigitalizacionScannedPage = {
  id: string;
  index: number;
  label: string;
};

export type DigitalizacionGeneratedPdf = {
  fileName: string;
  sizeBytes: number;
  pageCount: number;
};

export type DigitalizacionMetadataState = {
  required: boolean;
  checklistReady: boolean;
  trd: DigitalizacionTrdMetadata | null;
  errors: string[];
};

export type DigitalizacionOperationState =
  | { status: "idle" }
  | { status: "scanning" }
  | { status: "generatingPdf" }
  | { status: "uploading" }
  | { status: "resolvingMetadata" }
  | { status: "saving" }
  | { status: "completed" }
  | { status: "error"; error: DigitalizacionFunctionalError };

export type DigitalizacionState = {
  context: DigitalizacionContext | null;
  validationError: DigitalizacionFunctionalError | null;
  scanner: DigitalizacionScannerState;
  metadata: DigitalizacionMetadataState;
  operation: DigitalizacionOperationState;
  generation: number;
};
