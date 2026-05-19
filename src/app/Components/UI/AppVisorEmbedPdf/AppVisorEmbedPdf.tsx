import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useZoom } from "@embedpdf/plugin-zoom/react";
import { ThumbnailsPane, ThumbImg } from "@embedpdf/plugin-thumbnail/react";
import { useScroll } from "@embedpdf/plugin-scroll/react";
import { Rotate, useRotate } from "@embedpdf/plugin-rotate/react";
import { useViewportCapability } from "@embedpdf/plugin-viewport/react";
import { LeftOutlined, RightOutlined, UpOutlined } from "@ant-design/icons";
import { usePrint } from "@embedpdf/plugin-print/react";
import { useExport } from "@embedpdf/plugin-export/react";
import { PdfErrorCode } from "@embedpdf/models";
import { AnnotationLayer } from "@embedpdf/plugin-annotation/react";
import { PagePointerProvider } from "@embedpdf/plugin-interaction-manager/react";
import { useAnnotation, useAnnotationCapability } from "@embedpdf/plugin-annotation/react";
import { LockModeType } from "@embedpdf/plugin-annotation";
import { useSelectionCapability } from "@embedpdf/plugin-selection/react";
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
import styles from "./styles/AppVisorEmbedPdf.module.css";
import type { AppVisorEmbedPdfProps } from "./types/AppVisorEmbedPdfProps";

function cx(...parts: Array<string | undefined>) {
  return parts.filter(Boolean).join(" ");
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

async function saveBlobToIndexedDb(params: { documentId: string; name: string; blob: Blob }) {
  const { documentId, name, blob } = params;
  const db = await new Promise<IDBDatabase>((resolve, reject) => {
    const request = indexedDB.open("docuarchi-appvisor", 1);
    request.onupgradeneeded = () => {
      const database = request.result;
      if (!database.objectStoreNames.contains("signed_pdfs")) {
        database.createObjectStore("signed_pdfs", { keyPath: "documentId" });
      }
    };
    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error ?? new Error("IndexedDB open failed"));
  });

  await new Promise<void>((resolve, reject) => {
    const tx = db.transaction("signed_pdfs", "readwrite");
    tx.oncomplete = () => resolve();
    tx.onerror = () => reject(tx.error ?? new Error("IndexedDB transaction failed"));
    tx.objectStore("signed_pdfs").put({ documentId, name, blob, savedAt: new Date().toISOString() });
  });

  db.close();
}

/**
 * AppVisorEmbedPdf (01-FE)
 *
 * NOTA: Este componente encapsula EmbedPDF/Pdfium y expone una API mÃ­nima.
 * Se prohÃ­be filtrar detalles del engine hacia mÃ³dulos consumidores.
 */
export function AppVisorEmbedPdf({ fileUrl, className, style }: AppVisorEmbedPdfProps) {
  const demoUrl = useDemoPdfUrl();
  const effectiveFileUrl = fileUrl?.trim() ? fileUrl.trim() : demoUrl;

  const engineState = useEmbedPdfEngine();
  const pluginRegistration = useMemo(() => createBasicPluginRegistration(), []);

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
        <EmptyState />
      </div>
    );
  }

  return (
    <div className={cx(styles.root, className)} style={style} role="status" aria-label="Zona de documento">
      <EmbedPDF engine={engineState.engine} plugins={pluginRegistration}>
        <EmbedPdfDocumentHost fileUrl={effectiveFileUrl} />
      </EmbedPDF>
    </div>
  );
}

function EmbedPdfDocumentHost({ fileUrl }: { fileUrl: string }) {
  const { provides } = useDocumentManagerCapability();
  const { activeDocumentId } = useActiveDocument();

  const [password, setPassword] = useState<string | null>(null);
  const [passwordAttempt, setPasswordAttempt] = useState(0);
  const [passwordPromptOpen, setPasswordPromptOpen] = useState(false);
  const [invalidPassword, setInvalidPassword] = useState(false);
  const [isSubmittingPassword, setIsSubmittingPassword] = useState(false);

  const openedDocumentIdRef = useRef<string | null>(null);
  const lastOpenedRef = useRef<
    { url: string; password: string | null; attempt: number } | null
  >(null);
  const lastAttemptHadPasswordRef = useRef(false);

  useEffect(() => {
    // Al cambiar el documento, reiniciar el estado del prompt/password (enterprise hardening).
    setPassword(null);
    setPasswordAttempt(0);
    setPasswordPromptOpen(false);
    setInvalidPassword(false);
    setIsSubmittingPassword(false);
    openedDocumentIdRef.current = null;
    lastOpenedRef.current = null;
    lastAttemptHadPasswordRef.current = false;
  }, [fileUrl]);

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
        openedDocumentIdRef.current = response.documentId;

        // Esperar el task interno de carga del PDF para cerrar el estado "Validando…"
        // incluso si el documento no llega a activarse (activeDocumentId sigue null).
        response.task.wait(
          () => {
            if (cancelled) return;
            setIsSubmittingPassword(false);
            setPasswordPromptOpen(false);
            setInvalidPassword(false);
          },
          () => {
            if (cancelled) return;
            setIsSubmittingPassword(false);
            setPasswordPromptOpen(true);
            setInvalidPassword(lastAttemptHadPasswordRef.current);
          },
        );
      },
      () => {
        if (cancelled) return;
        setIsSubmittingPassword(false);
        setPasswordPromptOpen(true);
        setInvalidPassword(lastAttemptHadPasswordRef.current);
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

      setIsSubmittingPassword(false);
      setPasswordPromptOpen(true);
      setInvalidPassword(lastAttemptHadPasswordRef.current);
    });
    return () => {
      off?.();
    };
  }, [provides]);
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
        setIsSubmittingPassword(false);
        setPasswordPromptOpen(false);
        setInvalidPassword(false);
      },
      () => {
        setIsSubmittingPassword(false);
        setPasswordPromptOpen(true);
        setInvalidPassword(true);
      },
        );
      },
      () => {
        setIsSubmittingPassword(false);
        setPasswordPromptOpen(true);
        setInvalidPassword(true);
      },
    );
  }, [provides]);

  const onPasswordError = useCallback(() => {
    // Si el documento vuelve a fallar luego de enviar password, dejar de "validar"
    // y mostrar estado inválido para permitir reintento.
    setIsSubmittingPassword(false);
    setPasswordPromptOpen(true);
    setInvalidPassword(lastAttemptHadPasswordRef.current);
  }, []);

  if (!activeDocumentId) {
    return (
      <>
        <DocumentLoadingState />
        {passwordPromptOpen ? (
          <AppPdfPasswordPrompt
            isInvalidPassword={invalidPassword}
            isLoading={isSubmittingPassword}
            onSubmit={onSubmitPassword}
          />
        ) : null}
      </>
    );
  }

  return (
    <DocumentContent documentId={activeDocumentId}>
      {({ isLoaded, isError, isLoading }) => (
        <EmbedPdfDocumentStateView
          documentId={activeDocumentId}
          isLoaded={isLoaded}
          isError={isError}
          isLoading={isLoading}
          passwordPromptOpen={passwordPromptOpen}
          invalidPassword={invalidPassword}
          isSubmittingPassword={isSubmittingPassword}
          lastAttemptHadPassword={lastAttemptHadPasswordRef.current}
          onSubmitPassword={onSubmitPassword}
          onPasswordError={onPasswordError}
        />
      )}
    </DocumentContent>
  );
}

function EmbedPdfDocumentStateView({
  documentId,
  isLoaded,
  isError,
  isLoading,
  passwordPromptOpen,
  invalidPassword,
  isSubmittingPassword,
  lastAttemptHadPassword,
  onSubmitPassword,
  onPasswordError,
}: {
  documentId: string;
  isLoaded: boolean;
  isError: boolean;
  isLoading: boolean;
  passwordPromptOpen: boolean;
  invalidPassword: boolean;
  isSubmittingPassword: boolean;
  lastAttemptHadPassword: boolean;
  onSubmitPassword(password: string): void;
  onPasswordError(): void;
}) {
  useEffect(() => {
    if (!isError) return;
    onPasswordError();
  }, [isError, onPasswordError]);

  if (isLoaded) {
    return (
      <>
        <EmbedPdfLoadedDocumentView documentId={documentId} />
        {passwordPromptOpen ? (
          <AppPdfPasswordPrompt
            isInvalidPassword={invalidPassword}
            isLoading={isSubmittingPassword}
            onSubmit={onSubmitPassword}
          />
        ) : null}
      </>
    );
  }

  if (isError) {
    return (
      <>
        <ErrorState />
        <AppPdfPasswordPrompt
          isInvalidPassword={lastAttemptHadPassword}
          isLoading={isSubmittingPassword}
          onSubmit={onSubmitPassword}
        />
      </>
    );
  }

  if (isLoading) return <DocumentLoadingState />;
  return <DocumentLoadingState />;
}

function EmbedPdfLoadedDocumentView({ documentId }: { documentId: string }) {
  const zoom = useZoom(documentId);
  const zoomLevel = typeof zoom.state.currentZoomLevel === "number" ? zoom.state.currentZoomLevel : 1;
  const [isThumbnailOpen, setIsThumbnailOpen] = useState(false);
  const [isSignatureModalOpen, setIsSignatureModalOpen] = useState(false);

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

  const downloadBuffer = useCallback((buffer: ArrayBuffer | Uint8Array, filename: string) => {
    const blob = new Blob([buffer], { type: "application/pdf" });
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
    // Mantener plugin `selection` (requerido por `annotation`) pero desactivar su comportamiento
    // para evitar overlays/hitboxes de texto.
    selection.provides.enableForMode(
      "default",
      {
        enableSelection: false,
        enableMarquee: false,
        showSelectionRects: false,
        showMarqueeRects: false,
      },
      documentId,
    );
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
    if (!exportApi.provides) return;

    setIsSavingSignedPdf(true);
    try {
      // Asegurar que la firma quede aplicada en el documento antes de exportar.
      if (annotationCap.provides?.commit) {
        await waitPdfTask<void>(annotationCap.provides.commit());
      }

      const buffer = await waitPdfTask<ArrayBuffer | Uint8Array>(exportApi.provides.saveAsCopy(documentId) as any);
      const blob = new Blob([buffer], { type: "application/pdf" });
      await saveBlobToIndexedDb({ documentId, name: `signed-${documentId}.pdf`, blob });

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
  }, [annotationCap.provides, documentId, exportApi.provides, hasAnySignaturePlaced, isSignatureLocked, unlockSignatures]);


  const getViewportCenter = useCallback(() => {
    const scope = viewport.provides?.forDocument(documentId);
    const m = scope?.getMetrics();
    if (!m) return undefined;
    return { vx: m.clientWidth / 2, vy: m.clientHeight / 2 };
  }, [viewport.provides, documentId]);

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
  const onResetRotation = useCallback(() => rotate.provides?.setRotation(0), [rotate.provides]);

  const onToggleSignatureModal = useCallback(() => {
    // UX: si estaba bloqueado y el usuario intenta adjuntar otra firma, desbloquear automÃ¡ticamente.
    if (isSignatureLocked) unlockSignatures();
    setIsSignatureModalOpen((v) => !v);
  }, [isSignatureLocked, unlockSignatures]);
  const onPrint = useCallback(async () => {
    // Mantener plugin oficial para print, pero garantizando commit primero.
    try {
      if (annotationCap.provides?.commit) await waitPdfTask<void>(annotationCap.provides.commit());
    } catch {
      // ignore
    }
    print.provides?.print();
  }, [annotationCap.provides, print.provides]);

  const onExport = useCallback(async () => {
    // Opción 4 (sin parpadeo): exportar buffer ya "materializado" y descargarlo nosotros.
    // Esto evita que el plugin `download()` use un snapshot anterior.
    try {
      if (annotationCap.provides?.commit) await waitPdfTask<void>(annotationCap.provides.commit());
    } catch {
      // ignore
    }

    if (!exportApi.provides) return;
    const buffer = await waitPdfTask<ArrayBuffer | Uint8Array>(exportApi.provides.saveAsCopy(documentId) as any);
    downloadBuffer(buffer, `document-${documentId}.pdf`);
  }, [annotationCap.provides, documentId, downloadBuffer, exportApi.provides]);

  const onStartSignaturePlacement = useCallback(
    (signature: SignatureFieldDefinition) => {
      if (!signatureCap.provides) return;
      const entryId = signatureCap.provides.addEntry({ signature });
      signatureCap.provides.forDocument(documentId).activateSignaturePlacement(entryId);
      setIsSignatureModalOpen(false);
    },
    [documentId, signatureCap.provides],
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

  return (
    <>
      <div className={styles.toolbarShell} role="toolbar" aria-label="Toolbar PDF">
        <AppPdfToolbar
          zoomLevel={zoomLevel}
          onZoomIn={onZoomIn}
          onZoomOut={onZoomOut}
          onResetZoom={onResetZoom}
          onToggleThumbnails={onToggleThumbnails}
          isThumbnailOpen={isThumbnailOpen}
          isZoomDisabled={isZoomDisabled}
          onRotateLeft={onRotateLeft}
          onRotateRight={onRotateRight}
          onToggleSignatureModal={onToggleSignatureModal}
          isSignatureModalOpen={isSignatureModalOpen}
          onDeleteSelectedSignature={onDeleteSelectedSignature}
          canDeleteSelectedSignature={Boolean(getSelectedSignature()) && !isSignatureLocked}
          onSaveSignedPdf={onSaveSignedPdf}
          isSignatureLocked={isSignatureLocked}
          isSaveSignedPdfDisabled={!hasAnySignaturePlaced && !isSignatureLocked}
          isSavingSignedPdf={isSavingSignedPdf}
          onPrint={onPrint}
          onExport={onExport}
        />
      </div>
      <AppPdfSignatureModal
        isOpen={isSignatureModalOpen}
        onClose={() => setIsSignatureModalOpen(false)}
        onStartPlacement={onStartSignaturePlacement}
        isPlacementReady={isSignaturePlacementReady && Boolean(signatureCap.provides)}
      />
      <div
        className={styles.main}
        data-signature-active-placement={activePlacement ? "true" : "false"}
        data-signature-entry-count={String(signatureEntries.entries?.length ?? 0)}
      >
        <div className={styles.paginationOverlay} role="group" aria-label="PaginaciÃ³n">
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
        <Viewport documentId={documentId} className={styles.viewport}>
            <Scroller
              documentId={documentId}
            renderPage={({ pageIndex, width, height, rotatedWidth, rotatedHeight }) => (
              <div
                className={styles.pageLayer}
                // `width/height` aquÃ­ representan el "slot" calculado por el Scroll plugin
                // para la pÃ¡gina actual (ya considera rotaciÃ³n/escala).
                style={{
                  width: Math.ceil(rotationSteps % 2 === 1 ? rotatedWidth : width),
                  // Guardrail: algunos PDFs/escala generan rounding y el slot queda 1-2px corto,
                  // Mantener el slot exactamente como lo calcula EmbedPDF para evitar
                  // diferencias visuales vs. rotaciÃ³n 0 y evitar solapamientos.
                  height: Math.ceil(rotationSteps % 2 === 1 ? rotatedHeight : height),
                }}
              >
                {rotationSteps === 0 ? (
                  <PagePointerProvider
                    documentId={documentId}
                    pageIndex={pageIndex}
                    scale={zoomLevel}
                    rotation={rotationRaw as any}
                  >
                    <RenderLayer documentId={documentId} pageIndex={pageIndex} />
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
                        : undefined
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
            )}
          />
        </Viewport>
      </div>
    </>
  );
}
