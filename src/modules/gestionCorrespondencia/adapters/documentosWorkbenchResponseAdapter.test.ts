import { describe, expect, it } from "vitest";
import {
  adaptListaDocumentosRadicadosToWorkbenchModel,
  resolveDocumentWorkbenchRowId,
  resolveListaDocumentosRadicadosTotal,
} from "./documentosWorkbenchResponseAdapter";
import type { ListaDocumentosRadicadosQueryData } from "../types/listaDocumentosRadicados.types";

describe("[SPEC:APPTREETABLE-217] documentosWorkbenchResponseAdapter", () => {
  const dynamicTable = {
    TableId: "InboxListaDocumentosRadicado",
    Columns: [
      { ColumnKey: "TIPODOCUMENTO", DataIndex: "TIPODOCUMENTO", HeaderName: "Tipo documento" },
      {
        ColumnKey: "ACCIONES",
        DataIndex: "ACCIONES",
        HeaderName: "Acciones",
        IsActionColumn: true,
        RenderType: "actions",
      },
    ],
    RowActions: [],
    MenuActions: [
      {
        ActionId: "ver_documento",
        Label: "Ver documento",
        Behavior: "client_event",
        Presentation: "menu_item",
      },
    ],
    CellActions: [
      {
        ColumnKey: "ACCIONES",
        Action: {
          ActionId: "acciones_menu",
          Label: "Acciones",
          Behavior: "client_event",
          Presentation: "icon_button",
          BehaviorConfig: { menuItems: ["ver_documento"] },
        },
      },
    ],
  };

  it("mapea Rows a AppTreeTableRow preservando Meta y HasChildren", () => {
    const data: ListaDocumentosRadicadosQueryData = {
      Rows: [
        {
          RowId: "r1",
          Values: { ID: 1, NOMBRE: "Doc 1" },
          Meta: {
            NodeType: "documento",
            ParentId: null,
            HasChildren: true,
            DocumentId: 99,
            NombreGabinete: "GAB",
          },
        },
      ],
      Config: null,
      Columns: null,
    };

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "hierarchical" });
    expect(model.rows).toHaveLength(1);
    expect(model.rows[0].id).toBe("r1");
    expect(model.rows[0].hasChildren).toBe(true);
    expect(model.rows[0].children).toEqual([]);
    expect(model.rows[0].values?.NOMBRE).toBe("Doc 1");
    expect(model.rows[0].meta).toMatchObject({ NodeType: "documento", DocumentId: 99, NombreGabinete: "GAB" });
  });

  it("prioriza TIPODOCUMENTO como label en flatDocuments y limita columnas", () => {
    const data: ListaDocumentosRadicadosQueryData = {
      Rows: [
        {
          RowId: "r1",
          Values: { ID: 15416, TIPODOCUMENTO: "DOC 15416", PAG: 10 },
          Meta: { NodeType: "documento", HasChildren: false, DocumentId: 15416, NombreGabinete: "WF_DOCS" },
        },
      ],
      Columns: ["ID", "TIPODOCUMENTO", "PAG"],
      Config: null,
    };

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "flatDocuments" });
    expect(model.rows[0].label).toBe("DOC 15416");
    expect(model.columns).toEqual(["TIPODOCUMENTO"]);
  });

  it("expone tableId cuando viene Config (Dynamic UI)", () => {
    const data: ListaDocumentosRadicadosQueryData = {
      Rows: [],
      Columns: null,
      Config: dynamicTable,
    } as unknown as ListaDocumentosRadicadosQueryData;

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "flatDocuments" });
    expect(model.tableId).toBe("InboxListaDocumentosRadicado");
    expect(model.tableColumns?.length).toBeGreaterThan(0);
  });

  it("mapea tableId cuando la tabla viene en data directo", () => {
    const data = {
      Rows: [],
      Columns: dynamicTable.Columns,
      TableId: "InboxListaDocumentosRadicado",
      RowActions: dynamicTable.RowActions,
      MenuActions: dynamicTable.MenuActions,
      CellActions: dynamicTable.CellActions,
    } as unknown as ListaDocumentosRadicadosQueryData;

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "flatDocuments" });
    expect(model.tableId).toBe("InboxListaDocumentosRadicado");
    expect(model.tableColumns?.length).toBeGreaterThan(0);
  });

  it("en flatDocuments conserva columna principal y acciones de CellActions/MenuActions", () => {
    const data: ListaDocumentosRadicadosQueryData = {
      Rows: [
        {
          RowId: "r-action",
          Values: { TIPODOCUMENTO: "DOC 2001" },
          Meta: { NodeType: "documento", HasChildren: false },
        },
      ],
      Columns: ["TIPODOCUMENTO"],
      Config: dynamicTable,
    } as unknown as ListaDocumentosRadicadosQueryData;

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "flatDocuments" });
    expect(model.columns).toEqual(["TIPODOCUMENTO"]);

    const actionColumn = model.tableColumns?.find((column) => column.field === "ACCIONES");
    const rendererParams = actionColumn?.cellRendererParams as
      | { actions?: unknown[]; menuActions?: unknown[] }
      | undefined;

    expect(rendererParams?.actions?.length).toBeGreaterThan(0);
    expect(rendererParams?.menuActions?.length).toBeGreaterThan(0);
  });

  it("fuerza acciones dynamic UI a client_event para que el Workbench ejecute su endpoint propio", () => {
    const data: ListaDocumentosRadicadosQueryData = {
      Rows: [
        {
          RowId: "r-action",
          Values: { TIPODOCUMENTO: "DOC 2001" },
          Meta: { NodeType: "documento", HasChildren: false },
        },
      ],
      Columns: ["TIPODOCUMENTO"],
      Config: {
        ...dynamicTable,
        MenuActions: [
          {
            ActionId: "eliminar_item",
            Label: "Eliminar",
            Behavior: "api_call",
            Presentation: "menu_item",
          },
        ],
      },
    } as unknown as ListaDocumentosRadicadosQueryData;

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "flatDocuments" });
    const actionColumn = model.tableColumns?.find((column) => column.field === "ACCIONES");
    const rendererParams = actionColumn?.cellRendererParams as
      | {
          actions?: Array<{ behavior?: string }>;
          menuActions?: Array<{ actionId?: string; behavior?: string }>;
        }
      | undefined;

    expect(rendererParams?.actions?.every((action) => action.behavior === "client_event")).toBe(true);
    expect(rendererParams?.menuActions?.find((action) => action.actionId === "eliminar_item")?.behavior).toBe(
      "client_event",
    );
  });

  it("[SPEC:APPTREETABLE-225-001] en Workbench limita Dynamic UI a 2 columnas y aplica sizing preset", () => {
    const dynamicTableWithLegacy = {
      ...dynamicTable,
      Columns: [
        { ColumnKey: "PAG", DataIndex: "PAG", HeaderName: "PAG" },
        ...(dynamicTable.Columns ?? []),
      ],
    };

    const data: ListaDocumentosRadicadosQueryData = {
      Rows: [
        {
          RowId: "r-action",
          Values: { TIPODOCUMENTO: "DOC 2001", PAG: 10 },
          Meta: { NodeType: "documento", HasChildren: false },
        },
      ],
      Columns: ["TIPODOCUMENTO", "PAG"],
      Config: dynamicTableWithLegacy as unknown as ListaDocumentosRadicadosQueryData["Config"],
    } as unknown as ListaDocumentosRadicadosQueryData;

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "flatDocuments" });
    expect(model.tableId).toBe("InboxListaDocumentosRadicado");
    expect(model.tableColumns).toBeDefined();
    expect(model.tableColumns).toHaveLength(2);

    expect(model.tableColumns?.[0]).toEqual(
      expect.objectContaining({
        field: "TIPODOCUMENTO",
        flex: 2,
        minWidth: 60,
      }),
    );

    expect(model.tableColumns?.[1]).toEqual(
      expect.objectContaining({
        field: "ACCIONES",
        flex: 1,
        minWidth: 80,
      }),
    );
  });

  it("normaliza row id cuando RowId no viene y evita undefined", () => {
    const rowWithoutId = {
      RowId: "" as unknown as string,
      Values: { IdDocumento: 15416, TIPODOCUMENTO: "DOC 15416" },
      Meta: { NodeType: "documento", HasChildren: false },
    };

    expect(resolveDocumentWorkbenchRowId(rowWithoutId, 0)).toBe("15416");

    const data = {
      Rows: [rowWithoutId],
      Config: null,
      Columns: ["TIPODOCUMENTO"],
    } as unknown as ListaDocumentosRadicadosQueryData;

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "flatDocuments" });
    expect(model.rows[0].id).toBe("15416");
  });

  it("preserva pagination y total cuando el backend lo entrega", () => {
    const data: ListaDocumentosRadicadosQueryData = {
      Rows: [
        {
          RowId: "r1",
          Values: { TIPODOCUMENTO: "DOC 1" },
          Meta: { NodeType: "documento", HasChildren: false },
        },
      ],
      Columns: ["TIPODOCUMENTO"],
      Config: null,
      pagination: {
        page: 3,
        pageSize: 25,
        total: 77,
      },
    };

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "flatDocuments" });
    expect(model.pagination).toEqual({ page: 3, pageSize: 25, total: 77 });
    expect(model.total).toBe(77);
    expect(resolveListaDocumentosRadicadosTotal(data)).toBe(77);
  });

  it("resuelve total desde Pagination si viene en mayuscula", () => {
    const data = {
      Rows: [],
      Columns: null,
      Config: null,
      Pagination: { Page: 2, PageSize: 10, Total: 12 },
    } as unknown as ListaDocumentosRadicadosQueryData;

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data, { viewMode: "flatDocuments" });
    expect(model.pagination).toEqual({ page: 2, pageSize: 10, total: 12 });
    expect(resolveListaDocumentosRadicadosTotal(data)).toBe(12);
  });
});
