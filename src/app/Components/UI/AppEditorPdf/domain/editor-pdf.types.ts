import type {
  AppEditorHeadingLevel,
  AppEditorPaginationMode,
  AppEditorPageFormat,
  AppEditorPageMargins,
  AppEditorPageOrientation,
  AppEditorProps,
  AppEditorThemeMode,
  UseAppEditorOptions,
  UseAppEditorResult,
} from "../../AppEditor/domain/editor.types";

export type AppEditorPdfHeadingLevel = AppEditorHeadingLevel;
export type AppEditorPdfPaginationMode = AppEditorPaginationMode;
export type AppEditorPdfPageFormat = AppEditorPageFormat;
export type AppEditorPdfPageMargins = AppEditorPageMargins;
export type AppEditorPdfPageOrientation = AppEditorPageOrientation;
export type AppEditorPdfThemeMode = AppEditorThemeMode;
export type UseAppEditorPdfOptions = UseAppEditorOptions;
export type UseAppEditorPdfResult = UseAppEditorResult;

export type AppEditorPdfVisualGuides = {
  enabled?: boolean;
  showPageBoundaries?: boolean;
  showReadingFrame?: boolean;
  readingFrameInset?: number;
};

export type AppEditorPdfVisualMetrics = {
  documentSource: string;
  currentPage: number;
  totalPages: number;
  zoomLevel: number;
  pageWidth: number;
  pageHeight: number;
  contentWidth: number;
  contentHeight: number;
  pageMargins: AppEditorPdfPageMargins;
};

export type AppEditorPdfProps = AppEditorProps & {
  documentSource?: string;
  totalPages?: number;
  activePage?: number;
  defaultActivePage?: number;
  onActivePageChange?: (page: number) => void;
  visualGuides?: AppEditorPdfVisualGuides;
  onMetricsChange?: (metrics: AppEditorPdfVisualMetrics) => void;
};
