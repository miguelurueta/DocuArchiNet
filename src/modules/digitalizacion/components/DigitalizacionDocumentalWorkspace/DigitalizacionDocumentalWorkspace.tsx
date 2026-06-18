import {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type CSSProperties,
  type DragEvent,
} from "react";
import {
  BorderOutlined,
  ClearOutlined,
  ColumnWidthOutlined,
  DeleteOutlined,
  FileTextOutlined,
  FullscreenExitOutlined,
  FullscreenOutlined,
  ProfileOutlined,
  RotateLeftOutlined,
  RotateRightOutlined,
  ScanOutlined,
  SettingOutlined,
  ZoomInOutlined,
  ZoomOutOutlined,
} from "@ant-design/icons";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
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
type PreviewFitMode = "custom" | "fitWidth" | "fitPage";

const MIN_PREVIEW_ZOOM = 50;
const MAX_PREVIEW_ZOOM = 200;
const PREVIEW_ZOOM_STEP = 25;
const PANEL_PREFERENCES_STORAGE_KEY = "docuarchi:digitalizacion:panel-preferences";

type PanelPreferences = {
  showThumbnails: boolean;
  showConfiguration: boolean;
};

const defaultPanelPreferences: PanelPreferences = {
  showThumbnails: true,
  showConfiguration: true,
};

const readPanelPreferences = (): PanelPreferences => {
  if (typeof window === "undefined") {
    return defaultPanelPreferences;
  }

  try {
    const raw = window.localStorage.getItem(PANEL_PREFERENCES_STORAGE_KEY);
    if (!raw) {
      return defaultPanelPreferences;
    }

    const parsed = JSON.parse(raw) as Partial<PanelPreferences>;
    return {
      showThumbnails:
        typeof parsed.showThumbnails === "boolean"
          ? parsed.showThumbnails
          : defaultPanelPreferences.showThumbnails,
      showConfiguration:
        typeof parsed.showConfiguration === "boolean"
          ? parsed.showConfiguration
          : defaultPanelPreferences.showConfiguration,
    };
  } catch {
    return defaultPanelPreferences;
  }
};

const writePanelPreferences = (preferences: PanelPreferences) => {
  if (typeof window === "undefined") {
    return;
  }

  try {
    window.localStorage.setItem(PANEL_PREFERENCES_STORAGE_KEY, JSON.stringify(preferences));
  } catch {
    // Storage can be blocked or full; panel toggles still work in memory.
  }
};

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
  const [removeBlankPages, setRemoveBlankPages] = useState(false);
  const [deskewEnabled, setDeskewEnabled] = useState(false);
  const [autoCropEnabled, setAutoCropEnabled] = useState(false);
  const [autoRotateEnabled, setAutoRotateEnabled] = useState(false);
  const [colorMode, setColorMode] = useState<(typeof colorOptions)[number]["value"]>("color");
  const [resolutionDpi, setResolutionDpi] = useState<(typeof resolutionOptions)[number]>(200);
  const [draggedPageId, setDraggedPageId] = useState<string | null>(null);
  const [dragOverPageId, setDragOverPageId] = useState<string | null>(null);
  const [previewZoom, setPreviewZoom] = useState(100);
  const [previewFitMode, setPreviewFitMode] = useState<PreviewFitMode>("fitPage");
  const [previewExpanded, setPreviewExpanded] = useState(false);
  const [panelPreferences, setPanelPreferences] = useState<PanelPreferences>(
    readPanelPreferences,
  );
  const previewPanelRef = useRef<HTMLElement | null>(null);
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
    reorderPages,
    generatePdf,
    selectDevice,
  } = scanner;

  const handleOperationCompleted = useCallback(
    (result: DigitalizacionResult) => {
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

  useEffect(() => {
    writePanelPreferences(panelPreferences);
  }, [panelPreferences]);

  const handleCancel = useCallback(() => {
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
      removeBlankPages,
      automaticProcessing:
        captureMode === "docuarchi"
          ? {
              deskew: deskewEnabled,
              autoCrop: autoCropEnabled,
              autoRotate: autoRotateEnabled,
            }
          : undefined,
    });
  }, [
    adfEnabled,
    autoCropEnabled,
    autoRotateEnabled,
    captureMode,
    colorMode,
    deskewEnabled,
    duplexEnabled,
    removeBlankPages,
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

  const handleThumbnailDragStart = useCallback(
    (event: DragEvent<HTMLButtonElement>, pageId: string) => {
      setDraggedPageId(pageId);
      event.dataTransfer.effectAllowed = "move";
      event.dataTransfer.setData("text/plain", pageId);
    },
    [],
  );

  const handleThumbnailDragOver = useCallback(
    (event: DragEvent<HTMLButtonElement>, pageId: string) => {
      event.preventDefault();
      event.dataTransfer.dropEffect = "move";
      if (pageId !== draggedPageId) {
        setDragOverPageId(pageId);
      }
    },
    [draggedPageId],
  );

  const handleThumbnailDrop = useCallback(
    (event: DragEvent<HTMLButtonElement>, targetPageId: string) => {
      event.preventDefault();
      const sourcePageId = event.dataTransfer.getData("text/plain") || draggedPageId;
      setDraggedPageId(null);
      setDragOverPageId(null);

      if (!sourcePageId || sourcePageId === targetPageId) {
        return;
      }

      const sourceIndex = scanner.pages.findIndex((page) => page.id === sourcePageId);
      const targetIndex = scanner.pages.findIndex((page) => page.id === targetPageId);
      if (sourceIndex === -1 || targetIndex === -1) {
        return;
      }

      const nextPages = [...scanner.pages];
      const [movedPage] = nextPages.splice(sourceIndex, 1);
      if (!movedPage) {
        return;
      }

      const insertIndex = sourceIndex < targetIndex ? targetIndex - 1 : targetIndex;
      nextPages.splice(insertIndex, 0, movedPage);
      void reorderPages(nextPages.map((page) => page.id));
    },
    [draggedPageId, reorderPages, scanner.pages],
  );

  const handleThumbnailDragEnd = useCallback(() => {
    setDraggedPageId(null);
    setDragOverPageId(null);
  }, []);

  const handleGeneratePdf = useCallback(() => {
    const fileName =
      activeContext?.radicado || activeContext?.idDocumentoDestino
        ? `digitalizacion-${activeContext?.radicado ?? activeContext?.idDocumentoDestino}`
        : "digitalizacion-documental";
    void generatePdf(fileName);
  }, [activeContext, generatePdf]);

  const handleZoomOut = useCallback(() => {
    setPreviewFitMode("custom");
    setPreviewZoom((current) => Math.max(MIN_PREVIEW_ZOOM, current - PREVIEW_ZOOM_STEP));
  }, []);

  const handleZoomIn = useCallback(() => {
    setPreviewFitMode("custom");
    setPreviewZoom((current) => Math.min(MAX_PREVIEW_ZOOM, current + PREVIEW_ZOOM_STEP));
  }, []);

  const handleFitWidth = useCallback(() => {
    setPreviewFitMode("fitWidth");
    setPreviewZoom(100);
  }, []);

  const handleFitPage = useCallback(() => {
    setPreviewFitMode("fitPage");
    setPreviewZoom(100);
  }, []);

  const handleTogglePreviewExpanded = useCallback(() => {
    const panel = previewPanelRef.current;
    const ownerDocument = panel?.ownerDocument;

    if (
      !panel ||
      !ownerDocument ||
      typeof panel.requestFullscreen !== "function" ||
      typeof ownerDocument.exitFullscreen !== "function"
    ) {
      setPreviewExpanded((current) => !current);
      return;
    }

    if (ownerDocument.fullscreenElement === panel) {
      void ownerDocument.exitFullscreen().catch(() => {
        setPreviewExpanded(false);
      });
      return;
    }

    void panel.requestFullscreen().catch(() => {
      setPreviewExpanded((current) => !current);
    });
  }, []);

  const handleToggleThumbnails = useCallback(() => {
    setPanelPreferences((current) => ({
      ...current,
      showThumbnails: !current.showThumbnails,
    }));
  }, []);

  const handleToggleConfiguration = useCallback(() => {
    setPanelPreferences((current) => ({
      ...current,
      showConfiguration: !current.showConfiguration,
    }));
  }, []);

  useEffect(() => {
    if (typeof document === "undefined") {
      return undefined;
    }

    const handleFullscreenChange = () => {
      setPreviewExpanded(document.fullscreenElement === previewPanelRef.current);
    };

    document.addEventListener("fullscreenchange", handleFullscreenChange);
    return () => {
      document.removeEventListener("fullscreenchange", handleFullscreenChange);
    };
  }, []);

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
  const thumbnailsCollapsed = !panelPreferences.showThumbnails;
  const configurationCollapsed = !panelPreferences.showConfiguration;
  const previewPanelClassName = [
    styles.panel,
    styles.previewPanel,
    previewExpanded ? styles.previewPanelExpanded : "",
  ]
    .filter(Boolean)
    .join(" ");
  const previewViewportClassName = [
    styles.previewViewport,
    previewFitMode === "custom" ? styles.previewViewportCustom : "",
    previewFitMode === "fitWidth" ? styles.previewViewportFitWidth : "",
    previewFitMode === "fitPage" ? styles.previewViewportFitPage : "",
  ]
    .filter(Boolean)
    .join(" ");
  const previewImageStyle =
    previewFitMode === "custom"
      ? ({ "--preview-zoom": `${previewZoom}%` } as CSSProperties)
      : undefined;

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

      <div className={styles.toolbar} role="toolbar" aria-label="Herramientas de digitalizacion">
        <div className={styles.toolbarGroup} data-priority="primary" role="group" aria-label="Captura">
          <AppButton
            variant="secondary"
            size="sm"
            icon={<ScanOutlined />}
            aria-label="Escanear"
            tooltip="Escanear"
            onClick={handleScan}
            disabled={!scanner.selectedDeviceId || scanner.loading || Boolean(state.validationError)}
          />
        </div>

        <div className={styles.toolbarGroup} data-priority="output" role="group" aria-label="Salida">
          <AppButton
            size="sm"
            icon={<FileTextOutlined />}
            aria-label="Generar PDF"
            tooltip="Generar PDF"
            onClick={handleGeneratePdf}
            disabled={!canGeneratePdf}
          />
        </div>
      </div>

      <main
        className={styles.main}
        data-thumbnails-collapsed={thumbnailsCollapsed}
        data-configuration-collapsed={configurationCollapsed}
      >
        <AppCollapseRail
          title={`Miniaturas (${scanner.pages.length})`}
          collapsed={thumbnailsCollapsed}
          onToggle={handleToggleThumbnails}
          placement="left"
          variant="inline"
          railLabel="Miniaturas"
          railIcon={<ProfileOutlined />}
          panelId="digitalizacion-thumbnails-panel"
          className={styles.collapseRail}
        >
          {scanner.pages.length > 0 ? (
            <div className={styles.thumbnailList}>
              {scanner.pages.map((page, pageOrderIndex) => (
                  <button
                    className={styles.thumbnailButton}
                    data-selected={page.id === selectedPageId}
                    data-dragging={page.id === draggedPageId}
                    data-drop-target={page.id === dragOverPageId}
                    key={page.id}
                    type="button"
                    draggable={!thumbnailsCollapsed}
                    tabIndex={thumbnailsCollapsed ? -1 : 0}
                    onClick={() => setSelectedPageId(page.id)}
                    onDragStart={(event) => handleThumbnailDragStart(event, page.id)}
                    onDragOver={(event) => handleThumbnailDragOver(event, page.id)}
                    onDragLeave={() => {
                      if (dragOverPageId === page.id) {
                        setDragOverPageId(null);
                      }
                    }}
                    onDrop={(event) => handleThumbnailDrop(event, page.id)}
                    onDragEnd={handleThumbnailDragEnd}
                  >
                    {page.thumbnailUrl ? (
                      <img
                        src={page.thumbnailUrl}
                        alt={`Pagina ${pageOrderIndex + 1}`}
                      />
                    ) : (
                      <span>{pageOrderIndex + 1}</span>
                    )}
                    <small>Pagina {pageOrderIndex + 1}</small>
                  </button>
                ))}
            </div>
          ) : (
            <div className={styles.panelBody}>
              <span className={styles.placeholderTitle}>Sin paginas</span>
              <span>0 paginas capturadas</span>
            </div>
          )}
        </AppCollapseRail>

        <section
          className={previewPanelClassName}
          aria-label="Preview digitalizacion"
          ref={previewPanelRef}
        >
          <div className={`${styles.panelHeader} ${styles.previewHeader}`}>
            <span>Preview PDF</span>
            <div className={styles.previewControls} role="toolbar" aria-label="Visualizacion preview">
              <AppButton
                variant="ghost"
                size="sm"
                icon={<RotateLeftOutlined />}
                aria-label="Rotar izquierda"
                tooltip="Rotar izquierda"
                onClick={() => handleRotateSelected(270)}
                disabled={!selectedPage}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<RotateRightOutlined />}
                aria-label="Rotar derecha"
                tooltip="Rotar derecha"
                onClick={() => handleRotateSelected(90)}
                disabled={!selectedPage}
              />
              <AppButton
                variant="danger"
                size="sm"
                icon={<DeleteOutlined />}
                aria-label="Eliminar página"
                tooltip="Eliminar página"
                onClick={handleRemoveSelected}
                disabled={!selectedPage}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<ClearOutlined />}
                aria-label="Limpiar lote"
                tooltip="Limpiar lote"
                onClick={handleClear}
                disabled={scanner.loading || !hasPages}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<ZoomOutOutlined />}
                aria-label="Reducir zoom"
                tooltip="Zoom -"
                onClick={handleZoomOut}
                disabled={!selectedPage || previewZoom <= MIN_PREVIEW_ZOOM}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<ZoomInOutlined />}
                aria-label="Aumentar zoom"
                tooltip="Zoom +"
                onClick={handleZoomIn}
                disabled={!selectedPage || previewZoom >= MAX_PREVIEW_ZOOM}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<ColumnWidthOutlined />}
                aria-label="Ajustar ancho"
                tooltip="Ajustar ancho"
                onClick={handleFitWidth}
                disabled={!selectedPage}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<BorderOutlined />}
                aria-label="Ajustar pagina"
                tooltip="Ajustar pagina"
                onClick={handleFitPage}
                disabled={!selectedPage}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={previewExpanded ? <FullscreenExitOutlined /> : <FullscreenOutlined />}
                aria-label={previewExpanded ? "Restaurar preview" : "Expandir preview"}
                aria-pressed={previewExpanded}
                tooltip={previewExpanded ? "Restaurar preview" : "Pantalla completa"}
                onClick={handleTogglePreviewExpanded}
              />
            </div>
          </div>
          <div className={`${styles.panelBody} ${styles.preview}`}>
            {selectedPage ? (
              <>
                <div className={previewViewportClassName}>
                  {selectedPage.imageUrl ? (
                    <img
                      className={styles.previewImage}
                      src={selectedPage.imageUrl}
                      alt={getPageLabel(selectedPage)}
                      style={previewImageStyle}
                    />
                  ) : (
                    <span className={styles.previewPage}>{selectedPage.index + 1}</span>
                  )}
                </div>
                <div className={styles.previewMeta}>
                  <span className={styles.placeholderTitle}>{getPageLabel(selectedPage)}</span>
                  <span>{scanner.pdf ? scanner.pdf.file.name : "PDF pendiente"}</span>
                </div>
              </>
            ) : (
              <div className={previewViewportClassName}>
                <div className={styles.previewMeta}>
                  <span className={styles.placeholderTitle}>PDF pendiente</span>
                  <span>Capture paginas para habilitar la generacion.</span>
                </div>
              </div>
            )}
          </div>
        </section>

        <AppCollapseRail
          title="Configuracion de Escaneo"
          collapsed={configurationCollapsed}
          onToggle={handleToggleConfiguration}
          placement="right"
          variant="inline"
          railLabel="Configuracion"
          railIcon={<SettingOutlined />}
          panelId="digitalizacion-configuration-panel"
          className={styles.collapseRail}
        >
          <div className={styles.settingsPanel}>
            <label className={styles.settingField}>
              <span>Scanner</span>
              <select
                value={scanner.selectedDeviceId ?? ""}
                onChange={(event) => {
                  const deviceId = event.target.value;
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
                <label className={styles.checkField}>
                  <input
                    type="checkbox"
                    checked={removeBlankPages}
                    onChange={(event) => setRemoveBlankPages(event.target.checked)}
                  />
                  <span>Eliminar paginas en blanco</span>
                </label>
                <fieldset className={styles.settingGroup}>
                  <legend>Procesamiento automatico</legend>
                  <label>
                    <input
                      type="checkbox"
                      checked={deskewEnabled}
                      onChange={(event) => setDeskewEnabled(event.target.checked)}
                    />
                    <span>Deskew</span>
                  </label>
                  <label>
                    <input
                      type="checkbox"
                      checked={autoCropEnabled}
                      onChange={(event) => setAutoCropEnabled(event.target.checked)}
                    />
                    <span>Auto Crop</span>
                  </label>
                  <label>
                    <input
                      type="checkbox"
                      checked={autoRotateEnabled}
                      onChange={(event) => setAutoRotateEnabled(event.target.checked)}
                    />
                    <span>Auto Rotate</span>
                  </label>
                </fieldset>
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
              <span>Blancas {removeBlankPages ? "si" : "no"}</span>
              <span>Deskew {deskewEnabled ? "si" : "no"}</span>
              <span>Crop {autoCropEnabled ? "si" : "no"}</span>
              <span>AutoRot {autoRotateEnabled ? "si" : "no"}</span>
              <span>{captureMode === "driver" ? "PaperStream" : colorOptions.find((option) => option.value === colorMode)?.label}</span>
              <span>{captureMode === "driver" ? "UI driver" : `${resolutionDpi} dpi`}</span>
            </div>
          </div>
        </AppCollapseRail>
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
