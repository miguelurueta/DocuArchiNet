import { access, mkdir, readFile, writeFile } from "node:fs/promises";
import path from "node:path";
import {
  getArchitectureProfile,
  getTechnologyProfile,
  normalizeArchitectureProfile,
  normalizeTechnologyProfile,
} from "./opsxjProfileCatalog.js";
import { auditRefinement } from "./refinementService.js";
import { readRunChecklist, resolveRunChecklistStage } from "./runChecklistService.js";

export const LEGACY_IMPACT_CATALOG = Object.freeze({
  docs_only: {
    label: "Documentacion",
    documents: ["01-ResumenTecnico.md"],
    evidence: [],
  },
  frontend_legacy: {
    label: "Cliente legacy",
    documents: ["01-ResumenTecnico.md", "02-ImpactoUI.md", "05-PruebasEvidencia.md"],
    evidence: ["unit"],
  },
  webforms_ui: {
    label: "WebForms UI",
    documents: ["01-ResumenTecnico.md", "02-ImpactoUI.md", "03-FlujoWebForms.md", "05-PruebasEvidencia.md"],
    evidence: ["manual_qa"],
  },
  backend_vb: {
    label: "Backend VB.NET",
    documents: ["01-ResumenTecnico.md", "03-ServiciosYReglas.md", "05-PruebasEvidencia.md"],
    evidence: ["unit"],
  },
  handler_integration: {
    label: "Handler o integracion",
    documents: ["01-ResumenTecnico.md", "03-ServiciosYReglas.md", "04-ContratosIntegracion.md", "05-PruebasEvidencia.md"],
    evidence: ["manual_qa"],
  },
  database: {
    label: "Base de datos",
    documents: ["01-ResumenTecnico.md", "03-ServiciosYReglas.md", "04-ContratosIntegracion.md", "05-PruebasEvidencia.md"],
    evidence: ["manual_qa"],
  },
  cross_cutting: {
    label: "Transversal",
    documents: ["01-ResumenTecnico.md", "02-ImpactoUI.md", "03-ServiciosYReglas.md", "04-ContratosIntegracion.md", "05-PruebasEvidencia.md"],
    evidence: ["unit", "manual_qa"],
  },
});

const DOCUMENTATION_SECTION_CATALOG = Object.freeze({
  "01-ResumenTecnico.md": ["## Objetivo", "## Alcance y compatibilidad"],
  "02-ImpactoUI.md": ["## Superficies UI", "## Validacion visual"],
  "03-FlujoWebForms.md": ["## Flujo", "## Riesgos"],
  "03-ServiciosYReglas.md": ["## Servicios y reglas"],
  "04-ContratosIntegracion.md": ["## Contratos e integraciones"],
  "05-PruebasEvidencia.md": ["## Evidencia requerida", "## QA/E2E WebForms"],
});

const CLOSURE_RESTRICTIONS = Object.freeze([
  { name: "tbd", pattern: /\bTBD\b/i },
  { name: "template_comment", pattern: /<!--|-->/ },
  { name: "open_checklist", pattern: /^\s*-\s+\[\s\]/m },
  {
    name: "template_instruction",
    pattern:
      /^(?!\s*\|)\s*(?:describir|documentar|registrar|completar)\s+(?:el|la|los|las|aqui|aquí|pendiente|<)/im,
  },
]);

const exists = async (targetPath) => access(targetPath).then(() => true).catch(() => false);
const normalizeIssueKey = (value) => String(value ?? "").trim().toUpperCase();
const getChangeRootPath = ({ baseDir, changeName, changePath }) =>
  changePath ?? path.join(baseDir, "openspec", "changes", changeName);
const toRepoPath = (value) => String(value).replace(/\\/g, "/");
const resolveChangeArtifactRelativePath = ({ baseDir, changeName, changePath, relativePath }) => {
  const normalized = toRepoPath(relativePath);
  if (!changePath) return normalized;

  const activeRoot = path.posix.join("openspec", "changes", changeName);
  const actualRoot = toRepoPath(path.relative(baseDir, getChangeRootPath({ baseDir, changeName, changePath })));
  if (normalized === activeRoot) return actualRoot;
  return normalized.startsWith(`${activeRoot}/`)
    ? `${actualRoot}${normalized.slice(activeRoot.length)}`
    : normalized;
};

export const normalizeImpact = (value) => {
  const impact = String(value ?? "cross_cutting").trim().toLowerCase();
  if (!LEGACY_IMPACT_CATALOG[impact]) {
    throw new Error(`Impacto no soportado: ${value}. Use: ${Object.keys(LEGACY_IMPACT_CATALOG).join(", ")}.`);
  }
  return impact;
};

export const buildTechnicalDocumentation = ({ issueKey, changeName, summary, impact }) => {
  const resolvedImpact = normalizeImpact(impact);
  const catalog = LEGACY_IMPACT_CATALOG[resolvedImpact];
  const context = [
    `# ${summary || changeName}`,
    "",
    `- Ticket: ${normalizeIssueKey(issueKey)}`,
    `- Cambio OpenSpec: ${changeName}`,
    `- Clasificacion: ${resolvedImpact} (${catalog.label})`,
    "",
  ].join("\n");
  const templates = {
    "01-ResumenTecnico.md": `${context}## Objetivo\n\nDescribir el problema, la solucion aprobada y los componentes legacy afectados.\n\n## Alcance y compatibilidad\n\n- [ ] Identificar paginas, controles, servicios o scripts afectados.\n- [ ] Registrar comportamiento preservado y estrategia de reversa.\n`,
    "02-ImpactoUI.md": `${context}## Superficies UI\n\n- [ ] Paginas WebForms, UserControls, modales y tablas afectadas.\n- [ ] Estados de foco, hover, seleccion, responsive y accesibilidad.\n\n## Validacion visual\n\nRegistrar captura o recorrido manual reproducible cuando aplique.\n`,
    "03-FlujoWebForms.md": `${context}## Flujo\n\nDocumentar evento, postback/UpdatePanel, estado oculto, code-behind y respuesta visible.\n\n## Riesgos\n\n- [ ] ViewState, validacion, callbacks y compatibilidad con navegadores soportados.\n`,
    "03-ServiciosYReglas.md": `${context}## Servicios y reglas\n\nDocumentar clases VB.NET/C#, reglas de negocio, dependencias y manejo de errores.\n`,
    "04-ContratosIntegracion.md": `${context}## Contratos e integraciones\n\nDocumentar handler, endpoint, payload, autenticacion, impacto de esquema y compatibilidad.\n`,
    "05-PruebasEvidencia.md": `${context}## Evidencia requerida\n\n${catalog.evidence.length === 0 ? "No exige evidencia ejecutable adicional por clasificacion; registrar revision documental." : catalog.evidence.map((type) => `- [ ] ${type}: comando, resultado, fecha y referencia verificable.`).join("\n")}\n\n## QA/E2E WebForms\n\nLas pruebas E2E automatizadas no se suponen disponibles. Cuando aplique, registrar ambiente, pasos manuales, resultado y limitacion; si hay automatizacion real, adjuntar comando y reporte.\n`,
  };
  return Object.fromEntries(catalog.documents.map((name) => [name, templates[name]]));
};

const buildDocumentationContract = ({ documentationPaths, issueKey, changeName, impact }) =>
  documentationPaths.map((relativePath) => {
    const fileName = path.basename(relativePath);
    return {
      path: relativePath,
      requiredSections: DOCUMENTATION_SECTION_CATALOG[fileName] ?? [],
      minimumContentLength: 180,
      identity: {
        issueKey: normalizeIssueKey(issueKey),
        changeName,
        impact,
      },
      closureRestrictions: CLOSURE_RESTRICTIONS.map((item) => item.name),
    };
  });

export const writeLegacyGovernanceArtifacts = async ({
  baseDir,
  issueKey,
  changeName,
  summary,
  impact = "cross_cutting",
  architectureProfile,
  technologyProfile,
  profileArtifactPaths,
  refinementPath,
}) => {
  const resolvedImpact = normalizeImpact(impact);
  const resolvedArchitectureProfile = normalizeArchitectureProfile(architectureProfile);
  const resolvedTechnologyProfile = normalizeTechnologyProfile(technologyProfile);
  const catalog = LEGACY_IMPACT_CATALOG[resolvedImpact];
  const relativeDocumentationDir = path.join("Doc", "Tecnica", "Opsxj", changeName);
  const documentationDir = path.join(baseDir, relativeDocumentationDir);
  const files = buildTechnicalDocumentation({ issueKey, changeName, summary, impact: resolvedImpact });
  await mkdir(documentationDir, { recursive: true });
  const documentationPaths = [];
  for (const [fileName, content] of Object.entries(files)) {
    const filePath = path.join(documentationDir, fileName);
    await writeFile(filePath, content, "utf8");
    documentationPaths.push(filePath);
  }

  const manifest = {
    version: refinementPath ? 3 : 2,
    issueKey: normalizeIssueKey(issueKey),
    changeName,
    impact: resolvedImpact,
    requiredEvidence: catalog.evidence,
    documentation: documentationPaths.map((filePath) => path.relative(baseDir, filePath).replace(/\\/g, "/")),
    documentationContract: buildDocumentationContract({
      documentationPaths: documentationPaths.map((filePath) => path.relative(baseDir, filePath).replace(/\\/g, "/")),
      issueKey,
      changeName,
      impact: resolvedImpact,
    }),
    ...(refinementPath
      ? {
          refinement: {
            version: 1,
            required: true,
            path: refinementPath,
            state: "approved",
            taskOriginFormat: "Origen: D-XX, RQ-XX",
            enforcement:
              "Cada decision debe llegar a design, spec y tasks; no se admiten reglas de framework ajeno al perfil tecnologico.",
          },
        }
      : {}),
    ...(resolvedTechnologyProfile
      ? {
          technologyProfile: {
            ...getTechnologyProfile(resolvedTechnologyProfile),
          },
        }
      : {}),
    ...(resolvedArchitectureProfile
      ? {
          architectureProfile: {
            ...getArchitectureProfile(resolvedArchitectureProfile),
            artifactPaths: profileArtifactPaths ?? {},
          },
        }
      : {}),
  };
  const manifestPath = path.join(baseDir, "openspec", "changes", changeName, "opsxj-governance.json");
  await writeFile(manifestPath, `${JSON.stringify(manifest, null, 2)}\n`, "utf8");
  return { manifest, manifestPath, documentationPaths };
};

export const readLegacyGovernanceManifest = async ({ baseDir, changeName, changePath }) => {
  const manifestPath = path.join(getChangeRootPath({ baseDir, changeName, changePath }), "opsxj-governance.json");
  if (!(await exists(manifestPath))) return null;
  return { manifestPath, manifest: JSON.parse(await readFile(manifestPath, "utf8")) };
};

const countPendingTasks = async (tasksPath) => {
  const content = await readFile(tasksPath, "utf8").catch(() => "");
  return (content.match(/^\s*-\s+\[\s\]/gm) ?? []).length;
};

const getDocumentationChecks = async ({ baseDir, contract }) => {
  const checks = [];
  const relativePath = contract.path;
  const absolutePath = path.join(baseDir, relativePath);
  const present = await exists(absolutePath);
  checks.push({ name: `document:${relativePath}:exists`, status: present ? "PASS" : "FAIL" });
  if (!present) return checks;

  const content = await readFile(absolutePath, "utf8");
  const substantive = content.trim().length >= contract.minimumContentLength;
  checks.push({
    name: `document:${relativePath}:content`,
    status: substantive ? "PASS" : "FAIL",
    details: { minimumContentLength: contract.minimumContentLength, actualContentLength: content.trim().length },
  });

  for (const section of contract.requiredSections ?? []) {
    checks.push({
      name: `document:${relativePath}:section:${section}`,
      status: content.includes(section) ? "PASS" : "FAIL",
    });
  }

  const identityMarkers = [
    `- Ticket: ${contract.identity.issueKey}`,
    `- Cambio OpenSpec: ${contract.identity.changeName}`,
    `- Clasificacion: ${contract.identity.impact}`,
  ];
  for (const marker of identityMarkers) {
    checks.push({
      name: `document:${relativePath}:identity:${marker}`,
      status: content.includes(marker) ? "PASS" : "FAIL",
    });
  }

  for (const restrictionName of contract.closureRestrictions ?? []) {
    const restriction = CLOSURE_RESTRICTIONS.find((item) => item.name === restrictionName);
    if (!restriction) continue;
    checks.push({
      name: `document:${relativePath}:closure:${restrictionName}`,
      status: restriction.pattern.test(content) ? "FAIL" : "PASS",
    });
  }
  return checks;
};

const getArchitectureProfileChecks = async ({ baseDir, changeName, changePath, manifest }) => {
  const profile = manifest.architectureProfile;
  if (!profile) return [];

  const checks = [];
  let catalogProfile = null;
  try {
    catalogProfile = getArchitectureProfile(profile.name);
  } catch {
    catalogProfile = null;
  }
  const isKnown = Boolean(catalogProfile) && catalogProfile.version === profile.version;
  checks.push({
    name: "architecture_profile:catalog",
    status: isKnown ? "PASS" : "FAIL",
    details: { name: profile.name, version: profile.version },
  });
  if (!isKnown) return checks;

  for (const [artifact, marker] of Object.entries(profile.artifactMarkers ?? {})) {
    const relativePath = profile.artifactPaths?.[artifact];
    if (!relativePath) {
      checks.push({ name: `architecture_profile:${artifact}:path`, status: "FAIL" });
      continue;
    }
    const absolutePath = path.join(
      baseDir,
      resolveChangeArtifactRelativePath({ baseDir, changeName, changePath, relativePath }),
    );
    const present = await exists(absolutePath);
    checks.push({ name: `architecture_profile:${artifact}:exists`, status: present ? "PASS" : "FAIL" });
    if (!present) continue;
    const content = await readFile(absolutePath, "utf8");
    checks.push({
      name: `architecture_profile:${artifact}:marker`,
      status: content.includes(marker) ? "PASS" : "FAIL",
      details: { marker },
    });
  }

  const tasksPath = profile.artifactPaths?.tasks;
  const resolvedTasksPath = tasksPath
    ? resolveChangeArtifactRelativePath({ baseDir, changeName, changePath, relativePath: tasksPath })
    : null;
  if (resolvedTasksPath && (await exists(path.join(baseDir, resolvedTasksPath)))) {
    const tasksContent = await readFile(path.join(baseDir, resolvedTasksPath), "utf8");
    for (const marker of profile.requiredTaskMarkers ?? []) {
      checks.push({
        name: `architecture_profile:task:${marker}`,
        status: tasksContent.includes(marker) ? "PASS" : "FAIL",
      });
    }
  }
  return checks;
};

export const validateLegacyGovernance = async ({
  baseDir,
  changeName,
  changePath,
  env = process.env,
  currentSha = null,
}) => {
  const loaded = await readLegacyGovernanceManifest({ baseDir, changeName, changePath });
  if (!loaded) {
    return { applicable: false, status: "PASS", checks: [], message: "Cambio historico sin manifiesto de gobierno; compatibilidad preservada." };
  }
  const { manifest } = loaded;
  const checks = [];
  if (Array.isArray(manifest.documentationContract)) {
    for (const contract of manifest.documentationContract) {
      checks.push(...(await getDocumentationChecks({ baseDir, contract })));
    }
  } else {
    for (const relativePath of manifest.documentation ?? []) {
      checks.push({ name: `document:${relativePath}`, status: (await exists(path.join(baseDir, relativePath))) ? "PASS" : "FAIL" });
    }
  }
  checks.push(...(await getArchitectureProfileChecks({ baseDir, changeName, changePath, manifest })));
  if (manifest.refinement?.required) {
    const refinement = await auditRefinement({ baseDir, changeName, changePath });
    checks.push(...refinement.checks);
  }
  const pendingTasks = await countPendingTasks(path.join(getChangeRootPath({ baseDir, changeName, changePath }), "tasks.md"));
  checks.push({ name: "openspec_tasks", status: pendingTasks === 0 ? "PASS" : "FAIL", details: { pendingTasks } });
  if (env.OPSXJ_OPENSPEC_REVIEW_CONFIRMED) {
    checks.push({
      name: "openspec_review",
      status: "PASS",
      details: {
        state: "CONFIRMED",
        source: "environment",
        actor: env.OPSXJ_OPENSPEC_REVIEWED_BY || undefined,
      },
    });
  } else {
    const runChecklist = await readRunChecklist({ baseDir, issueKey: manifest.issueKey });
    const review = resolveRunChecklistStage({
      readResult: runChecklist,
      stage: "review",
      currentSha,
    });
    const status = review.state === "COMPLETE" ? "PASS" : "FAIL";
    const detail = review.state === "STALE"
      ? "La revision OpenSpec persistida corresponde a otro SHA; confirme nuevamente la revision actual."
      : review.state === "BLOCKED"
        ? "La ultima revision OpenSpec para el SHA actual fue rechazada."
        : "Falta una revision OpenSpec persistida para el SHA actual.";
    checks.push({
      name: "openspec_review",
      status,
      message: detail,
      details: {
        state: review.state,
        recordedAtUtc: review.recordedAtUtc,
        sha: review.sha,
        reference: review.reference,
        detail: review.detail,
      },
    });
  }

  const evidencePath = path.join(baseDir, ".opsxj", "evidence", `${manifest.issueKey}.json`);
  const evidence = (await exists(evidencePath)) ? JSON.parse(await readFile(evidencePath, "utf8")) : null;
  for (const requiredType of manifest.requiredEvidence) {
    const item = evidence?.items?.find((entry) => entry.type === requiredType && entry.status === "pass");
    const fresh = item && (!currentSha || item.sha === currentSha);
    checks.push({ name: `evidence:${requiredType}`, status: fresh ? "PASS" : "FAIL", details: item ?? null });
  }
  const failures = checks.filter((check) => check.status === "FAIL");
  return { applicable: true, status: failures.length === 0 ? "PASS" : "FAIL", manifest, checks, evidencePath, message: failures.length === 0 ? "Gobierno legacy validado." : `${failures.length} requisito(s) de gobierno pendiente(s).` };
};

export const writeValidationEvidence = async ({ baseDir, issueKey, type, status, reference, sha }) => {
  const normalizedType = String(type ?? "").trim().toLowerCase();
  if (!normalizedType) throw new Error("Falta --type para la evidencia.");
  if (!["unit", "manual_qa", "e2e", "build", "documentation"].includes(normalizedType)) {
    throw new Error("Tipo de evidencia no soportado. Use: unit, manual_qa, e2e, build, documentation.");
  }
  const normalizedStatus = String(status ?? "pass").trim().toLowerCase();
  if (!["pass", "fail", "not_applicable"].includes(normalizedStatus)) throw new Error("Estado de evidencia no soportado.");
  const filePath = path.join(baseDir, ".opsxj", "evidence", `${normalizeIssueKey(issueKey)}.json`);
  const prior = (await exists(filePath)) ? JSON.parse(await readFile(filePath, "utf8")) : { version: 1, issueKey: normalizeIssueKey(issueKey), items: [] };
  const nextItem = { type: normalizedType, status: normalizedStatus, reference: String(reference ?? "").trim(), sha: String(sha ?? "").trim(), recordedAtUtc: new Date().toISOString() };
  prior.items = prior.items.filter((item) => item.type !== normalizedType);
  prior.items.push(nextItem);
  await mkdir(path.dirname(filePath), { recursive: true });
  await writeFile(filePath, `${JSON.stringify(prior, null, 2)}\n`, "utf8");
  return { filePath, evidence: prior, item: nextItem };
};
