export type ConfiguracionUploadCorrespondenciaBackendItem = {
  IdConfigUploadGestion?: number;
  ExtensionUpload?: string;
  LengUpload?: number;
  NameProceso?: string;
  EstadoProceso?: number;
  idConfigUploadGestion?: number;
  extensionUpload?: string;
  lengUpload?: number;
  nameProceso?: string;
  estadoProceso?: number;
};

export type ConfiguracionUploadCorrespondenciaResponse = {
  success: boolean;
  message: string;
  data: ConfiguracionUploadCorrespondenciaBackendItem[];
  meta?: unknown;
  errors?: unknown[];
};

export type ConfiguracionUploadCorrespondencia = {
  nameProceso: "CORRESPO";
  accept: string;
  allowedExtensions: string[];
  maxSizeBytes: number;
};

