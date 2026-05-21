import { describe, expect, it } from "vitest";
import { buildListaDocumentosRadicadosActionRequest } from "./documentosWorkbenchActionMapper";

describe("[SPEC:APPTREETABLE-217] documentosWorkbenchActionMapper", () => {
  it("construye action request ver_documento con payload esperado", () => {
    const request = buildListaDocumentosRadicadosActionRequest({
      context: { tableId: "InboxListaRadicados", viewMode: "flatDocuments" },
      actionId: "ver_documento",
      rowId: "r1",
      nodeType: "documento",
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
      DocumentId: 10,
      NombreGabinete: "GAB",
    });
  });
});

