import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import clienteApi from "../../../../../api/Clienteaxios";

type ApiStatus = "idle" | "loading" | "ready" | "error" | "empty";

type AppMeta = { Status?: string } | undefined;

type ApiErrorItem = {
  Type?: string;
  Field?: string;
  Message?: string;
};

export type FirmaTemporalUsuarioWorkflowDto = {
  IdUsuarioWorkflow: number;
  FileName: string;
  ContentType: string;
  RelativePath: string;
  UrlTemporal: string;
  ExpiresAt: string;
};

type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T | null;
  meta?: AppMeta;
  errors?: ApiErrorItem[];
};

export type WorkflowPersonalSignatureMeta = {
  fileName: string;
  contentType: string;
  expiresAt: string;
  urlTemporal: string;
};

export type WorkflowPersonalSignatureState = {
  status: ApiStatus;
  meta: WorkflowPersonalSignatureMeta | null;
  blobUrl: string | null;
  imageData: ArrayBuffer | null;
  errorMessage: string | null;
  load(): Promise<void>;
  reload(): Promise<void>;
  clear(): void;
};

function getBaseUrl(): string {
  const fromAxios = String(clienteApi.defaults.baseURL ?? "").trim();
  if (fromAxios) return fromAxios.replace(/\/+$/, "");
  const fromEnv = String(import.meta.env.VITE_API_URL ?? "").trim();
  return fromEnv.replace(/\/+$/, "");
}

function buildDownloadUrl(baseUrl: string, urlTemporal: string): string {
  const trimmed = urlTemporal.trim();
  if (/^https?:\/\//i.test(trimmed)) return trimmed;
  // UrlTemporal contractual: normalmente inicia con "/api/..."
  if (!baseUrl) return trimmed;
  if (!trimmed.startsWith("/")) return `${baseUrl}/${trimmed}`;
  return `${baseUrl}${trimmed}`;
}

function toMeta(dto: FirmaTemporalUsuarioWorkflowDto): WorkflowPersonalSignatureMeta {
  return {
    fileName: dto.FileName,
    contentType: dto.ContentType,
    expiresAt: dto.ExpiresAt,
    urlTemporal: dto.UrlTemporal,
  };
}

export function useWorkflowPersonalSignature(): WorkflowPersonalSignatureState {
  const baseUrl = useMemo(() => getBaseUrl(), []);
  const [status, setStatus] = useState<ApiStatus>("idle");
  const [meta, setMeta] = useState<WorkflowPersonalSignatureMeta | null>(null);
  const [blobUrl, setBlobUrl] = useState<string | null>(null);
  const [imageData, setImageData] = useState<ArrayBuffer | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const objectUrlRef = useRef<string | null>(null);
  const requestSeq = useRef(0);

  const revokeObjectUrl = useCallback(() => {
    if (!objectUrlRef.current) return;
    URL.revokeObjectURL(objectUrlRef.current);
    objectUrlRef.current = null;
  }, []);

  const clear = useCallback(() => {
    requestSeq.current += 1; // invalida requests en vuelo
    revokeObjectUrl();
    setBlobUrl(null);
    setImageData(null);
    setMeta(null);
    setErrorMessage(null);
    setStatus("idle");
  }, [revokeObjectUrl]);

  useEffect(() => {
    return () => {
      revokeObjectUrl();
    };
  }, [revokeObjectUrl]);

  const fetchMeta = useCallback(async (): Promise<FirmaTemporalUsuarioWorkflowDto | null> => {
    const res = await clienteApi.get<ApiResponse<FirmaTemporalUsuarioWorkflowDto>>(
      "/api/workflow/usuarios/firma-temporal"
    );

    const payload = res.data;
    if (!payload?.success) {
      const msg =
        payload?.message ||
        payload?.errors?.map((e) => e.Message).filter(Boolean).join(" | ") ||
        "No fue posible obtener metadata de firma";
      throw new Error(msg);
    }

    return payload.data;
  }, []);

  const fetchBlob = useCallback(
    async (downloadUrl: string): Promise<Blob> => {
      // Importante: mantener Authorization Bearer (interceptor axios) y no manipular token.
      // Para UrlTemporal absoluta necesitamos salirnos de baseURL; axios permite URL absoluta.
      const res = await clienteApi.get(downloadUrl, { responseType: "blob" });
      return res.data as Blob;
    },
    []
  );

  const load = useCallback(async () => {
    const seq = (requestSeq.current += 1);

    setStatus("loading");
    setErrorMessage(null);

    revokeObjectUrl();
    setBlobUrl(null);
    setImageData(null);
    setMeta(null);

    try {
      const dto = await fetchMeta();
      if (requestSeq.current !== seq) return;

      if (!dto) {
        setStatus("empty");
        return;
      }

      const nextMeta = toMeta(dto);
      setMeta(nextMeta);

      const downloadUrl = buildDownloadUrl(baseUrl, dto.UrlTemporal);

      try {
        const blob = await fetchBlob(downloadUrl);
        if (requestSeq.current !== seq) return;

        const url = URL.createObjectURL(blob);
        objectUrlRef.current = url;
        setBlobUrl(url);
        setImageData(await blob.arrayBuffer());
        setStatus("ready");
      } catch (e: unknown) {
        const statusCode = (e as any)?.response?.status as number | undefined;
        // Regla contractual: 404 => re-solicitar metadata y reintentar 1 vez
        if (statusCode === 404) {
          const dtoRetry = await fetchMeta();
          if (requestSeq.current !== seq) return;
          if (!dtoRetry) {
            setStatus("empty");
            return;
          }

          setMeta(toMeta(dtoRetry));
          const downloadUrlRetry = buildDownloadUrl(baseUrl, dtoRetry.UrlTemporal);
          const blobRetry = await fetchBlob(downloadUrlRetry);
          if (requestSeq.current !== seq) return;

          const url = URL.createObjectURL(blobRetry);
          objectUrlRef.current = url;
          setBlobUrl(url);
          setImageData(await blobRetry.arrayBuffer());
          setStatus("ready");
          return;
        }

        throw e;
      }
    } catch (e: unknown) {
      if (requestSeq.current !== seq) return;
      const msg =
        e instanceof Error
          ? e.message
          : "No fue posible cargar la firma personal";
      setErrorMessage(msg);
      setStatus("error");
    }
  }, [baseUrl, fetchBlob, fetchMeta, revokeObjectUrl]);

  const reload = useCallback(async () => {
    await load();
  }, [load]);

  return { status, meta, blobUrl, imageData, errorMessage, load, reload, clear };
}
