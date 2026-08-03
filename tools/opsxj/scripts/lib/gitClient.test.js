import { describe, expect, it } from "vitest";
import { buildFeatureBranchName } from "./gitClient.js";

describe("gitClient", () => {
  it("buildFeatureBranchName uppercases issue key", () => {
    expect(buildFeatureBranchName("scrum-10")).toBe("feature/SCRUM-10");
  });
});

