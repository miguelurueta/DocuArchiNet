import { LeftOutlined, ReloadOutlined, RightOutlined } from "@ant-design/icons";
import type { ReactNode } from "react";
import { AppButton } from "../AppButton";
import { AppDropdown } from "../AppDropdown";
import { AppInputSearch } from "../AppInputSearch";
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
  paginationActions?: ReactNode;
  children: ReactNode;
  className?: string;
  pageSizeOptions?: number[];
  searchPlaceholder?: string;
  showSearch?: boolean;
  showPagination?: boolean;
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
  paginationActions,
  children,
  className,
  pageSizeOptions = [...DEFAULT_PAGE_SIZE_OPTIONS],
  searchPlaceholder = "Buscar en la tabla",
  showSearch = true,
  showPagination = true,
}: AppTableQueryWrapperProps) {
  const totalPages =
    total > 0
      ? Math.max(1, Math.ceil(total / Math.max(queryState.pageSize, 1)))
      : 1;
  const canGoPrevious = queryState.page > 1;
  const canGoNext = queryState.page < totalPages;
  const pageSizeLabel = `${queryState.pageSize} por página`;
  const pageSizeItems = pageSizeOptions.map((option) => ({
    key: String(option),
    label: `${option} por página`,
    onSelect: () => {
      onQueryChange({ pageSize: option });
    },
  }));

  return (
    <section
      className={joinClasses(styles.root, className)}
      data-testid="app-table-query-wrapper"
    >
      <div className={styles.header}>
        <div className={styles.searchGroup}>
          {showSearch ? (
            <AppInputSearch
              className={styles.searchInput}
              placeholder={searchPlaceholder}
              value={queryState.search}
              onChange={(search) => onQueryChange({ search })}
              aria-label="Buscar en la tabla"
            />
          ) : null}
          {onRefresh ? (
            <AppButton
              icon={<ReloadOutlined />}
              aria-label="Actualizar tabla"
              tooltip="Actualizar tabla"
              variant="ghost"
              size="md"
              onClick={onRefresh}
              loading={loading}
            />
          ) : null}
        </div>

        {headerActions ? (
          <div className={styles.headerActions}>{headerActions}</div>
        ) : null}
      </div>

      {showPagination ? (
        <div className={styles.controlsBand}>
          <div className={styles.paginationInfo}>
            <span className={styles.range} data-testid="app-table-query-range">
              {getVisibleRange(queryState.page, queryState.pageSize, total)}
            </span>

            <div className={styles.paginationActions}>
              <AppDropdown
                ariaLabel="Cantidad de registros por página"
                className={styles.pageSizeControl}
                items={pageSizeItems}
                trigger={
                  <AppButton
                    variant="primary"
                    size="sm"
                    className={styles.pageSizeTrigger}
                    aria-label="Cantidad de registros por página"
                  >
                    {pageSizeLabel}
                  </AppButton>
                }
              />

              <div className={styles.navButtons}>
                <AppButton
                  icon={<LeftOutlined />}
                  aria-label="Página anterior"
                  tooltip="Página anterior"
                  variant="ghost"
                  size="md"
                  className={styles.navButton}
                  disabled={!canGoPrevious || loading}
                  onClick={() => {
                    if (canGoPrevious) {
                      onQueryChange({ page: queryState.page - 1 });
                    }
                  }}
                />
                <AppButton
                  icon={<RightOutlined />}
                  aria-label="Página siguiente"
                  tooltip="Página siguiente"
                  variant="ghost"
                  size="md"
                  className={styles.navButton}
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

          {paginationActions ? (
            <div
              className={styles.paginationSideActions}
              data-testid="app-table-pagination-actions"
            >
              {paginationActions}
            </div>
          ) : null}
        </div>
      ) : null}

      <div className={styles.tableContainer}>{children}</div>
    </section>
  );
}
