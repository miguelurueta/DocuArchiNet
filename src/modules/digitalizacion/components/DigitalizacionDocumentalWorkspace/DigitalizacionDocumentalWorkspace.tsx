import {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type CSSProperties,
  type DragEvent,
  type MouseEvent,
  type PointerEvent,
} from "react";
import {
  BorderOutlined,
  CheckSquareOutlined,
  ClearOutlined,
  CloseOutlined,
  ColumnWidthOutlined,
  CopyOutlined,
  DeleteOutlined,
  DownOutlined,
  FileAddOutlined,
  FileTextOutlined,
  FullscreenExitOutlined,
  FullscreenOutlined,
  AppstoreOutlined,
  InsertRowAboveOutlined,
  InsertRowBelowOutlined,
  PlusOutlined,
  ProfileOutlined,
  RotateLeftOutlined,
  RotateRightOutlined,
  ScanOutlined,
  SelectOutlined,
  SettingOutlined,
  ScissorOutlined,
  SwapOutlined,
  ZoomInOutlined,
  ZoomOutOutlined,
  CompressOutlined,
} from "@ant-design/icons";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import { AppContasoftLoader } from "../../../../app/Components/UI/AppContasoftLoader/AppContasoftLoader";
import { AppDropdown } from "../../../../app/Components/UI/AppDropdown";
import { useDigitalizacionDocumentalState } from "../../hooks/useDigitalizacionDocumentalState";
import { useDigitalizacionOperationOrchestrator } from "../../hooks/useDigitalizacionOperationOrchestrator";
import { useDigitalizacionScanner } from "../../hooks/useDigitalizacionScanner";
import type {
  DigitalizacionDocumentalWorkspaceProps,
  DigitalizacionFunctionalError,
  DigitalizacionResult,
} from "../../types/digitalizacion.types";
import {
  DYNAMSOFT_CONTAINER_ID,
  type CaptureOperation,
  type PageCropSelection,
  type ScanPage,
  type ScanProgressSnapshot,
} from "../../infrastructure/dynamsoft";
import { unavailableScannerClient } from "./digitalizacionWorkspace.helpers";
import { PageNavigatorFloating } from "./PageNavigatorFloating";
import styles from "./DigitalizacionDocumentalWorkspace.module.css";

type CaptureMode = "docuarchi" | "driver";
type PreviewFitMode = "custom" | "fitWidth" | "fitPage";
type ThumbnailViewMode = "grid1" | "grid2" | "grid3" | "grid4" | "grid5" | "grid6";
type PageOrganizerDensity =
  | "densityAuto"
  | "density2"
  | "density3"
  | "density4"
  | "density5"
  | "density6";
type CropDraft = {
  pageId: string;
  start: { x: number; y: number };
  current: { x: number; y: number };
};
type CropSelectionState = {
  pageId: string;
  selection: PageCropSelection;
};
type SelectedPageIds = Set<string>;

const MIN_PREVIEW_ZOOM = 50;
const MAX_PREVIEW_ZOOM = 200;
const PREVIEW_ZOOM_STEP = 25;
const PANEL_PREFERENCES_STORAGE_KEY = "docuarchi:digitalizacion:panel-preferences";
const PAGE_HIGHLIGHT_DURATION_MS = 1400;
const THUMBNAIL_VIRTUALIZATION_THRESHOLD = 100;
const PAGE_ORGANIZER_VIRTUALIZATION_THRESHOLD = 100;
const pageOrganizerDensityModes: Array<{ label: string; value: PageOrganizerDensity }> = [
  { label: "2x2", value: "density2" },
  { label: "3x3", value: "density3" },
  { label: "4x4", value: "density4" },
  { label: "5x5", value: "density5" },
  { label: "6x6", value: "density6" },
  { label: "Auto", value: "densityAuto" },
];

const pageOrganizerDensityColumns: Record<Exclude<PageOrganizerDensity, "densityAuto">, number> = {
  density2: 2,
  density3: 3,
  density4: 4,
  density5: 5,
  density6: 6,
};
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

const getPageOrientation = (page: ScanPage) => {
  if (page.orientation && page.orientation !== "unknown") {
    return page.orientation;
  }

  if (page.width && page.height) {
    if (page.width > page.height) {
      return "landscape";
    }

    if (page.height > page.width) {
      return "portrait";
    }

    return "square";
  }

  return "portrait";
};

const getPageAspectRatioStyle = (page: ScanPage): CSSProperties | undefined => {
  if (!page.width || !page.height) {
    return undefined;
  }

  return {
    "--page-aspect-ratio": `${page.width} / ${page.height}`,
  } as CSSProperties;
};

const getOverlayProgressLabel = (progress: ScanProgressSnapshot | null) => {
  if (!progress) {
    return "Escaneando documentos";
  }

  if (progress.stage === "generatingPdf") {
    return "Generando PDF";
  }

  if (progress.stage === "applyingDeskew") {
    return "Corrigiendo inclinacion";
  }

  if (progress.stage === "acquiring" || progress.stage === "preparingDocument") {
    return "Escaneando documentos";
  }

  return "Procesando documentos";
};

const getFooterProgressLabel = (progress: ScanProgressSnapshot | null) =>
  progress ? getOverlayProgressLabel(progress) : null;

const getScannerLoadingProgress = ({
  loading,
  status,
  pageCount,
}: {
  loading: boolean;
  status: string;
  pageCount: number;
}): ScanProgressSnapshot | null => {
  if (!loading) {
    return null;
  }

  if (status === "generatingPdf") {
    return {
      stage: "generatingPdf",
      label: "Generando PDF",
      totalPages: pageCount,
      cancellable: false,
    };
  }

  if (status === "scanning") {
    return {
      stage: "acquiring",
      label: "Escaneando documentos",
      cancellable: true,
    };
  }

  return {
    stage: "preparingDocument",
    label: "Escaneando documentos",
    cancellable: false,
  };
};

const clampNumber = (value: number, min: number, max: number) =>
  Math.min(max, Math.max(min, value));

const normalizeCropSelection = (draft: CropDraft): PageCropSelection => {
  const x = Math.min(draft.start.x, draft.current.x);
  const y = Math.min(draft.start.y, draft.current.y);
  const width = Math.abs(draft.current.x - draft.start.x);
  const height = Math.abs(draft.current.y - draft.start.y);

  return { x, y, width, height };
};

const getCropSelectionStyle = (
  selection: PageCropSelection,
  page: ScanPage,
): CSSProperties => {
  const width = page.width || selection.x + selection.width;
  const height = page.height || selection.y + selection.height;

  return {
    insetBlockStart: `${(selection.y / height) * 100}%`,
    insetInlineStart: `${(selection.x / width) * 100}%`,
    blockSize: `${(selection.height / height) * 100}%`,
    inlineSize: `${(selection.width / width) * 100}%`,
  };
};

const resolvePageOrganizerColumns = ({
  density,
  pageCount,
  width,
  height,
}: {
  density: PageOrganizerDensity;
  pageCount: number;
  width: number;
  height: number;
}) => {
  if (density !== "densityAuto") {
    return pageOrganizerDensityColumns[density];
  }

  if (pageCount <= 0) {
    return 2;
  }

  const viewportRatio = width > 0 && height > 0 ? width / height : 1;
  const visiblePageCount = Math.min(pageCount, pageOrganizerDensityColumns.density6 ** 2);
  const idealColumns = Math.ceil(Math.sqrt(visiblePageCount * viewportRatio));

  return clampNumber(idealColumns, 2, pageOrganizerDensityColumns.density6);
};

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
  const [showPageOrganizer, setShowPageOrganizer] = useState(false);
  const [areaSelectionEnabled, setAreaSelectionEnabled] = useState(false);
  const [cropDraft, setCropDraft] = useState<CropDraft | null>(null);
  const [cropSelection, setCropSelection] = useState<CropSelectionState | null>(null);
  const [highlightedPageId, setHighlightedPageId] = useState<string | null>(null);
  const thumbnailViewMode: ThumbnailViewMode = "grid1";
  const [pageOrganizerDensity, setPageOrganizerDensity] =
    useState<PageOrganizerDensity>("density2");
  const [pageOrganizerViewport, setPageOrganizerViewport] = useState({
    width: 0,
    height: 0,
  });
  const [selectedPageIdsState, setSelectedPageIds] = useState<SelectedPageIds>(() => new Set());
  const [panelPreferences, setPanelPreferences] = useState<PanelPreferences>(
    readPanelPreferences,
  );
  const previewPanelRef = useRef<HTMLElement | null>(null);
  const previewImageRef = useRef<HTMLImageElement | null>(null);
  const pageOrganizerGridRef = useRef<HTMLDivElement | null>(null);
  const thumbnailButtonRefs = useRef(new Map<string, HTMLButtonElement>());
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
    duplicatePage,
    rotatePage,
    cropPage,
    removePage,
    reorderPages,
    generatePdf,
    selectDevice,
    deskewPage,
  } = scanner;

  const handleOperationCompleted = useCallback(
    (result: DigitalizacionResult) => {
      clear();
      void dispose();
      setSelectedPageId(null);
      setSelectedPageIds(new Set());
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
  const selectedPage =
    scanner.pages.find((page) => page.id === selectedPageId) ?? scanner.pages[0] ?? null;
  const selectedPageVisualIndex = selectedPage
    ? scanner.pages.findIndex((page) => page.id === selectedPage.id)
    : -1;
  const currentPageNumber = selectedPageVisualIndex >= 0 ? selectedPageVisualIndex + 1 : 0;
  const availablePageIds = useMemo(
    () => new Set(scanner.pages.map((page) => page.id)),
    [scanner.pages],
  );
  const selectedPageIds = useMemo(
    () =>
      new Set(
        Array.from(selectedPageIdsState).filter((pageId) => availablePageIds.has(pageId)),
      ),
    [availablePageIds, selectedPageIdsState],
  );

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
    setSelectedPageIds(new Set());
    onCompleted({ accion: "cancelado" });
    onCancel?.();
  }, [clear, dispose, onCancel, onCompleted, operation]);

  const handleClear = useCallback(() => {
    clearPages();
    setSelectedPageId(null);
    setSelectedPageIds(new Set());
    void clearScanner();
  }, [clearPages, clearScanner]);

  const executeCapture = useCallback((captureOperation?: CaptureOperation) => {
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
      captureOperation,
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

  const handleScan = useCallback(() => {
    executeCapture();
  }, [executeCapture]);

  const handleNewCapture = useCallback(() => {
    if (!hasPages) {
      executeCapture({ type: "NEW" });
      return;
    }

    const confirmed =
      typeof window === "undefined" ||
      typeof window.confirm !== "function" ||
      window.confirm(
        "Se encontraron paginas en el documento actual. Desea descartarlas e iniciar una nueva captura?",
      ) !== false;

    if (!confirmed) {
      return;
    }

    clearPages();
    setSelectedPageId(null);
    setSelectedPageIds(new Set());
    void clearScanner().then(() => {
      executeCapture({ type: "NEW" });
    });
  }, [clearPages, clearScanner, executeCapture, hasPages]);

  const handleReplaceCapture = useCallback(() => {
    if (!selectedPage) {
      return;
    }

    executeCapture({ type: "REPLACE", targetPageId: selectedPage.id });
  }, [executeCapture, selectedPage]);

  const handleInsertCapture = useCallback(
    (type: Extract<CaptureOperation["type"], "INSERT_BEFORE" | "INSERT_AFTER">) => {
      if (!selectedPage) {
        return;
      }

      executeCapture({ type, targetPageId: selectedPage.id });
    },
    [executeCapture, selectedPage],
  );

  const handleAppendCapture = useCallback(() => {
    executeCapture({ type: "APPEND" });
  }, [executeCapture]);

  const handleRotateSelected = useCallback((degrees: 90 | 270 = 90) => {
    if (selectedPageIds.size > 0) {
      const pageIds = scanner.pages
        .map((page) => page.id)
        .filter((pageId) => selectedPageIds.has(pageId));

      pageIds.forEach((pageId) => {
        void rotatePage(pageId, degrees);
      });
      return;
    }

    const pageId = selectedPageId ?? scanner.pages[0]?.id;
    if (!pageId) return;
    void rotatePage(pageId, degrees);
  }, [rotatePage, scanner.pages, selectedPageId, selectedPageIds]);

  const handleDeskewSelected = useCallback(() => {
    if (selectedPageIds.size > 0) {
      const pageIds = scanner.pages
        .map((page) => page.id)
        .filter((pageId) => selectedPageIds.has(pageId));

      void (async () => {
        for (const pageId of pageIds) {
          await deskewPage(pageId);
        }
      })();
      return;
    }

    const pageId = selectedPageId ?? scanner.pages[0]?.id;
    if (!pageId) return;
    void deskewPage(pageId);
  }, [deskewPage, scanner.pages, selectedPageId, selectedPageIds]);

  const handleRemoveSelected = useCallback(() => {
    if (selectedPageIds.size > 0) {
      const pageIds = scanner.pages
        .map((page) => page.id)
        .filter((pageId) => selectedPageIds.has(pageId));

      if (pageIds.length === 0) {
        return;
      }

      const confirmed =
        typeof window === "undefined" ||
        typeof window.confirm !== "function" ||
        window.confirm(
          `Eliminar ${pageIds.length} ${
            pageIds.length === 1 ? "pagina seleccionada" : "paginas seleccionadas"
          }?`,
        ) !== false;

      if (!confirmed) {
        return;
      }

      pageIds.forEach((pageId) => {
        void removePage(pageId);
      });
      setSelectedPageIds(new Set());
      return;
    }

    const pageId = selectedPageId ?? scanner.pages[0]?.id;
    if (!pageId) return;
    void removePage(pageId);
  }, [removePage, scanner.pages, selectedPageId, selectedPageIds]);

  const handleDuplicateSelected = useCallback(() => {
    const sourcePage = selectedPage;
    if (!sourcePage) {
      return;
    }

    const sourceVisualIndex = scanner.pages.findIndex((page) => page.id === sourcePage.id);
    void duplicatePage(sourcePage.id).then((pages) => {
      if (!pages) {
        return;
      }

      const duplicatedPage = pages[sourceVisualIndex + 1] ?? pages[sourceVisualIndex] ?? null;
      if (duplicatedPage) {
        setSelectedPageId(duplicatedPage.id);
        setHighlightedPageId(duplicatedPage.id);
      }
    });
  }, [duplicatePage, scanner.pages, selectedPage]);

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

  const handleTogglePageSelection = useCallback((pageId: string, checked: boolean) => {
    setSelectedPageIds((current) => {
      const next = new Set(current);
      if (checked) {
        next.add(pageId);
      } else {
        next.delete(pageId);
      }
      return next;
    });
  }, []);

  const handleTogglePageSelectionById = useCallback((pageId: string) => {
    setSelectedPageIds((current) => {
      const next = new Set(current);
      if (next.has(pageId)) {
        next.delete(pageId);
      } else {
        next.add(pageId);
      }
      return next;
    });
  }, []);

  const handleThumbnailClick = useCallback(
    (event: MouseEvent<HTMLButtonElement>, pageId: string) => {
      setSelectedPageId(pageId);
      if (event.ctrlKey || event.metaKey) {
        handleTogglePageSelectionById(pageId);
      }
    },
    [handleTogglePageSelectionById],
  );

  const handleOrganizerPageClick = useCallback((pageId: string) => {
    setSelectedPageId(pageId);
    setHighlightedPageId(pageId);
  }, []);

  const handleRotateOrganizerSelection = useCallback((degrees: 90 | 270) => {
    const pageIds = scanner.pages
      .map((page) => page.id)
      .filter((pageId) => selectedPageIds.has(pageId));

    pageIds.forEach((pageId) => {
      void rotatePage(pageId, degrees);
    });
  }, [rotatePage, scanner.pages, selectedPageIds]);

  const handleDeskewOrganizerSelection = useCallback(() => {
    const pageIds = scanner.pages
      .map((page) => page.id)
      .filter((pageId) => selectedPageIds.has(pageId));

    void (async () => {
      for (const pageId of pageIds) {
        await deskewPage(pageId);
      }
    })();
  }, [deskewPage, scanner.pages, selectedPageIds]);

  const handleRemoveOrganizerSelection = useCallback(() => {
    const pageIds = scanner.pages
      .map((page) => page.id)
      .filter((pageId) => selectedPageIds.has(pageId));

    if (pageIds.length === 0) {
      return;
    }

    const confirmed =
      typeof window === "undefined" ||
      typeof window.confirm !== "function" ||
      window.confirm(
        `Eliminar ${pageIds.length} ${
          pageIds.length === 1 ? "pagina seleccionada" : "paginas seleccionadas"
        }?`,
      ) !== false;

    if (!confirmed) {
      return;
    }

    pageIds.forEach((pageId) => {
      void removePage(pageId);
    });
    setSelectedPageIds(new Set());
  }, [removePage, scanner.pages, selectedPageIds]);

  const handleSelectAllPages = useCallback(() => {
    setSelectedPageIds(new Set(scanner.pages.map((page) => page.id)));
  }, [scanner.pages]);

  const handleClearPageSelection = useCallback(() => {
    setSelectedPageIds(new Set());
  }, []);

  const handleGoToPage = useCallback((requestedPage: number) => {
    if (!Number.isInteger(requestedPage) || scanner.pages.length === 0) {
      return;
    }

    const targetIndex = clampNumber(requestedPage, 1, scanner.pages.length) - 1;
    const targetPage = scanner.pages[targetIndex];
    if (!targetPage) {
      return;
    }

    setSelectedPageId(targetPage.id);
    setHighlightedPageId(targetPage.id);
  }, [scanner.pages]);

  const handleGoToFirstPage = useCallback(() => {
    handleGoToPage(1);
  }, [handleGoToPage]);

  const handleGoToLastPage = useCallback(() => {
    handleGoToPage(scanner.pages.length);
  }, [handleGoToPage, scanner.pages.length]);

  const handleGoToPreviousPage = useCallback(() => {
    if (currentPageNumber <= 1) {
      return;
    }

    handleGoToPage(currentPageNumber - 1);
  }, [currentPageNumber, handleGoToPage]);

  const handleGoToNextPage = useCallback(() => {
    if (currentPageNumber >= scanner.pages.length) {
      return;
    }

    handleGoToPage(currentPageNumber + 1);
  }, [currentPageNumber, handleGoToPage, scanner.pages.length]);

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

  const handleClosePageOrganizer = useCallback(() => {
    setShowPageOrganizer(false);
  }, []);

  const getCropPointFromEvent = useCallback(
    (event: PointerEvent<HTMLElement>, page: ScanPage) => {
      const image = previewImageRef.current;
      if (!image) {
        return null;
      }

      const rect = image.getBoundingClientRect();
      if (rect.width <= 0 || rect.height <= 0) {
        return null;
      }

      const pageWidth = page.width || rect.width;
      const pageHeight = page.height || rect.height;
      const x = clampNumber(
        ((event.clientX - rect.left) / rect.width) * pageWidth,
        0,
        pageWidth,
      );
      const y = clampNumber(
        ((event.clientY - rect.top) / rect.height) * pageHeight,
        0,
        pageHeight,
      );

      return { x, y };
    },
    [],
  );

  const handleAreaSelectionPointerDown = useCallback(
    (event: PointerEvent<HTMLDivElement>) => {
      if (!areaSelectionEnabled || !selectedPage || showPageOrganizer) {
        return;
      }

      const point = getCropPointFromEvent(event, selectedPage);
      if (!point) {
        return;
      }

      event.preventDefault();
      event.currentTarget.setPointerCapture?.(event.pointerId);
      setCropSelection(null);
      setCropDraft({
        pageId: selectedPage.id,
        start: point,
        current: point,
      });
    },
    [areaSelectionEnabled, getCropPointFromEvent, selectedPage, showPageOrganizer],
  );

  const handleAreaSelectionPointerMove = useCallback(
    (event: PointerEvent<HTMLDivElement>) => {
      if (!cropDraft || !selectedPage || cropDraft.pageId !== selectedPage.id) {
        return;
      }

      const point = getCropPointFromEvent(event, selectedPage);
      if (!point) {
        return;
      }

      event.preventDefault();
      setCropDraft((current) =>
        current && current.pageId === selectedPage.id ? { ...current, current: point } : current,
      );
    },
    [cropDraft, getCropPointFromEvent, selectedPage],
  );

  const handleAreaSelectionPointerUp = useCallback(
    (event: PointerEvent<HTMLDivElement>) => {
      if (!cropDraft || !selectedPage || cropDraft.pageId !== selectedPage.id) {
        return;
      }

      event.currentTarget.releasePointerCapture?.(event.pointerId);
      const point = getCropPointFromEvent(event, selectedPage);
      const selection = normalizeCropSelection({
        ...cropDraft,
        current: point ?? cropDraft.current,
      });
      setCropDraft(null);
      if (selection.width < 2 || selection.height < 2) {
        setCropSelection(null);
        return;
      }

      setCropSelection({ pageId: selectedPage.id, selection });
    },
    [cropDraft, getCropPointFromEvent, selectedPage],
  );

  const handleResetCropSelection = useCallback(() => {
    setCropDraft(null);
    setCropSelection(null);
  }, []);

  const handleCancelCropSelection = useCallback(() => {
    setAreaSelectionEnabled(false);
    setCropDraft(null);
    setCropSelection(null);
  }, []);

  const handleApplyCropSelection = useCallback(() => {
    if (!selectedPage || !cropSelection) {
      return;
    }

    if (cropSelection.pageId !== selectedPage.id) {
      return;
    }

    void cropPage(selectedPage.id, cropSelection.selection).then(() => {
      setCropDraft(null);
      setCropSelection(null);
    });
  }, [cropPage, cropSelection, selectedPage]);

  useEffect(() => {
    if (!showPageOrganizer || typeof window === "undefined") {
      return undefined;
    }

    const grid = pageOrganizerGridRef.current;
    if (!grid) {
      return undefined;
    }

    const updateViewport = () => {
      const { width, height } = grid.getBoundingClientRect();
      setPageOrganizerViewport((current) =>
        current.width === width && current.height === height
          ? current
          : { width, height },
      );
    };

    const frameId = window.requestAnimationFrame(updateViewport);
    const observer =
      typeof ResizeObserver === "undefined"
        ? null
        : new ResizeObserver((entries) => {
            const entry = entries[0];
            const size = entry?.contentRect;
            if (!size) {
              return;
            }

            setPageOrganizerViewport((current) =>
              current.width === size.width && current.height === size.height
                ? current
                : { width: size.width, height: size.height },
            );
          });

    observer?.observe(grid);

    return () => {
      window.cancelAnimationFrame(frameId);
      observer?.disconnect();
    };
  }, [showPageOrganizer]);

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

  useEffect(() => {
    if (!highlightedPageId) {
      return undefined;
    }

    const thumbnailButton = thumbnailButtonRefs.current.get(highlightedPageId);
    thumbnailButton?.scrollIntoView?.({ block: "nearest", inline: "nearest" });

    const timeoutId = window.setTimeout(() => {
      setHighlightedPageId((current) => (current === highlightedPageId ? null : current));
    }, PAGE_HIGHLIGHT_DURATION_MS);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [highlightedPageId]);

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
  const selectedCropSelection =
    selectedPage && cropSelection?.pageId === selectedPage.id ? cropSelection.selection : null;
  const activeCropSelection =
    selectedPage && cropDraft?.pageId === selectedPage.id
      ? normalizeCropSelection(cropDraft)
      : selectedCropSelection
        ? selectedCropSelection
        : null;
  const thumbnailsCollapsed = !panelPreferences.showThumbnails;
  const configurationCollapsed = !panelPreferences.showConfiguration;
  const thumbnailsVirtualized = scanner.pages.length > THUMBNAIL_VIRTUALIZATION_THRESHOLD;
  const organizerVirtualized = scanner.pages.length > PAGE_ORGANIZER_VIRTUALIZATION_THRESHOLD;
  const selectedPageCount = selectedPageIds.size;
  const hasPageSelection = selectedPageCount > 0;
  const allPagesSelected = scanner.pages.length > 0 && selectedPageCount === scanner.pages.length;
  const selectedPagesLabel = `${selectedPageCount} ${
    selectedPageCount === 1 ? "pagina seleccionada" : "paginas seleccionadas"
  }`;
  const pageOrganizerColumns = resolvePageOrganizerColumns({
    density: pageOrganizerDensity,
    pageCount: scanner.pages.length,
    width: pageOrganizerViewport.width,
    height: pageOrganizerViewport.height,
  });
  const pageOrganizerVisibleRows = clampNumber(
    Math.ceil(
      Math.min(
        Math.max(scanner.pages.length, 1),
        pageOrganizerColumns * pageOrganizerColumns,
      ) / pageOrganizerColumns,
    ),
    1,
    pageOrganizerColumns,
  );
  const pageOrganizerGridStyle = {
    "--page-organizer-columns": pageOrganizerColumns,
    "--page-organizer-visible-rows": pageOrganizerVisibleRows,
  } as CSSProperties;
  const hasCaptureTarget = Boolean(selectedPage);
  const primaryCaptureLabel = hasPages ? "Nuevo documento" : "Escanear";
  const primaryCaptureTooltip = hasPages
    ? "Descartar documento actual e iniciar uno nuevo"
    : "Iniciar captura documental";
  const handlePrimaryCapture = hasPages ? handleNewCapture : handleScan;
  const insertCaptureItems = [
    {
      key: "insert-before",
      label: "Insertar antes",
      leftIcon: <InsertRowAboveOutlined />,
      disabled: !hasCaptureTarget,
      onSelect: () => handleInsertCapture("INSERT_BEFORE"),
    },
    {
      key: "insert-after",
      label: "Insertar despues",
      leftIcon: <InsertRowBelowOutlined />,
      disabled: !hasCaptureTarget,
      onSelect: () => handleInsertCapture("INSERT_AFTER"),
    },
  ];
  const pageOrganizerDensityItems = pageOrganizerDensityModes.map((mode) => ({
    key: mode.value,
    label: mode.label,
    onSelect: () => {
      setPageOrganizerDensity(mode.value);
      setAreaSelectionEnabled(false);
      setCropDraft(null);
      setCropSelection(null);
      setShowPageOrganizer(true);
    },
  }));
  const hasOrganizerSelection = scanner.pages.some((page) => selectedPageIds.has(page.id));
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
  const previewSurfaceClassName = [
    styles.previewPageSurface,
    areaSelectionEnabled ? styles.previewPageSurfaceSelecting : "",
  ]
    .filter(Boolean)
    .join(" ");
  const activeProgress =
    scanner.progress ??
    getScannerLoadingProgress({
      loading: scanner.loading,
      status: scanner.status,
      pageCount: scanner.pages.length,
    });
  const activeProgressLabel = getOverlayProgressLabel(activeProgress);
  const footerProgressLabel = getFooterProgressLabel(activeProgress);

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
            icon={hasPages ? <FileAddOutlined /> : <ScanOutlined />}
            aria-label={primaryCaptureLabel}
            tooltip={primaryCaptureTooltip}
            onClick={handlePrimaryCapture}
            disabled={!scanner.selectedDeviceId || scanner.loading || Boolean(state.validationError)}
          />
          <AppButton
            variant="ghost"
            size="sm"
            icon={<SwapOutlined />}
            aria-label="Reemplazar"
            tooltip="Reemplazar la pagina actual"
            onClick={handleReplaceCapture}
            disabled={
              !scanner.selectedDeviceId ||
              scanner.loading ||
              Boolean(state.validationError) ||
              !hasCaptureTarget
            }
          />
          <AppDropdown
            ariaLabel="Insertar paginas"
            placement="bottomLeft"
            items={insertCaptureItems}
            disabled={
              !scanner.selectedDeviceId ||
              scanner.loading ||
              Boolean(state.validationError) ||
              !hasCaptureTarget
            }
            trigger={
              <AppButton
                variant="ghost"
                size="sm"
                leftIcon={<PlusOutlined />}
                rightIcon={<DownOutlined />}
                aria-label="Insertar"
                tooltip="Insertar paginas antes o despues de la actual"
                disabled={
                  !scanner.selectedDeviceId ||
                  scanner.loading ||
                  Boolean(state.validationError) ||
                  !hasCaptureTarget
                }
              >
                Insertar
              </AppButton>
            }
          />
          <AppButton
            variant="ghost"
            size="sm"
            icon={<InsertRowBelowOutlined />}
            aria-label="Agregar"
            tooltip="Agregar paginas al final del documento"
            onClick={handleAppendCapture}
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
            <div className={styles.thumbnailPanelContent}>
              <div className={styles.thumbnailSelectionBar}>
                <span aria-live="polite">{selectedPagesLabel}</span>
                <div className={styles.thumbnailSelectionActions}>
                  <AppButton
                    variant="ghost"
                    size="sm"
                    leftIcon={<CheckSquareOutlined />}
                    aria-label={allPagesSelected ? "Deseleccionar todo" : "Seleccionar todo"}
                    onClick={allPagesSelected ? handleClearPageSelection : handleSelectAllPages}
                  >
                    {allPagesSelected ? "Deseleccionar todo" : "Seleccionar todo"}
                  </AppButton>
                </div>
              </div>
              <div
                className={styles.thumbnailList}
                data-view-mode={thumbnailViewMode}
                data-virtualized={thumbnailsVirtualized}
              >
                {scanner.pages.map((page, pageOrderIndex) => (
                  <button
                    className={styles.thumbnailButton}
                    data-selected={page.id === selectedPageId}
                    data-checked={selectedPageIds.has(page.id)}
                    data-dragging={page.id === draggedPageId}
                    data-drop-target={page.id === dragOverPageId}
                    data-highlighted={page.id === highlightedPageId}
                    key={page.id}
                    ref={(element) => {
                      if (element) {
                        thumbnailButtonRefs.current.set(page.id, element);
                        return;
                      }
                      thumbnailButtonRefs.current.delete(page.id);
                    }}
                    type="button"
                    draggable={!thumbnailsCollapsed}
                    tabIndex={thumbnailsCollapsed ? -1 : 0}
                    onClick={(event) => handleThumbnailClick(event, page.id)}
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
                    <label
                      className={styles.thumbnailCheck}
                      onClick={(event) => event.stopPropagation()}
                    >
                      <input
                        type="checkbox"
                        checked={selectedPageIds.has(page.id)}
                        onChange={(event) =>
                          handleTogglePageSelection(page.id, event.target.checked)
                        }
                        aria-label={`Seleccionar pagina ${pageOrderIndex + 1}`}
                      />
                    </label>
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
        data-progress-active={activeProgress ? "true" : "false"}
        ref={previewPanelRef}
      >
          <div className={`${styles.panelHeader} ${styles.previewHeader}`}>
            <div className={styles.previewControls} role="toolbar" aria-label="Visualizacion preview">
              <div className={styles.previewControlGroup} role="group" aria-label="Edicion">
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
                variant="ghost"
                size="sm"
                icon={<CompressOutlined />}
                aria-label="Deskew"
                tooltip="Corregir inclinacion de la pagina"
                onClick={handleDeskewSelected}
                disabled={!selectedPage || scanner.loading}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<CopyOutlined />}
                aria-label="Duplicar pagina"
                tooltip="Duplicar pagina"
                onClick={handleDuplicateSelected}
                disabled={!selectedPage || scanner.loading}
              />
              <AppButton
                variant={areaSelectionEnabled ? "secondary" : "ghost"}
                size="sm"
                icon={<SelectOutlined />}
                aria-label="Seleccionar area"
                aria-pressed={areaSelectionEnabled}
                tooltip="Seleccionar area"
                onClick={() => {
                  setAreaSelectionEnabled((current) => !current);
                  setCropDraft(null);
                  setCropSelection(null);
                }}
                disabled={!selectedPage || showPageOrganizer}
              />
              <AppButton
                variant="danger"
                size="sm"
                icon={<DeleteOutlined />}
                aria-label="Eliminar pagina"
                tooltip="Eliminar pagina"
                onClick={handleRemoveSelected}
                disabled={!selectedPage}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<ClearOutlined />}
                aria-label="Limpiar documento"
                tooltip="Limpiar documento"
                onClick={handleClear}
                disabled={scanner.loading || !hasPages}
              />
              </div>
              <div className={styles.previewControlGroup} role="group" aria-label="Visualizacion">
              <AppButton
                variant="ghost"
                size="sm"
                icon={<ZoomOutOutlined />}
                aria-label="Reducir zoom"
                tooltip="Reducir zoom"
                onClick={handleZoomOut}
                disabled={!selectedPage || previewZoom <= MIN_PREVIEW_ZOOM}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<ZoomInOutlined />}
                aria-label="Aumentar zoom"
                tooltip="Aumentar zoom"
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
              <div className={styles.previewControlGroup} role="group" aria-label="Organizacion">
              <AppDropdown
                ariaLabel="Organizar paginas"
                placement="bottomLeft"
                items={pageOrganizerDensityItems}
                disabled={!hasPages}
                trigger={
                  <AppButton
                    className={styles.organizerMenuButton}
                    variant="ghost"
                    size="sm"
                    leftIcon={<AppstoreOutlined />}
                    rightIcon={<DownOutlined />}
                    aria-label="Organizar paginas"
                    tooltip="Organizar paginas"
                    disabled={!hasPages}
                  />
                }
              />
              </div>
              <div className={styles.previewControlGroup} role="group" aria-label="Navegacion">
              {hasPageSelection ? (
                <span className={styles.selectedPagesBadge} aria-live="polite">
                  {selectedPageCount} seleccionadas
                </span>
              ) : null}
              </div>
            </div>
          </div>
          <div className={`${styles.panelBody} ${styles.preview}`}>
            {selectedPage ? (
              <>
                <div className={previewViewportClassName}>
                  {selectedPage.imageUrl ? (
                    <div
                      className={previewSurfaceClassName}
                      style={previewImageStyle}
                      onPointerDown={handleAreaSelectionPointerDown}
                      onPointerMove={handleAreaSelectionPointerMove}
                      onPointerUp={handleAreaSelectionPointerUp}
                      onPointerCancel={handleAreaSelectionPointerUp}
                    >
                      <img
                        ref={previewImageRef}
                        className={styles.previewImage}
                        src={selectedPage.imageUrl}
                        alt={getPageLabel(selectedPage)}
                        draggable={false}
                      />
                      {activeCropSelection ? (
                        <div
                          className={styles.cropSelectionRect}
                          aria-label="Area seleccionada"
                          style={getCropSelectionStyle(activeCropSelection, selectedPage)}
                        />
                      ) : null}
                      {selectedCropSelection ? (
                        <div
                          className={styles.cropActions}
                          role="toolbar"
                          aria-label="Acciones de seleccion"
                          onClick={(event) => event.stopPropagation()}
                          onPointerDown={(event) => event.stopPropagation()}
                          onPointerMove={(event) => event.stopPropagation()}
                          onPointerUp={(event) => event.stopPropagation()}
                        >
                          <AppButton
                            variant="secondary"
                            size="sm"
                            icon={<ScissorOutlined />}
                            aria-label="Recortar seleccion"
                            tooltip="Recortar"
                            onClick={handleApplyCropSelection}
                          />
                          <AppButton
                            variant="ghost"
                            size="sm"
                            icon={<ClearOutlined />}
                            aria-label="Reiniciar seleccion"
                            tooltip="Reiniciar seleccion"
                            onClick={handleResetCropSelection}
                          />
                          <AppButton
                            variant="ghost"
                            size="sm"
                            icon={<CloseOutlined />}
                            aria-label="Cancelar seleccion"
                            tooltip="Cancelar"
                            onClick={handleCancelCropSelection}
                          />
                        </div>
                      ) : null}
                    </div>
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
          <PageNavigatorFloating
            currentPage={currentPageNumber}
            totalPages={scanner.pages.length}
            onFirstPage={handleGoToFirstPage}
            onPreviousPage={handleGoToPreviousPage}
            onNextPage={handleGoToNextPage}
            onLastPage={handleGoToLastPage}
            onGoToPage={handleGoToPage}
          />
          {activeProgress ? (
            <div
              className={styles.scanProgressOverlay}
              role="status"
              aria-live="polite"
              aria-label={activeProgressLabel}
            >
              <div className={styles.scanProgressPanel}>
                <div className={styles.scanProgressIcon} aria-hidden="true">
                  <AppContasoftLoader size={72} label="Progreso Contasoft" />
                </div>
                <div className={styles.scanProgressContent}>
                  <span className={styles.scanProgressTitle}>{activeProgressLabel}</span>
                </div>
                {activeProgress.cancellable ? (
                  <AppButton
                    variant="ghost"
                    size="sm"
                    icon={<CloseOutlined />}
                    aria-label="Cancelar operacion"
                    tooltip="Cancelar operacion"
                    onClick={handleCancel}
                  />
                ) : null}
              </div>
            </div>
          ) : null}
          {showPageOrganizer ? (
            <div
              className={styles.pageOrganizerOverlay}
              role="region"
              aria-label="Organizador de paginas"
            >
              <div className={styles.pageOrganizerHeader}>
                <span>Organizar paginas</span>
                <div
                  className={styles.pageOrganizerActions}
                  role="toolbar"
                  aria-label="Acciones organizador"
                >
                  <AppButton
                    variant="ghost"
                    size="sm"
                    icon={<RotateLeftOutlined />}
                    aria-label="Rotar izquierda seleccionadas"
                    tooltip="Rotar izquierda"
                    onClick={() => handleRotateOrganizerSelection(270)}
                    disabled={!hasOrganizerSelection}
                  />
                  <AppButton
                    variant="ghost"
                    size="sm"
                    icon={<RotateRightOutlined />}
                    aria-label="Rotar derecha seleccionadas"
                    tooltip="Rotar derecha"
                    onClick={() => handleRotateOrganizerSelection(90)}
                    disabled={!hasOrganizerSelection}
                  />
                  <AppButton
                    variant="ghost"
                    size="sm"
                    icon={<ColumnWidthOutlined />}
                    aria-label="Deskew paginas seleccionadas"
                    tooltip="Corregir inclinacion de paginas"
                    onClick={handleDeskewOrganizerSelection}
                    disabled={!hasOrganizerSelection || scanner.loading}
                  />
                  <AppButton
                    variant="danger"
                    size="sm"
                    icon={<DeleteOutlined />}
                    aria-label="Eliminar paginas seleccionadas"
                    tooltip="Eliminar paginas"
                    onClick={handleRemoveOrganizerSelection}
                    disabled={!hasOrganizerSelection}
                  />
                  <AppButton
                    variant="ghost"
                    size="sm"
                    icon={<CloseOutlined />}
                    aria-label="Cerrar organizacion"
                    tooltip="Volver al visor"
                    onClick={handleClosePageOrganizer}
                  />
                </div>
              </div>
              <div
                className={styles.pageOrganizerGrid}
                ref={pageOrganizerGridRef}
                data-density={pageOrganizerDensity}
                data-columns={pageOrganizerColumns}
                data-virtualized={organizerVirtualized}
                style={pageOrganizerGridStyle}
              >
                {scanner.pages.map((page, pageOrderIndex) => {
                  const pageOrientation = getPageOrientation(page);
                  const pageAspectRatioStyle = getPageAspectRatioStyle(page);

                  return (
                    <button
                      className={styles.pageOrganizerItem}
                      data-selected={page.id === selectedPageId}
                      data-checked={selectedPageIds.has(page.id)}
                      data-dragging={page.id === draggedPageId}
                      data-drop-target={page.id === dragOverPageId}
                      data-orientation={pageOrientation}
                      key={page.id}
                      type="button"
                      draggable
                      style={pageAspectRatioStyle}
                      onClick={() => handleOrganizerPageClick(page.id)}
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
                      <label
                        className={styles.pageOrganizerCheck}
                        onClick={(event) => event.stopPropagation()}
                      >
                        <input
                          type="checkbox"
                          checked={selectedPageIds.has(page.id)}
                          onChange={(event) =>
                            handleTogglePageSelection(page.id, event.target.checked)
                          }
                          aria-label={`Seleccionar pagina ${pageOrderIndex + 1}`}
                        />
                      </label>
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
                  );
                })}
              </div>
            </div>
          ) : null}
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
          <span>
            {footerProgressLabel ??
              (operation.loading ? `Operacion ${operation.status}` : "Listo para operar")}
          </span>
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
