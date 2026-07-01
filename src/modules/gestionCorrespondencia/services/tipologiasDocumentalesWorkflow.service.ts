import clienteApi from "../../../api/Clienteaxios";
import type {
  TipologiaDocumentalWorkflowDto,
  TipologiaDocumentalWorkflowOption,
  TipologiaDocumentalWorkflowQuery,
  TipologiasDocumentalesWorkflowResponse,
} from "../types/tipologiasDocumentalesWorkflow.types";

export const TIPOLOGIAS_DOCUMENTALES_WORKFLOW_ENDPOINT =
  "/api/gestor-documental/tipologias-documentales";

export class TipologiasDocumentalesWorkflowError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "TipologiasDocumentalesWorkflowError";
  }
}

export async function getTipologiasDocumentalesWorkflow(
  query: TipologiaDocumentalWorkflowQuery,
  options: { signal?: AbortSignal } = {},
): Promise<TipologiaDocumentalWorkflowOption[]> {
  assertPositiveNumber(query.idTareaWf, "idTareaWf");
  assertPositiveNumber(query.idRutaWf, "idRutaWf");

  const { data } = await clienteApi.get<TipologiasDocumentalesWorkflowResponse>(
    TIPOLOGIAS_DOCUMENTALES_WORKFLOW_ENDPOINT,
    {
      params: {
        Contexto: "WORKFLOW",
        IdTareaWf: query.idTareaWf,
        IdRutaWf: query.idRutaWf,
      },
      signal: options.signal,
    },
  );

  return normalizeWorkflowTypologyResponse(data);
}

export function normalizeWorkflowTypologyResponse(
  payload: unknown,
): TipologiaDocumentalWorkflowOption[] {
  if (!isRecord(payload)) {
    throw new TipologiasDocumentalesWorkflowError(
      "La respuesta de tipologias documentales no tiene un formato valido.",
    );
  }

  const success = payload.success;
  if (success !== true) {
    throw new TipologiasDocumentalesWorkflowError(readFunctionalErrorMessage(payload));
  }

  const data = payload.data;
  if (!Array.isArray(data)) {
    throw new TipologiasDocumentalesWorkflowError(
      "La respuesta de tipologias documentales no contiene una lista valida.",
    );
  }

  return data.map((item, index) => normalizeWorkflowTypologyItem(item, index));
}

function normalizeWorkflowTypologyItem(
  item: unknown,
  index: number,
): TipologiaDocumentalWorkflowOption {
  if (!isRecord(item)) {
    throw new TipologiasDocumentalesWorkflowError(
      `La tipologia documental en la posicion ${index + 1} no tiene un formato valido.`,
    );
  }

  const dto = item as Partial<TipologiaDocumentalWorkflowDto>;
  const id = dto.Id;
  const descripcion = dto.Descripcion;

  if (typeof id !== "number" || !Number.isFinite(id) || id <= 0) {
    throw new TipologiasDocumentalesWorkflowError(
      `La tipologia documental en la posicion ${index + 1} no tiene un Id valido.`,
    );
  }

  if (typeof descripcion !== "string" || descripcion.trim().length === 0) {
    throw new TipologiasDocumentalesWorkflowError(
      `La tipologia documental en la posicion ${index + 1} no tiene una descripcion valida.`,
    );
  }

  const label = descripcion.trim();

  return {
    value: id,
    label,
    idTipoDocumento: id,
    nombreTipoDocumento: label,
  };
}

function assertPositiveNumber(value: number, fieldName: string): void {
  if (typeof value !== "number" || !Number.isFinite(value) || value <= 0) {
    throw new TipologiasDocumentalesWorkflowError(`${fieldName} debe ser un numero positivo.`);
  }
}

function readFunctionalErrorMessage(payload: Record<string, unknown>): string {
  const firstError = Array.isArray(payload.errors) ? payload.errors[0] : undefined;
  const userMessage = readStringProperty(firstError, "UserMessage", "userMessage");
  const message = readStringProperty(payload, "message", "Message");

  return (
    userMessage ??
    message ??
    "No fue posible cargar las tipologias documentales del workflow."
  );
}

function readStringProperty(source: unknown, ...keys: string[]): string | undefined {
  if (!isRecord(source)) return undefined;

  for (const key of keys) {
    const value = source[key];
    if (typeof value === "string" && value.trim().length > 0) {
      return value.trim();
    }
  }

  return undefined;
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return !!value && typeof value === "object" && !Array.isArray(value);
}
