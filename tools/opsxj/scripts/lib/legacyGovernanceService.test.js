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

const completeDocumentation = async ({ baseDir, manifest }) => {
  for (const contract of manifest.documentationContract) {
    const sections = contract.requiredSections.join("\n\n");
    await writeFile(
      path.join(baseDir, contract.path),
      [
        "# Evidencia tecnica cerrada",
        "",
        `- Ticket: ${contract.identity.issueKey}`,
        `- Cambio OpenSpec: ${contract.identity.changeName}`,
        `- Clasificacion: ${contract.identity.impact}`,
        "",
        sections,
        "",
        "La implementacion fue revisada contra el alcance acordado. Se preserva la compatibilidad necesaria, se registran las decisiones relevantes y la evidencia de prueba es reproducible para el equipo responsable.",
      ].join("\n"),
      "utf8",
    );
  }
};

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
      expect(manifest.documentationContract[0]).toMatchObject({
        minimumContentLength: 180,
        identity: { issueKey: "SCRUM-90", changeName, impact: "webforms_ui" },
      });
      expect(manifest.architectureProfile).toBeUndefined();
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
      const generated = await writeLegacyGovernanceArtifacts({ baseDir, issueKey: "SCRUM-91", changeName, summary: "Regla VB", impact: "backend_vb" });
      const initial = await validateLegacyGovernance({ baseDir, changeName, env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" }, currentSha: "abc" });
      expect(initial.status).toBe("FAIL");
      await completeDocumentation({ baseDir, manifest: generated.manifest });
      const firstDocument = generated.manifest.documentationContract[0].path;
      const firstDocumentPath = path.join(baseDir, firstDocument);
      await writeFile(
        firstDocumentPath,
        `${await readFile(firstDocumentPath, "utf8")}\n\n| Acción | Resultado |\n| --- | --- |\n| Auditoría | Registrar el resultado sin exponerlo al navegador. |\n`,
        "utf8",
      );
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

  it("reports independent strict-documentation failures", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-strict-docs-"));
    const changeName = "scrum-92-docs";
    try {
      await mkdir(path.join(baseDir, "openspec", "changes", changeName), { recursive: true });
      await writeFile(path.join(baseDir, "openspec", "changes", changeName, "tasks.md"), "- [x] terminado\n", "utf8");
      const generated = await writeLegacyGovernanceArtifacts({ baseDir, issueKey: "SCRUM-92", changeName, summary: "Docs", impact: "docs_only" });
      const contract = generated.manifest.documentationContract[0];
      const generatedResult = await validateLegacyGovernance({
        baseDir,
        changeName,
        env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" },
      });
      expect(generatedResult.checks).toEqual(expect.arrayContaining([
        expect.objectContaining({ name: `document:${contract.path}:closure:template_instruction`, status: "FAIL" }),
        expect.objectContaining({ name: `document:${contract.path}:closure:open_checklist`, status: "FAIL" }),
      ]));
      await rm(path.join(baseDir, contract.path));
      const missing = await validateLegacyGovernance({
        baseDir,
        changeName,
        env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" },
      });
      expect(missing.checks).toEqual(expect.arrayContaining([
        expect.objectContaining({ name: `document:${contract.path}:exists`, status: "FAIL" }),
      ]));
      await writeFile(path.join(baseDir, contract.path), "", "utf8");
      const empty = await validateLegacyGovernance({
        baseDir,
        changeName,
        env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" },
      });
      expect(empty.checks).toEqual(expect.arrayContaining([
        expect.objectContaining({ name: `document:${contract.path}:content`, status: "FAIL" }),
      ]));
      await writeFile(
        path.join(baseDir, contract.path),
        [
          "# Incompleto",
          "",
          "- Ticket: SCRUM-OTRO",
          `- Cambio OpenSpec: ${changeName}`,
          "- Clasificacion: docs_only",
          "",
          "## Objetivo",
          "",
          "TBD",
          "",
          "- [ ] pendiente",
        ].join("\n"),
        "utf8",
      );
      const result = await validateLegacyGovernance({
        baseDir,
        changeName,
        env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" },
      });
      const failedNames = result.checks.filter((check) => check.status === "FAIL").map((check) => check.name);
      expect(failedNames).toEqual(expect.arrayContaining([
        `document:${contract.path}:content`,
        `document:${contract.path}:section:## Alcance y compatibilidad`,
        `document:${contract.path}:identity:- Ticket: SCRUM-92`,
        `document:${contract.path}:closure:tbd`,
        `document:${contract.path}:closure:open_checklist`,
      ]));
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("validates profile markers and reports a removed legacy boundary", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-profile-governance-"));
    const changeName = "scrum-93-profile";
    try {
      const changeDir = path.join(baseDir, "openspec", "changes", changeName);
      await mkdir(path.join(changeDir, "specs", "capacidad"), { recursive: true });
      const profileArtifacts = {
        proposal: `openspec/changes/${changeName}/proposal.md`,
        design: `openspec/changes/${changeName}/design.md`,
        spec: `openspec/changes/${changeName}/specs/capacidad/spec.md`,
        tasks: `openspec/changes/${changeName}/tasks.md`,
      };
      await writeFile(path.join(baseDir, profileArtifacts.proposal), "## Politica de modernizacion enterprise legacy\n", "utf8");
      await writeFile(path.join(baseDir, profileArtifacts.design), "## Arquitectura de modernizacion enterprise legacy\n", "utf8");
      await writeFile(path.join(baseDir, profileArtifacts.spec), "### Requirement: Frontera de capacidad legacy\n", "utf8");
      await writeFile(
        path.join(baseDir, profileArtifacts.tasks),
        [
          "## Gobierno de modernizacion enterprise legacy",
          "Gateway/Adapter tipado por capacidad",
          "pruebas de equivalencia",
          "piloto y rollback",
        ].join("\n"),
        "utf8",
      );
      const generated = await writeLegacyGovernanceArtifacts({
        baseDir,
        issueKey: "SCRUM-93",
        changeName,
        summary: "Perfil",
        impact: "docs_only",
        architectureProfile: "enterprise-legacy-modernization",
        profileArtifactPaths: profileArtifacts,
      });
      await completeDocumentation({ baseDir, manifest: generated.manifest });
      const valid = await validateLegacyGovernance({
        baseDir,
        changeName,
        env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" },
      });
      expect(valid.status).toBe("PASS");

      await writeFile(path.join(baseDir, profileArtifacts.design), "## Context\n", "utf8");
      const invalid = await validateLegacyGovernance({
        baseDir,
        changeName,
        env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" },
      });
      expect(invalid.checks).toEqual(expect.arrayContaining([
        expect.objectContaining({
          name: "architecture_profile:design:marker",
          status: "FAIL",
        }),
      ]));
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });
});
