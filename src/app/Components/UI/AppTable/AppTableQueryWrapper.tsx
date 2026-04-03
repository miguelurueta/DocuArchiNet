import { LeftOutlined, ReloadOutlined, RightOutlined } from "@ant-design/icons";
import type { ReactNode } from "react";
import { AppIconActionButton } from "../AppButton";
import { AppInput } from "../AppInput";
import type { AppTableQueryState } from "./types/appTableQueryState.types";
import styles from "./AppTableQueryWrapper.module.css";

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const DEFAULT_PAGE_SIZE_OPTIONS = [10, 25, 50, 100] as const;

export type AppTableQueryWrapperProps = {
  queryState: AppTableQueryState;
  onQueryChange: (patch: Partial<AppTableQueryState>) => void;
  onRefresh?: () => void;
  total: number;
  loading?: boolean;
  headerActions?: ReactNode;
  children: ReactNode;
  className?: string;
  pageSizeOptions?: number[];
  searchPlaceholder?: string;
};

const getVisibleRange = (page: number, pageSize: number, total: number) => {
  if (total <= 0) {
    return "0-0 de 0";
  }

  const safePage = Number.isFinite(page) && page > 0 ? page : 1;
  const safePageSize = Number.isFinite(pageSize) && pageSize > 0 ? pageSize : 1;
  const start = (safePage - 1) * safePageSize + 1;
  const end = Math.min(start + safePageSize - 1, total);

  return `${start}-${end} de ${total}`;
};

export function AppTableQueryWrapper({
  queryState,
  onQueryChange,
  onRefresh,
  total,
  loading = false,
  headerActions,
  children,
  className,
  pageSizeOptions = [...DEFAULT_PAGE_SIZE_OPTIONS],
  searchPlaceholder = "Buscar en la tabla",
}: AppTableQueryWrapperProps) {
  const totalPages =
    total > 0 ? Math.max(1, Math.ceil(total / Math.max(queryState.pageSize, 1))) : 1;
  const canGoPrevious = queryState.page > 1;
  const canGoNext = queryState.page < totalPages;
  const pageSizeValue = pageSizeOptions.includes(queryState.pageSize)
    ? queryState.pageSize
    : undefined;

  return (
    <section className={joinClasses(styles.root, className)} data-testid="app-table-query-wrapper">
      <div className={styles.header}>
        <div className={styles.searchGroup}>
          <AppInput
            className={styles.searchInput}
            placeholder={searchPlaceholder}
            value={queryState.search}
            onChange={(event) => onQueryChange({ search: event.target.value })}
            aria-label="Buscar en la tabla"
          />
          {onRefresh ? (
            <AppIconActionButton
              icon={<ReloadOutlined />}
              aria-label="Actualizar tabla"
              tooltip="Actualizar tabla"
              onClick={onRefresh}
              loading={loading}
            />
          ) : null}
        </div>

        {headerActions ? <div className={styles.headerActions}>{headerActions}</div> : null}
      </div>

      <div className={styles.tableContainer}>{children}</div>

      <div className={styles.pagination}>
        <span className={styles.range} data-testid="app-table-query-range">
          {getVisibleRange(queryState.page, queryState.pageSize, total)}
        </span>

        <div className={styles.paginationActions}>
          <AppInput
            type="select"
            className={styles.pageSizeControl}
            aria-label="Cantidad de registros por página"
            placeholder="Tamaño"
            value={pageSizeValue}
            options={pageSizeOptions.map((option) => ({
              label: `${option} por página`,
              value: option,
            }))}
            onChange={(value) => {
              if (typeof value === "number") {
                onQueryChange({ pageSize: value });
              }
            }}
          />

          <div className={styles.navButtons}>
            <AppIconActionButton
              icon={<LeftOutlined />}
              aria-label="Página anterior"
              tooltip="Página anterior"
              disabled={!canGoPrevious || loading}
              onClick={() => {
                if (canGoPrevious) {
                  onQueryChange({ page: queryState.page - 1 });
                }
              }}
            />
            <AppIconActionButton
              icon={<RightOutlined />}
              aria-label="Página siguiente"
              tooltip="Página siguiente"
              disabled={!canGoNext || loading}
              onClick={() => {
                if (canGoNext) {
                  onQueryChange({ page: queryState.page + 1 });
                }
              }}
            />
          </div>
        </div>
      </div>
    </section>
  );
}
