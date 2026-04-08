import { useEffect, useRef, useState } from "react";
import type {
  AppTableLayoutMode,
  AppTablePresentationMode,
  AppTableProps,
  AppTableResponsivePresentation,
  AppTableRow,
} from "./AppTable.types";
import { AppTableCardRenderer } from "./renderers/AppTableCardRenderer";
import { AppTableCardSkeleton } from "./renderers/AppTableCardSkeleton";
import { AppTableGridRenderer } from "./renderers/AppTableGridRenderer";
import { AppTableGridSkeleton } from "./renderers/AppTableGridSkeleton";
import styles from "./AppTable.module.css";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";

const DEFAULT_CARDS_BELOW = 768;
const DEFAULT_LOADING_MODE = "skeleton";

const resolveLayoutMode = (
  layoutMode: AppTableLayoutMode | undefined,
  domLayout: AppTableProps<AppTableRow>["domLayout"] | undefined,
): AppTableLayoutMode => {
  if (layoutMode) {
    return layoutMode;
  }

  return domLayout === "normal" ? "fill" : "content";
};

const resolveResponsivePresentation = (
  responsivePresentation: AppTableResponsivePresentation | undefined,
) => ({
  enabled: responsivePresentation?.enabled === true,
  cardsBelow:
    typeof responsivePresentation?.cardsBelow === "number" &&
    Number.isFinite(responsivePresentation.cardsBelow)
      ? responsivePresentation.cardsBelow
      : DEFAULT_CARDS_BELOW,
});

const resolvePresentationMode = (
  explicitPresentationMode: AppTablePresentationMode | undefined,
  responsivePresentation: ReturnType<typeof resolveResponsivePresentation>,
  containerWidth: number | null,
): AppTablePresentationMode => {
  if (explicitPresentationMode) {
    return explicitPresentationMode;
  }

  if (
    responsivePresentation.enabled &&
    typeof containerWidth === "number" &&
    containerWidth < responsivePresentation.cardsBelow
  ) {
    return "cards";
  }

  return "table";
};

export default function AppTable<T extends AppTableRow>(props: AppTableProps<T>) {
  const hostRef = useRef<HTMLDivElement | null>(null);
  const [containerWidth, setContainerWidth] = useState<number | null>(null);

  useEffect(() => {
    if (!hostRef.current || typeof ResizeObserver === "undefined") {
      return undefined;
    }

    const element = hostRef.current;
    const observer = new ResizeObserver((entries) => {
      const entry = entries[0];
      const nextWidth = entry?.contentRect?.width;
      if (typeof nextWidth === "number" && Number.isFinite(nextWidth)) {
        setContainerWidth(nextWidth);
      }
    });

    observer.observe(element);
    setContainerWidth(element.getBoundingClientRect().width || null);

    return () => {
      observer.disconnect();
    };
  }, []);

  const resolvedLayoutMode = resolveLayoutMode(
    props.layoutMode,
    props.domLayout as AppTableProps<AppTableRow>["domLayout"],
  );
  const responsivePresentation = resolveResponsivePresentation(props.responsivePresentation);
  const resolvedPresentationMode = resolvePresentationMode(
    props.presentationMode,
    responsivePresentation,
    containerWidth,
  );
  const resolvedLoadingMode = props.loadingMode ?? DEFAULT_LOADING_MODE;
  const hasRenderableRows = props.rows.length > 0;
  const shouldRenderSkeleton =
    resolvedLoadingMode === "skeleton" && props.loading === true && !hasRenderableRows;

  return (
    <div
      ref={hostRef}
      className={styles.host}
      data-responsive-enabled={responsivePresentation.enabled ? "true" : "false"}
      data-responsive-width={containerWidth ?? undefined}
    >
      {shouldRenderSkeleton ? (
        resolvedPresentationMode === "cards" ? (
          <AppTableCardSkeleton
            className={props.className}
            resolvedLayoutMode={resolvedLayoutMode}
          />
        ) : (
          <AppTableGridSkeleton
            className={props.className}
            resolvedLayoutMode={resolvedLayoutMode}
          />
        )
      ) : resolvedPresentationMode === "cards" ? (
        <AppTableCardRenderer
          rows={props.rows}
          columns={props.columns}
          cardFields={props.cardFields}
          loading={props.loading}
          total={props.total}
          className={props.className}
          onRowClicked={props.onRowClicked}
          onActionTriggered={props.onActionTriggered}
          resolvedLayoutMode={resolvedLayoutMode}
        />
      ) : (
        <AppTableGridRenderer {...props} resolvedLayoutMode={resolvedLayoutMode} />
      )}
    </div>
  );
}
