import { mkdtemp, readFile, rm } from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { describe, expect, it } from "vitest";
import {
  buildChangeNameFromJiraSummary,
  buildProposalContent,
  writeProposalFile,
} from "./proposalGenerator.js";

describe("proposalGenerator", () => {
  it("builds a proposal with required sections", () => {
    const content = buildProposalContent({
      issueKey: "ABC-123",
      summary: "Resumen",
      description: "Detalle del ticket",
    });

    expect(content).toContain("## Why");
    expect(content).toContain("## What Changes");
    expect(content).toContain("## Capabilities");
    expect(content).toContain("## Impact");
    expect(content).toContain("ABC-123");
  });

  it("builds kebab-case change name from issue key + summary", () => {
    const changeName = buildChangeNameFromJiraSummary({
      issueKey: "SCRUM-8",
      summary: "ARC-SPEC-RAD-20260224:auto-complente-asunto",
    });

    expect(changeName).toBe("scrum-8-arc-spec-rad-20260224-auto-complente-asunto");
  });

  it("writes proposal into a provided OpenSpec change folder", async () => {
    const tempDir = await mkdtemp(path.join(os.tmpdir(), "proposal-generator-"));
    const content = "## Why\n\nDemo\n";

    try {
      const filePath = await writeProposalFile({
        issueKey: "SCRUM-8",
        changeName: "scrum-8-demo-change",
        content,
        baseDir: tempDir,
      });

      expect(filePath).toContain(
        path.join("openspec", "changes", "scrum-8-demo-change", "proposal.md"),
      );
      const saved = await readFile(filePath, "utf8");
      expect(saved).toBe(content);
    } finally {
      await rm(tempDir, { recursive: true, force: true });
    }
  });
});
