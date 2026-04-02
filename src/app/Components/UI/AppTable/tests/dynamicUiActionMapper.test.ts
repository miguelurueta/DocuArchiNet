import { describe, expect, it } from "vitest";
import { groupCellActionsByColumnKey, mapDynamicUiActions } from "../utils/dynamicUiActionMapper";
import type { UiCellActionDto } from "../types/dynamicUiTable.types";

describe("[SPEC:CREATE-COTRATO-AG-GRID-FASE-2] dynamicUiActionMapper", () => {
  it("mapea metadata completa sin ejecutar comportamiento", () => {
    const result = mapDynamicUiActions([
      {
        ActionId: "approve",
        Label: "Aprobar",
        Placement: "cell",
        Presentation: "button",
        Behavior: "emit-event",
        BehaviorConfig: { channel: "workflow" },
        Request: { method: "POST" },
        Icon: "check",
        Tone: "success",
        RequiresConfirm: true,
        ConfirmTitle: "Confirmar",
        ConfirmMessage: "Desea aprobar el registro?",
        RequiredClaimsAny: ["workflow.approve"],
        RequiredClaimsAll: ["workflow.read"],
        ClaimKey: "workflow.approve",
        Rules: { enabled: true },
        Metadata: { source: "contract" },
        Payload: { rowId: 1 },
      },
    ]);

    expect(result).toEqual([
      {
        actionId: "approve",
        label: "Aprobar",
        placement: "cell",
        presentation: "button",
        behavior: "emit-event",
        behaviorConfig: { channel: "workflow" },
        request: { method: "POST" },
        icon: "check",
        tone: "success",
        requiresConfirm: true,
        confirmTitle: "Confirmar",
        confirmMessage: "Desea aprobar el registro?",
        requiredClaimsAny: ["workflow.approve"],
        requiredClaimsAll: ["workflow.read"],
        claimKey: "workflow.approve",
        rules: { enabled: true },
        metadata: { source: "contract" },
        payload: { rowId: 1 },
      },
    ]);
  });

  it("agrupa CellActions por ColumnKey", () => {
    const actions: UiCellActionDto[] = [
      {
        ColumnKey: "actions",
        Action: {
          ActionId: "edit",
          Label: "Editar",
          Behavior: "navigate",
          Presentation: "link",
        },
      },
      {
        ColumnKey: "actions",
        Action: {
          ActionId: "delete",
          Label: "Eliminar",
          Behavior: "confirm-delete",
          Presentation: "button",
        },
      },
    ];

    const result = groupCellActionsByColumnKey(actions);

    expect(result.actions).toHaveLength(2);
    expect(result.actions?.[0]?.actionId).toBe("edit");
    expect(result.actions?.[1]?.actionId).toBe("delete");
  });

  it("mantiene behavior y presentation extensibles", () => {
    const result = mapDynamicUiActions([
      {
        ActionId: "custom",
        Label: "Custom",
        Behavior: "custom.behavior",
        Presentation: "floating-chip",
      },
    ]);

    expect(result[0]?.behavior).toBe("custom.behavior");
    expect(result[0]?.presentation).toBe("floating-chip");
  });

  it("soporta CellActions con Action anidada del backend real", () => {
    const result = mapDynamicUiActions([
      {
        ColumnKey: "acciones",
        Action: {
          ActionId: "gestionar_tramite",
          Label: "Gestionar tramite",
          Placement: "row",
          Presentation: "icon_button",
          Behavior: "client_event",
          BehaviorConfig: {
            menuItems: ["gestionar_tramite_menu"],
          },
          Request: {
            RowIdField: "id_tarea",
          },
          Icon: "gestionar_tramite",
          Tone: "primary",
        },
      },
    ]);

    expect(result).toEqual([
      expect.objectContaining({
        actionId: "gestionar_tramite",
        presentation: "icon_button",
        behavior: "client_event",
        behaviorConfig: { menuItems: ["gestionar_tramite_menu"] },
        request: { RowIdField: "id_tarea" },
      }),
    ]);
  });
});
