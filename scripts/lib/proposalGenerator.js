import { mkdir, writeFile } from "node:fs/promises";
import path from "node:path";

const toBullet = (items) => items.map((item) => `- ${item}`).join("\n");
const MAX_CHANGE_NAME_LENGTH = 96;

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

export const buildProposalContent = ({ issueKey, summary, description }) => {
  const safeSummary = summary?.trim() || `Propuesta basada en ${issueKey}`;
  const safeDescription = description?.trim() || "";

  const why = safeDescription
    ? `${safeSummary}. ${safeDescription.split("\n")[0]}`
    : `${safeSummary}. Se requiere estandarizar la propuesta OpenSpec desde Jira.`;

  const whatChanges = toBullet([
    `Se genera automaticamente una propuesta OpenSpec basada en el issue ${issueKey}.`,
    "Se incluye el resumen y descripcion del ticket como contexto inicial.",
    "Se guarda la propuesta en el arbol de cambios de OpenSpec.",
  ]);

  const capabilities = [
    "### New Capabilities",
    `- \`jira-proposal-generator\`: Generacion automatica de propuestas OpenSpec desde Jira.`,
    "",
    "### Modified Capabilities",
    "- ",
  ].join("\n");

  const impact = toBullet([
    "Nuevo script de generacion en `scripts/`.",
    "Nuevo archivo `openspec/changes/<issueKey>/proposal.md`.",
  ]);

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
