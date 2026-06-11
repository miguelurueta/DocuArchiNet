import {
  forwardRef,
  useCallback,
  useEffect,
  useImperativeHandle,
  useMemo,
  useState,
} from "react";
import { AppButton } from "../AppButton";
import { useDigitalizacionDocumentalState } from "../../../../modules/digitalizacion/hooks/useDigitalizacionDocumentalState";
import { useDigitalizacionOperationOrchestrator } from "../../../../modules/digitalizacion/hooks/useDigitalizacionOperationOrchestrator";
import { useDigitalizacionScanner } from "../../../../modules/digitalizacion/hooks/useDigitalizacionScanner";
import { DynamsoftScannerError } from "../../../../modules/digitalizacion/infrastructure/dynamsoft";
import type { DigitalizacionScannerClient, ScanPage } from "../../../../modules/digitalizacion/infrastructure/dynamsoft";
import type {
  DigitalizacionContext,
  DigitalizacionDocumentalError,
  DigitalizacionFunctionalError,
  DigitalizacionResult,
} from "../../../../modules/digitalizacion/types/digitalizacion.types";
import type { DigitalizacionApiClient } from "../../../../modules/digitalizacion/types/digitalizacionApi.types";
import styles from "./AppDigitalizador.module.css";

export type AppDigitalizadorHandle = {
  close: () => void;
  cancel: () => void;
  submit: () => void;
};

export type AppDigitalizadorProps = {
  context: DigitalizacionContext | null;
  active?: boolean;
  autoInitialize?: boolean;
  scannerClient?: DigitalizacionScannerClient;
  apiClient?: DigitalizacionApiClient;
  showFooterActions?: boolean;
  disposeOnClose?: boolean;
  onClose?: () => void;
  onCompleted: (result: DigitalizacionResult) => void;
  onError?: (error: DigitalizacionDocumentalError) => void;
};

const buildTitle = (modo?: string) =>
  modo === "adjuntar" ? "Adjuntar digitalizacion" : "Digitalizar documento";

const readableMode = (modo?: string) => (modo === "adjuntar" ? "adjuntar" : "crear");

const unavailableScannerClient: DigitalizacionScannerClient = {
  initialize: async () => undefined,
  listDevices: async () => [],
  selectDevice: async () => undefined,
  scan: async () => {
    throw new DynamsoftScannerError({
      code: "SCANNER_NOT_SELECTED",
      message: "Seleccione un scanner antes de escanear.",
    });
  },
  rotatePage: async () => undefined,
  removePage: async () => undefined,
  clear: async () => undefined,
  generatePdf: async () => {
    throw new DynamsoftScannerError({
      code: "PDF_EMPTY",
      message: "No hay paginas para generar PDF.",
    });
  },
  dispose: async () => undefined,
};

const getVisualStateLabel = ({
  contextInvalid,
  scannerStatus,
  deviceCount,
  pageCount,
  pdfReady,
}: {
  contextInvalid: boolean;
  scannerStatus: string;
  deviceCount: number;
  pageCount: number;
  pdfReady: boolean;
}) => {
  if (contextInvalid) return "contextInvalid";
  if (scannerStatus === "initializing") return "initializingScanner";
  if (scannerStatus === "error") return "error";
  if (scannerStatus === "scanning") return "scanning";
  if (scannerStatus === "generatingPdf") return "generatingPdf";
  if (deviceCount === 0) return "noScanner";
  if (pdfReady) return "success";
  if (pageCount > 0) return "pagesCaptured";
  return "readyEmpty";
};

const getPageLabel = (page: ScanPage) => `Pagina ${page.index + 1}`;

export const AppDigitalizador = forwardRef<AppDigitalizadorHandle, AppDigitalizadorProps>(
  function AppDigitalizador(
    {
      context,
      active = true,
      autoInitialize = true,
      scannerClient = unavailableScannerClient,
      apiClient,
      showFooterActions = true,
      disposeOnClose = false,
      onClose,
      onCompleted,
      onError,
    },
    ref,
  ) {
    const [selectedPageId, setSelectedPageId] = useState<string | null>(null);
    const handleInvalidContext = useCallback(
      (error: DigitalizacionFunctionalError) => {
        onError?.(error);
      },
      [onError],
    );

    const { state, clear, clearPages, canSubmit } = useDigitalizacionDocumentalState({
      open: active,
      context,
      onInvalidContext: handleInvalidContext,
    });
    const scanner = useDigitalizacionScanner({ client: scannerClient });
    const {
      initialize,
      dispose,
      clear: clearScanner,
      scan,
      rotatePage,
      removePage,
      generatePdf,
      selectDevice,
    } = scanner;

    const handleOperationCompleted = useCallback(
      (result: DigitalizacionResult) => {
        clear();
        if (disposeOnClose) {
          void dispose();
        }
        setSelectedPageId(null);
        onCompleted(result);
        onClose?.();
      },
      [clear, dispose, disposeOnClose, onClose, onCompleted],
    );

    const operation = useDigitalizacionOperationOrchestrator({
      apiClient,
      onCompleted: handleOperationCompleted,
      onError,
    });

    const activeContext = state.context ?? context;
    const title = activeContext?.titulo ?? buildTitle(activeContext?.modo);
    const primaryLabel =
      activeContext?.modo === "adjuntar" ? "Adjuntar digitalizacion" : "Guardar documento";
    const hasPages = scanner.pages.length > 0;
    const canGeneratePdf = Boolean(!state.validationError && hasPages && !scanner.loading);
    const metadataReady = Boolean(!state.metadata.required || state.metadata.trd);
    const canConfirm = Boolean(canSubmit && scanner.pdf && metadataReady && !operation.loading);
    const visualState = getVisualStateLabel({
      contextInvalid: Boolean(state.validationError),
      scannerStatus: operation.loading ? operation.status : scanner.status,
      deviceCount: scanner.devices.length,
      pageCount: scanner.pages.length,
      pdfReady: Boolean(scanner.pdf),
    });
    const submitDisabledReason =
      state.validationError?.message ??
      scanner.error?.message ??
      operation.error?.message ??
      (operation.loading
        ? `Operacion ${operation.status}`
        : scanner.pdf
          ? "PDF listo"
          : "Pendiente captura PDF");

    useEffect(() => {
      if (active && autoInitialize && scannerClient !== unavailableScannerClient) {
        void initialize();
      }
    }, [active, autoInitialize, initialize, scannerClient]);

    const clearWorkspace = useCallback(() => {
      operation.cancel();
      clear();
      if (disposeOnClose) {
        void dispose();
      }
      setSelectedPageId(null);
    }, [clear, dispose, disposeOnClose, operation]);

    const handleCancel = useCallback(() => {
      clearWorkspace();
      onCompleted({ accion: "cancelado" });
      onClose?.();
    }, [clearWorkspace, onClose, onCompleted]);

    const handleClose = useCallback(() => {
      clearWorkspace();
      onClose?.();
    }, [clearWorkspace, onClose]);

    const handleClear = useCallback(() => {
      clearPages();
      setSelectedPageId(null);
      void clearScanner();
    }, [clearPages, clearScanner]);

    const handleScan = useCallback(() => {
      if (!scanner.selectedDeviceId) {
        return;
      }

      void scan({ deviceId: scanner.selectedDeviceId });
    }, [scan, scanner.selectedDeviceId]);

    const handleRotateSelected = useCallback(() => {
      const pageId = selectedPageId ?? scanner.pages[0]?.id;
      if (!pageId) return;
      void rotatePage(pageId, 90);
    }, [rotatePage, scanner.pages, selectedPageId]);

    const handleRemoveSelected = useCallback(() => {
      const pageId = selectedPageId ?? scanner.pages[0]?.id;
      if (!pageId) return;
      void removePage(pageId);
    }, [removePage, scanner.pages, selectedPageId]);

    const handleGeneratePdf = useCallback(() => {
      const fileName =
        activeContext?.radicado || activeContext?.idDocumentoDestino
          ? `digitalizacion-${activeContext?.radicado ?? activeContext?.idDocumentoDestino}`
          : "digitalizacion-documental";
      void generatePdf(fileName);
    }, [activeContext, generatePdf]);

    const handleSubmit = useCallback(() => {
      if (!activeContext || !scanner.pdf) return;
      void operation
        .submit({
          context: activeContext,
          pdf: scanner.pdf.file,
          pageCount: scanner.pdf.pageCount,
          nombreDocumento: scanner.pdf.file.name,
          trd: state.metadata.trd,
        })
        .catch(() => undefined);
    }, [activeContext, operation, scanner.pdf, state.metadata.trd]);

    useImperativeHandle(
      ref,
      () => ({
        close: handleClose,
        cancel: handleCancel,
        submit: handleSubmit,
      }),
      [handleCancel, handleClose, handleSubmit],
    );

    const summaryItems = useMemo(
      () => [
        ["Gabinete", activeContext?.nombreGabinete || "Sin gabinete"],
        ["Radicado", activeContext?.radicado || "No informado"],
        [
          "Documento destino",
          activeContext?.idDocumentoDestino
            ? String(activeContext.idDocumentoDestino)
            : activeContext?.modo === "adjuntar"
              ? "Requerido"
              : "Nuevo documento",
        ],
      ],
      [activeContext],
    );
    const selectedPage =
      scanner.pages.find((page) => page.id === selectedPageId) ?? scanner.pages[0] ?? null;

    return (
      <section
        className={styles.shell}
        aria-label="Digitalizacion documental"
        data-active={active}
        data-testid="app-digitalizador"
      >
        <header className={styles.header}>
          <div className={styles.titleLine}>
            <div className={styles.titleGroup}>
              <span className={styles.title}>{title}</span>
              <span className={styles.modeBadge}>{readableMode(activeContext?.modo)}</span>
            </div>
            <span className={styles.stateBadge} data-state={visualState}>
              {visualState}
            </span>
          </div>
          <div className={styles.summary}>
            {summaryItems.map(([label, value]) => (
              <div className={styles.summaryItem} key={label}>
                <span className={styles.summaryLabel}>{label}</span>
                <span className={styles.summaryValue}>{value}</span>
              </div>
            ))}
          </div>
        </header>

        {state.validationError ? (
          <div className={styles.error} role="alert">
            {state.validationError.message}
          </div>
        ) : null}
        {scanner.error ? (
          <div className={styles.error} role="alert">
            {scanner.error.message}
          </div>
        ) : null}
        {operation.error ? (
          <div className={styles.error} role="alert">
            {operation.error.message}
          </div>
        ) : null}

        <div className={styles.toolbar}>
          <label className={styles.scannerSelect}>
            <span>Scanner</span>
            <select
              value={scanner.selectedDeviceId ?? ""}
              onChange={(event) => {
                void selectDevice(event.target.value);
              }}
              disabled={scanner.loading || scanner.devices.length === 0}
              aria-label="Seleccionar scanner"
            >
              <option value="">Sin seleccionar</option>
              {scanner.devices.map((device) => (
                <option key={device.id} value={device.id}>
                  {device.name}
                </option>
              ))}
            </select>
          </label>
          <AppButton
            variant="secondary"
            onClick={handleScan}
            disabled={!scanner.selectedDeviceId || scanner.loading || Boolean(state.validationError)}
          >
            Escanear
          </AppButton>
          <AppButton variant="secondary" onClick={initialize} disabled={scanner.loading}>
            Reintentar
          </AppButton>
          <AppButton variant="ghost" onClick={handleClear} disabled={scanner.loading}>
            Limpiar
          </AppButton>
          <AppButton variant="ghost" onClick={handleRotateSelected} disabled={!selectedPage}>
            Rotar
          </AppButton>
          <AppButton variant="ghost" onClick={handleRemoveSelected} disabled={!selectedPage}>
            Eliminar
          </AppButton>
          <AppButton onClick={handleGeneratePdf} disabled={!canGeneratePdf}>
            Generar PDF
          </AppButton>
        </div>

        <main className={styles.main}>
          <section className={styles.panel} aria-label="Miniaturas">
            <div className={styles.panelHeader}>Miniaturas ({scanner.pages.length})</div>
            {scanner.pages.length > 0 ? (
              <div className={styles.thumbnailList}>
                {scanner.pages.map((page) => (
                  <button
                    className={styles.thumbnailButton}
                    data-selected={page.id === selectedPageId}
                    key={page.id}
                    type="button"
                    onClick={() => setSelectedPageId(page.id)}
                  >
                    {page.thumbnailUrl ? (
                      <img src={page.thumbnailUrl} alt={getPageLabel(page)} />
                    ) : (
                      <span>{page.index + 1}</span>
                    )}
                    <small>{getPageLabel(page)}</small>
                  </button>
                ))}
              </div>
            ) : (
              <div className={styles.panelBody}>
                <span className={styles.placeholderTitle}>Sin paginas</span>
                <span>0 paginas capturadas</span>
              </div>
            )}
          </section>

          <section className={styles.panel} aria-label="Preview digitalizacion">
            <div className={styles.panelHeader}>Preview PDF</div>
            <div className={`${styles.panelBody} ${styles.preview}`}>
              {selectedPage ? (
                <>
                  <span className={styles.previewPage}>{selectedPage.index + 1}</span>
                  <span className={styles.placeholderTitle}>{getPageLabel(selectedPage)}</span>
                  <span>{scanner.pdf ? scanner.pdf.file.name : "PDF pendiente"}</span>
                </>
              ) : (
                <>
                  <span className={styles.placeholderTitle}>PDF pendiente</span>
                  <span>Capture paginas para habilitar la generacion.</span>
                </>
              )}
            </div>
          </section>

          <section className={styles.panel} aria-label="Metadata documental">
            <div className={styles.panelHeader}>Metadata</div>
            <div className={styles.panelBody}>
              <span className={styles.placeholderTitle}>
                {state.metadata.required ? "Metadata requerida" : "Metadata opcional"}
              </span>
              <span>{state.metadata.trd ? "TRD resuelto" : "TRD sin resolver"}</span>
              <span>
                {activeContext?.modo === "adjuntar" && activeContext.idDocumentoDestino
                  ? `Destino ${activeContext.idDocumentoDestino}`
                  : "Tipologia pendiente"}
              </span>
            </div>
          </section>
        </main>

        <footer className={styles.workbenchFooter}>
          <span>{submitDisabledReason}</span>
          <span>{scanner.loading || operation.loading ? "Operacion en curso" : "Listo para operar"}</span>
        </footer>

        {showFooterActions ? (
          <div className={styles.actions}>
            <AppButton variant="secondary" onClick={handleCancel}>
              Cancelar
            </AppButton>
            <AppButton onClick={handleSubmit} disabled={!canConfirm} loading={operation.loading}>
              {primaryLabel}
            </AppButton>
          </div>
        ) : null}
      </section>
    );
  },
);
