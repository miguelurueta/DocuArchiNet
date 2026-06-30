import type {
  AppUploadDocumentalModoDocumento,
  UploadDocumentalConfig,
  UploadDocumentalContext,
  UploadDocumentalProcessKey,
} from "../components/AppUploadDocumental/AppUploadDocumental.types";

export type UploadConfigLoaderInput = {
  proceso: UploadDocumentalProcessKey;
  context: UploadDocumentalContext;
  modoDocumento?: AppUploadDocumentalModoDocumento;
};

export type UploadConfigLoader = (input: UploadConfigLoaderInput) => Promise<UploadDocumentalConfig>;

export function normalizeUploadDocumentalConfig(config: UploadDocumentalConfig): UploadDocumentalConfig {
  const allowedExtensions = config.allowedExtensions
    .map((extension) => normalizeConfigExtension(extension))
    .filter((extension, index, values) => extension.length > 0 && values.indexOf(extension) === index);

  if (allowedExtensions.length === 0) {
    throw new TypeError("allowedExtensions must contain at least one extension");
  }

  if (!Number.isFinite(config.maxSizeBytes) || config.maxSizeBytes <= 0) {
    throw new TypeError("maxSizeBytes must be a positive number");
  }

  return {
    ...config,
    accept: config.accept.trim() || allowedExtensions.join(","),
    allowedExtensions,
    validationMode: config.validationMode ?? "reject",
  };
}

function normalizeConfigExtension(extension: string): string {
  const value = extension.trim().toLowerCase();
  if (!value) {
    return "";
  }

  return value.startsWith(".") ? value : `.${value}`;
}
