import { describe, expect, it } from "vitest";
import {
  mapDynamicUiColumnsToAppGridColumns,
  mapDynamicUiTableToAppDataTableAgGrid,
} from "../adapters/dynamicUiToAgGridColumns";
import type { DynamicUiTableDto } from "../types/dynamicUiTable.types";

describe("[SPEC:CREATE-COTRATO-AG-GRID-FASE-2] dynamicUiToAgGridColumns", () => {
  it("prioriza DataIndex sobre Field, ColumnName y Key", () => {
    const table: DynamicUiTableDto = {
      Columns: [
        {
          DataIndex: "codigoInterno",
          Field: "codigoField",
          ColumnName: "codigoColumn",
          Key: "codigoKey",
          Title: "Codigo",
        },
      ],
    };

    const result = mapDynamicUiColumnsToAppGridColumns(table);

    expect(result).toEqual([
      expect.objectContaining({
        field: "codigoInterno",
        headerName: "Codigo",
      }),
    ]);
  });

  it("omite columnas ocultas", () => {
    const table: DynamicUiTableDto = {
      Columns: [
        { DataIndex: "visible", Title: "Visible", Visible: true },
        { DataIndex: "oculta", Title: "Oculta", Visible: false },
      ],
    };

    const result = mapDynamicUiColumnsToAppGridColumns(table);

    expect(result).toHaveLength(1);
    expect(result[0]?.field).toBe("visible");
  });

  it("asocia CellActions por ColumnKey en columnas de accion", () => {
    const table: DynamicUiTableDto = {
      Columns: [
        {
          ColumnKey: "acciones",
          Title: "Acciones",
          IsActionColumn: true,
        },
      ],
      CellActions: [
        {
          ColumnKey: "acciones",
          Action: {
            ActionId: "edit",
            Label: "Editar",
            Behavior: "open-modal",
            Presentation: "button",
          },
        },
      ],
    };

    const result = mapDynamicUiColumnsToAppGridColumns(table);

    expect(result[0]).toEqual(
      expect.objectContaining({
        isActionColumn: true,
        actions: [
          expect.objectContaining({
            actionId: "edit",
            label: "Editar",
            behavior: "open-modal",
          }),
        ],
      }),
    );
  });

  it("usa RowActions cuando no existe CellActions para la columna de accion", () => {
    const table: DynamicUiTableDto = {
      Columns: [
        {
          ColumnKey: "row-actions",
          Title: "Acciones",
          IsActionColumn: true,
        },
      ],
      RowActions: [
        {
          ActionId: "view",
          Label: "Ver detalle",
          Behavior: "navigate",
          Presentation: "link",
        },
      ],
    };

    const result = mapDynamicUiColumnsToAppGridColumns(table);

    expect(result[0]?.actions).toEqual([
      expect.objectContaining({
        actionId: "view",
        presentation: "link",
      }),
    ]);
  });

  it("preserva orden y metadata de filtros del backend real", () => {
    const table: DynamicUiTableDto = {
      Columns: [
        {
          DataIndex: "segunda",
          Title: "Segunda",
          Order: 2,
          FilterType: "date",
          AgGridFilterType: "agDateColumnFilter",
        },
        {
          DataIndex: "primera",
          Title: "Primera",
          Order: 1,
          FilterType: "text",
          AgGridFilterType: "agTextColumnFilter",
        },
      ],
    };

    const result = mapDynamicUiColumnsToAppGridColumns(table);

    expect(result.map((column) => column.field)).toEqual(["primera", "segunda"]);
    expect(result[0]).toEqual(
      expect.objectContaining({
        filterType: "text",
        agGridFilterType: "agTextColumnFilter",
      }),
    );
  });

  it("ensambla el modelo completo AppDataTableAgGrid desde el payload real", () => {
    const table: DynamicUiTableDto = {
      TableId: "workflowInboxgestion",
      Title: "workflowInboxgestion",
      Columns: [
        {
          Key: "RADICADO",
          DataIndex: "RADICADO",
          HeaderName: "Radicado",
          Visible: true,
          Sortable: true,
          Order: 2,
          Width: 220,
          Align: "left",
          Filterable: true,
          FilterType: "text",
          AgGridFilterType: "agTextColumnFilter",
        },
        {
          Key: "acciones",
          DataIndex: "acciones",
          HeaderName: "Acciones",
          Visible: true,
          Sortable: false,
          Order: 9003,
          Width: 180,
          Align: "center",
          IsActionColumn: true,
          Filterable: false,
          RenderType: "grid_actions",
        },
      ],
      Rows: [
        {
          Id: "924",
          Values: {
            id_tarea: 924,
            RADICADO: "2500456700023",
            id: 924,
          },
          Meta: null,
        },
      ],
      CellActions: [
        {
          ColumnKey: "acciones",
          Action: {
            ActionId: "gestionar_tramite",
            Label: "Gestionar tramite",
            Placement: "row",
            Presentation: "icon_button",
            Behavior: "client_event",
            Request: {
              RowIdField: "id_tarea",
            },
          },
        },
      ],
      Pagination: {
        Page: 1,
        PageSize: 25,
        Total: 7,
      },
      Sorting: {
        SortField: "fecha_inicio",
        SortDir: "DESC",
      },
      meta: {
        source: "backend",
      },
      UserClaims: [],
    };

    const result = mapDynamicUiTableToAppDataTableAgGrid(table);

    expect(result).toEqual(
      expect.objectContaining({
        tableId: "workflowInboxgestion",
        title: "workflowInboxgestion",
        rows: [
          {
            id: "924",
            data: {
              id_tarea: 924,
              RADICADO: "2500456700023",
              id: 924,
            },
            meta: undefined,
          },
        ],
        pagination: {
          Page: 1,
          PageSize: 25,
          Total: 7,
        },
        sorting: {
          SortField: "fecha_inicio",
          SortDir: "DESC",
        },
        metadata: {
          source: "backend",
        },
      }),
    );

    expect(result.columns[1]).toEqual(
      expect.objectContaining({
        field: "acciones",
        isActionColumn: true,
        actions: [
          expect.objectContaining({
            actionId: "gestionar_tramite",
            presentation: "icon_button",
          }),
        ],
      }),
    );
  });
});
