import type { ReactNode } from "react";
import type { Editor } from "@tiptap/react";

export type AppEditorHeadingLevel = 1 | 2 | 3;
export type AppEditorThemeMode = "system" | "light" | "dark";
export type AppEditorPaginationMode = "none" | "visual";
export type AppEditorPageFormat = "A4";
export type AppEditorPageOrientation = "portrait" | "landscape";
export type AppEditorPageMargins = {
  top: number;
  right: number;
  bottom: number;
  left: number;
};
export type AppEditorPageContextSource = "cursor" | "scroll";
export type AppEditorPageContext = {
  currentPage: number;
  totalPages: number;
  source: AppEditorPageContextSource;
};

export type AppEditorProps = {
  value?: string;
  defaultValue?: string;
  onChange?: (value: string) => void;
  placeholder?: string;
  disabled?: boolean;
  readOnly?: boolean;
  label?: ReactNode;
  error?: ReactNode;
  helperText?: ReactNode;
  className?: string;
  title?: ReactNode;
  description?: ReactNode;
  headerActions?: ReactNode;
  toolbarActions?: ReactNode;
  surfaceClassName?: string;
  minHeight?: number | string;
  showThemeToggle?: boolean;
  themeMode?: AppEditorThemeMode;
  defaultThemeMode?: AppEditorThemeMode;
  onThemeModeChange?: (mode: AppEditorThemeMode) => void;
  paginationMode?: AppEditorPaginationMode;
  pageFormat?: AppEditorPageFormat;
  pageOrientation?: AppEditorPageOrientation;
  pageMargins?: Partial<AppEditorPageMargins>;
  zoomLevel?: number;
  defaultZoomLevel?: number;
  minZoomLevel?: number;
  maxZoomLevel?: number;
  onZoomChange?: (zoom: number) => void;
  onPageContextChange?: (context: AppEditorPageContext) => void;
  "aria-label"?: string;
};

export type UseAppEditorOptions = Pick<
  AppEditorProps,
  | "value"
  | "defaultValue"
  | "onChange"
  | "placeholder"
  | "disabled"
  | "readOnly"
  | "paginationMode"
  | "zoomLevel"
> & {
  pageHeight?: number;
  pageGap?: number;
  pageMargins?: AppEditorPageMargins;
};

export type UseAppEditorResult = {
  editor: Editor | null;
  isEditable: boolean;
  insertLocalImage: (file: File, width?: string) => Promise<void>;
};
