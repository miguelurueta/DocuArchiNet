import { describe, expect, it } from "vitest";
import { resolveDynamicUiActionBehavior } from "../utils/dynamicUiActionBehaviorResolver";

describe("[SPEC:CREA-ACTION-LAYER-AG-GRID] dynamicUiActionBehaviorResolver", () => {
  it("recognizes known behaviors and preserves config", () => {
    const result = resolveDynamicUiActionBehavior({
      actionId: "descargar",
      label: "Descargar",
      placement: "row",
      presentation: "icon_button",
      behavior: "download",
      behaviorConfig: {
        format: "pdf",
      },
    });

    expect(result).toEqual({
      kind: "download",
      rawValue: "download",
      isKnown: true,
      config: {
        format: "pdf",
      },
    });
  });

  it("keeps future behavior values without rigid enums", () => {
    const result = resolveDynamicUiActionBehavior({
      actionId: "future",
      label: "Future",
      placement: "row",
      presentation: "button",
      behavior: "open_panel",
    });

    expect(result).toEqual({
      kind: "open_panel",
      rawValue: "open_panel",
      isKnown: false,
      config: undefined,
    });
  });
});
