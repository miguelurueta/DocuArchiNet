import { mkdtemp, readFile, rm } from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { describe, expect, it } from "vitest";
import {
  buildChangeNameFromJiraSummary,
  buildProposalContent,
  inferProposalIntent,
  writeRefinementArtifacts,
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
    expect(content).toContain("## Jira Details");
    expect(content).toContain("## Capabilities");
    expect(content).toContain("## Impact");
    expect(content).toContain("ABC-123");
  });

  it("keeps multiline jira description in proposal details", () => {
    const content = buildProposalContent({
      issueKey: "ABC-124",
      summary: "Resumen",
      description: "Linea uno\nLinea dos\nLinea tres",
    });

    expect(content).toContain("> Linea uno");
    expect(content).toContain("> Linea dos");
    expect(content).toContain("> Linea tres");
  });

  it("infers app-toolbar capability for component creation tickets", () => {
    const intent = inferProposalIntent({
      summary: "CREA-COMPONENTE-TOOLBAR",
      description: "Crear componente reusable enterprise.",
    });

    expect(intent.capability).toBe("app-toolbar");
    expect(intent.kind).toBe("component");
    expect(intent.impact.join(" ")).toContain("src/app/Components/UI/AppToolbar/");
  });

  it("builds proposal content aligned to the jira ticket instead of jira-proposal-generator", () => {
    const content = buildProposalContent({
      issueKey: "SCRUMCORE-12",
      summary: "CREA-COMPONENTE-TOOLBAR",
      description: "Crear componente AppToolbar reutilizable Enterprise.",
    });

    expect(content).toContain("`app-toolbar`");
    expect(content).toContain("AppToolbar");
    expect(content).not.toContain("`jira-proposal-generator`");
    expect(content).toContain("src/app/Components/UI/AppToolbar/");
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

  it("writes initial design/spec/tasks artifacts from jira context", async () => {
    const tempDir = await mkdtemp(path.join(os.tmpdir(), "proposal-artifacts-"));

    try {
      const result = await writeRefinementArtifacts({
        issueKey: "SCRUM-8",
        changeName: "scrum-8-auto-complete-asunto",
        summary: "Auto complete asunto",
        description: "Detalle completo\nCon lineas",
        baseDir: tempDir,
      });

      const design = await readFile(result.designPath, "utf8");
      const spec = await readFile(result.specPath, "utf8");
      const tasks = await readFile(result.tasksPath, "utf8");

      expect(design).toContain("## Context");
      expect(design).toContain("SCRUM-8: Auto complete asunto");
      expect(spec).toContain("## ADDED Requirements");
      expect(tasks).toContain("## 1. Refinement");
    } finally {
      await rm(tempDir, { recursive: true, force: true });
    }
  });
});
