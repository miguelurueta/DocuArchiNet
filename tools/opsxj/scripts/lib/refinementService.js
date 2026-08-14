import { access, readFile, readdir, writeFile } from "node:fs/promises";
import path from "node:path";

export const REFINEMENT_VERSION = 1;
const DRAFT_MARKER = `<!-- opsxj:refinement version=${REFINEMENT_VERSION} state=draft -->`;
const APPROVED_MARKER = `<!-- opsxj:refinement version=${REFINEMENT_VERSION} state=approved -->`;
const PLACEHOLDER_PATTERN = /\b(?:tbd|todo|pendiente|por definir|por confirmar)\b|\[pendiente[^\]]*\]/i;
const TASK_PATTERN = /^\s*-\s+\[[ xX]\]\s+(.+)$/gm;

const exists = async (targetPath) => access(targetPath).then(() => true).catch(() => false);
const toRepoPath = (value) => String(value).replace(/\\/g, "/");

const getRefinementRelativePath = ({ changeName, manifest }) =>
  manifest?.refinement?.path ?? path.posix.join("openspec", "changes", changeName, "refinement.md");

const findSpecRelativePath = async ({ baseDir, changeName }) => {
  const specsDir = path.join(baseDir, "openspec", "changes", changeName, "specs");
  const visit = async (directory) => {
    const entries = await readdir(directory, { withFileTypes: true }).catch(() => []);
    for (const entry of entries) {
      const entryPath = path.join(directory, entry.name);
      if (entry.isFile() && entry.name === "spec.md") return entryPath;
      if (entry.isDirectory()) {
        const nested = await visit(entryPath);
        if (nested) return nested;
      }
    }
    return null;
  };
  const found = await visit(specsDir);
  return found ? toRepoPath(path.relative(baseDir, found)) : null;
};

const getArtifactRelativePaths = async ({ baseDir, changeName, manifest }) => {
  const profilePaths = manifest?.architectureProfile?.artifactPaths ?? {};
  const changeRoot = path.posix.join("openspec", "changes", changeName);
  return {
    design: profilePaths.design ?? path.posix.join(changeRoot, "design.md"),
    spec: profilePaths.spec ?? (await findSpecRelativePath({ baseDir, changeName })),
    tasks: profilePaths.tasks ?? path.posix.join(changeRoot, "tasks.md"),
  };
};

const unique = (values) => [...new Set(values)];
const stripTraceabilityHeader = (content) =>
  content.replace(/^<!-- opsxj:refinement-traceability version=\d+ artifact=[^\s]+ decisions=[^>]* -->\r?\n?/m, "");
const extractDecisionRows = (content) =>
  [...content.matchAll(/^\|\s*(D-\d{2,})\s*\|(.+)$/gm)].map((match) => ({
    id: match[1],
    row: match[0],
  }));
const extractDecisionIds = (content) => unique(extractDecisionRows(content).map((row) => row.id));
const extractRequirementIds = (content) => unique([...content.matchAll(/^\|\s*(RQ-\d{2,})\s*\|/gm)].map((match) => match[1]));

const buildArtifactHeader = ({ artifact, decisionIds }) =>
  [
    `<!-- opsxj:refinement-traceability version=${REFINEMENT_VERSION} artifact=${artifact} decisions=${decisionIds.join(",")} -->`,
    "",
  ].join("\n");

const upsertArtifactHeader = async ({ absolutePath, artifact, decisionIds }) => {
  const content = await readFile(absolutePath, "utf8");
  const header = buildArtifactHeader({ artifact, decisionIds });
  const markerPattern = /^<!-- opsxj:refinement-traceability version=\d+ artifact=[^\s]+ decisions=[^>]* -->\r?\n?/m;
  const next = markerPattern.test(content) ? content.replace(markerPattern, header) : `${header}${content}`;
  if (next !== content) await writeFile(absolutePath, next, "utf8");
};

export const buildInitialRefinementContent = ({ issueKey, changeName, summary, technologyProfile }) => {
  const profileLine = technologyProfile
    ? `- Perfil tecnologico: \`${technologyProfile}\`. Las reglas de framework solo aplican si corresponden a este perfil.`
    : "- Perfil tecnologico: no definido; no introducir reglas de framework hasta identificar la tecnologia afectada.";

  return [
    DRAFT_MARKER,
    "",
    `# Refinamiento - ${changeName}`,
    "",
    "## Fuente y alcance",
    "",
    `- Ticket: \`${issueKey}\` — ${summary || "(sin resumen)"}`,
    `- Cambio OpenSpec: \`${changeName}\``,
    `- Fuente Jira: \`specs/*/jira-context.md\``,
    profileLine,
    "",
    "Este artefacto es la compuerta entre el ticket y la implementacion. No se aprueba por generacion automatica: una persona responsable debe confirmar alcance, decisiones, compatibilidad y evidencia de codigo.",
    "",
    "## Contexto inspeccionado",
    "",
    "- [PENDIENTE: rutas, clases, handlers, scripts y datos legacy inspeccionados]",
    "- [PENDIENTE: comportamiento actual y compatibilidad que se debe preservar]",
    "",
    "## Decisiones aprobadas",
    "",
    "| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |",
    "| --- | --- | --- | --- | --- | --- |",
    "| D-01 | [PENDIENTE: decision concreta] | [PENDIENTE: ruta y simbolo] | D-01 | RQ-01 | Origen: D-01, RQ-01 |",
    "",
    "## Requisitos verificables",
    "",
    "| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |",
    "| --- | --- | --- | --- |",
    "| RQ-01 | [PENDIENTE: resultado] | [PENDIENTE: WHEN/THEN] | [PENDIENTE: regresion y rollback] |",
    "",
    "## Reglas de trazabilidad obligatorias",
    "",
    "1. Cada decision `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.",
    "2. Cada tarea con checkbox debe conservar su origen. Las tareas de validacion, rollout y documentacion tambien deben indicar la decision o requisito que verifican.",
    "3. Las reglas de frontend, WebForms, Node u otro framework solo se agregan cuando el perfil tecnologico y el codigo afectado las justifican.",
    "4. El estado solo puede cambiar a `approved` cuando no haya marcadores pendientes, las decisiones sean especificas y la matriz sea completa.",
    "",
    "## Resultado del refinamiento",
    "",
    "- Estado: borrador. Sustituya el marcador inicial por el estado `approved` despues de completar y revisar la matriz.",
    "- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- <ISSUE-KEY> --sync`.",
    "",
  ].join("\n");
};

export const writeInitialRefinementArtifact = async ({
  baseDir,
  issueKey,
  changeName,
  summary,
  technologyProfile,
}) => {
  const refinementPath = path.join(baseDir, "openspec", "changes", changeName, "refinement.md");
  if (!(await exists(refinementPath))) {
    await writeFile(
      refinementPath,
      buildInitialRefinementContent({ issueKey, changeName, summary, technologyProfile }),
      "utf8",
    );
  }
  return refinementPath;
};

const getRefinementChecks = async ({ baseDir, changeName, manifest }) => {
  const checks = [];
  const refinementRelativePath = getRefinementRelativePath({ changeName, manifest });
  const refinementPath = path.join(baseDir, refinementRelativePath);
  const present = await exists(refinementPath);
  checks.push({ name: "refinement:exists", status: present ? "PASS" : "FAIL", details: { path: toRepoPath(refinementRelativePath) } });
  if (!present) return { checks, decisionIds: [], artifactPaths: {} };

  const content = await readFile(refinementPath, "utf8");
  const approved = content.includes(APPROVED_MARKER);
  checks.push({ name: "refinement:approved", status: approved ? "PASS" : "FAIL" });
  checks.push({ name: "refinement:no_placeholders", status: PLACEHOLDER_PATTERN.test(content) ? "FAIL" : "PASS" });
  const decisionRows = extractDecisionRows(content);
  const decisionIds = extractDecisionIds(content);
  const requirementIds = extractRequirementIds(content);
  const malformedDecisionRows = decisionRows.filter((item) => PLACEHOLDER_PATTERN.test(item.row));
  checks.push({
    name: "refinement:decisions",
    status: decisionIds.length > 0 && malformedDecisionRows.length === 0 ? "PASS" : "FAIL",
    details: { decisionIds, malformedDecisionRows: malformedDecisionRows.map((item) => item.id) },
  });
  checks.push({ name: "refinement:requirements", status: requirementIds.length > 0 ? "PASS" : "FAIL", details: { requirementIds } });

  const artifactPaths = await getArtifactRelativePaths({ baseDir, changeName, manifest });
  for (const [artifact, relativePath] of Object.entries(artifactPaths)) {
    if (!relativePath) {
      checks.push({ name: `refinement:${artifact}:path`, status: "FAIL" });
      continue;
    }
    const artifactPath = path.join(baseDir, relativePath);
    const artifactPresent = await exists(artifactPath);
    checks.push({ name: `refinement:${artifact}:exists`, status: artifactPresent ? "PASS" : "FAIL" });
    if (!artifactPresent) continue;
    const artifactContent = stripTraceabilityHeader(await readFile(artifactPath, "utf8"));
    const missingDecisionIds = decisionIds.filter((id) => !artifactContent.includes(id));
    checks.push({
      name: `refinement:${artifact}:decisions`,
      status: missingDecisionIds.length === 0 ? "PASS" : "FAIL",
      details: { missingDecisionIds },
    });
    if (artifact === "tasks") {
      const taskLines = [...artifactContent.matchAll(TASK_PATTERN)].map((match) => match[1]);
      const origins = taskLines.map((line) => ({
        line,
        match: line.match(/\bOrigen:\s*(D-\d{2,})\s*,\s*(RQ-\d{2,})\b/i),
      }));
      const missingOrigins = origins
        .filter((item) => !item.match || !decisionIds.includes(item.match[1]) || !requirementIds.includes(item.match[2]))
        .map((item) => item.line);
      checks.push({
        name: "refinement:tasks:origins",
        status: missingOrigins.length === 0 && taskLines.length > 0 ? "PASS" : "FAIL",
        details: { missingOrigins },
      });
    }
  }

  const technologyProfile = manifest?.technologyProfile?.name;
  if (technologyProfile && technologyProfile !== "frontend-react-ts") {
    const forbiddenMarkers = ["AppResponses<T>", "src/shared/api/appResponseError.ts", "getUserVisibleAppResponseMessage"];
    for (const [artifact, relativePath] of Object.entries(artifactPaths)) {
      if (!relativePath || !(await exists(path.join(baseDir, relativePath)))) continue;
      const artifactContent = await readFile(path.join(baseDir, relativePath), "utf8");
      const forbidden = forbiddenMarkers.filter((marker) => artifactContent.includes(marker));
      checks.push({
        name: `refinement:profile:${artifact}:frontend_policy`,
        status: forbidden.length === 0 ? "PASS" : "FAIL",
        details: { technologyProfile, forbidden },
      });
    }
  }

  return { checks, decisionIds, artifactPaths, refinementPath, refinementRelativePath };
};

export const auditRefinement = async ({ baseDir, changeName, sync = false, bootstrap = false }) => {
  const manifestPath = path.join(baseDir, "openspec", "changes", changeName, "opsxj-governance.json");
  let manifest = (await exists(manifestPath)) ? JSON.parse(await readFile(manifestPath, "utf8")) : null;
  let bootstrapped = false;
  if (manifest && !manifest.refinement?.required && bootstrap) {
    const refinementRelativePath = path.posix.join("openspec", "changes", changeName, "refinement.md");
    manifest = {
      ...manifest,
      version: Math.max(Number(manifest.version) || 1, 3),
      refinement: {
        version: REFINEMENT_VERSION,
        required: true,
        path: refinementRelativePath,
        state: "approved",
        taskOriginFormat: "Origen: D-XX, RQ-XX",
        enforcement:
          "Cada decision debe llegar a design, spec y tasks; no se admiten reglas de framework ajeno al perfil tecnologico.",
      },
    };
    await writeFile(manifestPath, `${JSON.stringify(manifest, null, 2)}\n`, "utf8");
    await writeInitialRefinementArtifact({
      baseDir,
      issueKey: manifest.issueKey,
      changeName,
      summary: `Migracion controlada de ${changeName}`,
      technologyProfile: manifest.technologyProfile?.name,
    });
    bootstrapped = true;
  }
  if (!manifest || !manifest.refinement?.required) {
    return {
      applicable: false,
      status: "PASS",
      checks: [],
      message:
        "Cambio sin compuerta de refinement v1; compatibilidad historica preservada. Use --bootstrap para incorporarlo explicitamente al flujo nuevo.",
    };
  }

  const firstPass = await getRefinementChecks({ baseDir, changeName, manifest });
  const refinementReadyForSync = [
    "refinement:approved",
    "refinement:no_placeholders",
    "refinement:decisions",
    "refinement:requirements",
  ].every((name) => firstPass.checks.some((check) => check.name === name && check.status === "PASS"));
  if (sync && refinementReadyForSync && firstPass.refinementPath && firstPass.decisionIds.length > 0) {
    for (const [artifact, relativePath] of Object.entries(firstPass.artifactPaths)) {
      if (!relativePath) continue;
      const artifactPath = path.join(baseDir, relativePath);
      if (await exists(artifactPath)) {
        await upsertArtifactHeader({ absolutePath: artifactPath, artifact, decisionIds: firstPass.decisionIds });
      }
    }
  }

  const result = sync
    ? await getRefinementChecks({ baseDir, changeName, manifest })
    : firstPass;
  const failures = result.checks.filter((check) => check.status === "FAIL");
  return {
    applicable: true,
    status: failures.length === 0 ? "PASS" : "FAIL",
    checks: result.checks,
    refinementPath: result.refinementRelativePath,
    synced: sync,
    bootstrapped,
    message:
      failures.length === 0
        ? "Refinement aprobado y trazable con design, spec y tasks."
        : `${failures.length} requisito(s) de refinement pendiente(s).`,
  };
};
