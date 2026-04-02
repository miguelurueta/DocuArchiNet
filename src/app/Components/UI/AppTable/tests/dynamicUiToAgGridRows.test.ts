import { describe, expect, it } from "vitest";
import { mapDynamicUiRowsToAppGridRows } from "../adapters/dynamicUiToAgGridRows";
import type { UiRowDto } from "../types/dynamicUiTable.types";

describe("[SPEC:CREATE-COTRATO-AG-GRID-FASE-2] dynamicUiToAgGridRows", () => {
  it("aplana Values en data y preserva Meta por separado", () => {
    const rows: UiRowDto[] = [
      {
        Id: "row-1",
        Values: {
          asunto: "Contrato marco",
          prioridad: "Alta",
        },
        Meta: {
          source: "backend",
        },
      },
    ];

    const result = mapDynamicUiRowsToAppGridRows(rows);

    expect(result).toEqual([
      {
        id: "row-1",
        data: {
          asunto: "Contrato marco",
          prioridad: "Alta",
        },
        meta: {
          source: "backend",
        },
      },
    ]);
  });

  it("garantiza un id estable usando key cuando falta id", () => {
    const rows: UiRowDto[] = [
      {
        Key: 77,
        Values: {
          asunto: "Seguimiento",
        },
      },
    ];

    const result = mapDynamicUiRowsToAppGridRows(rows);

    expect(result[0]?.id).toBe("77");
  });

  it("retorna empty state compatible cuando rows es null", () => {
    expect(mapDynamicUiRowsToAppGridRows(null)).toEqual([]);
    expect(mapDynamicUiRowsToAppGridRows(undefined)).toEqual([]);
  });

  it("no mezcla meta con data", () => {
    const rows: UiRowDto[] = [
      {
        Id: "row-2",
        Values: { estado: "Pendiente" },
        Meta: { hidden: true },
      },
    ];

    const result = mapDynamicUiRowsToAppGridRows(rows);

    expect(result[0]?.data).toEqual({ estado: "Pendiente" });
    expect(result[0]?.data).not.toHaveProperty("hidden");
    expect(result[0]?.meta).toEqual({ hidden: true });
  });
});
