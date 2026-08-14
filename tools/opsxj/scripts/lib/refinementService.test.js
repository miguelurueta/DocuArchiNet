import { mkdir, mkdtemp, readFile, rm, writeFile } from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { describe, expect, it } from "vitest";
import { auditRefinement, buildInitialRefinementContent } from "./refinementService.js";

const writeApprovedChange = async ({ baseDir, changeName = "scrum-9-refinement" }) => {
  const changeDir = path.join(baseDir, "openspec", "changes", changeName);
  const specDir = path.join(changeDir, "specs", "capacidad");
  await mkdir(specDir, { recursive: true });
  await writeFile(
    path.join(changeDir, "opsxj-governance.json"),
    JSON.stringify(
      {
        version: 3,
        issueKey: "SCRUM-9",
        changeName,
        technologyProfile: { name: "legacy-webforms-vb", version: 1 },
        refinement: {
          version: 1,
          required: true,
          path: `openspec/changes/${changeName}/refinement.md`,
        },
        architectureProfile: {
          artifactPaths: {
            design: `openspec/changes/${changeName}/design.md`,
            spec: `openspec/changes/${changeName}/specs/capacidad/spec.md`,
            tasks: `openspec/changes/${changeName}/tasks.md`,
          },
        },
      },
      null,
      2,
    ),
    "utf8",
  );
  await writeFile(
    path.join(changeDir, "refinement.md"),
    [
      "<!-- opsxj:refinement version=1 state=approved -->",
      "",
      "# Refinamiento aprobado",
      "",
      "| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |",
      "| --- | --- | --- | --- | --- | --- |",
      "| D-01 | Mantener la transicion legacy encapsulada | `workflow/modern/Infrastructure/LegacyAdapters/WorkflowLegacyExecutorAdapter.vb` | D-01 | RQ-01 | Origen: D-01, RQ-01 |",
      "",
      "| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |",
      "| --- | --- | --- | --- |",
      "| RQ-01 | La transicion conserva el contrato existente | WHEN se solicita terminar THEN se ejecuta la frontera tipada | Se conserva rollback legacy |",
      "",
    ].join("\n"),
    "utf8",
  );
  await writeFile(path.join(changeDir, "design.md"), "## Decisions\n\nD-01 mantiene la frontera tipada.\n", "utf8");
  await writeFile(
    path.join(specDir, "spec.md"),
    "### Requirement: RQ-01\n\nD-01 conserva el contrato observable.\n",
    "utf8",
  );
  await writeFile(
    path.join(changeDir, "tasks.md"),
    "- [ ] Implementar adapter tipado. Origen: D-01, RQ-01\n",
    "utf8",
  );
  return { changeDir, specDir, changeName };
};

describe("refinementService", () => {
  it("generates a draft that cannot be confused with an approved decision", () => {
    const content = buildInitialRefinementContent({
      issueKey: "SCRUM-9",
      changeName: "scrum-9-refinement",
      summary: "Refinar transicion",
      technologyProfile: "legacy-webforms-vb",
    });

    expect(content).toContain("state=draft");
    expect(content).toContain("Origen: D-XX, RQ-XX");
    expect(content).toContain("solo aplican si corresponden a este perfil");
  });

  it("requires approved decisions, artifact traceability and task origins", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-refinement-"));
    try {
      const { changeDir, specDir, changeName } = await writeApprovedChange({ baseDir });
      const result = await auditRefinement({ baseDir, changeName, sync: true });

      expect(result).toMatchObject({ applicable: true, status: "PASS", synced: true });
      expect(await readFile(path.join(changeDir, "design.md"), "utf8")).toContain(
        "opsxj:refinement-traceability",
      );
      expect(await readFile(path.join(specDir, "spec.md"), "utf8")).toContain(
        "opsxj:refinement-traceability",
      );

      await writeFile(path.join(changeDir, "design.md"), "D-01 AppResponses<T>\n", "utf8");
      const invalid = await auditRefinement({ baseDir, changeName });
      expect(invalid.checks).toEqual(
        expect.arrayContaining([
          expect.objectContaining({
            name: "refinement:profile:design:frontend_policy",
            status: "FAIL",
          }),
        ]),
      );
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("only migrates an existing governed change when --bootstrap is explicit", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-refinement-bootstrap-"));
    const changeName = "scrum-10-existing";
    const changeDir = path.join(baseDir, "openspec", "changes", changeName);
    try {
      await mkdir(changeDir, { recursive: true });
      await writeFile(
        path.join(changeDir, "opsxj-governance.json"),
        JSON.stringify({ version: 2, issueKey: "SCRUM-10", changeName }),
        "utf8",
      );

      const compatible = await auditRefinement({ baseDir, changeName });
      expect(compatible).toMatchObject({ applicable: false, status: "PASS" });
      expect(await readFile(path.join(changeDir, "opsxj-governance.json"), "utf8")).not.toContain(
        '"refinement"',
      );

      const migrated = await auditRefinement({ baseDir, changeName, bootstrap: true });
      expect(migrated).toMatchObject({ applicable: true, status: "FAIL", bootstrapped: true });
      expect(await readFile(path.join(changeDir, "opsxj-governance.json"), "utf8")).toContain(
        '"refinement"',
      );
      expect(await readFile(path.join(changeDir, "refinement.md"), "utf8")).toContain("state=draft");
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });
});
