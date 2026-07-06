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
    const onClientEvent = () => undefined;
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
      onClientEvent,
    });

    expect(actionColumn.sortable).toBe(false);
    expect(actionColumn.filter).toBe(false);
    expect(actionColumn.cellClass).toBe("app-table-action-cell");
    expect(actionColumn.cellStyle).toEqual(
      expect.objectContaining({
        alignItems: "center",
        display: "flex",
        height: "100%",
        justifyContent: "center",
        textAlign: "center",
      }),
    );
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
      onClientEvent,
      suppressMouseEventHandling: expect.any(Function),
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

  it("formats date columns from dynamic ISO values", () => {
    const [dateColumn, dateTimeColumn] = mapAppGridColumnsToAppTableColumns([
      {
        field: "FECHAVENCIMIENTO",
        headerName: "Fecha vencimiento",
        visible: true,
        sortable: true,
        filterable: true,
        dataType: "date",
        renderType: "grid_datetime",
      },
      {
        field: "FECHARADICADO",
        headerName: "Fecha radicado",
        visible: true,
        sortable: true,
        filterable: true,
        dataType: "datetime",
        renderType: "grid_datetime",
      },
    ]);

    const dateFormatter = dateColumn?.valueFormatter;
    const dateTimeFormatter = dateTimeColumn?.valueFormatter;

    expect(typeof dateFormatter).toBe("function");
    expect(typeof dateTimeFormatter).toBe("function");

    if (typeof dateFormatter !== "function" || typeof dateTimeFormatter !== "function") {
      throw new Error("Expected date value formatters");
    }

    expect(dateFormatter({ value: "2025-04-08T00:00:00" } as never)).toBe("08/04/2025");
    expect(dateTimeFormatter({ value: "2025-04-08T13:45:10" } as never)).toBe(
      "08/04/2025 13:45",
    );
  });

  it("[SPEC:APPTREETABLE-225-001] scopes Workbench table to two columns and applies sizing preset", () => {
    const result = mapAppGridColumnsToAppTableColumns(
      [
        {
          field: "ID",
          headerName: "ID",
          visible: true,
          sortable: true,
          filterable: true,
        },
        {
          field: "PAG",
          headerName: "PAG",
          visible: true,
          sortable: true,
          filterable: true,
        },
        {
          field: "TIPODOCUMENTO",
          headerName: "Documento",
          visible: true,
          sortable: true,
          filterable: true,
        },
        {
          field: "acciones",
          headerName: "Acciones",
          visible: true,
          sortable: true,
          filterable: true,
          isActionColumn: true,
          actions: [
            {
              actionId: "ver_documento",
              label: "Ver",
              placement: "row",
              presentation: "icon_button",
              behavior: "client_event",
            },
          ],
        },
      ],
      {
        tableId: "InboxListaDocumentosRadicado",
        userClaims: [],
        menuActions: [],
        onClientEvent: () => undefined,
      },
    );

    expect(result).toHaveLength(2);
    expect(result[0]).toEqual(
      expect.objectContaining({
        field: "TIPODOCUMENTO",
        flex: 2,
        minWidth: 60,
      }),
    );
    expect(result[1]).toEqual(
      expect.objectContaining({
        field: "acciones",
        flex: 1,
        minWidth: 80,
      }),
    );
  });
});
