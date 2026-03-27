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

export const buildProposalContent = ({ issueKey, summary, description }) => {
  const safeSummary = summary?.trim() || `Propuesta basada en ${issueKey}`;
  const safeDescription = description?.trim() || "";
  const intent = inferProposalIntent({ summary: safeSummary, description: safeDescription });
  const descriptionLead = safeDescription.split("\n")[0]?.trim();

  const why = descriptionLead
    ? `${intent.whyDescription}. ${descriptionLead}`
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

  return [
    "## Why",
    "",
    why,
    "",
    "## What Changes",
    "",
    whatChanges,
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
