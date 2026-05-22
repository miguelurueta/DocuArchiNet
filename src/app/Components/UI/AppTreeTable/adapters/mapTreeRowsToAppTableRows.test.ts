import { describe, expect, it } from "vitest";
import { mapTreeRowsToAppTableRows } from "./mapTreeRowsToAppTableRows";

describe("mapTreeRowsToAppTableRows", () => {
  it("preserva el id interno aunque Values incluya id undefined", () => {
    const rows = [
      {
        id: "row-1",
        label: "Documento 1",
        level: 0,
        parentId: null,
        expanded: false,
        hasChildren: false,
        selectable: true,
        originalNode: {
          id: "row-1",
          label: "Documento 1",
          values: {
            id: null,
            TIPODOCUMENTO: "DOC 1",
          },
        },
      },
    ];

    const result = mapTreeRowsToAppTableRows({
      rows,
      columns: ["id", "TIPODOCUMENTO"],
      loadingChildrenIds: new Set<string>(),
    });

    expect(result[0].__rowId).toBe("row-1");
    expect(result[0].id).toBe("row-1");
    expect(result[0].TIPODOCUMENTO).toBe("DOC 1");
  });
});
