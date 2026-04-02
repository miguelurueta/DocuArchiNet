import { describe, expect, it } from "vitest";
import { resolveDynamicUiActionPresentation } from "../utils/dynamicUiActionPresentationResolver";

describe("[SPEC:CREA-ACTION-LAYER-AG-GRID] dynamicUiActionPresentationResolver", () => {
  it("recognizes known presentations and preserves metadata as config", () => {
    const result = resolveDynamicUiActionPresentation({
      actionId: "gestionar",
      label: "Gestionar",
      placement: "row",
      presentation: "icon_button",
      behavior: "client_event",
      metadata: {
        size: "small",
      },
    });

    expect(result).toEqual({
      kind: "icon_button",
      rawValue: "icon_button",
      isKnown: true,
      config: {
        size: "small",
      },
    });
  });

  it("keeps future presentation values without coupling to UI design", () => {
    const result = resolveDynamicUiActionPresentation({
      actionId: "future",
      label: "Future",
      placement: "toolbar",
      presentation: "split_button",
      behavior: "custom",
    });

    expect(result).toEqual({
      kind: "split_button",
      rawValue: "split_button",
      isKnown: false,
      config: undefined,
    });
  });
});
