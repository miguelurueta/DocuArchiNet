import styles from "../AppTable.module.css";

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

type AppTableCardSkeletonProps = {
  className?: string;
  resolvedLayoutMode: "content" | "fill";
};

export function AppTableCardSkeleton({
  className,
  resolvedLayoutMode,
}: AppTableCardSkeletonProps) {
  return (
    <div
      className={joinClasses(
        styles.root,
        resolvedLayoutMode === "fill" && styles.rootFill,
        className,
      )}
      data-layout-mode={resolvedLayoutMode}
      data-presentation-mode="cards"
      data-typography="inbox"
      data-loading-mode="skeleton"
    >
      <div
        className={joinClasses(
          styles.cards,
          resolvedLayoutMode === "fill" && styles.cardsFill,
        )}
        data-testid="app-table-card-skeleton"
      >
        {Array.from({ length: 3 }).map((_, index) => (
          <article key={`card-${index}`} className={styles.card}>
            <div className={styles.cardBody}>
              {Array.from({ length: 3 }).map((__, fieldIndex) => (
                <div key={`field-${index}-${fieldIndex}`} className={styles.cardField}>
                  <span className={styles.skeletonCardLabel} />
                  <span className={styles.skeletonCardValue} />
                </div>
              ))}
            </div>
            <div className={styles.cardActions}>
              <span className={styles.skeletonCardAction} />
            </div>
          </article>
        ))}
      </div>
    </div>
  );
}
