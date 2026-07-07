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
    expect(request.DocumentRelationScope).toBe("documentsOnly");
    expect(request.EnablePagination).toBe(true);
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
    expect(request.DocumentRelationScope).toBe("documentsOnly");
    expect(request.EnablePagination).toBe(true);
  });

  it("permite scope y paginacion explicitos en el query root", () => {
    const request = buildListaDocumentosRadicadosRootQuery({
      idTareaWf: 123,
      nombreGabinete: "GAB-1",
      radicado: "2025-0001",
      documentRelationScope: "includeResponseAttachments",
      enablePagination: true,
      page: 3,
      pageSize: 50,
      search: "  contrato  ",
      searchType: 2,
    });

    expect(request.DocumentRelationScope).toBe("includeResponseAttachments");
    expect(request.EnablePagination).toBe(true);
    expect(request.Page).toBe(3);
    expect(request.PageSize).toBe(50);
    expect(request.Search).toBe("contrato");
    expect(request.SearchType).toBe(2);
  });

  it("permite scope y paginacion explicitos en el query children", () => {
    const request = buildListaDocumentosRadicadosChildrenQuery({
      nombreGabinete: "GAB-1",
      radicado: "2025-0002",
      parentRowId: "row-1",
      level: 2,
      documentRelationScope: "responseAttachmentsOnly",
      enablePagination: true,
      page: 2,
      pageSize: 10,
    });

    expect(request.DocumentRelationScope).toBe("responseAttachmentsOnly");
    expect(request.EnablePagination).toBe(true);
    expect(request.Page).toBe(2);
    expect(request.PageSize).toBe(10);
  });
});

