import styles from "../AppTable.module.css";

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

type AppTableGridSkeletonProps = {
  className?: string;
  resolvedLayoutMode: "content" | "fill";
};

export function AppTableGridSkeleton({
  className,
  resolvedLayoutMode,
}: AppTableGridSkeletonProps) {
  return (
    <div
      className={joinClasses(
        styles.root,
        resolvedLayoutMode === "fill" && styles.rootFill,
        className,
      )}
      data-layout-mode={resolvedLayoutMode}
      data-presentation-mode="table"
      data-typography="inbox"
      data-loading-mode="skeleton"
    >
      <div
        className={joinClasses(
          styles.skeletonRoot,
          resolvedLayoutMode === "fill" && styles.skeletonRootFill,
        )}
        data-testid="app-table-grid-skeleton"
      >
        <div className={styles.skeletonTable}>
          <div className={styles.skeletonHeaderRow}>
            {Array.from({ length: 5 }).map((_, index) => (
              <span key={`header-${index}`} className={styles.skeletonHeaderCell} />
            ))}
          </div>
          <div className={styles.skeletonBody}>
            {Array.from({ length: 6 }).map((_, rowIndex) => (
              <div key={`row-${rowIndex}`} className={styles.skeletonBodyRow}>
                {Array.from({ length: 5 }).map((_, cellIndex) => (
                  <span
                    key={`cell-${rowIndex}-${cellIndex}`}
                    className={styles.skeletonBodyCell}
                  />
                ))}
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
