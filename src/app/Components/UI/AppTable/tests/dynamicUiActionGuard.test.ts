import { describe, expect, it } from "vitest";
import { evaluateDynamicUiActionAvailability } from "../utils/dynamicUiActionGuard";

const baseAction = {
  actionId: "archivar_tramite",
  label: "Archivar",
  placement: "row",
  presentation: "button",
  behavior: "api_call",
};

describe("[SPEC:CREA-ACTION-LAYER-AG-GRID] dynamicUiActionGuard", () => {
  it("enables an action when RequiredClaimsAny is satisfied", () => {
    const result = evaluateDynamicUiActionAvailability(
      {
        ...baseAction,
        requiredClaimsAny: ["tramites.gestionar", "tramites.archivar"],
      },
      {
        userClaims: ["tramites.archivar"],
      },
    );

    expect(result).toEqual({
      isVisible: true,
      isEnabled: true,
      reasons: undefined,
    });
  });

  it("blocks an action when RequiredClaimsAll or ClaimKey are missing", () => {
    const result = evaluateDynamicUiActionAvailability(
      {
        ...baseAction,
        requiredClaimsAll: ["tramites.gestionar", "tramites.aprobar"],
        claimKey: "tramites.archivar",
      },
      {
        userClaims: ["tramites.gestionar"],
      },
    );

    expect(result.isVisible).toBe(false);
    expect(result.isEnabled).toBe(false);
    expect(result.reasons).toEqual([
      "Missing required claims: tramites.aprobar",
      "Missing claim key: tramites.archivar",
    ]);
  });

  it("documents unsupported rules instead of inventing frontend semantics", () => {
    const result = evaluateDynamicUiActionAvailability(
      {
        ...baseAction,
        rules: {
          visible: true,
          enabled: false,
          expression: "backend-only",
        },
      },
      {
        userClaims: ["tramites.gestionar"],
      },
    );

    expect(result).toEqual({
      isVisible: true,
      isEnabled: false,
      reasons: ["Rules not safely evaluated in frontend: expression"],
    });
  });
});
