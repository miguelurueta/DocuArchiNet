import { access, mkdir, readFile, writeFile } from "node:fs/promises";
import path from "node:path";

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

const exists = async (targetPath) => access(targetPath).then(() => true).catch(() => false);
const normalizeIssueKey = (value) => String(value ?? "").trim().toUpperCase();

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

export const writeLegacyGovernanceArtifacts = async ({ baseDir, issueKey, changeName, summary, impact = "cross_cutting" }) => {
  const resolvedImpact = normalizeImpact(impact);
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
    version: 1,
    issueKey: normalizeIssueKey(issueKey),
    changeName,
    impact: resolvedImpact,
    requiredEvidence: catalog.evidence,
    documentation: documentationPaths.map((filePath) => path.relative(baseDir, filePath).replace(/\\/g, "/")),
  };
  const manifestPath = path.join(baseDir, "openspec", "changes", changeName, "opsxj-governance.json");
  await writeFile(manifestPath, `${JSON.stringify(manifest, null, 2)}\n`, "utf8");
  return { manifest, manifestPath, documentationPaths };
};

export const readLegacyGovernanceManifest = async ({ baseDir, changeName }) => {
  const manifestPath = path.join(baseDir, "openspec", "changes", changeName, "opsxj-governance.json");
  if (!(await exists(manifestPath))) return null;
  return { manifestPath, manifest: JSON.parse(await readFile(manifestPath, "utf8")) };
};

const countPendingTasks = async (tasksPath) => {
  const content = await readFile(tasksPath, "utf8").catch(() => "");
  return (content.match(/^\s*-\s+\[\s\]/gm) ?? []).length;
};

export const validateLegacyGovernance = async ({ baseDir, changeName, env = process.env, currentSha = null }) => {
  const loaded = await readLegacyGovernanceManifest({ baseDir, changeName });
  if (!loaded) {
    return { applicable: false, status: "PASS", checks: [], message: "Cambio historico sin manifiesto de gobierno; compatibilidad preservada." };
  }
  const { manifest } = loaded;
  const checks = [];
  for (const relativePath of manifest.documentation) {
    checks.push({ name: `document:${relativePath}`, status: (await exists(path.join(baseDir, relativePath))) ? "PASS" : "FAIL" });
  }
  const pendingTasks = await countPendingTasks(path.join(baseDir, "openspec", "changes", changeName, "tasks.md"));
  checks.push({ name: "openspec_tasks", status: pendingTasks === 0 ? "PASS" : "FAIL", details: { pendingTasks } });
  checks.push({ name: "openspec_review", status: env.OPSXJ_OPENSPEC_REVIEW_CONFIRMED ? "PASS" : "FAIL" });

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
