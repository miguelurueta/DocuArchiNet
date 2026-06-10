import { useCallback, useEffect, useRef, useState } from "react";
import { digitalizacionApiClient } from "../services/digitalizacionApi";
import {
  assertPdfFile,
  createDigitalizacionApiError,
  toDigitalizacionApiError,
  validateDigitalizacionApiContext,
} from "../services/digitalizacionApiClient";
import type {
  DigitalizacionApiClient,
  DigitalizacionApiError,
  UploadTemporalPdfProgress,
} from "../types/digitalizacionApi.types";
import type {
  DigitalizacionContext,
  DigitalizacionResult,
  DigitalizacionTrdMetadata,
} from "../types/digitalizacion.types";

export type DigitalizacionOrchestratorStatus =
  | "idle"
  | "validatingContext"
  | "validatingTarget"
  | "scanning"
  | "generatingPdf"
  | "resolvingMetadata"
  | "uploading"
  | "creatingDocument"
  | "attachingDocument"
  | "completed"
  | "error"
  | "cancelled";

export type DigitalizacionOperationSubmitInput = {
  context: DigitalizacionContext | null;
  pdf: File | null;
  pageCount: number;
  requestId?: string;
  nombreDocumento?: string;
  idConfiguracionDigitalizacion?: number;
  idTipoListaChequeo?: number;
  trd?: DigitalizacionTrdMetadata | null;
};

export type DigitalizacionOperationOrchestratorState = {
  status: DigitalizacionOrchestratorStatus;
  loading: boolean;
  error: DigitalizacionApiError | null;
  result: DigitalizacionResult | null;
  progress: UploadTemporalPdfProgress | null;
};

type OrchestratorCallbacks = {
  onCompleted?: (result: DigitalizacionResult) => void;
  onError?: (error: DigitalizacionApiError) => void;
};

const initialState: DigitalizacionOperationOrchestratorState = {
  status: "idle",
  loading: false,
  error: null,
  result: null,
  progress: null,
};

const buildNombreDocumento = (input: DigitalizacionOperationSubmitInput) =>
  input.nombreDocumento?.trim() || input.pdf?.name || `digitalizacion-${Date.now()}.pdf`;

const assertPageCount = (pageCount: number) => {
  if (!Number.isFinite(pageCount) || pageCount <= 0) {
    throw createDigitalizacionApiError(
      "PAGES_REQUIRED",
      "Debe existir al menos una pagina digitalizada.",
      "validation",
      "pageCount",
    );
  }
};

const assertMetadata = (context: DigitalizacionContext, input: DigitalizacionOperationSubmitInput) => {
  if (context.requiereMetadata && !input.trd) {
    throw createDigitalizacionApiError(
      "METADATA_REQUIRED",
      "La metadata documental es obligatoria.",
      "validation",
      "trd",
    );
  }
};

const assertCurrentResult = (result: DigitalizacionResult) => {
  if (result.accion === "cancelado") return result;
  if (!Number.isFinite(result.idDocumento) || result.idDocumento <= 0) {
    throw createDigitalizacionApiError(
      "ID_DOCUMENTO_INVALID",
      "La respuesta no contiene un documento valido.",
      "error",
      "idDocumento",
    );
  }
  if (!result.nombreGabinete.trim()) {
    throw createDigitalizacionApiError(
      "NOMBRE_GABINETE_REQUIRED",
      "La respuesta no contiene gabinete.",
      "error",
      "nombreGabinete",
    );
  }
  return result;
};

export function useDigitalizacionOperationOrchestrator({
  apiClient = digitalizacionApiClient,
  onCompleted,
  onError,
}: {
  apiClient?: DigitalizacionApiClient;
} & OrchestratorCallbacks = {}) {
  const generationRef = useRef(0);
  const loadingRef = useRef(false);
  const completedGenerationRef = useRef<number | null>(null);
  const controllerRef = useRef<AbortController | null>(null);
  const callbacksRef = useRef<OrchestratorCallbacks>({ onCompleted, onError });
  const [state, setState] = useState<DigitalizacionOperationOrchestratorState>(initialState);

  callbacksRef.current = { onCompleted, onError };

  const setCurrentState = useCallback(
    (
      generation: number,
      updater: (
        current: DigitalizacionOperationOrchestratorState,
      ) => DigitalizacionOperationOrchestratorState,
    ) => {
      if (generation === generationRef.current) {
        setState(updater);
      }
    },
    [],
  );

  const cancel = useCallback(() => {
    generationRef.current += 1;
    controllerRef.current?.abort();
    controllerRef.current = null;
    loadingRef.current = false;
    setState((current) => ({
      ...current,
      status: current.status === "idle" ? "idle" : "cancelled",
      loading: false,
      progress: null,
    }));
  }, []);

  const reset = useCallback(() => {
    controllerRef.current?.abort();
    controllerRef.current = null;
    loadingRef.current = false;
    completedGenerationRef.current = null;
    generationRef.current += 1;
    setState(initialState);
  }, []);

  const submit = useCallback(
    async (input: DigitalizacionOperationSubmitInput) => {
      if (loadingRef.current) {
        const concurrentError = createDigitalizacionApiError(
          "SUBMIT_ALREADY_IN_PROGRESS",
          "Ya existe una operacion de digitalizacion activa.",
          "validation",
        );
        setState((current) => ({ ...current, error: concurrentError.detail }));
        throw concurrentError;
      }

      const generation = generationRef.current + 1;
      generationRef.current = generation;
      completedGenerationRef.current = null;
      const controller = new AbortController();
      controllerRef.current = controller;
      loadingRef.current = true;
      setState({
        status: "validatingContext",
        loading: true,
        error: null,
        result: null,
        progress: null,
      });

      try {
        const assertStillCurrent = () => {
          if (generation !== generationRef.current || controller.signal.aborted) {
            throw createDigitalizacionApiError(
              "OPERATION_STALE",
              "Operacion de digitalizacion reemplazada o cancelada.",
              "stale",
            );
          }
        };

        const context = validateDigitalizacionApiContext(input.context);
        const pdf = assertPdfFile(input.pdf);
        assertPageCount(input.pageCount);
        assertMetadata(context, input);

        if (context.modo === "adjuntar") {
          setCurrentState(generation, (current) => ({ ...current, status: "validatingTarget" }));
          const validation = await apiClient.validarAdjuntarDigitalizacion(
            context.idDocumentoDestino ?? 0,
            {
              NombreGabinete: context.nombreGabinete,
              ...(context.radicado ? { Radicado: context.radicado } : {}),
            },
            { signal: controller.signal },
          );
          assertStillCurrent();

          if (!validation.permitido) {
            throw createDigitalizacionApiError(
              validation.codigoBloqueo || "ADJUNTAR_NOT_ALLOWED",
              validation.mensajeBloqueo || "El documento destino no permite adjuntar digitalizacion.",
              "conflict",
              "idDocumentoDestino",
            );
          }
        }

        setCurrentState(generation, (current) => ({ ...current, status: "uploading" }));
        const temporal = await apiClient.uploadPdfTemporal(pdf, {
          signal: controller.signal,
          requestId: input.requestId,
          onProgress: (progress) => {
            setCurrentState(generation, (current) => ({ ...current, progress }));
          },
        });
        assertStillCurrent();

        const commonRequest = {
          NombreGabinete: context.nombreGabinete,
          RutaTemporalId: temporal.rutaTemporalId,
          ArchivoTemporalId: temporal.archivoTemporalId,
          ...(input.requestId ? { RequestId: input.requestId } : {}),
          ...(context.radicado ? { Radicado: context.radicado } : {}),
          ...(context.idTareaWorkflow ? { IdTareaWorkflow: context.idTareaWorkflow } : {}),
          ...(context.idRutaWorkflow ? { IdRutaWorkflow: context.idRutaWorkflow } : {}),
        };

        const result =
          context.modo === "crear"
            ? await (async () => {
                setCurrentState(generation, (current) => ({ ...current, status: "creatingDocument" }));
                const response = await apiClient.crearDocumentoDigitalizado(
                  {
                    ...commonRequest,
                    NombreDocumento: buildNombreDocumento(input),
                    ...(input.idConfiguracionDigitalizacion
                      ? { IdConfiguracionDigitalizacion: input.idConfiguracionDigitalizacion }
                      : {}),
                    ...(input.idTipoListaChequeo ? { IdTipoListaChequeo: input.idTipoListaChequeo } : {}),
                    ...(input.trd ? { Trd: input.trd } : {}),
                    NumeroPaginasDeclaradas: input.pageCount,
                  },
                  { signal: controller.signal },
                );

                return assertCurrentResult({
                  accion: "documento-creado",
                  idDocumento: response.idDocumento,
                  nombreGabinete: response.nombreGabinete,
                  numeroPaginas: response.numeroPaginas,
                  ...(input.trd ? { trd: input.trd } : {}),
                });
              })()
            : await (async () => {
                setCurrentState(generation, (current) => ({ ...current, status: "attachingDocument" }));
                const response = await apiClient.adjuntarDigitalizacion(
                  context.idDocumentoDestino ?? 0,
                  {
                    ...commonRequest,
                    ModuloRegistro: context.sourceModule,
                    TipologiaDocumental: input.trd?.nombreTipoDocumento,
                  },
                  { signal: controller.signal },
                );

                return assertCurrentResult({
                  accion: "documento-adjuntado",
                  idDocumento: response.idDocumento,
                  nombreGabinete: response.nombreGabinete,
                  numeroPaginas: response.numeroPaginasFinal,
                });
              })();

        if (generation === generationRef.current && !controller.signal.aborted) {
          completedGenerationRef.current = generation;
          setState({
            status: "completed",
            loading: false,
            error: null,
            result,
            progress: null,
          });
          callbacksRef.current.onCompleted?.(result);
        }

        return result;
      } catch (error) {
        const apiError = toDigitalizacionApiError(error);
        if (generation === generationRef.current && !controller.signal.aborted) {
          setState({
            status: "error",
            loading: false,
            error: apiError,
            result: null,
            progress: null,
          });
          if (completedGenerationRef.current !== generation) {
            callbacksRef.current.onError?.(apiError);
          }
        }
        throw error;
      } finally {
        if (generation === generationRef.current) {
          loadingRef.current = false;
          controllerRef.current = null;
        }
      }
    },
    [apiClient, setCurrentState],
  );

  useEffect(() => cancel, [cancel]);

  return {
    ...state,
    submit,
    cancel,
    reset,
  };
}
