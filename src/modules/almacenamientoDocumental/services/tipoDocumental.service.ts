import type {
  TipoDocumentalOption,
  UploadDocumentalContext,
  UploadDocumentalProcessKey,
} from "../components/AppUploadDocumental/AppUploadDocumental.types";

export type TipoDocumentalLoaderInput = {
  proceso: UploadDocumentalProcessKey;
  context: UploadDocumentalContext;
};

export type TipoDocumentalLoader = (input: TipoDocumentalLoaderInput) => Promise<TipoDocumentalOption[]>;

export function normalizeTipoDocumentalOptions(options: TipoDocumentalOption[]): TipoDocumentalOption[] {
  const seen = new Set<number>();

  return options.reduce<TipoDocumentalOption[]>((current, option) => {
    if (
      !Number.isFinite(option.idTipoDocumento) ||
      option.idTipoDocumento <= 0 ||
      option.nombreTipoDocumento.trim().length === 0 ||
      seen.has(option.idTipoDocumento)
    ) {
      return current;
    }

    seen.add(option.idTipoDocumento);
    current.push({
      idTipoDocumento: option.idTipoDocumento,
      nombreTipoDocumento: option.nombreTipoDocumento.trim(),
    });
    return current;
  }, []);
}
