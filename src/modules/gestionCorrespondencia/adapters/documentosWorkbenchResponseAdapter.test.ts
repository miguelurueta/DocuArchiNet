import { describe, expect, it } from "vitest";
import { adaptListaDocumentosRadicadosToWorkbenchModel } from "./documentosWorkbenchResponseAdapter";
import type { ListaDocumentosRadicadosQueryData } from "../types/listaDocumentosRadicados.types";

describe("[SPEC:APPTREETABLE-217] documentosWorkbenchResponseAdapter", () => {
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

    const model = adaptListaDocumentosRadicadosToWorkbenchModel(data);
    expect(model.rows).toHaveLength(1);
    expect(model.rows[0].id).toBe("r1");
    expect(model.rows[0].hasChildren).toBe(true);
    expect(model.rows[0].children).toEqual([]);
    expect(model.rows[0].values?.NOMBRE).toBe("Doc 1");
    expect(model.rows[0].meta).toMatchObject({ NodeType: "documento", DocumentId: 99, NombreGabinete: "GAB" });
  });
});

