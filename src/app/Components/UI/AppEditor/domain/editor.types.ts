import type { ReactNode } from "react";
import type { Editor } from "@tiptap/react";

export type AppEditorHeadingLevel = 1 | 2 | 3;
export type AppEditorThemeMode = "system" | "light" | "dark";

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
  surfaceClassName?: string;
  minHeight?: number | string;
  showThemeToggle?: boolean;
  themeMode?: AppEditorThemeMode;
  defaultThemeMode?: AppEditorThemeMode;
  onThemeModeChange?: (mode: AppEditorThemeMode) => void;
  "aria-label"?: string;
};

export type UseAppEditorOptions = Pick<
  AppEditorProps,
  "value" | "defaultValue" | "onChange" | "placeholder" | "disabled" | "readOnly"
>;

export type UseAppEditorResult = {
  editor: Editor | null;
  isEditable: boolean;
};
