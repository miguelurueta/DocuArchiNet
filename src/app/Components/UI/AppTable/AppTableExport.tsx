import { DownloadOutlined } from "@ant-design/icons";
import { useMemo, useState, startTransition } from "react";
import { AppButton } from "../AppButton";
import { AppDropdown, type AppDropdownItem } from "../AppDropdown";
import type { AppTableExportFormat, AppTableExportMode, AppTableExportProps } from "./AppTableExport.types";
import { getAvailableAppTableExportModes } from "./AppTableExport.types";
import {
  downloadAppTableExportFile,
  getAppTableExportRows,
  getAppTableExportableColumns,
} from "./AppTableExport.utils";
import type { AppTableRow } from "./AppTable.types";
import styles from "./AppTableExport.module.css";

const LOCAL_EXPORT_MODES = [
  "currentPage",
  "selectedRows",
  "allLoaded",
] as const satisfies readonly AppTableExportMode[];
const SUPPORTED_FORMATS = new Set<AppTableExportFormat>(["csv"]);

const MODE_LABELS: Record<(typeof LOCAL_EXPORT_MODES)[number], string> = {
  currentPage: "Página actual",
  selectedRows: "Seleccionados",
  allLoaded: "Todos los cargados",
};

const FORMAT_LABELS: Record<AppTableExportFormat, string> = {
  csv: "CSV",
  xlsx: "Excel",
  pdf: "PDF",
};

const buildItemKey = (format: AppTableExportFormat, mode: AppTableExportMode) => `${format}-${mode}`;
const EXPORT_TRIGGER_ICON = <span className={styles.triggerIcon} aria-hidden="true"><DownloadOutlined /></span>;

export function AppTableExport<T extends AppTableRow>({
  columns,
  dataSource,
  formats,
  reportMeta,
  enabledModes = [...LOCAL_EXPORT_MODES],
  fileName,
  triggerLabel = "Exportar",
  disabled = false,
}: AppTableExportProps<T>) {
  const [exportLoading, setExportLoading] = useState(false);

  const exportableColumns = useMemo(() => getAppTableExportableColumns(columns), [columns]);
  const availableModes = useMemo(
    () => getAvailableAppTableExportModes(dataSource, enabledModes).filter((mode) =>
      LOCAL_EXPORT_MODES.includes(mode as (typeof LOCAL_EXPORT_MODES)[number]),
    ) as Array<(typeof LOCAL_EXPORT_MODES)[number]>,
    [dataSource, enabledModes],
  );

  const selectedRows = useMemo(
    () => (typeof dataSource.getSelectedRows === "function" ? dataSource.getSelectedRows() : []),
    [dataSource],
  );

  const selectedRowsCount = selectedRows.length;
  const selectedRowsAvailable = availableModes.includes("selectedRows");

  const handleExport = async (
    format: AppTableExportFormat,
    mode: (typeof LOCAL_EXPORT_MODES)[number],
  ) => {
    if (exportLoading || disabled || !SUPPORTED_FORMATS.has(format)) {
      return;
    }

    if (mode === "selectedRows" && selectedRowsCount === 0) {
      return;
    }

    const rows = getAppTableExportRows({
      mode,
      getCurrentPageRows: dataSource.getCurrentPageRows,
      getSelectedRows: dataSource.getSelectedRows,
      getAllLoadedRows: dataSource.getAllLoadedRows,
    });

    setExportLoading(true);
    try {
      downloadAppTableExportFile({
        columns: exportableColumns,
        rows,
        format,
        mode,
        reportMeta: {
          ...reportMeta,
          rowCount: rows.length,
        },
        fileName,
      });
    } finally {
      startTransition(() => {
        setExportLoading(false);
      });
    }
  };

  const items = useMemo<AppDropdownItem[]>(() => {
    return formats.map((format) => {
      const formatSupported = SUPPORTED_FORMATS.has(format);
      const children = availableModes.map<AppDropdownItem>((mode) => {
        const noSelection = mode === "selectedRows" && selectedRowsCount === 0;
        return {
          key: buildItemKey(format, mode),
          label: noSelection ? `${MODE_LABELS[mode]} (sin selección)` : MODE_LABELS[mode],
          disabled: exportLoading || !formatSupported || noSelection,
          onSelect: () => {
            void handleExport(format, mode);
          },
        };
      });

      return {
        key: format,
        label: formatSupported
          ? `Exportar en ${FORMAT_LABELS[format]}`
          : `Exportar en ${FORMAT_LABELS[format]} (próximamente)`,
        disabled: exportLoading || !formatSupported || children.length === 0,
        children,
      };
    });
  }, [availableModes, exportLoading, formats, selectedRowsCount]);

  return (
    <AppDropdown
      ariaLabel={typeof triggerLabel === "string" ? triggerLabel : "Exportar"}
      disabled={disabled || exportableColumns.length === 0 || items.length === 0}
      items={items}
      trigger={
        <AppButton
          aria-label={typeof triggerLabel === "string" ? triggerLabel : "Exportar"}
          variant="ghost"
          size="sm"
          loading={exportLoading}
          icon={EXPORT_TRIGGER_ICON}
          className={styles.triggerButton}
        />
      }
    />
  );
}

export default AppTableExport;
