import { mkdir, writeFile } from "node:fs/promises";
import path from "node:path";
import { writeLegacyGovernanceArtifacts } from "./legacyGovernanceService.js";
import {
  getArchitectureProfile,
  getTechnologyProfile,
  normalizeArchitectureProfile,
  normalizeTechnologyProfile,
} from "./opsxjProfileCatalog.js";
import { writeInitialRefinementArtifact } from "./refinementService.js";

const toBullet = (items) => items.map((item) => `- ${item}`).join("\n");
const MAX_CHANGE_NAME_LENGTH = 96;
const COMPONENT_SUMMARY_PREFIXES = [
  "crea-componente-",
  "crear-componente-",
  "creacion-componente-",
  "componente-",
];

export const slugifyForOpenSpec = (value) => {
  const normalized = (value ?? "")
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .replace(/-{2,}/g, "-");
  return normalized;
};

export const buildChangeNameFromJiraSummary = ({ issueKey, summary }) => {
  const issueSlug = slugifyForOpenSpec(issueKey);
  const summarySlug = slugifyForOpenSpec(summary);
  const base = summarySlug ? `${issueSlug}-${summarySlug}` : issueSlug;
  return base.slice(0, MAX_CHANGE_NAME_LENGTH).replace(/-+$/g, "");
};

const toTitleCase = (value) =>
  String(value ?? "")
    .split("-")
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
    .join("");

const normalizeLineEndings = (value) => String(value ?? "").replace(/\r\n/g, "\n");

const collapseBlankLines = (value) =>
  normalizeLineEndings(value)
    .replace(/\n{3,}/g, "\n\n")
    .trim();

const APP_RESPONSE_POLICY_TASKS = [
  "Crear o reutilizar `src/shared/api/appResponseError.ts` antes de agregar nuevos parsers locales de `AppResponses<T>`.",
  "Usar `getUserVisibleAppResponseMessage` para mensajes visibles de UI.",
  "Priorizar `UserMessage/userMessage` antes de `Message/message/errorMessage`.",
  "Usar `response.message` solo si el helper confirma que no contiene detalle tecnico.",
  "No mostrar `requestId`, `code=`, SQL, rutas, stack trace, tokens ni mensajes internos en UI.",
  "Agregar prueba donde `UserMessage` gane sobre un `response.message` tecnico con `code` y `requestId`.",
  "Registrar diagnostico tecnico completo solo con `logAppResponseErrorDiagnostic` y solo bajo `window.__APP_RESPONSE_DEBUG__ = true`.",
  "Exponer `errorsDebugOn()` / `errorsDebugOff()` como alias de consola para activar y apagar `window.__APP_RESPONSE_DEBUG__`.",
  "No crear `console.error`, `console.warn` o `console.info` locales que impriman payloads completos de `AppResponses<T>` fuera del helper.",
];

const buildAppResponsePolicyTasks = () => [
  "## Politica Frontend AppResponses<T>",
  "",
  "Cuando el ticket cree o modifique servicios, hooks, componentes o flujos que consuman APIs con `AppResponses<T>`:",
  "",
  ...APP_RESPONSE_POLICY_TASKS.map((item) => `- [ ] ${item}`),
  "",
  "Bloqueo estricto gradual: si `src/shared/api/appResponseError.ts` aun no existe, sembrar como primer paso obligatorio crearlo o reutilizarlo; despues de existir, los nuevos consumidores deben delegar la resolucion de errores al helper.",
  "",
].join("\n");

const buildAppResponsePolicyDesign = () => [
  "## Politica AppResponses<T>",
  "",
  "- Los tickets que consuman `AppResponses<T>` deben centralizar mensajes visibles en `src/shared/api/appResponseError.ts`.",
  "- No se deben duplicar parsers locales para resolver `UserMessage`, `requestId`, `code` o sanitizacion de mensajes tecnicos.",
  "- `response.message` se considera potencialmente tecnico y solo puede mostrarse si el helper confirma que no contiene senales internas.",
  "- El diagnostico completo queda limitado a `logAppResponseErrorDiagnostic` con `window.__APP_RESPONSE_DEBUG__ = true`; la consola puede activarse con `errorsDebugOn()` y apagarse con `errorsDebugOff()`.",
  "- Esta politica es gradual: el bloqueo estricto de nuevos consumidores aplica cuando el helper existe fisicamente.",
  "",
].join("\n");

const buildAppResponsePolicySpec = () => [
  "### Requirement: Politica Frontend AppResponses",
  "El sistema SHALL sembrar reglas de consumo seguro de `AppResponses<T>` en los artefactos iniciales cuando un ticket cree o modifique consumidores de API.",
  "",
  "#### Scenario: No filtrado de mensajes tecnicos",
  "- **WHEN** un endpoint `AppResponses<T>` retorna `errors[0].UserMessage` y un `response.message` con `code`, `requestId`, SQL, rutas, stack trace, tokens o detalle interno",
  "- **THEN** la UI muestra el mensaje funcional resuelto por `getUserVisibleAppResponseMessage` y no muestra el detalle tecnico.",
  "",
  "#### Scenario: Diagnostico tecnico controlado",
  "- **WHEN** soporte activa `errorsDebugOn()` o `window.__APP_RESPONSE_DEBUG__ = true` desde la consola",
  "- **THEN** el diagnostico completo puede registrarse solo con `logAppResponseErrorDiagnostic` y sin persistir ni transmitir payloads tecnicos.",
  "",
].join("\n");

const shouldApplyAppResponsePolicy = (technologyProfile) =>
  normalizeTechnologyProfile(technologyProfile) === "frontend-react-ts";

const buildTechnologyProfileSection = (technologyProfile) => {
  if (!technologyProfile) return "";
  const profile = getTechnologyProfile(technologyProfile);
  return [
    "## Perfil tecnologico",
    "",
    `- Perfil de revision: ${profile.name} (v${profile.version}).`,
    `- Alcance: ${profile.description}`,
    "- Este perfil orienta la revision tecnica; no reemplaza los requisitos funcionales ni arquitectonicos del ticket.",
    "",
  ].join("\n");
};

const buildEnterpriseLegacyProposalPolicy = () => [
  "## Politica de modernizacion enterprise legacy",
  "",
  "- Separar Presentation, Application, Domain e Infrastructure; Presentation no contiene reglas de negocio.",
  "- Usar DTOs y modelos tipados en fronteras; validar las reglas de seguridad y negocio en servidor.",
  "- Reutilizar infraestructura de datos comun, pero mantener repositorios especificos por dominio con consultas parametrizadas.",
  "- No crear un `GenericRepository` que mezcle dominios o contratos de negocio.",
  "- Definir un Gateway/Adapter tipado por capacidad como unico acceso nuevo al comportamiento legacy.",
  "- Presentation y Application no invocan directamente clases legacy, Session, controles UI, SQL legacy ni cambios de estado.",
  "- Aplicar convivencia gradual, bandera de funcionalidad, piloto, rollback y pruebas de equivalencia antes de sustituir el flujo anterior.",
  "",
].join("\n");

const buildEnterpriseLegacyDesignPolicy = () => [
  "## Arquitectura de modernizacion enterprise legacy",
  "",
  "1. Presentation recibe eventos y renderiza resultados; delega el caso de uso a Application sin reglas de negocio.",
  "2. Application orquesta casos de uso mediante DTOs y contratos tipados; Domain concentra reglas e invariantes.",
  "3. Infrastructure comparte conexiones, transacciones, telemetria y acceso parametrizado; cada dominio mantiene su repositorio propio.",
  "4. Cada capacidad legacy expone un Gateway/Adapter tipado. El adapter encapsula conversiones, permisos y trazabilidad sin un dispatcher dinamico ni un adaptador universal.",
  "5. Ningun codigo nuevo de Presentation/Application lee Session, controles Web Forms, SQL legacy o cambia estado directamente; usa el contrato de la capacidad.",
  "6. La entrega mantiene el camino anterior hasta validar equivalencia mediante piloto, feature flag y rollback documentado.",
  "",
].join("\n");

const buildEnterpriseLegacySpecPolicy = () => [
  "### Requirement: Frontera de capacidad legacy",
  "El sistema SHALL encapsular toda capacidad legacy modernizada mediante un Gateway/Adapter tipado por dominio y SHALL mantener separadas Presentation, Application, Domain e Infrastructure.",
  "",
  "#### Scenario: Invocacion desde codigo nuevo",
  "- **WHEN** Presentation o Application requieren ejecutar una capacidad legacy",
  "- **THEN** usan el contrato tipado del Gateway/Adapter y no invocan directamente clases legacy, Session, controles de interfaz, SQL legacy ni cambios de estado",
  "",
  "#### Scenario: Convivencia gradual",
  "- **WHEN** se habilita la implementacion modernizada",
  "- **THEN** existe validacion de servidor, pruebas de equivalencia, piloto, feature flag y rollback antes de retirar el camino anterior",
  "",
].join("\n");

const buildEnterpriseLegacyTasks = () => [
  "## Gobierno de modernizacion enterprise legacy",
  "",
  "- [ ] Definir el Gateway/Adapter tipado por capacidad y sus DTOs/modelos; no crear un adaptador universal.",
  "- [ ] Mantener Presentation/Application sin acceso directo a clases legacy, Session, controles UI, SQL legacy ni cambios de estado.",
  "- [ ] Usar Infrastructure comun reutilizable y repositorio por dominio con consultas parametrizadas; no crear GenericRepository interdominio.",
  "- [ ] Implementar validacion de servidor y manejo de errores compatible con el flujo actual.",
  "- [ ] Ejecutar pruebas de equivalencia contra el camino legacy y registrar sus resultados.",
  "- [ ] Definir piloto y rollback antes de habilitar la capacidad mediante feature flag o mecanismo equivalente.",
  "",
].join("\n");

const profilePolicy = ({ architectureProfile, technologyProfile, artifact }) => {
  const normalizedArchitectureProfile = normalizeArchitectureProfile(architectureProfile);
  const normalizedTechnologyProfile = normalizeTechnologyProfile(technologyProfile);
  const technology = buildTechnologyProfileSection(normalizedTechnologyProfile);
  if (normalizedArchitectureProfile !== "enterprise-legacy-modernization") return technology;
  const enterprise = {
    proposal: buildEnterpriseLegacyProposalPolicy,
    design: buildEnterpriseLegacyDesignPolicy,
    spec: buildEnterpriseLegacySpecPolicy,
    tasks: buildEnterpriseLegacyTasks,
  }[artifact]();
  return [technology, enterprise].filter(Boolean).join("\n");
};

const toMarkdownQuote = (value) =>
  collapseBlankLines(value)
    .split("\n")
    .map((line) => `> ${line}`)
    .join("\n");

const renderMetadataSection = (metadata) => {
  const metadataLines = [];
  if (metadata?.issueType) metadataLines.push(`- Tipo: ${metadata.issueType}`);
  if (metadata?.priority) metadataLines.push(`- Prioridad: ${metadata.priority}`);
  if (Array.isArray(metadata?.components) && metadata.components.length > 0) {
    metadataLines.push(`- Componentes: ${metadata.components.join(", ")}`);
  }
  if (Array.isArray(metadata?.labels) && metadata.labels.length > 0) {
    metadataLines.push(`- Labels: ${metadata.labels.join(", ")}`);
  }
  if (Array.isArray(metadata?.subtasks) && metadata.subtasks.length > 0) {
    metadataLines.push(
      ...metadata.subtasks.map((item) => `- Subtask ${item.key}: ${item.summary || "(sin resumen)"}`),
    );
  }
  if (Array.isArray(metadata?.comments) && metadata.comments.length > 0) {
    metadataLines.push(
      ...metadata.comments.map((item) => `- Comment ${item.id}: ${item.body || "(sin contenido)"}`),
    );
  }
  return metadataLines;
};

export const inferProposalIntent = ({ summary, description }) => {
  const safeSummary = summary?.trim() || "";
  const summarySlug = slugifyForOpenSpec(safeSummary);
  const componentPrefix = COMPONENT_SUMMARY_PREFIXES.find((prefix) =>
    summarySlug.startsWith(prefix),
  );

  if (componentPrefix) {
    const componentSlug = summarySlug.slice(componentPrefix.length);

    if (componentSlug) {
      const capability = `app-${componentSlug}`;
      const componentName = `App${toTitleCase(componentSlug)}`;

      return {
        kind: "component",
        capability,
        capabilityDescription: `Componente reusable ${componentName} para la capa UI compartida del proyecto.`,
        whatChanges: [
          `Se formaliza la propuesta OpenSpec para implementar ${componentName} a partir del ticket Jira.`,
          `Se define la capability \`${capability}\` como parte de la capa UI reutilizable.`,
          "Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.",
        ],
        impact: [
          `Nuevo componente compartido en \`src/app/Components/UI/${componentName}/\`.`,
          "Posible integracion inicial en un modulo consumidor real del proyecto.",
          "Nuevas pruebas de comportamiento para el contrato reusable del componente.",
        ],
        whyDescription: safeSummary,
      };
    }
  }

  const fallbackSlug = summarySlug || "ticket-change";
  return {
    kind: "generic",
    capability: fallbackSlug,
    capabilityDescription:
      "Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.",
    whatChanges: [
      "Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.",
      "Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.",
      "Se deja lista una base coherente para continuar con design, specs y tasks.",
    ],
    impact: [
      "Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.",
      "Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.",
    ],
    whyDescription: description?.trim() ? safeSummary : `${safeSummary}. Se requiere refinar el alcance funcional del ticket en OpenSpec.`,
  };
};

const buildInitialDesignContent = ({ issueKey, summary, description, architectureProfile, technologyProfile }) => {
  const cleanDescription = collapseBlankLines(description);
  const detailsSection = cleanDescription
    ? ["## Jira Details", "", toMarkdownQuote(cleanDescription), ""].join("\n")
    : "";

  return [
    "## Context",
    "",
    `${issueKey}: ${summary}`,
    "",
    detailsSection,
    "## Goals / Non-Goals",
    "",
    "**Goals**",
    "- Refinar alcance tecnico usando el contexto completo de Jira.",
    "- Definir decisiones arquitectonicas, riesgos y plan de migracion.",
    "",
    "**Non-Goals**",
    "- Cambios fuera del alcance descrito por el ticket.",
    "",
    "## Decisions",
    "",
    ...(shouldApplyAppResponsePolicy(technologyProfile)
      ? [
          "1. Aplicar politica central de AppResponses<T> para evitar parsers locales y filtrado de mensajes tecnicos en UI.",
          "",
          buildAppResponsePolicyDesign(),
        ]
      : ["1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.", ""]),
    profilePolicy({ architectureProfile, technologyProfile, artifact: "design" }),
    "## Risks / Trade-offs",
    "",
    "- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.",
    "",
    "## Migration Plan",
    "",
    "1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.",
    "2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.",
    "",
    "## Open Questions",
    "",
    "- TBD",
    "",
  ].join("\n");
};

const buildInitialSpecContent = ({ issueKey, summary, description, architectureProfile, technologyProfile }) => {
  const cleanDescription = collapseBlankLines(description);

  return [
    "## ADDED Requirements",
    "",
    `### Requirement: ${summary}`,
    `El sistema SHALL implementar el alcance definido para ${issueKey}.`,
    "",
    "#### Scenario: Flujo principal",
    "- **WHEN** se ejecuta el caso de uso principal del ticket",
    "- **THEN** el comportamiento coincide con las reglas funcionales esperadas",
    "",
    "#### Scenario: No-regresion",
    "- **WHEN** se valida el modulo afectado",
    "- **THEN** no se rompen flujos existentes",
    "",
    ...(shouldApplyAppResponsePolicy(technologyProfile) ? [buildAppResponsePolicySpec()] : []),
    profilePolicy({ architectureProfile, technologyProfile, artifact: "spec" }),
    cleanDescription
      ? ["### Requirement: Detalle funcional Jira", "El sistema SHALL considerar las reglas detalladas del ticket.", "", "#### Scenario: Reglas del ticket", ...cleanDescription.split("\n").map((line) => `- ${line}`), ""].join("\n")
      : "",
  ]
    .filter(Boolean)
    .join("\n");
};

const buildInitialTasksContent = ({ architectureProfile, technologyProfile } = {}) => [
  "## 1. Refinement",
  "",
  "- [ ] 1.1 Consolidar alcance final desde Jira + contexto de codigo.",
  "- [ ] 1.2 Ajustar design/spec con decisiones y riesgos definitivos.",
  "",
  "## 2. Implementacion",
  "",
  "- [ ] 2.1 Implementar cambios funcionales del ticket.",
  "- [ ] 2.2 Mantener compatibilidad y evitar regresiones.",
  "",
  ...(shouldApplyAppResponsePolicy(technologyProfile) ? [buildAppResponsePolicyTasks()] : []),
  profilePolicy({ architectureProfile, technologyProfile, artifact: "tasks" }),
  "## 3. Pruebas",
  "",
  "- [ ] 3.1 Agregar/ajustar pruebas unitarias e integracion.",
  "- [ ] 3.2 Ejecutar suite afectada y registrar evidencia.",
  "",
  "## 4. Cierre",
  "",
  "- [ ] 4.1 Validar OpenSpec.",
  "- [ ] 4.2 Documentar diff final y decisiones de arquitectura.",
  "",
].join("\n");

export const buildProposalContent = ({ issueKey, summary, description, metadata, architectureProfile, technologyProfile }) => {
  const safeSummary = summary?.trim() || `Propuesta basada en ${issueKey}`;
  const safeDescription = description?.trim() || "";
  const intent = inferProposalIntent({ summary: safeSummary, description: safeDescription });
  const cleanDescription = collapseBlankLines(safeDescription);
  const why = cleanDescription
    ? `${intent.whyDescription}. Ver detalle funcional completo del ticket en la seccion Jira Details.`
    : intent.whyDescription;

  const whatChanges = toBullet([
    `Se genera automaticamente una propuesta OpenSpec basada en el issue ${issueKey}.`,
    ...intent.whatChanges,
  ]);

  const capabilities = [
    "### New Capabilities",
    `- \`${intent.capability}\`: ${intent.capabilityDescription}`,
    "",
    "### Modified Capabilities",
    "- ",
  ].join("\n");

  const impact = toBullet(intent.impact);
  const metadataLines = renderMetadataSection(metadata);

  return [
    "## Why",
    "",
    why,
    "",
    "## What Changes",
    "",
    whatChanges,
    "",
    "## Jira Details",
    "",
    cleanDescription ? toMarkdownQuote(cleanDescription) : "> (Sin descripcion detallada en Jira)",
    ...(metadataLines.length > 0 ? ["", "## Jira Metadata", "", ...metadataLines] : []),
    "",
    "## Capabilities",
    "",
    capabilities,
    "",
    "## Impact",
    "",
    impact,
    "",
    profilePolicy({ architectureProfile, technologyProfile, artifact: "proposal" }),
  ].join("\n");
};

const buildCapabilityName = ({ issueKey, changeName, fallbackCapability }) => {
  const issueSlug = slugifyForOpenSpec(issueKey);
  const normalizedChange = slugifyForOpenSpec(changeName);
  const suffix = normalizedChange.startsWith(`${issueSlug}-`)
    ? normalizedChange.slice(issueSlug.length + 1)
    : normalizedChange;

  const chosen = suffix || slugifyForOpenSpec(fallbackCapability) || issueSlug;
  return chosen || "ticket-change";
};

const buildJiraContextContent = ({ issueKey, summary, description, metadata }) => {
  const cleanDescription = collapseBlankLines(description);
  const metadataLines = renderMetadataSection(metadata);

  return [
    `# Jira Context - ${issueKey}`,
    "",
    `## Summary`,
    "",
    summary || "(sin resumen)",
    "",
    "## Description",
    "",
    cleanDescription ? toMarkdownQuote(cleanDescription) : "> (Sin descripcion detallada en Jira)",
    ...(metadataLines.length > 0 ? ["", "## Metadata", "", ...metadataLines] : []),
    "",
  ].join("\n");
};

export const writeRefinementArtifacts = async ({
  issueKey,
  changeName,
  summary,
  description,
  metadata,
  impact = "cross_cutting",
  architectureProfile,
  technologyProfile,
  baseDir,
}) => {
  const resolvedChangeName = slugifyForOpenSpec(changeName || issueKey);
  if (!resolvedChangeName) {
    throw new Error("No se pudo construir un nombre de carpeta OpenSpec valido.");
  }

  const intent = inferProposalIntent({ summary, description });
  const capability = buildCapabilityName({
    issueKey,
    changeName: resolvedChangeName,
    fallbackCapability: intent.capability,
  });

  const changeDir = path.join(baseDir, "openspec", "changes", resolvedChangeName);
  const specsDir = path.join(changeDir, "specs", capability);

  await mkdir(specsDir, { recursive: true });

  const designPath = path.join(changeDir, "design.md");
  const tasksPath = path.join(changeDir, "tasks.md");
  const specPath = path.join(specsDir, "spec.md");
  const jiraContextPath = path.join(specsDir, "jira-context.md");
  const refinementPath = await writeInitialRefinementArtifact({
    baseDir,
    issueKey,
    changeName: resolvedChangeName,
    summary,
    technologyProfile,
  });

  await writeFile(
    designPath,
    buildInitialDesignContent({ issueKey, summary, description, architectureProfile, technologyProfile }),
    "utf8",
  );
  await writeFile(specPath, buildInitialSpecContent({ issueKey, summary, description, architectureProfile, technologyProfile }), "utf8");
  await writeFile(tasksPath, buildInitialTasksContent({ architectureProfile, technologyProfile }), "utf8");
  await writeFile(
    jiraContextPath,
    buildJiraContextContent({ issueKey, summary, description, metadata }),
    "utf8",
  );

  const governanceArtifacts = await writeLegacyGovernanceArtifacts({
    baseDir,
    issueKey,
    changeName: resolvedChangeName,
    summary,
    impact,
    architectureProfile,
    technologyProfile,
    profileArtifactPaths: {
      proposal: path.join("openspec", "changes", resolvedChangeName, "proposal.md").replace(/\\/g, "/"),
      design: path.relative(baseDir, designPath).replace(/\\/g, "/"),
      spec: path.relative(baseDir, specPath).replace(/\\/g, "/"),
      tasks: path.relative(baseDir, tasksPath).replace(/\\/g, "/"),
    },
    refinementPath: path.relative(baseDir, refinementPath).replace(/\\/g, "/"),
  });

  return {
    designPath,
    specPath,
    tasksPath,
    jiraContextPath,
    refinementPath,
    capability,
    governanceArtifacts,
  };
};

export const writeProposalFile = async ({
  issueKey,
  changeName,
  content,
  baseDir,
}) => {
  const resolvedChangeName = slugifyForOpenSpec(changeName || issueKey);
  if (!resolvedChangeName) {
    throw new Error("No se pudo construir un nombre de carpeta OpenSpec valido.");
  }

  const changeDir = path.join(baseDir, "openspec", "changes", resolvedChangeName);
  await mkdir(changeDir, { recursive: true });
  const proposalPath = path.join(changeDir, "proposal.md");
  await writeFile(proposalPath, content, "utf8");
  return proposalPath;
};
