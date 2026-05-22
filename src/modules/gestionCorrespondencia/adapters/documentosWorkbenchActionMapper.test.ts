import { describe, expect, it } from "vitest";
import { buildListaDocumentosRadicadosActionRequest } from "./documentosWorkbenchActionMapper";

describe("[SPEC:APPTREETABLE-217] documentosWorkbenchActionMapper", () => {
  it("prioriza IdDocumento cuando ambos identificadores estan disponibles", () => {
    const request = buildListaDocumentosRadicadosActionRequest({
      context: { tableId: "InboxListaRadicados", viewMode: "flatDocuments" },
      actionId: "ver_documento",
      rowId: "r1",
      nodeType: "documento",
      idDocumento: 10,
      documentId: 10,
      nombreGabinete: "GAB",
    });

    expect(request.TableId).toBe("InboxListaRadicados");
    expect(request.ViewMode).toBe("flatDocuments");
    expect(request.ActionId).toBe("ver_documento");
    expect(request.RowId).toBe("r1");
    expect(request.NodeType).toBe("documento");
    expect(request.Payload).toMatchObject({
      IdDocumento: 10,
      NombreGabinete: "GAB",
    });
    expect(request.Payload.DocumentId).toBeUndefined();
  });

  it("usa DocumentId cuando IdDocumento no viene disponible", () => {
    const request = buildListaDocumentosRadicadosActionRequest({
      context: { tableId: "InboxListaRadicados", viewMode: "flatDocuments" },
      actionId: "ver_documento",
      rowId: "r2",
      nodeType: "documento",
      documentId: 22,
      nombreGabinete: "GAB",
    });

    expect(request.Payload).toMatchObject({
      DocumentId: 22,
      NombreGabinete: "GAB",
    });
    expect(request.Payload.IdDocumento).toBeUndefined();
  });
});

