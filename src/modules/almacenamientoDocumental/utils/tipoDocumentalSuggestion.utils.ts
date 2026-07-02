import type {
  TipoDocumentalOption,
  UploadDocumentalFileMetadata,
} from "../components/AppUploadDocumental/AppUploadDocumental.types";

export type SuggestTipoDocumentalInput = {
  fileName: string;
  options: TipoDocumentalOption[];
  minTokenLength?: number;
  threshold?: number;
};

export type TipoDocumentalSuggestion = {
  option: TipoDocumentalOption;
  score: number;
};

export type PreparedTipoDocumentalOption = {
  option: TipoDocumentalOption;
  tokens: string[];
};

const DEFAULT_MIN_TOKEN_LENGTH = 4;
const DEFAULT_THRESHOLD = 0.45;

export function normalizeTipoDocumentalText(value: string): string {
  return value
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .toUpperCase()
    .replace(/[^A-Z0-9]+/g, " ")
    .trim();
}

export function tokenizeTipoDocumentalText(value: string, minTokenLength = DEFAULT_MIN_TOKEN_LENGTH): string[] {
  const tokens = normalizeTipoDocumentalText(value)
    .split(/\s+/)
    .filter((token) => token.length >= minTokenLength);

  return Array.from(new Set(tokens));
}

export function suggestTipoDocumental(input: SuggestTipoDocumentalInput): TipoDocumentalSuggestion | null {
  return suggestTipoDocumentalFromPrepared({
    ...input,
    preparedOptions: prepareTipoDocumentalOptions(input.options, input.minTokenLength),
  });
}

export function prepareTipoDocumentalOptions(
  options: TipoDocumentalOption[],
  minTokenLength = DEFAULT_MIN_TOKEN_LENGTH,
): PreparedTipoDocumentalOption[] {
  return options.map((option) => ({
    option,
    tokens: tokenizeTipoDocumentalText(option.nombreTipoDocumento, minTokenLength),
  }));
}

export function suggestTipoDocumentalFromPrepared(
  input: Omit<SuggestTipoDocumentalInput, "options"> & {
    preparedOptions: PreparedTipoDocumentalOption[];
  },
): TipoDocumentalSuggestion | null {
  const threshold = input.threshold ?? DEFAULT_THRESHOLD;
  const minTokenLength = input.minTokenLength ?? DEFAULT_MIN_TOKEN_LENGTH;
  const fileBaseName = input.fileName.replace(/\.[^.]+$/, "");
  const fileTokens = tokenizeTipoDocumentalText(fileBaseName, minTokenLength);

  if (fileTokens.length === 0 || input.preparedOptions.length === 0) {
    return null;
  }

  let best: TipoDocumentalSuggestion | null = null;

  for (const prepared of input.preparedOptions) {
    if (prepared.tokens.length === 0) {
      continue;
    }

    const matches = prepared.tokens.filter((token) =>
      fileTokens.some((fileToken) => fileToken === token || fileToken.includes(token) || token.includes(fileToken)),
    );
    const score = matches.length / Math.max(prepared.tokens.length, fileTokens.length);

    if (!best || score > best.score) {
      best = { option: prepared.option, score };
    }
  }

  return best && best.score >= threshold ? best : null;
}

export function applyTipoDocumentalSuggestion(
  metadata: UploadDocumentalFileMetadata,
  suggestion: TipoDocumentalSuggestion | null,
): UploadDocumentalFileMetadata {
  if (!suggestion || metadata.tipologiaManual || metadata.idTipoDocumento) {
    return metadata;
  }

  return {
    ...metadata,
    idTipoDocumento: suggestion.option.idTipoDocumento,
    nombreTipoDocumento: suggestion.option.nombreTipoDocumento,
    suggestionConfidence: suggestion.score,
  };
}

export function isValidDocumentalDate(value: string): boolean {
  if (!/^\d{4}-\d{2}-\d{2}$/.test(value)) {
    return false;
  }

  const [year, month, day] = value.split("-").map(Number);
  const date = new Date(Date.UTC(year, month - 1, day));
  const currentYear = new Date().getFullYear();

  return (
    date.getUTCFullYear() === year &&
    date.getUTCMonth() === month - 1 &&
    date.getUTCDate() === day &&
    year <= currentYear
  );
}
