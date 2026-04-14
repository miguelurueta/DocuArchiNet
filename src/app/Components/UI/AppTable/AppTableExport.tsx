import { DownloadOutlined } from "@ant-design/icons";
import { useMemo, useState, startTransition } from "react";
import { AppButton } from "../AppButton";
import { AppDropdown, type AppDropdownItem } from "../AppDropdown";
import type { AppTableExportFormat, AppTableExportMode, AppTableExportProps } from "./AppTableExport.types";
import {
  getAvailableAppTableExportModes,
  isAppTableExportExecutable,
  shouldUseBackendAppTableExport,
} from "./AppTableExport.types";
import {
  buildAppTableExportFileName,
  downloadBlobAppTableExportFile,
  downloadAppTableExportFile,
  getAppTableExportRows,
  getAppTableExportableColumns,
} from "./AppTableExport.utils";
import type { AppTableRow } from "./AppTable.types";
import styles from "./AppTableExport.module.css";

const EXECUTABLE_EXPORT_MODES = [
  "currentPage",
  "selectedRows",
  "allLoaded",
  "allMatching",
] as const satisfies readonly AppTableExportMode[];

const MODE_LABELS: Record<(typeof EXECUTABLE_EXPORT_MODES)[number], string> = {
  currentPage: "Página actual",
  selectedRows: "Seleccionados",
  allLoaded: "Todos los cargados",
  allMatching: "Todos los resultados",
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
  enabledModes = [...EXECUTABLE_EXPORT_MODES],
  fileName,
  triggerLabel = "Exportar",
  disabled = false,
}: AppTableExportProps<T>) {
  const [exportLoading, setExportLoading] = useState(false);

  const exportableColumns = useMemo(() => getAppTableExportableColumns(columns), [columns]);
  const availableModes = useMemo(
    () => getAvailableAppTableExportModes(dataSource, enabledModes).filter((mode) =>
      EXECUTABLE_EXPORT_MODES.includes(mode as (typeof EXECUTABLE_EXPORT_MODES)[number]),
    ) as Array<(typeof EXECUTABLE_EXPORT_MODES)[number]>,
    [dataSource, enabledModes],
  );

  const selectedRows = useMemo(
    () => (typeof dataSource.getSelectedRows === "function" ? dataSource.getSelectedRows() : []),
    [dataSource],
  );

  const selectedRowsCount = selectedRows.length;

  const handleExport = async (
    format: AppTableExportFormat,
    mode: (typeof EXECUTABLE_EXPORT_MODES)[number],
  ) => {
    if (exportLoading || disabled || !isAppTableExportExecutable(dataSource, format, mode)) {
      return;
    }

    if (mode === "selectedRows" && selectedRowsCount === 0) {
      return;
    }

    setExportLoading(true);
    try {
      if (shouldUseBackendAppTableExport(dataSource, format, mode)) {
        const backendFile = await dataSource.getBackendExportFile?.({
          columns: exportableColumns,
          format,
          mode,
          reportMeta,
          fileName,
        });

        if (!backendFile) {
          throw new Error("Backend export returned no file");
        }

        downloadBlobAppTableExportFile({
          ...backendFile,
          fileName:
            backendFile.fileName ??
            buildAppTableExportFileName({
              reportMeta,
              mode,
              format,
              fileName,
            }),
        });
        return;
      }

      const rows =
        mode === "allMatching"
          ? (await dataSource.getAllMatchingRows?.()) ?? []
          : getAppTableExportRows({
              mode,
              getCurrentPageRows: dataSource.getCurrentPageRows,
              getSelectedRows: dataSource.getSelectedRows,
              getAllLoadedRows: dataSource.getAllLoadedRows,
            });

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
    } catch (error) {
      console.error("AppTable export failed", error);
    } finally {
      startTransition(() => {
        setExportLoading(false);
      });
    }
  };

  const items = useMemo<AppDropdownItem[]>(() => {
    return formats.map((format) => {
      const children = availableModes.map<AppDropdownItem>((mode) => {
        const noSelection = mode === "selectedRows" && selectedRowsCount === 0;
        const executable = isAppTableExportExecutable(dataSource, format, mode);
        return {
          key: buildItemKey(format, mode),
          label: noSelection ? `${MODE_LABELS[mode]} (sin selección)` : MODE_LABELS[mode],
          disabled: exportLoading || !executable || noSelection,
          onSelect: () => {
            void handleExport(format, mode);
          },
        };
      });
      const hasExecutableChildren = children.some((child) => !child.disabled);

      return {
        key: format,
        label: hasExecutableChildren
          ? `Exportar en ${FORMAT_LABELS[format]}`
          : `Exportar en ${FORMAT_LABELS[format]} (próximamente)`,
        disabled: exportLoading || children.length === 0 || !hasExecutableChildren,
        children,
      };
    });
  }, [availableModes, dataSource, exportLoading, formats, selectedRowsCount]);

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
