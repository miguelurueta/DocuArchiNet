import { describe, expect, it } from "vitest";
import {
  buildListaDocumentosRadicadosChildrenQuery,
  buildListaDocumentosRadicadosRootQuery,
} from "./gestionRespuestaDocumentosRequestMapper";

describe("[SPEC:APPTREETABLE-217] gestionRespuestaDocumentosRequestMapper", () => {
  it("construye query root flatDocuments con IncludeConfig true y Parent null", () => {
    const request = buildListaDocumentosRadicadosRootQuery({
      idTareaWf: 123,
      nombreGabinete: "GAB-1",
      radicado: " 2025-0001 ",
    });

    expect(request.ViewMode).toBe("flatDocuments");
    expect(request.IncludeConfig).toBe(true);
    expect(request.ParentRowId).toBeNull();
    expect(request.ParentNodeType).toBeNull();
    expect(request.Level).toBe(1);
    expect(request.NombreGabinete).toBe("GAB-1");
    expect(request.CampoRadicado).toBe("ENLASE");
    expect(request.Radicado).toBe("2025-0001");
  });

  it("construye query children hierarchical con ParentRowId y ParentNodeType", () => {
    const request = buildListaDocumentosRadicadosChildrenQuery({
      nombreGabinete: "GAB-1",
      radicado: "2025-0002",
      parentRowId: "row-1",
      parentNodeType: "folder",
      level: 2,
    });

    expect(request.ViewMode).toBe("hierarchical");
    expect(request.ParentRowId).toBe("row-1");
    expect(request.ParentNodeType).toBe("folder");
    expect(request.Level).toBe(2);
    expect(request.NombreGabinete).toBe("GAB-1");
    expect(request.CampoRadicado).toBe("ENLASE");
    expect(request.Radicado).toBe("2025-0002");
  });
});

