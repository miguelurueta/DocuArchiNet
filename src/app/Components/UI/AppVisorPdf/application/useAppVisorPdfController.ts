import { useCallback, useMemo, useState } from "react";
import type { AppVisorPdfProps, AppVisorPdfTool } from "../domain/visorPdf.types";

const clampInteger = (value: number, min: number) => {
  if (!Number.isFinite(value)) return min;
  return Math.max(min, Math.floor(value));
};

const clampZoom = (value: number) => {
  if (!Number.isFinite(value)) return 1;
  return Math.max(0.1, Math.min(10, value));
};

export function useAppVisorPdfController({
  page,
  defaultPage = 1,
  onPageChange,
  zoom,
  defaultZoom = 1,
  onZoomChange,
  tool,
  defaultTool = "pan",
  onToolChange,
}: Pick<
  AppVisorPdfProps,
  | "page"
  | "defaultPage"
  | "onPageChange"
  | "zoom"
  | "defaultZoom"
  | "onZoomChange"
  | "tool"
  | "defaultTool"
  | "onToolChange"
>) {
  const isControlledPage = typeof page === "number";
  const isControlledZoom = typeof zoom === "number";
  const isControlledTool = typeof tool === "string";

  const [uncontrolledPage, setUncontrolledPage] = useState(() =>
    clampInteger(defaultPage, 1),
  );
  const [uncontrolledZoom, setUncontrolledZoom] = useState(() =>
    clampZoom(defaultZoom),
  );
  const [uncontrolledTool, setUncontrolledTool] = useState<AppVisorPdfTool>(() =>
    defaultTool ?? "pan",
  );

  const currentPage = useMemo(
    () => clampInteger(isControlledPage ? (page ?? 1) : uncontrolledPage, 1),
    [isControlledPage, page, uncontrolledPage],
  );
  const currentZoom = useMemo(
    () => clampZoom(isControlledZoom ? (zoom ?? defaultZoom) : uncontrolledZoom),
    [defaultZoom, isControlledZoom, uncontrolledZoom, zoom],
  );
  const currentTool = useMemo(
    () => (isControlledTool ? (tool ?? defaultTool ?? "pan") : uncontrolledTool),
    [defaultTool, isControlledTool, tool, uncontrolledTool],
  );

  const setPage = useCallback(
    (nextPage: number) => {
      const normalized = clampInteger(nextPage, 1);
      if (!isControlledPage) {
        setUncontrolledPage(normalized);
      }
      onPageChange?.(normalized);
    },
    [isControlledPage, onPageChange],
  );

  const setZoom = useCallback(
    (nextZoom: number) => {
      const normalized = clampZoom(nextZoom);
      if (!isControlledZoom) {
        setUncontrolledZoom(normalized);
      }
      onZoomChange?.(normalized);
    },
    [isControlledZoom, onZoomChange],
  );

  const setTool = useCallback(
    (nextTool: AppVisorPdfTool) => {
      if (!isControlledTool) {
        setUncontrolledTool(nextTool);
      }
      onToolChange?.(nextTool);
    },
    [isControlledTool, onToolChange],
  );

  return {
    page: currentPage,
    zoom: currentZoom,
    tool: currentTool,
    setPage,
    setZoom,
    setTool,
  };
}

