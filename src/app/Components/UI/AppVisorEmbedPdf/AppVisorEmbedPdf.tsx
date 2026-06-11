import { forwardRef, useCallback, useEffect, useImperativeHandle, useMemo, useRef, useState } from "react";
import type { MutableRefObject } from "react";
import { useZoom } from "@embedpdf/plugin-zoom/react";
import { ThumbnailsPane, ThumbImg } from "@embedpdf/plugin-thumbnail/react";
import { useScroll } from "@embedpdf/plugin-scroll/react";
import { Rotate, useRotate } from "@embedpdf/plugin-rotate/react";
import { useViewportCapability } from "@embedpdf/plugin-viewport/react";
import { LeftOutlined, RightOutlined, UpOutlined } from "@ant-design/icons";
import { Skeleton, Spin } from "antd";
import { usePrint } from "@embedpdf/plugin-print/react";
import { useExport } from "@embedpdf/plugin-export/react";
import { PdfErrorCode } from "@embedpdf/models";
import { AnnotationLayer } from "@embedpdf/plugin-annotation/react";
import { PagePointerProvider } from "@embedpdf/plugin-interaction-manager/react";
import { useAnnotation, useAnnotationCapability } from "@embedpdf/plugin-annotation/react";
import { LockModeType } from "@embedpdf/plugin-annotation";
import {
  SelectionLayer,
  useSelectionCapability,
  type SelectionSelectionMenuProps,
} from "@embedpdf/plugin-selection/react";
import {
  deserializeEntries,
  serializeEntries,
  useActivePlacement,
  useSignatureCapability,
  useSignatureEntries,
} from "@embedpdf/plugin-signature/react";
import type { SignatureFieldDefinition } from "@embedpdf/plugin-signature";

import {
  DocumentContent,
  EmbedPDF,
  RenderLayer,
  Scroller,
  useActiveDocument,
  useDocumentState,
  useDocumentManagerCapability,
  Viewport,
} from "./engine/embedPdfAdapter";
import { useEmbedPdfEngine } from "./engine/useEmbedPdfEngine";
import { useDemoPdfUrl } from "./hooks/useDemoPdfUrl";
import { createBasicPluginRegistration } from "./plugins/pluginRegistration";
import {
  DocumentLoadingState,
  EmptyState,
  EngineLoadingState,
  ErrorState,
} from "./presentation/States";
import { AppPdfToolbar } from "./presentation/AppPdfToolbar";
import { AppPdfPasswordPrompt } from "./presentation/AppPdfPasswordPrompt";
import { AppPdfSignatureModal } from "./presentation/AppPdfSignatureModal";
import { AppGuideTour } from "../AppGuideTour";
import type { AppGuideTourEvent, AppGuideTourRef } from "../AppGuideTour";
import styles from "./styles/AppVisorEmbedPdf.module.css";
import type { AppVisorEmbedPdfProps } from "./types/AppVisorEmbedPdfProps";
import type {
  AppVisorExportAnnotatedPdfPagesOptions,
  AppVisorExportAnnotatedPdfPagesResult,
  AppVisorEmbedPdfRef,
  AppVisorLoadInput,
  AppVisorLoadResult,
  ViewerEffectivePermissions,
} from "./AppVisorEmbedPdf.types";
import {
  applySignedOverride,
  failClosedEffectivePermissions,
  mapPermisosVisorPdfToEffectivePermissions,
  resolveCodigoImplementacion,
} from "./AppVisorEmbedPdf.permissions";
import { fetchMisPermisosVisorPdf } from "./AppVisorEmbedPdf.service";
import { applyAutoFitOnce } from "./autoFit/autoFit.apply";
import type { FitMode } from "./autoFit/autoFit.math";
import {
  APP_VISOR_EMBED_PDF_GUIDE_STEPS,
  APP_VISOR_EMBED_PDF_GUIDE_TOUR_ID,
} from "./AppVisorEmbedPdf.guideTour";
import { calculateBlobSha256 } from "./utils/hashSha256";
import { getAnnotatedPageNumbers } from "./utils/pdfPageAnnotations";

function cx(...parts: Array<string | undefined>) {
  return parts.filter(Boolean).join(" ");
}

function dvDebugEnabled(): boolean {
  return typeof window !== "undefined" && Boolean((window as any).__DV_DEBUG__);
}

function dvLog(...args: unknown[]) {
  if (!dvDebugEnabled()) return;
  // eslint-disable-next-line no-console
  console.log(...args);
}

function summarizeAnnotationPages(
  pages: Record<string, unknown>,
  getAnnotationById?: (uid: string) => unknown,
) {
  return Object.fromEntries(
    Object.entries(pages).map(([pageIndex, ids]) => [
      pageIndex,
      Array.isArray(ids)
        ? ids.map((uid) => {
            const annotation = typeof uid === "string" ? (getAnnotationById?.(uid) as any) : undefined;
            return {
              uid,
              objectId: annotation?.object?.id,
              type: annotation?.object?.type,
              subject: annotation?.object?.subject,
              pageIndex: annotation?.object?.pageIndex,
              rect: annotation?.object?.rect,
              unrotatedRect: annotation?.object?.unrotatedRect,
              rotation: annotation?.object?.rotation,
            };
          })
        : ids,
    ]),
  );
}

function summarizeSignatureEntries(entries: unknown[]) {
  return entries.map((entry: any) => ({
    id: entry?.id,
    type: entry?.type,
    pageIndex: entry?.pageIndex,
    pageNumber: entry?.pageNumber,
    signatureType: entry?.signature?.type,
    signatureKeys: entry?.signature ? Object.keys(entry.signature) : [],
  }));
}

function getByteLength(value: unknown): number | undefined {
  if (!value) return undefined;
  if (value instanceof ArrayBuffer) return value.byteLength;
  if (ArrayBuffer.isView(value)) return value.byteLength;
  if (value instanceof Blob) return value.size;
  return undefined;
}

function blobPartFromBytes(bytes: Uint8Array<ArrayBufferLike>): BlobPart {
  const copy = new Uint8Array(bytes.byteLength);
  copy.set(bytes);
  return copy.buffer;
}

function waitPdfTask<T>(task: { wait(onOk: (value: T) => void, onErr: (err: unknown) => void): void }) {
  return new Promise<T>((resolve, reject) => {
    try {
      task.wait(resolve, reject);
    } catch (err) {
      reject(err);
    }
  });
}

function waitPdfTaskVoid(task: { wait(onOk: (value: unknown) => void, onErr: (err: unknown) => void): void }) {
  return new Promise<void>((resolve, reject) => {
    try {
      task.wait(() => resolve(), reject);
    } catch (err) {
      reject(err);
    }
  });
}

function toPdfBlobPart(buffer: ArrayBuffer | Uint8Array<ArrayBufferLike>): BlobPart {
  if (buffer instanceof ArrayBuffer) return buffer;

  const source = new Uint8Array(buffer.buffer, buffer.byteOffset, buffer.byteLength);
  const copy = new Uint8Array(source.byteLength);
  copy.set(source);
  return copy.buffer;
}

/**
 * AppVisorEmbedPdf (01-FE)
 *
 * NOTA: Este componente encapsula EmbedPDF/Pdfium y expone una API mÃ­nima.
 * Se prohÃ­be filtrar detalles del engine hacia mÃ³dulos consumidores.
 */
export const AppVisorEmbedPdf = forwardRef<AppVisorEmbedPdfRef, AppVisorEmbedPdfProps>(function AppVisorEmbedPdf(
  {
    fileUrl,
    loading = false,
    className,
    style,
    onEmptyDocumentHintRequest,
    onSaveAnnotatedPages,
    isSaveAnnotatedPagesDisabled = false,
    isSavingAnnotatedPages = false,
    saveAnnotatedPagesProgress,
  }: AppVisorEmbedPdfProps,
  ref,
) {
  const demoUrl = useDemoPdfUrl();
  const [managedUrl, setManagedUrl] = useState<string | null>(null);
  const [managedPermissionsRaw, setManagedPermissionsRaw] = useState<Record<string, boolean>>({});
  const [managedPermissionsEffective, setManagedPermissionsEffective] = useState<ViewerEffectivePermissions>(
    failClosedEffectivePermissions(),
  );
  const [managedPermissionStatus, setManagedPermissionStatus] = useState<AppVisorLoadResult["permissionStatus"]>(
    "not_required",
  );
  const [managedSigned, setManagedSigned] = useState(false);
  const [managedErrors, setManagedErrors] = useState<string[]>([]);

  const loadSeqRef = useRef(0);
  const loadAbortRef = useRef<AbortController | null>(null);
  const lastLoadIdentityRef = useRef<{ attemptId?: number; documentKey?: string } | null>(null);
  const lastOpenResultRef = useRef<{ url: string; ok: boolean } | null>(null);
  const managedSnapshotRef = useRef<{
    permissionsRaw: Record<string, boolean>;
    permissionsEffective: ViewerEffectivePermissions;
    permissionStatus: AppVisorLoadResult["permissionStatus"];
    errors: string[];
    signed: boolean;
    url: string | null;
  }>({
    permissionsRaw: {},
    permissionsEffective: failClosedEffectivePermissions(),
    permissionStatus: "not_required",
    errors: [],
    signed: false,
    url: null,
  });
  const pendingLoadResolverRef = useRef<
    | {
        seq: number;
        resolve: (value: AppVisorLoadResult) => void;
      }
    | null
  >(null);
  const exportAnnotatedPdfPagesRef = useRef<
    ((options?: AppVisorExportAnnotatedPdfPagesOptions) => Promise<AppVisorExportAnnotatedPdfPagesResult>) | null
  >(null);
  const originalPdfPasswordRef = useRef<string | null>(null);

  const effectiveFileUrl = managedUrl?.trim()
    ? managedUrl.trim()
    : fileUrl?.trim()
      ? fileUrl.trim()
      : demoUrl;

  useEffect(() => {
    managedSnapshotRef.current = {
      permissionsRaw: managedPermissionsRaw,
      permissionsEffective: managedPermissionsEffective,
      permissionStatus: managedPermissionStatus,
      errors: managedErrors,
      signed: managedSigned,
      url: managedUrl,
    };
  }, [managedErrors, managedPermissionStatus, managedPermissionsEffective, managedPermissionsRaw, managedSigned, managedUrl]);

  const cancelCurrentLoad = useCallback(() => {
    dvLog("[DV][visor]", "cancelCurrentLoad()");
    loadAbortRef.current?.abort();
    loadAbortRef.current = null;
    exportAnnotatedPdfPagesRef.current = null;
    originalPdfPasswordRef.current = null;

    const pending = pendingLoadResolverRef.current;
    if (!pending) return;
    pendingLoadResolverRef.current = null;
    pending.resolve({
      ok: false,
      attemptId: lastLoadIdentityRef.current?.attemptId,
      documentKey: lastLoadIdentityRef.current?.documentKey,
      fileUrl: managedUrl,
      loadStatus: "cancelled",
      permissionsRaw: managedPermissionsRaw,
      permissionsEffective: managedPermissionsEffective,
      isElectronicallySigned: managedSigned,
      permissionStatus: managedPermissionStatus,
      errors: ["cancelled"],
    });
  }, [managedPermissionStatus, managedPermissionsEffective, managedPermissionsRaw, managedSigned, managedUrl]);

  const reset = useCallback(() => {
    cancelCurrentLoad();
    setManagedUrl(null);
    setManagedPermissionsRaw({});
    setManagedPermissionsEffective(failClosedEffectivePermissions());
    setManagedPermissionStatus("not_required");
    setManagedSigned(false);
    setManagedErrors([]);
    exportAnnotatedPdfPagesRef.current = null;
    originalPdfPasswordRef.current = null;
  }, [cancelCurrentLoad]);

  const load = useCallback(async (input: AppVisorLoadInput): Promise<AppVisorLoadResult> => {
    loadSeqRef.current += 1;
    const seq = loadSeqRef.current;
    dvLog("[DV][visor]", "load() start", { seq, attemptId: input.attemptId, documentKey: input.documentKey });

    loadAbortRef.current?.abort();
    const abortController = new AbortController();
    loadAbortRef.current = abortController;
    lastLoadIdentityRef.current = { attemptId: input.attemptId, documentKey: input.documentKey };

    setManagedErrors([]);
    setManagedSigned(Boolean(input.isElectronicallySigned));
    setManagedUrl(input.url);

    const last = lastOpenResultRef.current;
    if (last && last.url === input.url) {
      // Si el documento ya está abierto con la misma URL, este load() solo actualiza política/permisos.
      // No bloquea esperando un open redundante del engine.
      return {
        ok: last.ok,
        attemptId: input.attemptId,
        documentKey: input.documentKey,
        fileUrl: last.ok ? input.url : null,
        loadStatus: last.ok ? "loaded" : "failed",
        permissionsRaw: managedSnapshotRef.current.permissionsRaw,
        permissionsEffective: managedSnapshotRef.current.permissionsEffective,
        isElectronicallySigned: Boolean(input.isElectronicallySigned),
        permissionStatus: managedSnapshotRef.current.permissionStatus,
        errors: managedSnapshotRef.current.errors,
      };
    }

    const codigoImpl = resolveCodigoImplementacion(input.nombre_modulo);
    if (!codigoImpl) {
      setManagedPermissionStatus("failed");
      setManagedPermissionsRaw({});
      setManagedPermissionsEffective(
        applySignedOverride({
          effective: failClosedEffectivePermissions(),
          isElectronicallySigned: Boolean(input.isElectronicallySigned),
        }),
      );
      setManagedErrors(["MODULE_MAPPING_MISSING"]);
    } else {
      try {
        const perms = await fetchMisPermisosVisorPdf({ codigoImpl, signal: abortController.signal });
        if (seq !== loadSeqRef.current) {
          return {
            ok: false,
            attemptId: input.attemptId,
            documentKey: input.documentKey,
            fileUrl: input.url,
            loadStatus: "cancelled",
            permissionsRaw: {},
            permissionsEffective: failClosedEffectivePermissions(),
            isElectronicallySigned: Boolean(input.isElectronicallySigned),
            permissionStatus: "failed",
            errors: ["stale"],
          };
        }

        setManagedPermissionStatus("resolved");
        setManagedPermissionsRaw(perms.Permissions ?? {});
        const mapped = mapPermisosVisorPdfToEffectivePermissions(perms.Permissions ?? {});
        dvLog("[DV][visor][permissions]", {
          codigoImpl,
          response: perms,
          raw: perms.Permissions ?? {},
          effective: mapped,
        });
        setManagedPermissionsEffective(
          applySignedOverride({
            effective: mapped,
            isElectronicallySigned: Boolean(input.isElectronicallySigned),
          }),
        );
      } catch {
        if (seq !== loadSeqRef.current) {
          return {
            ok: false,
            attemptId: input.attemptId,
            documentKey: input.documentKey,
            fileUrl: input.url,
            loadStatus: "cancelled",
            permissionsRaw: {},
            permissionsEffective: failClosedEffectivePermissions(),
            isElectronicallySigned: Boolean(input.isElectronicallySigned),
            permissionStatus: "failed",
            errors: ["stale"],
          };
        }
        setManagedPermissionStatus("failed");
        setManagedPermissionsRaw({});
        setManagedPermissionsEffective(
          applySignedOverride({
            effective: failClosedEffectivePermissions(),
            isElectronicallySigned: Boolean(input.isElectronicallySigned),
          }),
        );
        setManagedErrors(["PERMISSIONS_FAILED"]);
      }
    }

    return await new Promise<AppVisorLoadResult>((resolve) => {
      dvLog("[DV][visor]", "load() pending handshake", { seq });
      pendingLoadResolverRef.current = { seq, resolve };
    });
  }, []);

  useImperativeHandle(
    ref,
    () => ({
      load,
      reset,
      cancelCurrentLoad,
      getOriginalPdfPassword: () => originalPdfPasswordRef.current ?? undefined,
      exportAnnotatedPdfPages: async (options) => {
        if (!exportAnnotatedPdfPagesRef.current) {
          throw new Error("El visor PDF no esta listo para exportar paginas anotadas.");
        }

        return exportAnnotatedPdfPagesRef.current(options);
      },
    }),
    [cancelCurrentLoad, load, reset],
  );

  const engineState = useEmbedPdfEngine();
  const pluginRegistration = useMemo(() => createBasicPluginRegistration(), []);
  const isManagedMode = Boolean(managedUrl && managedUrl.trim().length > 0);
  const legacyOpenPermissions: ViewerEffectivePermissions = useMemo(
    () => ({
      allowSignaturePlacement: true,
      allowSignatureDelete: true,
      allowSignatureLockToggle: true,
      allowAnnotationEdit: true,
      allowExport: true,
      allowPrint: true,
    }),
    [],
  );
  const permissionsForRender = isManagedMode ? managedPermissionsEffective : legacyOpenPermissions;

  if (engineState.status === "loading") {
    return (
      <div className={cx(styles.root, className)} style={style} role="status" aria-label="Zona de documento">
        <EngineLoadingState />
      </div>
    );
  }

  if (engineState.status === "error") {
    return (
      <div className={cx(styles.root, className)} style={style} role="status" aria-label="Zona de documento">
        <ErrorState />
      </div>
    );
  }

  if (!effectiveFileUrl) {
    return (
      <div className={cx(styles.root, className)} style={style} role="status" aria-label="Zona de documento">
        {loading ? <FullLoadingOverlay /> : null}
        <EmptyState onDocumentHintRequest={onEmptyDocumentHintRequest} />
      </div>
    );
  }

  return (
    <div className={cx(styles.root, className)} style={style} role="status" aria-label="Zona de documento">
      {loading ? <FullLoadingOverlay /> : null}
      <EmbedPDF engine={engineState.engine} plugins={pluginRegistration}>
        <EmbedPdfDocumentHost
          engine={engineState.engine}
          fileUrl={effectiveFileUrl}
          managedSeq={loadSeqRef.current}
          documentKey={lastLoadIdentityRef.current?.documentKey}
          loading={loading}
          permissionsEffective={permissionsForRender}
          onSaveAnnotatedPages={onSaveAnnotatedPages}
          isSaveAnnotatedPagesDisabled={isSaveAnnotatedPagesDisabled}
          isSavingAnnotatedPages={isSavingAnnotatedPages}
          saveAnnotatedPagesProgress={saveAnnotatedPagesProgress}
          onManagedOpenResult={(payload) => {
            const pending = pendingLoadResolverRef.current;
            if (!pending) return;
            if (payload.seq !== pending.seq) return;
            pendingLoadResolverRef.current = null;
            resolve({
              payload,
              fileUrl: effectiveFileUrl,
              permissionsRaw: managedPermissionsRaw,
              permissionsEffective: permissionsForRender,
              isElectronicallySigned: managedSigned,
              permissionStatus: managedPermissionStatus,
              extraErrors: managedErrors,
              identity: lastLoadIdentityRef.current ?? undefined,
              resolve: pending.resolve,
            });
            lastOpenResultRef.current = { url: effectiveFileUrl, ok: payload.ok };
          }}
          onExportAnnotatedPdfPagesReady={(handler) => {
            exportAnnotatedPdfPagesRef.current = handler;
          }}
          onOriginalPdfPasswordChange={(password) => {
            originalPdfPasswordRef.current = password?.trim() ? password : null;
          }}
        />
      </EmbedPDF>
    </div>
  );
});

function FullLoadingOverlay() {
  return (
    <div className={styles.fullLoadingOverlay} aria-label="Cargando documento" role="status" aria-busy="true">
      <div className={styles.fullLoadingSkeleton} aria-hidden="true">
        <div className={styles.fullLoadingSkeletonUniform}>
          <Skeleton.Button active size="default" shape="circle" />
          <Skeleton.Button active size="default" shape="round" />
          <Skeleton.Button active size="default" shape="round" />
          <Skeleton.Button active size="default" shape="round" />
          <Skeleton.Button active size="default" shape="circle" />
          <div className={styles.fullLoadingSkeletonImageWrap}>
            <Skeleton.Image active />
          </div>
          <div className={styles.fullLoadingSkeletonParagraph}>
            <Skeleton
              active
              title={false}
              paragraph={{
                rows: 6,
                width: ["100%", "100%", "100%", "100%", "100%", "100%"],
              }}
            />
          </div>
        </div>
      </div>
    </div>
  );
}

function resolve(params: {
  payload: { seq: number; ok: boolean; errors: string[] };
  fileUrl: string;
  permissionsRaw: Record<string, boolean>;
  permissionsEffective: ViewerEffectivePermissions;
  isElectronicallySigned: boolean;
  permissionStatus: AppVisorLoadResult["permissionStatus"];
  extraErrors: string[]; 
  identity?: { attemptId?: number; documentKey?: string };
  resolve: (value: AppVisorLoadResult) => void;
}) {
  const {
    payload,
    fileUrl,
    permissionsRaw,
    permissionsEffective,
    isElectronicallySigned,
    permissionStatus,
    extraErrors,
    resolve: resolveFn,
    identity,
  } = params;
  const errors = Array.from(new Set([...(extraErrors ?? []), ...(payload.errors ?? [])]));
  resolveFn({
    ok: payload.ok,
    attemptId: identity?.attemptId,
    documentKey: identity?.documentKey,
    fileUrl: payload.ok ? fileUrl : null,
    loadStatus: payload.ok ? "loaded" : "failed",
    permissionsRaw,
    permissionsEffective,
    isElectronicallySigned,
    permissionStatus,
    errors,
  });
}

function EmbedPdfDocumentHost(props: {
  engine: import("@embedpdf/engines/pdfium").PdfEngine<Blob>;
  fileUrl: string;
  managedSeq: number;
  documentKey?: string;
  loading?: boolean;
  permissionsEffective: ViewerEffectivePermissions;
  onSaveAnnotatedPages?: () => void;
  isSaveAnnotatedPagesDisabled?: boolean;
  isSavingAnnotatedPages?: boolean;
  saveAnnotatedPagesProgress?: number;
  onManagedOpenResult(payload: { seq: number; ok: boolean; errors: string[] }): void;
  onExportAnnotatedPdfPagesReady(
    handler: ((options?: AppVisorExportAnnotatedPdfPagesOptions) => Promise<AppVisorExportAnnotatedPdfPagesResult>) | null,
  ): void;
  onOriginalPdfPasswordChange(password: string | null): void;
}) {
  const {
    engine,
    fileUrl,
    managedSeq,
    documentKey,
    loading = false,
    permissionsEffective,
    onSaveAnnotatedPages,
    isSaveAnnotatedPagesDisabled = false,
    isSavingAnnotatedPages = false,
    saveAnnotatedPagesProgress,
    onManagedOpenResult,
    onExportAnnotatedPdfPagesReady,
    onOriginalPdfPasswordChange,
  } = props;
  const { provides } = useDocumentManagerCapability();
  const { activeDocumentId } = useActiveDocument();
  const fitMode: FitMode = "width";

  const [password, setPassword] = useState<string | null>(null);
  const [passwordAttempt, setPasswordAttempt] = useState(0);
  const [passwordPromptOpen, setPasswordPromptOpen] = useState(false);
  const [invalidPassword, setInvalidPassword] = useState(false);
  const [isSubmittingPassword, setIsSubmittingPassword] = useState(false);

  const openedDocumentIdRef = useRef<string | null>(null);
  const autoFitIntentRef = useRef<{ documentId: string; seq: number } | null>(null);
  const autoFitAppliedRef = useRef(false);
  const rotationByDocumentKeyRef = useRef<Map<string, number>>(new Map());
  const latestManagedSeqRef = useRef(managedSeq);
  const lastOpenedRef = useRef<
    { url: string; password: string | null; attempt: number } | null
  >(null);
  const lastAttemptHadPasswordRef = useRef(false);

  useEffect(() => {
    latestManagedSeqRef.current = managedSeq;
  }, [managedSeq]);

  useEffect(() => {
    // Al cambiar el documento, reiniciar el estado del prompt/password (enterprise hardening).
    setPassword(null);
    setPasswordAttempt(0);
    setPasswordPromptOpen(false);
    setInvalidPassword(false);
    setIsSubmittingPassword(false);
    lastOpenedRef.current = null;
    lastAttemptHadPasswordRef.current = false;
    autoFitIntentRef.current = null;
    autoFitAppliedRef.current = false;
    onOriginalPdfPasswordChange(null);
    return () => {
      onExportAnnotatedPdfPagesReady(null);
      onOriginalPdfPasswordChange(null);
    };
  }, [fileUrl, onExportAnnotatedPdfPagesReady, onOriginalPdfPasswordChange]);

  useEffect(() => {
    if (!activeDocumentId) return;
    // Documento activado: ya no se requiere "submitting" ni prompt activo.
    setIsSubmittingPassword(false);
    setPasswordPromptOpen(false);
    setInvalidPassword(false);
  }, [activeDocumentId]);

  useEffect(() => {
    if (!provides) return;
    if (!fileUrl) return;
    const last = lastOpenedRef.current;
    // Importante: permitir reintento aunque el usuario envíe la misma contraseña (mismo string).
    // Por eso incluimos `passwordAttempt` como parte de la identidad del intento.
    if (last && last.url === fileUrl && last.password === password && last.attempt === passwordAttempt) return;
    lastOpenedRef.current = { url: fileUrl, password, attempt: passwordAttempt };
    lastAttemptHadPasswordRef.current = Boolean(password);
    setIsSubmittingPassword(Boolean(password));

    // Enterprise hardening: el DocumentManager tiene un máximo de documentos abiertos (por defecto 10).
    // Para evitar `Maximum number of documents (10) reached`, aplicamos política "single-active document":
    // cerramos el documento previamente abierto antes de abrir uno nuevo.
    const previouslyOpenedId = openedDocumentIdRef.current;
    if (previouslyOpenedId && provides.isDocumentOpen(previouslyOpenedId)) {
      try {
        dvLog("[DV][visor]", "closeDocument before open (guard maxDocuments)", { managedSeq, documentId: previouslyOpenedId });
        // Best-effort: si falla, continuamos para no bloquear UX.
        void waitPdfTaskVoid(provides.closeDocument(previouslyOpenedId));
      } catch (err) {
        dvLog("[DV][visor]", "closeDocument threw (best-effort)", { managedSeq, err });
      }
    }

    dvLog("[DV][visor]", "openDocumentUrl start", { managedSeq });
    const openTask = provides.openDocumentUrl({
      url: fileUrl,
      name: "document.pdf",
      autoActivate: true,
      ...(password ? { password } : null),
    });

    // El DocumentManager devuelve un Task que resuelve con { documentId, task }.
    // Importante: el "éxito/fracaso real" de abrir el PDF ocurre en `response.task`
    // (carga del documento), no en el task externo (que puede resolver al despachar la carga).
    let cancelled = false;
    openTask.wait(
      (response) => {
        if (cancelled) return;
        // stale guard (latest-wins): si este efecto ya no corresponde al intento vigente, no mutar UI.
        if (managedSeq !== latestManagedSeqRef.current) return;
        openedDocumentIdRef.current = response.documentId;
        dvLog("[DV][visor]", "openDocumentUrl dispatched", { managedSeq, documentId: response.documentId });

        // Esperar el task interno de carga del PDF para cerrar el estado "Validando…"
        // incluso si el documento no llega a activarse (activeDocumentId sigue null).
        response.task.wait(
          () => {
            if (cancelled) return;
            setIsSubmittingPassword(false);
            setPasswordPromptOpen(false);
            setInvalidPassword(false);
            dvLog("[DV][visor]", "engine ready (task ok)", { managedSeq });
            autoFitIntentRef.current = { documentId: response.documentId, seq: managedSeq };
            autoFitAppliedRef.current = false;
            onManagedOpenResult({ seq: managedSeq, ok: true, errors: [] });
          },
          () => {
            if (cancelled) return;
            if (managedSeq !== latestManagedSeqRef.current) return;
            setIsSubmittingPassword(false);
            // OPEN_FAILED no implica contraseña; evitar prompt falso bajo cancelación/stale.
            setPasswordPromptOpen(false);
            dvLog("[DV][visor]", "engine open failed (task err)", { managedSeq });
            onManagedOpenResult({ seq: managedSeq, ok: false, errors: ["OPEN_FAILED"] });
          },
        );
      },
      (err) => {
        if (cancelled) return;
        if (managedSeq !== latestManagedSeqRef.current) return;
        setIsSubmittingPassword(false);
        // OPEN_FAILED no implica contraseña; evitar prompt falso bajo cancelación/stale.
        setPasswordPromptOpen(false);
        dvLog("[DV][visor]", "openDocumentUrl failed (outer task)", { managedSeq, err });
        onManagedOpenResult({ seq: managedSeq, ok: false, errors: ["OPEN_FAILED"] });
      },
    );

    return () => {
      cancelled = true;
    };
  }, [fileUrl, provides, password, passwordAttempt]);

  useEffect(() => {
    if (!provides) return;
    const off = provides.onDocumentError((evt) => {
      if (!openedDocumentIdRef.current) return;
      if (evt.documentId !== openedDocumentIdRef.current) return;
      if (evt.reason?.code !== PdfErrorCode.Password) return;
      // stale guard: solo el intento vigente puede abrir prompt.
      if (managedSeq !== latestManagedSeqRef.current) return;

      setIsSubmittingPassword(false);
      setPasswordPromptOpen(true);
      setInvalidPassword(lastAttemptHadPasswordRef.current);
      onManagedOpenResult({ seq: managedSeq, ok: false, errors: ["PASSWORD_REQUIRED"] });
    });
    return () => {
      off?.();
    };
  }, [managedSeq, onManagedOpenResult, provides]);
  const onSubmitPassword = useCallback((next: string) => {
    setPasswordPromptOpen(true);
    setInvalidPassword(false);
    setIsSubmittingPassword(true);
    lastAttemptHadPasswordRef.current = true;

    if (!provides) return;
    const openedId = openedDocumentIdRef.current;
    if (!openedId) return;

    // Reintento oficial del DocumentManager (sin reabrir URL / sin lógica custom)
    const retryTask = provides.retryDocument(openedId, { password: next });
    retryTask.wait(
      (response) => {
        response.task.wait(
          () => {
            onOriginalPdfPasswordChange(next);
            setIsSubmittingPassword(false);
            setPasswordPromptOpen(false);
            setInvalidPassword(false);
            onManagedOpenResult({ seq: managedSeq, ok: true, errors: [] });
          },
          () => {
            onOriginalPdfPasswordChange(null);
            setIsSubmittingPassword(false);
            setPasswordPromptOpen(true);
            setInvalidPassword(true);
            onManagedOpenResult({ seq: managedSeq, ok: false, errors: ["OPEN_FAILED"] });
          },
        );
      },
      () => {
        onOriginalPdfPasswordChange(null);
        setIsSubmittingPassword(false);
        setPasswordPromptOpen(true);
        setInvalidPassword(true);
        onManagedOpenResult({ seq: managedSeq, ok: false, errors: ["OPEN_FAILED"] });
      },
    );
  }, [managedSeq, onManagedOpenResult, onOriginalPdfPasswordChange, provides]);

  const onPasswordError = useCallback(() => {
    // Si el documento vuelve a fallar luego de enviar password, dejar de "validar"
    // y mostrar estado inválido para permitir reintento.
    setIsSubmittingPassword(false);
    setPasswordPromptOpen(true);
    setInvalidPassword(lastAttemptHadPasswordRef.current);
  }, []);

  if (!activeDocumentId) {
    return (
      <div className={styles.overlayScope}>
        <DocumentLoadingState />
        {passwordPromptOpen ? (
          <AppPdfPasswordPrompt
            isInvalidPassword={invalidPassword}
            isLoading={isSubmittingPassword}
            onSubmit={onSubmitPassword}
          />
        ) : null}
      </div>
    );
  }

  return (
    <DocumentContent documentId={activeDocumentId}>
      {({ isLoaded, isError, isLoading }) => (
        <EmbedPdfDocumentStateView
          engine={engine}
          documentId={activeDocumentId}
          isLoaded={isLoaded}
          isError={isError}
          isLoading={isLoading}
          loading={loading}
          permissionsEffective={permissionsEffective}
          onSaveAnnotatedPages={onSaveAnnotatedPages}
          isSaveAnnotatedPagesDisabled={isSaveAnnotatedPagesDisabled}
          isSavingAnnotatedPages={isSavingAnnotatedPages}
          saveAnnotatedPagesProgress={saveAnnotatedPagesProgress}
          fitMode={fitMode}
          autoFitIntentRef={autoFitIntentRef}
          autoFitAppliedRef={autoFitAppliedRef}
          documentKey={documentKey}
          rotationByDocumentKeyRef={rotationByDocumentKeyRef}
          passwordPromptOpen={passwordPromptOpen}
          invalidPassword={invalidPassword}
          isSubmittingPassword={isSubmittingPassword}
          lastAttemptHadPassword={lastAttemptHadPasswordRef.current}
          onSubmitPassword={onSubmitPassword}
          onPasswordError={onPasswordError}
          onExportAnnotatedPdfPagesReady={onExportAnnotatedPdfPagesReady}
        />
      )}
    </DocumentContent>
  );
}

function EmbedPdfDocumentStateView({
  engine,
  documentId,
  isLoaded,
  isError,
  isLoading,
  loading = false,
  permissionsEffective,
  onSaveAnnotatedPages,
  isSaveAnnotatedPagesDisabled,
  isSavingAnnotatedPages,
  saveAnnotatedPagesProgress,
  fitMode,
  autoFitIntentRef,
  autoFitAppliedRef,
  documentKey,
  rotationByDocumentKeyRef,
  passwordPromptOpen,
  invalidPassword,
  isSubmittingPassword,
  lastAttemptHadPassword,
  onSubmitPassword,
  onPasswordError,
  onExportAnnotatedPdfPagesReady,
}: {
  engine: import("@embedpdf/engines/pdfium").PdfEngine<Blob>;
  documentId: string;
  isLoaded: boolean;
  isError: boolean;
  isLoading: boolean;
  loading?: boolean;
  permissionsEffective: ViewerEffectivePermissions;
  onSaveAnnotatedPages?: () => void;
  isSaveAnnotatedPagesDisabled: boolean;
  isSavingAnnotatedPages: boolean;
  saveAnnotatedPagesProgress?: number;
  fitMode: FitMode;
  autoFitIntentRef: MutableRefObject<{ documentId: string; seq: number } | null>;
  autoFitAppliedRef: MutableRefObject<boolean>;
  documentKey?: string;
  rotationByDocumentKeyRef: MutableRefObject<Map<string, number>>;
  passwordPromptOpen: boolean;
  invalidPassword: boolean;
  isSubmittingPassword: boolean;
  lastAttemptHadPassword: boolean;
  onSubmitPassword(password: string): void;
  onPasswordError(): void;
  onExportAnnotatedPdfPagesReady(
    handler: ((options?: AppVisorExportAnnotatedPdfPagesOptions) => Promise<AppVisorExportAnnotatedPdfPagesResult>) | null,
  ): void;
}) {
  useEffect(() => {
    if (!isError) return;
    onPasswordError();
  }, [isError, onPasswordError]);

  if (isLoaded) {
    return (
      <div className={styles.overlayScope}>
        <EmbedPdfLoadedDocumentView
          engine={engine}
          documentId={documentId}
          permissionsEffective={permissionsEffective}
          onSaveAnnotatedPages={onSaveAnnotatedPages}
          isSaveAnnotatedPagesDisabled={isSaveAnnotatedPagesDisabled}
          isSavingAnnotatedPages={isSavingAnnotatedPages}
          saveAnnotatedPagesProgress={saveAnnotatedPagesProgress}
          loading={loading}
          fitMode={fitMode}
          autoFitIntentRef={autoFitIntentRef}
          autoFitAppliedRef={autoFitAppliedRef}
          documentKey={documentKey}
          rotationByDocumentKeyRef={rotationByDocumentKeyRef}
          onExportAnnotatedPdfPagesReady={onExportAnnotatedPdfPagesReady}
        />
        {passwordPromptOpen ? (
          <AppPdfPasswordPrompt
            isInvalidPassword={invalidPassword}
            isLoading={isSubmittingPassword}
            onSubmit={onSubmitPassword}
          />
        ) : null}
      </div>
    );
  }

  if (isError) {
    return (
      <div className={styles.overlayScope}>
        <ErrorState />
        <AppPdfPasswordPrompt
          isInvalidPassword={lastAttemptHadPassword}
          isLoading={isSubmittingPassword}
          onSubmit={onSubmitPassword}
        />
      </div>
    );
  }

  if (isLoading) return <DocumentLoadingState />;
  return <DocumentLoadingState />;
}

function EmbedPdfLoadedDocumentView(props: {
  engine: import("@embedpdf/engines/pdfium").PdfEngine<Blob>;
  documentId: string;
  permissionsEffective: ViewerEffectivePermissions;
  onSaveAnnotatedPages?: () => void;
  isSaveAnnotatedPagesDisabled: boolean;
  isSavingAnnotatedPages: boolean;
  saveAnnotatedPagesProgress?: number;
  loading?: boolean;
  fitMode: FitMode;
  autoFitIntentRef: MutableRefObject<{ documentId: string; seq: number } | null>;
  autoFitAppliedRef: MutableRefObject<boolean>;
  documentKey?: string;
  rotationByDocumentKeyRef: MutableRefObject<Map<string, number>>;
  onExportAnnotatedPdfPagesReady(
    handler: ((options?: AppVisorExportAnnotatedPdfPagesOptions) => Promise<AppVisorExportAnnotatedPdfPagesResult>) | null,
  ): void;
}) {
  const {
    engine,
    documentId,
    permissionsEffective,
    onSaveAnnotatedPages,
    isSaveAnnotatedPagesDisabled,
    isSavingAnnotatedPages,
    saveAnnotatedPagesProgress,
    loading = false,
    fitMode,
    autoFitIntentRef,
    autoFitAppliedRef,
    documentKey,
    rotationByDocumentKeyRef,
    onExportAnnotatedPdfPagesReady,
  } = props;
  const activeDocumentState = useDocumentState(documentId);
  const zoom = useZoom(documentId);
  const zoomLevel = typeof zoom.state.currentZoomLevel === "number" ? zoom.state.currentZoomLevel : 1;
  const [isThumbnailOpen, setIsThumbnailOpen] = useState(false);
  const [isSignatureModalOpen, setIsSignatureModalOpen] = useState(false);
  const guideTourRef = useRef<AppGuideTourRef | null>(null);

  // Signature (oficial): capability + entries para persistencia local (temporal).
  const signatureCap = useSignatureCapability();
  const signatureEntries = useSignatureEntries();
  const didRestoreEntriesRef = useRef(false);
  const [isSignaturePlacementReady, setIsSignaturePlacementReady] = useState(false);
  const activePlacement = useActivePlacement(documentId);

  const signatureStorageKey = useMemo(
    () => `appvisor:embedpdf:annotations:${documentId}`,
    [documentId],
  );

  useEffect(() => {
    if (!signatureCap.provides) return;
    if (didRestoreEntriesRef.current) return;
    didRestoreEntriesRef.current = true;
    try {
      const raw = localStorage.getItem(signatureStorageKey);
      if (!raw) return;
      const parsed = JSON.parse(raw) as unknown;
      const restored = deserializeEntries(parsed as any);
      signatureCap.provides.loadEntries(restored);
    } catch {
      // Guardrail: no bloquear el visor por storage corrupto.
    }
  }, [signatureCap.provides, signatureStorageKey]);

  useEffect(() => {
    let cancelled = false;
    setIsSignaturePlacementReady(false);
    signatureCap.ready
      .then(() => {
        if (cancelled) return;
        setIsSignaturePlacementReady(true);
      })
      .catch(() => {
        if (cancelled) return;
        setIsSignaturePlacementReady(false);
      });
    return () => {
      cancelled = true;
    };
  }, [signatureCap.ready]);

  useEffect(() => {
    const entries = signatureEntries.entries ?? [];
    try {
      localStorage.setItem(signatureStorageKey, JSON.stringify(serializeEntries(entries as any)));
    } catch {
      // No-op (quota/blocked storage)
    }
  }, [signatureEntries.entries, signatureStorageKey]);
  const scroll = useScroll(documentId);
  const rotate = useRotate(documentId);
  const rotationRaw = rotate.rotation ?? 0;
  // `Rotation` en EmbedPDF suele ser 0..3, pero en algunos adapters puede venir como grados (0/90/180/270).
  // Normalizamos a "steps" (0..3) para que las condiciones de layout sean correctas.
  const rotationSteps =
    typeof rotationRaw === "number" && rotationRaw > 3
      ? (((Math.round(rotationRaw / 90) % 4) + 4) % 4)
      : rotationRaw;
  const viewport = useViewportCapability();
  const [showScrollTop, setShowScrollTop] = useState(false);
  const rafRef = useRef<number | null>(null);
  const isZoomDisabled = rotationSteps !== 0;
  const print = usePrint(documentId);
  const exportApi = useExport(documentId);
  const annotation = useAnnotation(documentId);
  const annotationCap = useAnnotationCapability();
  const selection = useSelectionCapability();
  const [isSignatureLocked, setIsSignatureLocked] = useState(false);
  const [isSavingSignedPdf, setIsSavingSignedPdf] = useState(false);

  const downloadBuffer = useCallback((buffer: ArrayBuffer | Uint8Array<ArrayBufferLike>, filename: string) => {
    const blob = new Blob([toPdfBlobPart(buffer)], { type: "application/pdf" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    a.remove();
    setTimeout(() => URL.revokeObjectURL(url), 0);
  }, []);

  // DEV-only debug hook para diagnosticar selección/borrado de firmas sin adivinar.
  // Se elimina cuando resolvamos el flujo.
  // (cleanup) Se removiÃ³ el debug hook `__APPVISOR_DEBUG__`.

  useEffect(() => {
    if (!selection.provides) return;
    // Activar selección de texto en PDFs.
    selection.provides.enableForMode(
      "default",
      {
        enableSelection: true,
        // Selección tipo navegador: sin "marquee" overlay al arrastrar.
        enableMarquee: false,
        showSelectionRects: true,
        showMarqueeRects: false,
      },
      documentId,
    );
  }, [selection.provides, documentId]);

  const selectionMenu = useCallback(
    (props: SelectionSelectionMenuProps) => {
      const { rect, menuWrapperProps, placement } = props;
      const top = placement.suggestTop ? -40 : rect.size.height + 8;
      return (
        <div {...menuWrapperProps}>
          <div
            style={{
              position: "absolute",
              top,
              pointerEvents: "auto",
              cursor: "default",
            }}
          >
            <button
              type="button"
              className={styles.selectionCopyButton}
              onClick={() => {
                try {
                  const scope = selection.provides?.forDocument(documentId);
                  scope?.copyToClipboard?.();
                  scope?.clear?.();
                } catch {
                  // ignore
                }
              }}
            >
              Copy
            </button>
          </div>
        </div>
      );
    },
    [documentId, selection.provides],
  );

  // Copiar selección (tipo demo): intercepta Ctrl/Cmd+C cuando existe selección del plugin
  // y delega a `copyToClipboard()` para evitar depender del native selection del DOM.
  const hasSelectionRef = useRef(false);
  useEffect(() => {
    const provides = selection.provides;
    if (!provides) return;
    const scope = provides.forDocument(documentId);

    const off = scope.onSelectionChange?.((sel: unknown) => {
      hasSelectionRef.current = Boolean(sel);
    });

    const onKeyDown = (event: KeyboardEvent) => {
      const isCopy = (event.ctrlKey || event.metaKey) && (event.key === "c" || event.key === "C");
      if (!isCopy) return;
      try {
        if (!hasSelectionRef.current && !scope.getState().selection) return;
        event.preventDefault();
        scope.copyToClipboard?.();
      } catch {
        // ignore
      }
    };

    window.addEventListener("keydown", onKeyDown, { capture: true });
    return () => {
      window.removeEventListener("keydown", onKeyDown, { capture: true } as any);
      off?.();
      hasSelectionRef.current = false;
    };
  }, [selection.provides, documentId]);

  const getSelectedSignature = useCallback(() => {
    const scope = annotation.provides;
    if (!scope) return null;
    const selectedIds = scope.getSelectedAnnotationIds?.() ?? [];
    const selectedUid = selectedIds[0];
    if (!selectedUid) return null;
    const selected = scope.getAnnotationById?.(selectedUid);
    if (!selected) return null;

    // Heurística: las firmas se crean como STAMP o INK (según Signature plugin).
    const type = (selected.object as any)?.type;
    const subject = (selected.object as any)?.subject;
    const isSignature =
      type === "STAMP" ||
      type === "INK" ||
      type === "stamp" ||
      type === "ink" ||
      type === 15 ||
      subject === "Signature";
    if (!isSignature) return null;

    const pageIndexFromObject = (selected.object as any)?.pageIndex;
    let pageIndex: number | null = typeof pageIndexFromObject === "number" ? pageIndexFromObject : null;

    const pages = annotation.state.pages ?? {};
    if (pageIndex == null) {
      for (const [pageKey, ids] of Object.entries(pages)) {
        // `pages` guarda IDs internos (UIDs) del plugin, no necesariamente `object.id`.
        if (Array.isArray(ids) && ids.includes(selectedUid)) {
          pageIndex = Number(pageKey);
          break;
        }
      }
    }
    if (pageIndex == null || Number.isNaN(pageIndex)) return null;

    return { pageIndex, uid: selectedUid, objectId: selected.object.id };
  }, [annotation.provides, annotation.state.pages]);

  const hasAnySignaturePlaced = useMemo(() => {
    const scope = annotation.provides;
    if (!scope) return false;
    const pages = annotation.state.pages ?? {};
    for (const ids of Object.values(pages)) {
      if (!Array.isArray(ids) || ids.length === 0) continue;
      for (const uid of ids) {
        const ann = scope.getAnnotationById?.(uid);
        if (!ann) continue;
        const type = (ann.object as any)?.type;
        const subject = (ann.object as any)?.subject;
        const isSignature =
          type === "STAMP" ||
          type === "INK" ||
          type === "stamp" ||
          type === "ink" ||
          type === 15 ||
          subject === "Signature";
        if (isSignature) return true;
      }
    }
    return false;
  }, [annotation.provides, annotation.state.pages]);

  const onDeleteSelectedSignature = useCallback(async () => {
    if (isSignatureLocked) return;
    const selected = getSelectedSignature();
    if (!selected) return;
    const scope = annotation.provides;
    const cap = annotationCap.provides;
    if (!scope || !cap) return;

    // Importante: NO usar `purgeAnnotation` aquÃ­. `purgeAnnotation` solo afecta UI/state y puede
    // impedir que el commit elimine la anotaciÃ³n del PDF real (lo que rompe export/print).
    try {
      // Persistir el delete en el PDF real: en este plugin, `deleteAnnotation` opera por ID de objeto.
      cap.deleteAnnotation?.(selected.pageIndex, selected.objectId);
    } catch {
      // ignore
    }
    try {
      scope.deleteAnnotation?.(selected.pageIndex, selected.objectId);
    } catch {
      // ignore
    }

    // Fallback: en algunos builds, el delete funciona por uid.
    try {
      cap.deleteAnnotation?.(selected.pageIndex, selected.uid);
    } catch {
      // ignore
    }
    try {
      scope.deleteAnnotation?.(selected.pageIndex, selected.uid);
    } catch {
      // ignore
    }

    // Importante: algunas operaciones (purge/delete) actualizan UI inmediatamente, pero el PDF
    // subyacente requiere commit para que export/print reflejen el estado final.
    try {
      if (cap.commit) {
        await waitPdfTask<boolean>(cap.commit());
      }
    } catch {
      // ignore
    }
  }, [
    annotation.provides,
    annotationCap.provides,
    documentId,
    getSelectedSignature,
    isSignatureLocked,
  ]);

  const unlockSignatures = useCallback(() => {
    setIsSignatureLocked(false);
    try {
      // Rehabilitar anotaciones (incluye firmas) si estaban bloqueadas por "Guardar/Bloquear".
      annotationCap.provides?.setLocked?.({ type: LockModeType.None, categories: [] } as any, documentId);
    } catch {
      // ignore
    }
  }, [annotationCap.provides, documentId]);

  const onSaveSignedPdf = useCallback(async () => {
    if (isSignatureLocked) {
      unlockSignatures();
      return;
    }
    if (!hasAnySignaturePlaced) return;

    setIsSavingSignedPdf(true);
    try {
      dvLog("[DV][firma][save-signed:start]", {
        documentId,
        hasAnySignaturePlaced,
        signatureEntries: summarizeSignatureEntries(signatureEntries.entries ?? []),
        annotationPages: summarizeAnnotationPages(annotation.state.pages ?? {}, annotation.provides?.getAnnotationById),
      });

      // Asegurar que la firma quede aplicada en el documento antes de exportar.
      if (annotationCap.provides?.commit) {
        await waitPdfTaskVoid(annotationCap.provides.commit());
      }

      dvLog("[DV][firma][save-signed:after-commit]", {
        documentId,
        signatureEntries: summarizeSignatureEntries(signatureEntries.entries ?? []),
        annotationPages: summarizeAnnotationPages(annotation.state.pages ?? {}, annotation.provides?.getAnnotationById),
      });

      // Intento de lock por categorÃ­a (si el engine categoriza firmas).
      // El guardrail real se asegura por UI (deshabilitar eliminar/agregar firma luego de guardar).
      try {
        // Nota: las firmas subidas como PNG suelen materializarse como STAMP/INK sin categorÃ­a "signature"
        // consistente. Para garantizar que no se puedan remover/editar tras "Guardar", bloqueamos el
        // layer de anotaciones completo a nivel de documento.
        annotationCap.provides?.setLocked?.({ type: LockModeType.All, categories: [] } as any, documentId);
      } catch {
        // ignore
      }

      setIsSignatureLocked(true);
      setIsSignatureModalOpen(false);
    } finally {
      setIsSavingSignedPdf(false);
    }
  }, [
    annotation.provides,
    annotation.state.pages,
    annotationCap.provides,
    documentId,
    hasAnySignaturePlaced,
    isSignatureLocked,
    signatureEntries.entries,
    unlockSignatures,
  ]);


  const getViewportCenter = useCallback(() => {
    const scope = viewport.provides?.forDocument(documentId);
    const m = scope?.getMetrics();
    if (!m) return undefined;
    return { vx: m.clientWidth / 2, vy: m.clientHeight / 2 };
  }, [viewport.provides, documentId]);

  const persistRotationSteps = useCallback(
    (nextSteps: number) => {
      if (!documentKey) return;
      const normalized = ((nextSteps % 4) + 4) % 4;
      rotationByDocumentKeyRef.current.set(documentKey, normalized);
      try {
        // Persistimos el DELTA de UI (rotación del plugin) por documento.
        // No representa la rotación base del PDF (/Rotate). No debe “pisar” la base.
        localStorage.setItem(`appvisor:embedpdf:rotation_delta:${documentKey}`, String(normalized));
      } catch {
        // no-op (quota/blocked storage)
      }
    },
    [documentKey, rotationByDocumentKeyRef],
  );

  useEffect(() => {
    if (!documentKey) return;
    if (rotationByDocumentKeyRef.current.has(documentKey)) return;
    try {
      const raw = localStorage.getItem(`appvisor:embedpdf:rotation_delta:${documentKey}`);
      if (!raw) return;
      const parsed = Number.parseInt(raw, 10);
      if (!Number.isFinite(parsed)) return;
      const normalized = ((parsed % 4) + 4) % 4;
      rotationByDocumentKeyRef.current.set(documentKey, normalized);
    } catch {
      // ignore
    }
  }, [documentKey, rotationByDocumentKeyRef]);

  // Aplica el DELTA persistido solo después de que el documento está Loaded (base /Rotate ya aplicada por engine).
  // Guardrail: si el delta es 0, no forzar setRotation(0) (evita “pisar” visualmente en implementaciones donde el
  // plugin no modela delta puro).
  const appliedPersistedDeltaRef = useRef<{ documentId: string; documentKey?: string } | null>(null);
  useEffect(() => {
    if (!documentKey) return;
    if (!rotate.provides) return;
    const already = appliedPersistedDeltaRef.current;
    if (already && already.documentId === documentId && already.documentKey === documentKey) return;

    const persistedDelta = rotationByDocumentKeyRef.current.get(documentKey);
    if (typeof persistedDelta !== "number") return;
    if (persistedDelta === 0) {
      appliedPersistedDeltaRef.current = { documentId, documentKey };
      return;
    }

    // Apply-once por documento cargado. En caso de error, best-effort.
    try {
      // Espera un tick para evitar competir con activación interna del engine.
      const id = window.setTimeout(() => {
        try {
          rotate.provides?.setRotation(persistedDelta);
        } catch {
          // ignore
        }
      }, 0);
      return () => window.clearTimeout(id);
    } finally {
      appliedPersistedDeltaRef.current = { documentId, documentKey };
    }
  }, [documentId, documentKey, rotate.provides, rotationByDocumentKeyRef]);

  useEffect(() => {
    if (autoFitAppliedRef.current) return;
    const intent = autoFitIntentRef.current;
    if (!intent) return;
    if (intent.documentId !== documentId) return;

    const result = applyAutoFitOnce({
      documentId,
      fitMode,
      rotationSteps,
      zoomLevel,
      zoomProvides: zoom.provides ?? undefined,
      viewportProvides: viewport.provides ?? undefined,
    });

    if (!result.ok) return;
    autoFitAppliedRef.current = true;
    autoFitIntentRef.current = null;
    dvLog("[DV][visor]", "autoFit applied", { documentId, fitMode, appliedZoom: result.appliedZoom });
  }, [documentId, fitMode, viewport.provides, zoom.provides, zoomLevel, autoFitAppliedRef, autoFitIntentRef]);

  const onZoomIn = useCallback(() => {
    if (isZoomDisabled) return;
    // Usar API oficial con "center" explÃ­cito para evitar que el viewport se re-anclÃ©
    // al top/left al cambiar el scale (se mantiene centrado).
    zoom.provides?.requestZoomBy(0.1, getViewportCenter());
  }, [zoom.provides, isZoomDisabled, getViewportCenter]);

  const onZoomOut = useCallback(() => {
    if (isZoomDisabled) return;
    zoom.provides?.requestZoomBy(-0.1, getViewportCenter());
  }, [zoom.provides, isZoomDisabled, getViewportCenter]);
  const onResetZoom = useCallback(() => {
    if (isZoomDisabled) return;
    zoom.provides?.requestZoom(1, getViewportCenter());
  }, [zoom.provides, isZoomDisabled, getViewportCenter]);
  const onToggleThumbnails = useCallback(() => setIsThumbnailOpen((value) => !value), []);
  const currentPageIndex = Math.max(0, (scroll.state.currentPage || 1) - 1);
  const currentPage = scroll.state.currentPage || 1;
  const totalPages = scroll.state.totalPages || 0;
  const onPreviousPage = useCallback(() => {
    scroll.provides?.scrollToPreviousPage?.();
  }, [scroll.provides]);
  const onNextPage = useCallback(() => {
    scroll.provides?.scrollToNextPage?.();
  }, [scroll.provides]);

  const [isPaginationEditing, setIsPaginationEditing] = useState(false);
  const [paginationDraft, setPaginationDraft] = useState("");
  const paginationInputRef = useRef<HTMLInputElement | null>(null);

  const commitPaginationDraft = useCallback(() => {
    const raw = paginationDraft.trim();
    setIsPaginationEditing(false);
    setPaginationDraft("");

    if (!raw) return;
    const pageNumber = Number.parseInt(raw, 10);
    if (!Number.isFinite(pageNumber)) return;

    const clampedPage = Math.min(Math.max(pageNumber, 1), totalPages || 1);
    scroll.provides?.scrollToPage({ pageNumber: clampedPage, behavior: "smooth", alignY: 0 });
  }, [paginationDraft, scroll.provides, totalPages]);

  useEffect(() => {
    if (!isPaginationEditing) return;
    paginationInputRef.current?.focus();
    paginationInputRef.current?.select();
  }, [isPaginationEditing]);

  const onStartPaginationEdit = useCallback(() => {
    setPaginationDraft(String(currentPage));
    setIsPaginationEditing(true);
  }, [currentPage]);
  const onSelectThumbnail = useCallback(
    (pageIndex: number) => {
      scroll.provides?.scrollToPage({ pageNumber: pageIndex + 1, behavior: "smooth", alignY: 0 });
    },
    [scroll.provides],
  );

  const onRotateLeft = useCallback(() => rotate.provides?.rotateBackward(), [rotate.provides]);
  const onRotateRight = useCallback(() => rotate.provides?.rotateForward(), [rotate.provides]);
  const onRotateLeftPersisted = useCallback(() => {
    onRotateLeft();
    persistRotationSteps(((rotationSteps + 3) % 4 + 4) % 4);
  }, [onRotateLeft, persistRotationSteps, rotationSteps]);

  const onRotateRightPersisted = useCallback(() => {
    onRotateRight();
    persistRotationSteps(((rotationSteps + 1) % 4 + 4) % 4);
  }, [onRotateRight, persistRotationSteps, rotationSteps]);

  const onToggleSignatureModal = useCallback(() => {
    // UX: si estaba bloqueado y el usuario intenta adjuntar otra firma, desbloquear automÃ¡ticamente.
    if (isSignatureLocked) unlockSignatures();
    setIsSignatureModalOpen((v) => !v);
  }, [isSignatureLocked, unlockSignatures]);
  const onPrint = useCallback(async () => {
    // Mantener plugin oficial para print, pero garantizando commit primero.
    try {
      if (annotationCap.provides?.commit) await waitPdfTaskVoid(annotationCap.provides.commit());
    } catch {
      // ignore
    }
    print.provides?.print();
  }, [annotationCap.provides, print.provides]);

  const onExport = useCallback(async () => {
    // Opción 4 (sin parpadeo): exportar buffer ya "materializado" y descargarlo nosotros.
    // Esto evita que el plugin `download()` use un snapshot anterior.
    try {
      if (annotationCap.provides?.commit) await waitPdfTaskVoid(annotationCap.provides.commit());
    } catch {
      // ignore
    }

    if (!exportApi.provides) return;
    // Compatibilidad: si el plugin expone `download()`, usarlo directamente (legacy tests / consumers).
    if ((exportApi.provides as any).download) {
      try {
        (exportApi.provides as any).download();
      } catch {
        // ignore
      }
      return;
    }

    const buffer = await waitPdfTask<ArrayBuffer | Uint8Array<ArrayBufferLike>>(exportApi.provides.saveAsCopy());
    downloadBuffer(buffer, `document-${documentId}.pdf`);
  }, [annotationCap.provides, documentId, downloadBuffer, exportApi.provides]);

  const logSignatureExportDiagnostics = useCallback(
    async (stage: string, pageNumbers: number[]) => {
      if (!dvDebugEnabled()) return;
      const scope = annotation.provides;
      if (!scope) return;

      const signatureAnnotations = Object.entries(annotation.state.pages ?? {}).flatMap(([pageIndexKey, ids]) => {
        const pageIndex = Number(pageIndexKey);
        if (!Array.isArray(ids)) return [];
        return ids
          .map((uid) => {
            const tracked = typeof uid === "string" ? scope.getAnnotationById?.(uid) : null;
            return tracked ? { pageIndex, uid, tracked } : null;
          })
          .filter((item): item is { pageIndex: number; uid: string; tracked: any } => {
            if (!item) return false;
            return item.tracked.object?.subject === "Signature";
          });
      });

      const transferSummaries = [];
      for (const pageNumber of pageNumbers) {
        const pageIndex = pageNumber - 1;
        try {
          const exportedItems = await waitPdfTask<any[]>(scope.exportAnnotations({ pageIndex }));
          transferSummaries.push({
            pageIndex,
            items: exportedItems.map((item) => ({
              id: item?.annotation?.id,
              type: item?.annotation?.type,
              subject: item?.annotation?.subject,
              pageIndex: item?.annotation?.pageIndex,
              rect: item?.annotation?.rect,
              unrotatedRect: item?.annotation?.unrotatedRect,
              rotation: item?.annotation?.rotation,
              ctxKeys: item?.ctx ? Object.keys(item.ctx) : [],
              ctxDataBytes: getByteLength(item?.ctx?.data ?? item?.ctx?.imageData ?? item?.ctx?.appearance),
              mimeType: item?.ctx?.mimeType,
            })),
          });
        } catch (err) {
          transferSummaries.push({ pageIndex, error: err instanceof Error ? err.message : String(err) });
        }
      }

      const renderSummaries = [];
      for (const item of signatureAnnotations) {
        try {
          const rendered = await waitPdfTask<Blob>(
            scope.renderAnnotation({ pageIndex: item.pageIndex, annotation: item.tracked.object }),
          );
          renderSummaries.push({
            pageIndex: item.pageIndex,
            uid: item.uid,
            objectId: item.tracked.object?.id,
            type: item.tracked.object?.type,
            subject: item.tracked.object?.subject,
            rect: item.tracked.object?.rect,
            unrotatedRect: item.tracked.object?.unrotatedRect,
            rotation: item.tracked.object?.rotation,
            renderedSizeBytes: rendered.size,
            renderedType: rendered.type,
          });
        } catch (err) {
          renderSummaries.push({
            pageIndex: item.pageIndex,
            uid: item.uid,
            error: err instanceof Error ? err.message : String(err),
          });
        }
      }

      dvLog(`[DV][firma][${stage}:transfer-diagnostics]`, {
        documentId,
        transferSummaries,
        renderSummaries,
      });
    },
    [annotation.provides, annotation.state.pages, documentId],
  );

  const burnSignatureAppearancesIntoSinglePagePdf = useCallback(
    async (sourcePdf: Blob, pageNumber: number, signal?: AbortSignal): Promise<Blob> => {
      const scope = annotation.provides;
      if (!scope) return sourcePdf;

      const sourcePageIndex = pageNumber - 1;
      const pagesByIndex = (annotation.state.pages ?? {}) as Record<string, string[]>;
      const annotationIds = pagesByIndex[String(sourcePageIndex)] ?? [];
      const signatureAnnotations = Array.isArray(annotationIds)
        ? annotationIds
            .map((uid) => {
              const tracked = typeof uid === "string" ? scope.getAnnotationById?.(uid) : null;
              const object = tracked?.object as any;
              return tracked && object?.subject === "Signature" && object?.rect
                ? { sourcePageIndex, uid, annotation: object }
                : null;
            })
            .filter(
              (item): item is { sourcePageIndex: number; uid: string; annotation: any } =>
                Boolean(item),
            )
        : [];

      if (signatureAnnotations.length === 0) {
        return sourcePdf;
      }

      const renderedSignatures = [];
      for (const item of signatureAnnotations) {
        signal?.throwIfAborted();
        const rendered = await waitPdfTask<Blob>(
          scope.renderAnnotation({ pageIndex: item.sourcePageIndex, annotation: item.annotation }),
        );
        renderedSignatures.push({ ...item, rendered });
      }

      signal?.throwIfAborted();
      const { PDFDocument } = await import("pdf-lib");
      const sourceBytes = await sourcePdf.arrayBuffer();
      const pdfDocument = await PDFDocument.load(sourceBytes);
      const page = pdfDocument.getPage(0);

      for (const item of renderedSignatures) {
        signal?.throwIfAborted();
        if (!page) continue;

        const imageBytes = await item.rendered.arrayBuffer();
        const image = await pdfDocument.embedPng(imageBytes);
        const rect = item.annotation.unrotatedRect ?? item.annotation.rect;
        const x = rect.origin.x;
        const y = page.getHeight() - rect.origin.y - rect.size.height;

        page.drawImage(image, {
          x,
          y,
          width: rect.size.width,
          height: rect.size.height,
          rotate: undefined,
        });
      }

      const bytes = await pdfDocument.save();
      const pdfWithBurnedSignatures = new Blob([blobPartFromBytes(bytes)], { type: "application/pdf" });
      dvLog("[DV][firma][replace-export:burned-signatures-single-page]", {
        documentId,
        pageNumber,
        signatures: renderedSignatures.map((item) => ({
          sourcePageIndex: item.sourcePageIndex,
          targetPageIndex: 0,
          uid: item.uid,
          rect: item.annotation.rect,
          unrotatedRect: item.annotation.unrotatedRect,
          rotation: item.annotation.rotation,
          renderedSizeBytes: item.rendered.size,
        })),
        sourcePdfSizeBytes: sourcePdf.size,
        resultPdfSizeBytes: pdfWithBurnedSignatures.size,
      });

      return pdfWithBurnedSignatures;
    },
    [annotation.provides, annotation.state.pages, documentId],
  );

  const exportAnnotatedPdfPages = useCallback(
    async (
      options: AppVisorExportAnnotatedPdfPagesOptions = {},
    ): Promise<AppVisorExportAnnotatedPdfPagesResult> => {
      options.signal?.throwIfAborted();
      if (!permissionsEffective.allowAnnotationEdit) {
        throw new Error("No tiene permisos para guardar paginas anotadas.");
      }
      const activeDocument = activeDocumentState?.document;
      if (!activeDocument) {
        throw new Error("El visor PDF no tiene un documento cargado para extraer paginas anotadas.");
      }

      const pageNumbers = getAnnotatedPageNumbers(annotation.state.pages ?? {});
      dvLog("[DV][firma][replace-export:start]", {
        documentId,
        pageNumbers,
        hasAnySignaturePlaced,
        signatureEntries: summarizeSignatureEntries(signatureEntries.entries ?? []),
        annotationPages: summarizeAnnotationPages(annotation.state.pages ?? {}, annotation.provides?.getAnnotationById),
      });
      if (pageNumbers.length === 0) {
        return { hasAnnotations: false, annotatedPages: [], pageNumbers: [], pages: [] };
      }

      await logSignatureExportDiagnostics("replace-export:before-commit", pageNumbers);

      if (annotationCap.provides?.commit) {
        await waitPdfTaskVoid(annotationCap.provides.commit());
      }
      options.signal?.throwIfAborted();

      await logSignatureExportDiagnostics("replace-export:after-commit", pageNumbers);

      dvLog("[DV][firma][replace-export:after-commit]", {
        documentId,
        pageNumbers,
        hasAnySignaturePlaced,
        signatureEntries: summarizeSignatureEntries(signatureEntries.entries ?? []),
        annotationPages: summarizeAnnotationPages(annotation.state.pages ?? {}, annotation.provides?.getAnnotationById),
      });

      const pages = [];
      for (const pageNumber of pageNumbers) {
        options.signal?.throwIfAborted();
        const pageIndex = pageNumber - 1;
        const pageBuffer = await waitPdfTask<ArrayBuffer>(engine.extractPages(activeDocument, [pageIndex]));
        options.signal?.throwIfAborted();
        const singlePagePdf = new Blob([pageBuffer], { type: "application/pdf" });
        dvLog("[DV][firma][replace-export:after-extract-page]", {
          documentId,
          pageNumber,
          sourcePageIndex: pageIndex,
          singlePagePdfSizeBytes: singlePagePdf.size,
        });
        const blob = await burnSignatureAppearancesIntoSinglePagePdf(singlePagePdf, pageNumber, options.signal);
        options.signal?.throwIfAborted();
        const hashSha256 = options.calculateHashSha256 ? await calculateBlobSha256(blob) : undefined;
        pages.push({
          pageNumber,
          fileName: `document-${documentId}-page-${pageNumber}-annotated.pdf`,
          blob,
          sizeBytes: blob.size,
          hashSha256,
        });
      }

      dvLog("[DV][visor][reemplazo-paginas]", {
        documentId,
        pageNumbers,
        totalPages: pages.length,
        hashesCalculated: Boolean(options.calculateHashSha256),
        pageSizesBytes: pages.map((page) => ({ pageNumber: page.pageNumber, sizeBytes: page.sizeBytes })),
      });

      return { hasAnnotations: true, annotatedPages: pageNumbers, pageNumbers, pages };
    },
    [
      annotation.state.pages,
      annotation.provides,
      annotationCap.provides,
      activeDocumentState?.document,
      documentId,
      engine,
      burnSignatureAppearancesIntoSinglePagePdf,
      hasAnySignaturePlaced,
      logSignatureExportDiagnostics,
      permissionsEffective.allowAnnotationEdit,
      signatureEntries.entries,
    ],
  );

  useEffect(() => {
    onExportAnnotatedPdfPagesReady(exportAnnotatedPdfPages);
    return () => onExportAnnotatedPdfPagesReady(null);
  }, [exportAnnotatedPdfPages, onExportAnnotatedPdfPagesReady]);

  const onStartSignaturePlacement = useCallback(
    (signature: SignatureFieldDefinition) => {
      if (!signatureCap.provides) return;
      const entryId = signatureCap.provides.addEntry({ signature });
      signatureCap.provides.forDocument(documentId).activateSignaturePlacement(entryId);
      dvLog("[DV][firma][placement-started]", {
        documentId,
        entryId,
        signatureEntries: summarizeSignatureEntries(signatureEntries.entries ?? []),
        annotationPages: summarizeAnnotationPages(annotation.state.pages ?? {}, annotation.provides?.getAnnotationById),
      });
      setIsSignatureModalOpen(false);
    },
    [annotation.provides, annotation.state.pages, documentId, signatureCap.provides, signatureEntries.entries],
  );

  useEffect(() => {
    const provides = viewport.provides;
    if (!provides) return;

    const scope = provides.forDocument(documentId);

    const sync = () => {
      rafRef.current = null;
      const m = scope.getMetrics();
      // Comportamiento tipo WhatsApp: aparece solo cuando realmente estÃ¡s "abajo".
      setShowScrollTop(m.scrollTop > Math.max(120, m.clientHeight * 0.5));
    };

    const off = scope.onScrollChange(() => {
      if (rafRef.current != null) return;
      rafRef.current = requestAnimationFrame(sync);
    });

    sync();

    return () => {
      off?.();
      if (rafRef.current != null) cancelAnimationFrame(rafRef.current);
      rafRef.current = null;
    };
  }, [viewport.provides, documentId]);

  const onScrollToTop = useCallback(() => {
    const scope = viewport.provides?.forDocument(documentId);
    scope?.scrollTo({ x: 0, y: 0, behavior: "smooth" });
  }, [viewport.provides, documentId]);

  const onGuideTourEvent = useCallback((event: AppGuideTourEvent) => {
    if (typeof window === "undefined") return;
    window.dispatchEvent(new CustomEvent("app-guide-tour:event", { detail: event }));
  }, []);

  const onStartGuideTour = useCallback(() => {
    guideTourRef.current?.start();
  }, []);

  return (
    <>
      <AppGuideTour
        ref={guideTourRef}
        tourId={APP_VISOR_EMBED_PDF_GUIDE_TOUR_ID}
        steps={APP_VISOR_EMBED_PDF_GUIDE_STEPS}
        onEvent={onGuideTourEvent}
      />
      {/** overlay se monta en AppVisorEmbedPdf (raíz) para cubrir también primer click */}
      <div className={styles.toolbarShell} role="toolbar" aria-label="Toolbar PDF" data-guide-tour-id="pdf-toolbar">
        {loading ? (
          <div className={styles.toolbarSkeleton} aria-label="Cargando toolbar" role="status" aria-busy="true">
            <span className={styles.toolbarSkeletonBlock} />
            <span className={styles.toolbarSkeletonBlock} />
            <span className={styles.toolbarSkeletonBlock} />
            <span className={styles.toolbarSkeletonBlockWide} />
            <span className={styles.toolbarSkeletonBlock} />
            <span className={styles.toolbarSkeletonBlock} />
          </div>
        ) : (
          <AppPdfToolbar
            zoomLevel={zoomLevel}
            onZoomIn={onZoomIn}
            onZoomOut={onZoomOut}
            onResetZoom={onResetZoom}
            onToggleThumbnails={onToggleThumbnails}
            isThumbnailOpen={isThumbnailOpen}
            isZoomDisabled={isZoomDisabled}
            onRotateLeft={onRotateLeftPersisted}
            onRotateRight={onRotateRightPersisted}
            onToggleSignatureModal={onToggleSignatureModal}
            isSignatureModalOpen={isSignatureModalOpen}
            isSignatureDisabled={!permissionsEffective.allowSignaturePlacement}
            onDeleteSelectedSignature={onDeleteSelectedSignature}
            canDeleteSelectedSignature={Boolean(getSelectedSignature()) && !isSignatureLocked}
            isDeleteSelectedSignatureDisabled={!permissionsEffective.allowSignatureDelete}
            onSaveSignedPdf={onSaveSignedPdf}
            isSignatureLocked={isSignatureLocked}
            isSaveSignedPdfDisabled={!hasAnySignaturePlaced && !isSignatureLocked}
            isSavingSignedPdf={isSavingSignedPdf}
            isSignatureLockToggleDisabled={!permissionsEffective.allowSignatureLockToggle}
            onPrint={onPrint}
            onExport={onExport}
            isPrintDisabled={!permissionsEffective.allowPrint}
            isExportDisabled={!permissionsEffective.allowExport}
            onSaveAnnotatedPages={onSaveAnnotatedPages}
            isSaveAnnotatedPagesDisabled={isSaveAnnotatedPagesDisabled || !permissionsEffective.allowAnnotationEdit}
            isSavingAnnotatedPages={isSavingAnnotatedPages}
            saveAnnotatedPagesProgress={saveAnnotatedPagesProgress}
            onStartGuideTour={onStartGuideTour}
            isGuideTourAvailable
          />
        )}
      </div>
      <AppPdfSignatureModal
        isOpen={isSignatureModalOpen}
        onClose={() => setIsSignatureModalOpen(false)}
        onStartPlacement={onStartSignaturePlacement}
        isPlacementReady={isSignaturePlacementReady && Boolean(signatureCap.provides)}
      />
      {isSavingSignedPdf ? (
        <div className={styles.signatureSavingOverlay} role="status" aria-label="Bloqueando firma" aria-busy="true">
          <Spin size="large" tip="Bloqueando firma" />
        </div>
      ) : null}
      <div
        className={styles.main}
        data-signature-active-placement={activePlacement ? "true" : "false"}
        data-signature-entry-count={String(signatureEntries.entries?.length ?? 0)}
      >
        <div
          className={styles.paginationOverlay}
          role="group"
          aria-label="PaginaciÃ³n"
          data-guide-tour-id="pdf-pagination"
        >
          {loading ? (
            <div className={styles.paginationSkeleton} aria-label="Cargando paginación" role="status" aria-busy="true">
              <span className={styles.paginationSkeletonButton} />
              <span className={styles.paginationSkeletonIndicator} />
              <span className={styles.paginationSkeletonButton} />
            </div>
          ) : null}
          <button
            type="button"
            className={styles.paginationButton}
            onClick={onPreviousPage}
            aria-label="PÃ¡gina anterior"
            title="PÃ¡gina anterior"
          >
            <LeftOutlined aria-hidden="true" />
          </button>
          {isPaginationEditing ? (
            <input
              ref={paginationInputRef}
              className={styles.paginationInput}
              aria-label={`Ir a pÃƒÂ¡gina (1-${totalPages || 1})`}
              inputMode="numeric"
              value={paginationDraft}
              onChange={(event) => setPaginationDraft(event.target.value)}
              onBlur={commitPaginationDraft}
              onKeyDown={(event) => {
                if (event.key === "Enter") commitPaginationDraft();
                if (event.key === "Escape") {
                  setIsPaginationEditing(false);
                  setPaginationDraft("");
                }
              }}
            />
          ) : (
            <div
            className={styles.paginationIndicator}
            role="button"
            tabIndex={0}
            onClick={onStartPaginationEdit}
            onKeyDown={(event) => {
              if (event.key === "Enter" || event.key === " ") onStartPaginationEdit();
            }}
            aria-label={`PÃ¡gina ${currentPage} de ${totalPages}`}
            title={`PÃ¡gina ${currentPage} de ${totalPages}`}
          >
            {currentPage}/{totalPages}
          </div>
          )}
          <button
            type="button"
            className={styles.paginationButton}
            onClick={onNextPage}
            aria-label="PÃ¡gina siguiente"
            title="PÃ¡gina siguiente"
          >
            <RightOutlined aria-hidden="true" />
          </button>
        </div>
        <button
          type="button"
          className={`${styles.scrollTopFab} ${showScrollTop ? "" : styles.scrollTopFabHidden}`}
          onClick={onScrollToTop}
          data-guide-tour-id="pdf-scroll-top"
          aria-label="Ir arriba"
          title="Ir arriba"
        >
          <UpOutlined aria-hidden="true" />
        </button>
        {isThumbnailOpen ? (
          <aside className={styles.thumbnails} aria-label="Panel thumbnails">
            <ThumbnailsPane documentId={documentId} className={styles.thumbnailsPane}>
              {(meta) => (
                <div
                  key={meta.pageIndex}
                  className={`${styles.thumbRow} ${meta.pageIndex === currentPageIndex ? styles.thumbRowActive : ""}`}
                  style={{ top: meta.top, height: meta.wrapperHeight }}
                  role="button"
                  tabIndex={0}
                  aria-label={`Ir a pÃ¡gina ${meta.pageIndex + 1}`}
                  onClick={() => onSelectThumbnail(meta.pageIndex)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter" || e.key === " ") onSelectThumbnail(meta.pageIndex);
                  }}
                >
                  <ThumbImg documentId={documentId} meta={meta} className={styles.thumbImg} />
                  <div className={styles.thumbLabel}>{meta.pageIndex + 1}</div>
                </div>
              )}
            </ThumbnailsPane>
          </aside>
        ) : null}
        <Viewport
          documentId={documentId}
          className={styles.viewport}
          style={{ padding: 0, overflow: "auto" }}
        >
            <Scroller
              documentId={documentId}
            renderPage={({ pageIndex, width, height, rotatedWidth, rotatedHeight }) => {
              const slotWidth = Math.ceil(rotatedWidth);
              const slotHeight = Math.ceil(rotatedHeight);
              const baseWidth = Math.ceil(width);
              const baseHeight = Math.ceil(height);
              const slotLooksRotated = slotWidth !== baseWidth || slotHeight !== baseHeight;

              return (
              <div
                className={styles.pageLayer}
                // `width/height` aquÃ­ representan el "slot" calculado por el Scroll plugin
                // para la pÃ¡gina actual (ya considera rotaciÃ³n/escala).
                style={{
                  width: slotWidth,
                  height: slotHeight,
                  // Evitar clipping cuando el slot efectivo es rotado (/Rotate) y el contenido se envuelve en <Rotate>.
                  // Mantener visible el contenido mientras el engine termina de calcular y pintar.
                  overflow: "visible",
                }}
              >
                {rotationSteps === 0 && slotLooksRotated ? (
                  <Rotate
                    documentId={documentId}
                    pageIndex={pageIndex}
                    // Para 90/270 el clipping suele ocurrir en el eje "alto" del contenedor rotado.
                    // Reusamos la misma estrategia del branch normal de <Rotate>: el slot es (rotatedW x rotatedH),
                    // pero el contenedor rotado necesita suficiente "alto base" para que el contenido portrait no se recorte.
                    style={{ width: slotWidth + 2, height: baseHeight + 2 }}
                  >
                    <div style={{ width: baseWidth, height: baseHeight }}>
                  <PagePointerProvider
                    documentId={documentId}
                    pageIndex={pageIndex}
                    scale={zoomLevel}
                    rotation={1 as any}
                  >
                    <RenderLayer documentId={documentId} pageIndex={pageIndex} />
                    <SelectionLayer
                      documentId={documentId}
                      pageIndex={pageIndex}
                      selectionMenu={selectionMenu}
                    />
                    <AnnotationLayer
                      documentId={documentId}
                      pageIndex={pageIndex}
                      scale={zoomLevel}
                      rotation={1 as any}
                    />
                  </PagePointerProvider>
                    </div>
                  </Rotate>
                ) : rotationSteps === 0 ? (
                  <PagePointerProvider
                    documentId={documentId}
                    pageIndex={pageIndex}
                    scale={zoomLevel}
                    rotation={rotationRaw as any}
                  >
                    <RenderLayer documentId={documentId} pageIndex={pageIndex} />
                    <SelectionLayer
                      documentId={documentId}
                      pageIndex={pageIndex}
                      selectionMenu={selectionMenu}
                    />
                    <AnnotationLayer
                      documentId={documentId}
                      pageIndex={pageIndex}
                      scale={zoomLevel}
                      rotation={rotationRaw as any}
                    />
                  </PagePointerProvider>
                ) : (
                  <Rotate
                    documentId={documentId}
                    pageIndex={pageIndex}
                    // `Rotate` aplica `contain: ... paint` y define width/height del contenedor.
                    // En 90/270, 1px de rounding puede cortar contenido. Expandimos levemente
                    // el contenedor rotado (sin cambiar el slot del scroller) para evitar clipping.
                    style={
                      rotationSteps % 2 === 1
                        ? {
                            width: Math.ceil(rotatedWidth) + 2,
                            // Para 90/270 el clipping se manifiesta principalmente en el eje Y.
                            // Usamos el alto base (`height`) para evitar recorte del contenido rotado.
                            height: Math.ceil(height),
                          }
                        : {
                            // Para 180 (y 0 en teoría, aunque este branch no corre en 0),
                            // el slot no cambia de orientación pero puede haber clipping por rounding subpixel.
                            width: Math.ceil(width) + 2,
                            height: Math.ceil(height) + 2,
                          }
                    }
                  >
                    {/* 
                      Rotate aplica una transform matrix sobre un contenedor ABSOLUTE.
                      Para evitar "stretch" / clipping en 90/270, el contenido debe
                      mantener su tamaÃ±o base (sin rotaciÃ³n): (height x width).
                      El slot del scroller (width x height) ya es el tamaÃ±o rotado.
                    */}
                    <div
                      style={{
                        width: Math.ceil(width),
                        height: Math.ceil(height),
                      }}
                    >
                      <PagePointerProvider
                        documentId={documentId}
                        pageIndex={pageIndex}
                        scale={zoomLevel}
                        rotation={rotationRaw as any}
                      >
                        <RenderLayer documentId={documentId} pageIndex={pageIndex} />
                        <SelectionLayer
                          documentId={documentId}
                          pageIndex={pageIndex}
                          selectionMenu={selectionMenu}
                        />
                        <AnnotationLayer
                          documentId={documentId}
                          pageIndex={pageIndex}
                          scale={zoomLevel}
                          rotation={rotationRaw as any}
                        />
                      </PagePointerProvider>
                    </div>
                  </Rotate>
                )}
              </div>
              );
            }}
          />
        </Viewport>
      </div>
    </>
  );
}
