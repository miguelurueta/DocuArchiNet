import { describe, expect, it } from "vitest";
import { mapAppGridColumnsToAppTableColumns } from "../adapters/appGridToAppTableColumns";

describe("[SPEC:IMPLEMENTACION-LISTA-GESTION-CORRESPONDENCIA] appGridToAppTableColumns", () => {
  it("maps shared AppGrid columns to ColDef-compatible objects", () => {
    const result = mapAppGridColumnsToAppTableColumns([
      {
        field: "RADICADO",
        headerName: "Radicado",
        visible: true,
        sortable: true,
        filterable: true,
        width: 220,
        align: "left",
        agGridFilterType: "agTextColumnFilter",
      },
    ]);

    expect(result).toEqual([
      expect.objectContaining({
        field: "RADICADO",
        headerName: "Radicado",
        sortable: true,
        filter: "agTextColumnFilter",
        width: 220,
        hide: false,
      }),
    ]);
  });

  it("disables sort and filter for dynamic action columns without breaking AppTable", () => {
    const [actionColumn] = mapAppGridColumnsToAppTableColumns([
      {
        field: "acciones",
        headerName: "Acciones",
        visible: true,
        sortable: true,
        filterable: true,
        isActionColumn: true,
        actions: [
          {
            actionId: "gestionar_tramite",
            label: "Gestionar",
            placement: "row",
            presentation: "icon_button",
            behavior: "client_event",
          },
        ],
      },
    ], {
      tableId: "workflowInboxgestion",
      userClaims: ["tramites.gestionar"],
    });

    expect(actionColumn.sortable).toBe(false);
    expect(actionColumn.filter).toBe(false);
    expect(actionColumn.valueGetter?.({} as never)).toBe("");
    expect(actionColumn.cellRenderer).toBeDefined();
    expect(actionColumn.cellRendererParams).toEqual({
      appGridColumn: expect.objectContaining({
        field: "acciones",
      }),
      actions: [
        expect.objectContaining({
          actionId: "gestionar_tramite",
        }),
      ],
      tableId: "workflowInboxgestion",
      userClaims: ["tramites.gestionar"],
    });
  });
});
