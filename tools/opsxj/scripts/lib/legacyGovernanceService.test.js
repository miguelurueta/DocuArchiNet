import { mkdir, mkdtemp, readFile, rm, writeFile } from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { describe, expect, it } from "vitest";
import {
  LEGACY_IMPACT_CATALOG,
  normalizeImpact,
  validateLegacyGovernance,
  writeLegacyGovernanceArtifacts,
  writeValidationEvidence,
} from "./legacyGovernanceService.js";

describe("legacyGovernanceService", () => {
  it("exposes the legacy impact catalog and rejects unknown impacts", () => {
    expect(LEGACY_IMPACT_CATALOG.webforms_ui.evidence).toContain("manual_qa");
    expect(normalizeImpact("BACKEND_VB")).toBe("backend_vb");
    expect(() => normalizeImpact("react_only")).toThrow("Impacto no soportado");
  });

  it("writes technical documentation and its OpenSpec governance manifest", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-governance-"));
    try {
      const changeName = "scrum-90-webforms";
      await mkdir(path.join(baseDir, "openspec", "changes", changeName), { recursive: true });
      await writeFile(path.join(baseDir, "openspec", "changes", changeName, "tasks.md"), "- [ ] pendiente\n", "utf8");
      const result = await writeLegacyGovernanceArtifacts({
        baseDir,
        issueKey: "SCRUM-90",
        changeName,
        summary: "Actualizar tabla WebForms",
        impact: "webforms_ui",
      });
      expect(result.manifest.requiredEvidence).toEqual(["manual_qa"]);
      expect(result.documentationPaths).toHaveLength(4);
      const manifest = JSON.parse(await readFile(result.manifestPath, "utf8"));
      expect(manifest.documentation[0]).toContain("Doc/Tecnica/Opsxj/scrum-90-webforms");
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("requires current evidence and completed tasks for governed changes", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-validation-"));
    const changeName = "scrum-91-backend";
    try {
      await mkdir(path.join(baseDir, "openspec", "changes", changeName), { recursive: true });
      await writeFile(path.join(baseDir, "openspec", "changes", changeName, "tasks.md"), "- [x] terminado\n", "utf8");
      await writeLegacyGovernanceArtifacts({ baseDir, issueKey: "SCRUM-91", changeName, summary: "Regla VB", impact: "backend_vb" });
      const initial = await validateLegacyGovernance({ baseDir, changeName, env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" }, currentSha: "abc" });
      expect(initial.status).toBe("FAIL");
      await writeValidationEvidence({ baseDir, issueKey: "SCRUM-91", type: "unit", status: "pass", reference: "npm test", sha: "abc" });
      const valid = await validateLegacyGovernance({ baseDir, changeName, env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" }, currentSha: "abc" });
      expect(valid.status).toBe("PASS");
      const stale = await validateLegacyGovernance({ baseDir, changeName, env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" }, currentSha: "def" });
      expect(stale.status).toBe("FAIL");
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("preserves historical changes without a manifest", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-history-"));
    try {
      const result = await validateLegacyGovernance({ baseDir, changeName: "historical", currentSha: "abc" });
      expect(result).toMatchObject({ applicable: false, status: "PASS" });
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });
});
