import type { AppTableLayoutMode, AppTablePresentationMode, AppTableProps, AppTableRow } from "./AppTable.types";
import { AppTableCardRenderer } from "./renderers/AppTableCardRenderer";
import { AppTableGridRenderer } from "./renderers/AppTableGridRenderer";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";

const resolveLayoutMode = (
  layoutMode: AppTableLayoutMode | undefined,
  domLayout: AppTableProps<AppTableRow>["domLayout"] | undefined,
): AppTableLayoutMode => {
  if (layoutMode) {
    return layoutMode;
  }

  return domLayout === "normal" ? "fill" : "content";
};

const resolvePresentationMode = (
  presentationMode: AppTablePresentationMode | undefined,
): AppTablePresentationMode => presentationMode ?? "table";

export default function AppTable<T extends AppTableRow>(props: AppTableProps<T>) {
  const resolvedLayoutMode = resolveLayoutMode(
    props.layoutMode,
    props.domLayout as AppTableProps<AppTableRow>["domLayout"],
  );
  const resolvedPresentationMode = resolvePresentationMode(props.presentationMode);

  if (resolvedPresentationMode === "cards") {
    return (
      <AppTableCardRenderer
        rows={props.rows}
        columns={props.columns}
        cardFields={props.cardFields}
        loading={props.loading}
        total={props.total}
        className={props.className}
        onRowClicked={props.onRowClicked}
        resolvedLayoutMode={resolvedLayoutMode}
      />
    );
  }

  return <AppTableGridRenderer {...props} resolvedLayoutMode={resolvedLayoutMode} />;
}
