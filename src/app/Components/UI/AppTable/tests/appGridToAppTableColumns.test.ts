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
        pinned: "left",
        lockPinned: true,
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
        pinned: "left",
        lockPinned: true,
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
      menuActions: [
        {
          actionId: "reasignar_tramite",
          label: "Reasignar trámite",
          placement: "row",
          presentation: "menu_item",
          behavior: "api_call",
        },
      ],
      tableId: "workflowInboxgestion",
      userClaims: ["tramites.gestionar"],
    });

    expect(actionColumn.sortable).toBe(false);
    expect(actionColumn.filter).toBe(false);
    expect(typeof actionColumn.valueGetter).toBe("function");
    expect((actionColumn.valueGetter as (params: never) => string)({} as never)).toBe("");
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
      menuActions: [
        expect.objectContaining({
          actionId: "reasignar_tramite",
        }),
      ],
      tableId: "workflowInboxgestion",
      userClaims: ["tramites.gestionar"],
    });
  });

  it("does not invent pinning defaults for dynamic columns without pinning metadata", () => {
    const [column] = mapAppGridColumnsToAppTableColumns([
      {
        field: "ASUNTO",
        headerName: "Asunto",
        visible: true,
        sortable: true,
        filterable: true,
      },
    ]);

    expect(column.pinned).toBeUndefined();
    expect(column.lockPinned).toBeUndefined();
  });
});
