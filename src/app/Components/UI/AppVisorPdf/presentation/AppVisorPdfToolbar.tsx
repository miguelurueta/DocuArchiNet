import { useMemo } from "react";
import type { KeyboardEvent as ReactKeyboardEvent } from "react";
import type { AppVisorPdfTool } from "../domain/visorPdf.types";
import { AppButton } from "../../AppButton";
import { AppDropdown } from "../../AppDropdown";
import type { AppDropdownItem } from "../../AppDropdown";
import styles from "./AppVisorPdfToolbar.module.css";

type Props = {
  disabled?: boolean;
  page: number;
  onPageChange: (page: number) => void;
  zoom: number;
  onZoomChange: (zoom: number) => void;
  tool: AppVisorPdfTool;
  onToolChange: (tool: AppVisorPdfTool) => void;
  onUndo?: () => void;
  onRedo?: () => void;
  isCompact?: boolean;
  thumbnailsOpen?: boolean;
  onToggleThumbnails?: () => void;
  thumbnailsControlsId?: string;
  thumbnailsLabelId?: string;
};

const TOOL_LABELS: Record<AppVisorPdfTool, string> = {
  pan: "Pan",
  select: "Select",
  freehand: "Freehand",
  text: "Text",
  rect: "Rect",
  arrow: "Arrow",
  stamp_grafo: "Stamp",
};

export function AppVisorPdfToolbar({
  disabled = false,
  page,
  onPageChange,
  zoom,
  onZoomChange,
  tool,
  onToolChange,
  onUndo,
  onRedo,
  isCompact = false,
  thumbnailsOpen = false,
  onToggleThumbnails,
  thumbnailsControlsId,
  thumbnailsLabelId,
}: Props) {
  const toolItems = useMemo<AppDropdownItem[]>(
    () =>
      (Object.keys(TOOL_LABELS) as AppVisorPdfTool[]).map((key) => ({
        key,
        label: TOOL_LABELS[key],
        onSelect: () => onToolChange(key),
        disabled,
      })),
    [disabled, onToolChange],
  );

  const secondaryItems = useMemo<AppDropdownItem[]>(
    () => [
      {
        key: "undo",
        label: "Undo",
        onSelect: () => onUndo?.(),
        disabled: disabled || !onUndo,
      },
      {
        key: "redo",
        label: "Redo",
        onSelect: () => onRedo?.(),
        disabled: disabled || !onRedo,
      },
    ],
    [disabled, onRedo, onUndo],
  );

  return (
    <div className={styles.toolbar} data-compact={isCompact ? "true" : "false"}>
      <div className={styles.group}>
        <AppButton
          id={thumbnailsLabelId}
          aria-label="Thumbnails"
          variant="secondary"
          size="sm"
          disabled={disabled || !onToggleThumbnails}
          onClick={() => onToggleThumbnails?.()}
          aria-expanded={thumbnailsOpen}
          aria-controls={thumbnailsControlsId}
          onKeyDown={(event: ReactKeyboardEvent) => {
            if (disabled || !onToggleThumbnails) return;
            if (event.key === "Enter" || event.key === " ") {
              event.preventDefault();
              onToggleThumbnails();
            }
          }}
        >
          Pages
        </AppButton>
        <AppButton
          aria-label="Pagina anterior"
          variant="secondary"
          size="sm"
          disabled={disabled || page <= 1}
          onClick={() => onPageChange(page - 1)}
        >
          {"<"}
        </AppButton>
        <span className={styles.value} aria-label="Pagina actual">
          {page}
        </span>
        <AppButton
          aria-label="Pagina siguiente"
          variant="secondary"
          size="sm"
          disabled={disabled}
          onClick={() => onPageChange(page + 1)}
        >
          {">"}
        </AppButton>
      </div>

      <div className={styles.group}>
        <AppButton
          aria-label="Zoom out"
          variant="secondary"
          size="sm"
          disabled={disabled}
          onClick={() => onZoomChange(zoom - 0.1)}
        >
          {"-"}
        </AppButton>
        <span className={styles.value} aria-label="Zoom actual">
          {zoom.toFixed(2)}
        </span>
        <AppButton
          aria-label="Zoom in"
          variant="secondary"
          size="sm"
          disabled={disabled}
          onClick={() => onZoomChange(zoom + 0.1)}
        >
          {"+"}
        </AppButton>
      </div>

      <div className={styles.group}>
        {isCompact ? (
          <AppDropdown
            ariaLabel="Acciones"
            disabled={disabled}
            items={secondaryItems}
            trigger={
              <AppButton
                aria-label="Acciones"
                variant="secondary"
                size="sm"
                disabled={disabled}
              >
                More
              </AppButton>
            }
          />
        ) : (
          <>
            <AppButton
              aria-label="Undo"
              variant="secondary"
              size="sm"
              disabled={disabled || !onUndo}
              onClick={() => onUndo?.()}
            >
              Undo
            </AppButton>
            <AppButton
              aria-label="Redo"
              variant="secondary"
              size="sm"
              disabled={disabled || !onRedo}
              onClick={() => onRedo?.()}
            >
              Redo
            </AppButton>
          </>
        )}
      </div>

      <div className={styles.group}>
        <AppDropdown
          ariaLabel="Herramientas"
          disabled={disabled}
          items={toolItems}
          trigger={
            <AppButton
              aria-label="Herramientas"
              variant="secondary"
              size="sm"
              disabled={disabled}
            >
              {TOOL_LABELS[tool]}
            </AppButton>
          }
        />
      </div>
    </div>
  );
}

