import { useCallback, useEffect, useMemo, useState } from "react";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import { useDigitalizacionDocumentalState } from "../../hooks/useDigitalizacionDocumentalState";
import { useDigitalizacionOperationOrchestrator } from "../../hooks/useDigitalizacionOperationOrchestrator";
import { useDigitalizacionScanner } from "../../hooks/useDigitalizacionScanner";
import type {
  DigitalizacionDocumentalWorkspaceProps,
  DigitalizacionFunctionalError,
  DigitalizacionResult,
} from "../../types/digitalizacion.types";
import { DYNAMSOFT_CONTAINER_ID, type ScanPage } from "../../infrastructure/dynamsoft";
import { unavailableScannerClient } from "./digitalizacionWorkspace.helpers";
import styles from "./DigitalizacionDocumentalWorkspace.module.css";

type CaptureMode = "docuarchi" | "driver";

const colorOptions = [
  { label: "Color", value: "color" },
  { label: "Gris", value: "grayscale" },
  { label: "B/N", value: "blackWhite" },
] as const;

const resolutionOptions = [200, 300, 400, 600] as const;

const readableMode = (modo?: string) => (modo === "adjuntar" ? "adjuntar" : "crear");

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

export function DigitalizacionDocumentalWorkspace({
  active = true,
  context,
  scannerClient = unavailableScannerClient,
  apiClient,
  onCancel,
  onCompleted,
  onError,
}: DigitalizacionDocumentalWorkspaceProps) {
  const [selectedPageId, setSelectedPageId] = useState<string | null>(null);
  const [captureMode, setCaptureMode] = useState<CaptureMode>("docuarchi");
  const [adfEnabled, setAdfEnabled] = useState(true);
  const [duplexEnabled, setDuplexEnabled] = useState(false);
  const [colorMode, setColorMode] = useState<(typeof colorOptions)[number]["value"]>("color");
  const [resolutionDpi, setResolutionDpi] = useState<(typeof resolutionOptions)[number]>(200);
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
      console.log("DIGITALIZACION_WORKSPACE_OPERATION_COMPLETED_DISPOSE");
      clear();
      void dispose();
      setSelectedPageId(null);
      onCompleted(result);
    },
    [clear, dispose, onCompleted],
  );

  const operation = useDigitalizacionOperationOrchestrator({
    apiClient,
    onCompleted: handleOperationCompleted,
    onError,
  });

  const activeContext = state.context ?? context;
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
    if (active) {
      void initialize();
    }
  }, [active, initialize]);

  const handleCancel = useCallback(() => {
    console.log("DIGITALIZACION_WORKSPACE_CANCEL_DISPOSE");
    operation.cancel();
    clear();
    void dispose();
    setSelectedPageId(null);
    onCompleted({ accion: "cancelado" });
    onCancel?.();
  }, [clear, dispose, onCancel, onCompleted, operation]);

  const handleClear = useCallback(() => {
    clearPages();
    setSelectedPageId(null);
    void clearScanner();
  }, [clearPages, clearScanner]);

  const handleScan = useCallback(() => {
    if (!scanner.selectedDeviceId) {
      return;
    }

    void scan({
      deviceId: scanner.selectedDeviceId,
      colorMode,
      duplex: captureMode === "docuarchi" ? duplexEnabled : false,
      feederEnabled: captureMode === "docuarchi" ? adfEnabled : true,
      resolutionDpi,
      showScannerUi: captureMode === "driver",
    });
  }, [
    adfEnabled,
    captureMode,
    colorMode,
    duplexEnabled,
    resolutionDpi,
    scan,
    scanner.selectedDeviceId,
  ]);

  const handleRotateSelected = useCallback((degrees: 90 | 270 = 90) => {
    const pageId = selectedPageId ?? scanner.pages[0]?.id;
    if (!pageId) return;
    void rotatePage(pageId, degrees);
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

  console.log("PAGE_STATE", scanner.pages);
  if (selectedPage) {
    console.log("PAGE_PREVIEW_RENDER", {
      page: selectedPage,
      hasImageUrl: Boolean(selectedPage.imageUrl),
      hasThumbnailUrl: Boolean(selectedPage.thumbnailUrl),
    });
  }

  if (!active) {
    return null;
  }

  return (
    <section
      className={styles.shell}
      aria-label="Digitalizacion documental"
      data-testid="digitalizacion-workspace"
    >
      <div
        id={DYNAMSOFT_CONTAINER_ID}
        className={styles.dynamsoftContainer}
        aria-hidden="true"
      />
      <header className={styles.header}>
        <div className={styles.titleLine}>
          <span className={styles.modeBadge}>{readableMode(activeContext?.modo)}</span>
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
        <AppButton
          variant="secondary"
          onClick={handleScan}
          disabled={!scanner.selectedDeviceId || scanner.loading || Boolean(state.validationError)}
        >
          Escanear
        </AppButton>
        {scanner.status === "error" || scanner.devices.length === 0 ? (
          <AppButton variant="secondary" onClick={initialize} disabled={scanner.loading}>
            Reintentar
          </AppButton>
        ) : null}
        <AppButton variant="ghost" onClick={() => handleRotateSelected(270)} disabled={!selectedPage}>
          Rotar izq
        </AppButton>
        <AppButton variant="ghost" onClick={() => handleRotateSelected(90)} disabled={!selectedPage}>
          Rotar der
        </AppButton>
        <AppButton variant="ghost" onClick={handleRemoveSelected} disabled={!selectedPage}>
          Eliminar
        </AppButton>
        <AppButton variant="ghost" onClick={handleClear} disabled={scanner.loading}>
          Limpiar
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
              {scanner.pages.map((page) => {
                console.log("PAGE_THUMBNAIL_RENDER", {
                  page,
                  hasThumbnailUrl: Boolean(page.thumbnailUrl),
                  hasImageUrl: Boolean(page.imageUrl),
                });

                return (
                  <button
                    className={styles.thumbnailButton}
                    data-selected={page.id === selectedPageId}
                    key={page.id}
                    type="button"
                    onClick={() => setSelectedPageId(page.id)}
                  >
                    {page.thumbnailUrl ? (
                      <img
                        src={page.thumbnailUrl}
                        alt={getPageLabel(page)}
                        onLoad={(event) => {
                          const image = event.currentTarget;
                          console.log("THUMBNAIL_DIMENSIONS", {
                            width: image.naturalWidth,
                            height: image.naturalHeight,
                          });
                        }}
                      />
                    ) : (
                      <span>{page.index + 1}</span>
                    )}
                    <small>{getPageLabel(page)}</small>
                  </button>
                );
              })}
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
                {selectedPage.imageUrl ? (
                  <img
                    className={styles.previewImage}
                    src={selectedPage.imageUrl}
                    alt={getPageLabel(selectedPage)}
                    onLoad={(event) => {
                      const image = event.currentTarget;
                      console.log("PREVIEW_DIMENSIONS", {
                        width: image.naturalWidth,
                        height: image.naturalHeight,
                      });
                    }}
                  />
                ) : (
                  <span className={styles.previewPage}>{selectedPage.index + 1}</span>
                )}
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

        <section className={styles.panel} aria-label="Configuracion de escaneo">
          <div className={styles.panelHeader}>Configuracion de Escaneo</div>
          <div className={styles.settingsPanel}>
            <label className={styles.settingField}>
              <span>Scanner</span>
              <select
                value={scanner.selectedDeviceId ?? ""}
                onChange={(event) => {
                  const deviceId = event.target.value;
                  const selectedDevice = scanner.devices.find((device) => device.id === deviceId);
                  console.log("SELECT_CHANGE", deviceId);
                  console.debug("[DigitalizacionWorkspace]", "selectDevice.change", {
                    scannerName: selectedDevice?.name ?? "",
                    scannerIndex: selectedDevice?.index ?? Number(deviceId),
                  });
                  console.log("BEFORE_SELECT_DEVICE", deviceId);
                  void selectDevice(deviceId);
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

            <fieldset className={styles.settingGroup}>
              <legend>Modo de captura</legend>
              <label>
                <input
                  type="radio"
                  name="captureMode"
                  checked={captureMode === "docuarchi"}
                  onChange={() => setCaptureMode("docuarchi")}
                />
                <span>DocuArchi</span>
              </label>
              <label>
                <input
                  type="radio"
                  name="captureMode"
                  checked={captureMode === "driver"}
                  onChange={() => setCaptureMode("driver")}
                />
                <span>Driver del scanner</span>
              </label>
            </fieldset>

            {captureMode === "docuarchi" ? (
              <div className={styles.settingsStack}>
                <label className={styles.checkField}>
                  <input
                    type="checkbox"
                    checked={adfEnabled}
                    onChange={(event) => setAdfEnabled(event.target.checked)}
                  />
                  <span>ADF activado</span>
                </label>
                <label className={styles.checkField}>
                  <input
                    type="checkbox"
                    checked={duplexEnabled}
                    onChange={(event) => setDuplexEnabled(event.target.checked)}
                  />
                  <span>Duplex activado</span>
                </label>
                <label className={styles.settingField}>
                  <span>Color</span>
                  <select
                    value={colorMode}
                    onChange={(event) =>
                      setColorMode(event.target.value as typeof colorMode)
                    }
                  >
                    {colorOptions.map((option) => (
                      <option key={option.value} value={option.value}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                </label>
                <label className={styles.settingField}>
                  <span>Resolucion</span>
                  <select
                    value={resolutionDpi}
                    onChange={(event) =>
                      setResolutionDpi(Number(event.target.value) as typeof resolutionDpi)
                    }
                  >
                    {resolutionOptions.map((dpi) => (
                      <option key={dpi} value={dpi}>
                        {dpi} dpi
                      </option>
                    ))}
                  </select>
                </label>
              </div>
            ) : (
              <div className={styles.driverMode}>
                <span>Utilizar configuracion PaperStream</span>
                <AppButton
                  variant="secondary"
                  onClick={handleScan}
                  disabled={!scanner.selectedDeviceId || scanner.loading || Boolean(state.validationError)}
                >
                  Configurar scanner
                </AppButton>
              </div>
            )}

            <div className={styles.settingsSummary} aria-label="Resumen configuracion">
              <span>ADF {captureMode === "driver" ? "driver" : adfEnabled ? "si" : "no"}</span>
              <span>Duplex {captureMode === "driver" ? "driver" : duplexEnabled ? "si" : "no"}</span>
              <span>{captureMode === "driver" ? "PaperStream" : colorOptions.find((option) => option.value === colorMode)?.label}</span>
              <span>{captureMode === "driver" ? "UI driver" : `${resolutionDpi} dpi`}</span>
            </div>
          </div>
        </section>
      </main>

      <footer className={styles.workbenchFooter}>
        <span>{submitDisabledReason}</span>
        <div className={styles.footerActions}>
          <span>{scanner.loading || operation.loading ? "Operacion en curso" : "Listo para operar"}</span>
          <AppButton variant="ghost" onClick={handleCancel} disabled={operation.loading}>
            Cancelar
          </AppButton>
          <AppButton onClick={handleSubmit} disabled={!canConfirm} loading={operation.loading}>
            {primaryLabel}
          </AppButton>
        </div>
      </footer>
    </section>
  );
}
