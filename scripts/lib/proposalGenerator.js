import { mkdir, writeFile } from "node:fs/promises";
import path from "node:path";

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

const toMarkdownQuote = (value) =>
  collapseBlankLines(value)
    .split("\n")
    .map((line) => `> ${line}`)
    .join("\n");

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

const buildInitialDesignContent = ({ issueKey, summary, description }) => {
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
    "1. TBD",
    "",
    "## Risks / Trade-offs",
    "",
    "- TBD",
    "",
    "## Migration Plan",
    "",
    "1. TBD",
    "",
    "## Open Questions",
    "",
    "- TBD",
    "",
  ].join("\n");
};

const buildInitialSpecContent = ({ issueKey, summary, description }) => {
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
    cleanDescription
      ? ["### Requirement: Detalle funcional Jira", "El sistema SHALL considerar las reglas detalladas del ticket.", "", "#### Scenario: Reglas del ticket", ...cleanDescription.split("\n").map((line) => `- ${line}`), ""].join("\n")
      : "",
  ]
    .filter(Boolean)
    .join("\n");
};

const buildInitialTasksContent = () => [
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

export const buildProposalContent = ({ issueKey, summary, description, metadata }) => {
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

export const writeRefinementArtifacts = async ({
  issueKey,
  changeName,
  summary,
  description,
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

  await writeFile(
    designPath,
    buildInitialDesignContent({ issueKey, summary, description }),
    "utf8",
  );
  await writeFile(specPath, buildInitialSpecContent({ issueKey, summary, description }), "utf8");
  await writeFile(tasksPath, buildInitialTasksContent(), "utf8");

  return { designPath, specPath, tasksPath, capability };
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
