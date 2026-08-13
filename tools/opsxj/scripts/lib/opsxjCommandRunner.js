import path from "node:path";
import { execFile as execFileCb } from "node:child_process";
import { createInterface } from "node:readline/promises";
import { promisify } from "node:util";
import { createProposalFromJira } from "./jiraProposalService.js";
import { transitionJiraIssue } from "./jiraClient.js";
import {
  assertGitClean,
  assertGitCleanAndSynced,
  buildFeatureBranchName,
  setupProposalBranchAndCommit,
} from "./gitClient.js";
import { archiveWithPullRequest } from "./archiveWorkflowService.js";
import { closeIssueFromMergedPr } from "./closeWorkflowService.js";
import {
  applyTechnicalReviewCorrection,
  reviewTechnicalPrompt,
} from "./technicalPromptReviewService.js";
import { getOpsxjStatus } from "./opsxjStatusService.js";
import {
  normalizeImpact,
  validateLegacyGovernance,
  writeValidationEvidence,
} from "./legacyGovernanceService.js";
import {
  normalizeArchitectureProfile,
  normalizeTechnologyProfile,
} from "./opsxjProfileCatalog.js";

const execFile = promisify(execFileCb);

const usage = [
  "Uso:",
  "  node tools/opsxj/scripts/opsxj.js opsxj:new <ISSUE-KEY> [--impact <impact>] [--profile <architecture-profile>] [--tech-profile <technology-profile>]",
  "  node tools/opsxj/scripts/opsxj.js opsxj:orchestrate:new <ISSUE-KEY> [--impact <impact>] [--profile <architecture-profile>] [--tech-profile <technology-profile>]",
  "  node tools/opsxj/scripts/opsxj.js opsxj:validate <ISSUE-KEY|change-name> [--json]",
  "  node tools/opsxj/scripts/opsxj.js opsxj:validation:evidence <ISSUE-KEY> --type <unit|manual_qa|e2e|build|documentation> --reference <detalle>",
  "  node tools/opsxj/scripts/opsxj.js opsxj:prompt-review <PROMPT.md|ISSUE-KEY>",
  "  node tools/opsxj/scripts/opsxj.js opsxj:technical-review <PROMPT.md|ISSUE-KEY> [--tech-profile <technology-profile>]",
  "  node tools/opsxj/scripts/opsxj.js opsxj:status <ISSUE-KEY|change-name>",
  "  node tools/opsxj/scripts/opsxj.js opsxj:archive <ISSUE-KEY>",
  "  node tools/opsxj/scripts/opsxj.js opsxj:close <ISSUE-KEY>",
  "  npm run opsxj:new -- <ISSUE-KEY> [--impact <impact>] [--profile <architecture-profile>] [--tech-profile <technology-profile>]",
  "  npm run opsxj:orchestrate:new -- <ISSUE-KEY> [--impact <impact>] [--profile <architecture-profile>] [--tech-profile <technology-profile>]",
  "  npm run opsxj:prompt-review -- <PROMPT.md|ISSUE-KEY>",
  "  npm run opsxj:status -- <ISSUE-KEY|change-name>",
  "  npm run opsxj:archive -- <ISSUE-KEY>",
  "  npm run opsxj:close -- <ISSUE-KEY>",
].join("\n");

const parseBoolean = (value, fallback) => {
  if (value === undefined || value === null || value === "") {
    return fallback;
  }
  return String(value).toLowerCase() !== "false";
};

const buildNewContext = ({
  env,
  stdout,
  issueKey,
  issue,
  changeName,
  proposalPath,
  refinementArtifacts,
  baseDir,
  architectureProfile,
  technologyProfile,
}) => {
  const relativeProposalPath = path.relative(baseDir, proposalPath);
  const changeDir = path.join("openspec", "changes", changeName);

  stdout.write(`[opsxj:new] Ticket: ${issueKey}\n`);
  stdout.write(`[opsxj:new] Resumen Jira: ${issue.summary || "(sin resumen)"}\n`);
  stdout.write(`[opsxj:new] Carpeta OpenSpec: ${changeDir}\n`);
  stdout.write(`[opsxj:new] Proposal creado: ${relativeProposalPath}\n`);
  if (architectureProfile) {
    stdout.write(`[opsxj:new] Perfil de arquitectura: ${architectureProfile}\n`);
  }
  if (technologyProfile) {
    stdout.write(`[opsxj:new] Perfil tecnologico: ${technologyProfile}\n`);
  }
  if (refinementArtifacts) {
    const designRelative = path.relative(baseDir, refinementArtifacts.designPath);
    const specRelative = path.relative(baseDir, refinementArtifacts.specPath);
    const tasksRelative = path.relative(baseDir, refinementArtifacts.tasksPath);
    const jiraContextRelative = refinementArtifacts.jiraContextPath
      ? path.relative(baseDir, refinementArtifacts.jiraContextPath)
      : null;
    stdout.write(`[opsxj:new] Design creado: ${designRelative}\n`);
    stdout.write(`[opsxj:new] Spec creado: ${specRelative}\n`);
    stdout.write(`[opsxj:new] Tasks creado: ${tasksRelative}\n`);
    if (jiraContextRelative) {
      stdout.write(`[opsxj:new] Jira context creado: ${jiraContextRelative}\n`);
    }
    if (refinementArtifacts.governanceArtifacts) {
      stdout.write(
        `[opsxj:new] Gobierno creado: ${path.relative(baseDir, refinementArtifacts.governanceArtifacts.manifestPath)}\n`,
      );
    }
  }
  if (!env.JIRA_BASE_URL || !env.JIRA_EMAIL || !env.JIRA_API_TOKEN) {
    stdout.write(
      "[opsxj:new] Aviso: faltan variables JIRA_* en entorno; configure .env.jira para ejecucion estable.\n",
    );
  }
};

const printGitSummary = ({ stdout, gitResult }) => {
  stdout.write(`[opsxj:new] Rama Git: ${gitResult.branchName}\n`);
  if (gitResult.committed) {
    stdout.write(
      `[opsxj:new] Commit inicial creado con ${gitResult.proposalRelativePath}\n`,
    );
  } else {
    stdout.write(
      `[opsxj:new] Sin cambios para commit inicial (${gitResult.proposalRelativePath}).\n`,
    );
  }
  if (gitResult.pushed) {
    stdout.write("[opsxj:new] Rama enviada a remoto.\n");
  }
};

const moveJiraToInProgress = async ({
  env,
  issue,
  issueKey,
  stdout,
  transitionJiraIssueFn,
}) => {
  if (issue?.metadata?.statusCategory === "indeterminate") {
    stdout.write(
      `[opsxj:new] Jira ya esta en curso: ${issue.metadata.status || "estado indeterminado"}.\n`,
    );
    return null;
  }

  const transition = await transitionJiraIssueFn({
    issueKey,
    baseUrl: env.JIRA_BASE_URL,
    email: env.JIRA_EMAIL,
    apiToken: env.JIRA_API_TOKEN,
    target: "in_progress",
  });

  stdout.write(
    `[opsxj:new] Jira actualizado a: ${transition?.to?.name ?? transition?.name ?? "En curso"}.\n`,
  );
  return transition;
};

const printCodexAgentHint = ({ stdout, command }) => {
  if (command === "new") {
    stdout.write(
      "[opsxj:new] Sugerencia Codex: use subagente mini para lectura Jira, design/spec/tasks, creacion de artefactos e implementaciones pequenas; deje commit/push e integracion final al agente principal.\n",
    );
    return;
  }

  if (command === "archive") {
    stdout.write(
      "[opsxj:archive] Sugerencia Codex: use mini para consultas, chequeos focales y diagnostico puntual; deje verify, archive, revision final de diff y coordinacion del PR al agente principal.\n",
    );
    return;
  }

  if (command === "close") {
    stdout.write(
      "[opsxj:close] Sugerencia Codex: use mini solo para consulta de estado remoto; deje validar merge, cierre Jira y sincronizacion final del flujo al agente principal.\n",
    );
  }
};

const parseNewArgs = (rawArgs) => {
  const positional = [];
  const options = {};
  const optionNames = new Set(["--impact", "--profile", "--tech-profile"]);
  for (let index = 0; index < rawArgs.length; index += 1) {
    const value = String(rawArgs[index]);
    if (optionNames.has(value)) {
      const optionValue = rawArgs[index + 1];
      if (!optionValue || String(optionValue).startsWith("--")) {
        throw new Error(`Falta valor para ${value}.`);
      }
      options[value.slice(2)] = String(optionValue);
      index += 1;
      continue;
    }
    if (value.startsWith("--")) {
      throw new Error(`Opcion no soportada para opsxj:new: ${value}.`);
    }
    positional.push(value);
  }
  return { issueKey: positional[0] ?? "", options };
};

const runNew = async ({
  args,
  env,
  stdout,
  issueKeyFromArg,
  createProposalFn,
  baseDir,
  setupProposalFn,
  assertGitCleanFn,
  transitionJiraIssueFn,
}) => {
  const parsed = parseNewArgs(
    [issueKeyFromArg, ...args].filter((value) => value !== undefined && value !== null),
  );
  const impact = normalizeImpact(parsed.options.impact ?? env.OPSXJ_IMPACT ?? "cross_cutting");
  const architectureProfile = normalizeArchitectureProfile(
    parsed.options.profile ?? env.OPSXJ_ARCHITECTURE_PROFILE,
  );
  const technologyProfile = normalizeTechnologyProfile(
    parsed.options["tech-profile"] ?? env.OPSXJ_TECH_PROFILE,
  );
  const issueKey = parsed.issueKey || env.JIRA_ISSUE_KEY || "";
  if (!issueKey) {
    throw new Error(`Falta issueKey para opsxj:new.\n${usage}`);
  }

  const verifyGit = assertGitCleanFn ?? assertGitClean;
  await verifyGit({ baseDir, commandLabel: "opsxj:new" });

  const result = await createProposalFn({
    issueKey,
    baseUrl: env.JIRA_BASE_URL,
    email: env.JIRA_EMAIL,
    apiToken: env.JIRA_API_TOKEN,
    baseDir,
    commandName: "opsxj.js opsxj:new",
    folderStrategy: "summary",
    impact,
    architectureProfile,
    technologyProfile,
  });

  buildNewContext({
    env,
    stdout,
    issueKey,
    issue: result.issue,
    changeName: result.changeName,
    proposalPath: result.proposalPath,
    refinementArtifacts: result.refinementArtifacts,
    baseDir,
    architectureProfile,
    technologyProfile,
  });

  const gitResult = await setupProposalFn({
    baseDir,
    issueKey,
    proposalPath: result.proposalPath,
    autoPush: parseBoolean(env.GIT_AUTO_PUSH, false),
  });
  printGitSummary({ stdout, gitResult });
  await moveJiraToInProgress({
    env,
    issue: result.issue,
    issueKey,
    stdout,
    transitionJiraIssueFn,
  });
  printCodexAgentHint({ stdout, command: "new" });
};

const runArchive = async ({
  args,
  env,
  stdout,
  issueKeyFromArg,
  baseDir,
  archiveFn,
  assertGitCleanAndSyncedFn,
}) => {
  const issueKey = issueKeyFromArg ?? args[0] ?? env.JIRA_ISSUE_KEY ?? "";
  if (!issueKey) {
    throw new Error(`Falta issueKey para opsxj:archive.\n${usage}`);
  }

  const parseAlsoKeys = (rawArgs) => {
    const alsoIndex = rawArgs.findIndex((value) => value === "--also");
    if (alsoIndex === -1) return [];
    const raw = rawArgs[alsoIndex + 1] ?? "";
    return String(raw)
      .split(",")
      .map((value) => value.trim())
      .filter(Boolean);
  };

  const alsoIssueKeys = parseAlsoKeys(args);

  const branchName = buildFeatureBranchName(issueKey);
  const verifyGit = assertGitCleanAndSyncedFn ?? assertGitCleanAndSynced;
  await verifyGit({ baseDir, expectedBranchName: branchName });
  const result = await archiveFn({
    issueKey,
    alsoIssueKeys,
    baseDir,
    branchName,
    jira: {
      baseUrl: env.JIRA_BASE_URL,
      email: env.JIRA_EMAIL,
      apiToken: env.JIRA_API_TOKEN,
    },
    github: {
      token: env.GITHUB_TOKEN,
      repo: env.GITHUB_REPO,
      owner: env.GITHUB_OWNER,
      repoName: env.GITHUB_REPO_NAME,
      baseBranch: env.GITHUB_BASE_BRANCH || "main",
    },
    env,
  });

  const prUrl = result.pullRequest?.html_url ?? "(sin URL)";
  stdout.write(`[opsxj:archive] Ticket: ${String(issueKey).toUpperCase()}\n`);
  stdout.write(`[opsxj:archive] Cambio archivado: ${result.changeName}\n`);
  stdout.write(
    `[opsxj:archive] PR ${result.pullRequestCreated ? "creado" : "reutilizado"}: ${prUrl}\n`,
  );
  stdout.write(
    "[opsxj:archive] Jira comentado con enlace al PR. El cierre final se sincroniza cuando el PR se mergea/rechaza.\n",
  );
  if (result.archivedWithSkipSpecs) {
    stdout.write(
      "[opsxj:archive] Aviso: archive ejecuto fallback con --skip-specs.\n",
    );
  }
  if (alsoIssueKeys.length > 0) {
    stdout.write(
      `[opsxj:archive] Cambios adicionales movidos a archive: ${alsoIssueKeys.join(", ")}\n`,
    );
  }
  printCodexAgentHint({ stdout, command: "archive" });
};

const printPromptReviewResult = ({ stdout, result }) => {
  const label =
    result.status === "error"
      ? "ERROR prompt-review"
      : result.status === "fail"
        ? "FAIL prompt-review"
        : "PASS prompt-review";
  stdout.write(`${label}\n`);
  stdout.write(`Prompt: ${result.promptPath || "(sin prompt resuelto)"}\n`);
  stdout.write(`Report: ${result.reportPath}\n`);
  stdout.write(
    `Findings: ${result.summary.blockers} blockers, ${result.summary.major} major, ${result.summary.minor} minor, ${result.summary.info} info\n`,
  );
  if (result.error) {
    stdout.write(`${result.error}\n`);
    return;
  }

  for (const severity of ["BLOCKER", "MAJOR", "MINOR", "INFO"]) {
    const items = result.findings.filter((item) => item.severity === severity);
    if (items.length === 0) continue;
    stdout.write(`\n${severity === "INFO" ? "INFO" : `${severity}S`}:\n`);
    for (const item of items) {
      stdout.write(`- [${item.code}] ${item.message}\n`);
    }
  }
};

const getDisplayCheckState = (check) => {
  if (check.state) return check.state;
  if (check.name === "tasks") return check.status === "PASS" ? "COMPLETE" : "PENDING";
  if (check.name === "openspec_artifacts") {
    return check.status === "PASS" ? "PRESENT" : "MISSING";
  }
  if (check.name === "openspec_change") return check.status === "PASS" ? "FOUND" : "MISSING";
  if (check.name === "git_workspace") return check.status === "PASS" ? "CLEAN" : "DIRTY";
  if (check.status === "PASS") return "OBSERVED";
  if (check.status === "FAIL") return "MISSING";
  return "UNKNOWN";
};

const getDisplayCheckDescription = (check) => {
  if (check.description) return check.description;
  const state = getDisplayCheckState(check);
  if (check.name === "openspec_change") {
    return state === "FOUND"
      ? "Se encontro un cambio OpenSpec activo o archivado para la entrada."
      : "No se encontro un cambio OpenSpec activo ni archivado para la entrada.";
  }
  if (check.name === "lifecycle") {
    return "Muestra la ubicacion operativa del cambio: active, archived o not_started.";
  }
  if (check.name === "openspec_artifacts") {
    return state === "PRESENT"
      ? "Los artefactos minimos de OpenSpec existen; no valida calidad tecnica del contenido."
      : "Faltan artefactos minimos de OpenSpec requeridos por el flujo.";
  }
  if (check.name === "tasks") {
    return state === "COMPLETE"
      ? "No hay tareas pendientes marcadas como - [ ] en tasks.md."
      : "Hay tareas pendientes marcadas como - [ ] en tasks.md.";
  }
  if (check.name === "openspec_review") {
    return state === "CONFIRMED"
      ? "Existe confirmacion observable de revision OpenSpec en el entorno."
      : "No hay confirmacion observable de revision OpenSpec en esta ejecucion.";
  }
  if (check.name === "pull_request") {
    if (state === "MERGED") return "El pull request asociado aparece mergeado.";
    if (state === "OPEN") return "El pull request asociado aparece abierto y pendiente de merge.";
    if (state === "MISSING") return "No se encontro pull request asociado a la rama esperada.";
    return "No se pudo consultar el estado del pull request.";
  }
  if (check.name === "jira_status") {
    if (state === "DONE") return "Jira aparece en categoria finalizada.";
    if (state === "NOT_DONE") return "Jira aparece en una categoria no finalizada.";
    return "No se pudo consultar el estado de Jira.";
  }
  if (check.name === "git_workspace") {
    if (state === "CLEAN") return "El workspace no tiene cambios locales sin commit.";
    if (state === "DIRTY") return "El workspace tiene cambios locales sin commit.";
    return "No se pudo inspeccionar el workspace Git.";
  }
  return "Indicador observable del flujo opsxj.";
};

const parsePromptReviewArgs = (rawArgs) => {
  const flags = new Set();
  const positional = [];
  let technologyProfile = null;
  for (let index = 0; index < rawArgs.length; index += 1) {
    const arg = String(rawArgs[index]);
    if (arg === "--tech-profile") {
      const value = rawArgs[index + 1];
      if (!value || String(value).startsWith("--")) {
        throw new Error("Falta valor para --tech-profile.");
      }
      technologyProfile = normalizeTechnologyProfile(value);
      index += 1;
      continue;
    }
    if (arg.startsWith("-")) {
      flags.add(arg);
      continue;
    }
    positional.push(arg);
  }

  return {
    promptInput: positional[0] ?? "",
    applyFix: flags.has("--fix") || flags.has("--apply-fix"),
    noFix:
      flags.has("--no-fix") ||
      flags.has("-NonInteractive") ||
      flags.has("--non-interactive"),
    technologyProfile,
  };
};

const shouldOfferPromptReviewCorrection = ({ result }) =>
  !result.error &&
  result.findings.some((finding) =>
    ["BLOCKER", "MAJOR", "MINOR"].includes(finding.severity),
  );

const MAX_PROMPT_REVIEW_FIX_PASSES = 4;

const confirmPromptReviewCorrection = async ({ stdin, stdout }) => {
  if (!stdin?.isTTY || !stdout?.isTTY) {
    return false;
  }

  const rl = createInterface({ input: stdin, output: stdout });
  try {
    const answer = await rl.question(
      "opsxj:prompt-review encontro hallazgos corregibles. ¿Aplicar correcciones al prompt? [s/N] ",
    );
    return /^(s|si|sí|y|yes)$/i.test(String(answer).trim());
  } finally {
    rl.close();
  }
};

const runPromptReview = async ({
  args,
  env,
  stdin,
  stdout,
  issueKeyFromArg,
  baseDir,
  promptReviewFn,
  promptCorrectionFn,
}) => {
  const parsed = parsePromptReviewArgs(
    [issueKeyFromArg, ...args].filter((arg) => arg !== undefined && arg !== null),
  );
  const promptInput = parsed.promptInput;
  const review = promptReviewFn ?? reviewTechnicalPrompt;
  const correct = promptCorrectionFn ?? applyTechnicalReviewCorrection;
  const reviewInput = (input) =>
    parsed.technologyProfile
      ? { baseDir, promptInput: input, technologyProfile: parsed.technologyProfile }
      : { baseDir, promptInput: input };
  let result = await review(reviewInput(promptInput));
  printPromptReviewResult({ stdout, result });

  const shouldAsk =
    !parsed.noFix &&
    parseBoolean(env.OPSXJ_PROMPT_REVIEW_FIX_INTERACTIVE, true) &&
    shouldOfferPromptReviewCorrection({ result });
  const shouldApplyFix =
    parsed.applyFix ||
    (shouldAsk && (await confirmPromptReviewCorrection({ stdin, stdout })));

  if (shouldApplyFix) {
    for (let pass = 1; pass <= MAX_PROMPT_REVIEW_FIX_PASSES; pass += 1) {
      if (!shouldOfferPromptReviewCorrection({ result })) break;
      const correction = await correct({
        promptPath: result.promptPath,
        findings: result.findings,
        baseDir,
      });
      if (!correction.applied) {
        stdout.write(
          "\n[opsxj:prompt-review] No hay correcciones automaticas disponibles para estos hallazgos.\n",
        );
        break;
      }

      stdout.write(
        `\n[opsxj:prompt-review] Correcciones aplicadas al prompt (pasada ${pass}).\n`,
      );
      stdout.write("[opsxj:prompt-review] Reejecutando validacion.\n\n");
      result = await review(reviewInput(result.promptPath));
      printPromptReviewResult({ stdout, result });
    }
  }

  return { exitCode: result.exitCode };
};

const parseStatusArgs = (rawArgs) => {
  const positional = [];
  let output = "text";
  let help = false;

  for (let index = 0; index < rawArgs.length; index += 1) {
    const arg = String(rawArgs[index]);
    if (arg === "--help" || arg === "-h" || arg === "/?") {
      help = true;
      continue;
    }
    if (arg === "--json") {
      output = "json";
      continue;
    }
    if (arg.toLowerCase() === "-output") {
      output = String(rawArgs[index + 1] ?? "").toLowerCase() === "json" ? "json" : output;
      index += 1;
      continue;
    }
    positional.push(arg);
  }

  return {
    input: positional[0] ?? "",
    output,
    help,
  };
};

const opsxjStatusHelp = [
  "Uso:",
  "  npm run opsxj:status -- <SCRUM-XXX|SCRUMCORE-XXX|change-name> [--json]",
  "  npm run opsxj:orchestrate:status -- <SCRUM-XXX|SCRUMCORE-XXX|change-name> [--json]",
  "  node tools/opsxj/scripts/opsxj.js opsxj:status <SCRUM-XXX|SCRUMCORE-XXX|change-name> [--json]",
  "",
  "Descripcion:",
  "  Consulta el estado local de un cambio OpenSpec en este repo legacy.",
  "  Muestra estados observables; no ejecuta validaciones tecnicas, no corrige y no modifica archivos.",
  "  Solo consulta Jira/GitHub cuando el cambio ya esta archivado y hay credenciales disponibles.",
  "",
  "Entradas:",
  "  SCRUMCORE-346                         Busca cambio activo o archivado por issue key.",
  "  scrumcore-346-implementacion-status   Busca por nombre exacto del change.",
  "",
  "Opciones:",
  "  --json                                Imprime resultado JSON.",
  "  -Output json                          Variante compatible para JSON.",
  "  --help, -h, /?                        Muestra esta ayuda.",
  "",
  "Estados:",
  "  NOT_STARTED  No existe cambio activo ni archivado.",
  "  BLOCKED      Faltan artefactos OpenSpec requeridos.",
  "  IN_PROGRESS  Hay tareas pendientes en tasks.md.",
  "  READY        Artefactos completos y sin tareas pendientes.",
  "  ARCHIVED     El cambio esta archivado.",
  "  WARN         Solo hay advertencias no bloqueantes.",
  "",
  "Indicadores observables:",
  "  openspec_change, lifecycle, openspec_artifacts, tasks, openspec_review, pull_request, jira_status, git_workspace.",
  "  Cada indicador muestra state, significado y detalle; status queda en JSON por compatibilidad interna.",
  "",
  "Estados observables:",
  "  FOUND/MISSING     Existencia del cambio OpenSpec.",
  "  OBSERVED          Lifecycle detectado.",
  "  PRESENT/MISSING   Artefactos requeridos presentes o faltantes.",
  "  COMPLETE/PENDING  Tareas completas o pendientes en tasks.md.",
  "  CONFIRMED/UNKNOWN Revision OpenSpec confirmada o no observable.",
  "  OPEN/MERGED       Pull request abierto o mergeado.",
  "  DONE/NOT_DONE     Jira finalizado o pendiente.",
  "  CLEAN/DIRTY       Workspace limpio o con cambios locales.",
  "",
  "JSON parseable:",
  "  npm --silent run opsxj:status -- SCRUMCORE-346 --json",
  "  node tools/opsxj/scripts/opsxj.js opsxj:status SCRUMCORE-346 --json",
].join("\n");

const printOpsxjStatusText = ({ stdout, result }) => {
  stdout.write(`OPSXJ Status: ${result.issueKey ?? result.changeName ?? "(sin entrada)"}\n`);
  stdout.write(`Change: ${result.changeName ?? "(not started)"}\n`);
  stdout.write(`Lifecycle: ${result.lifecycle}\n`);
  stdout.write(`Status: ${result.status}\n`);
  stdout.write(`Next action: ${result.nextAction}\n`);
  stdout.write("\nObserved states:\n");
  for (const check of result.checks) {
    stdout.write(`[${getDisplayCheckState(check)}] ${check.name}\n`);
    stdout.write(`  Significado: ${getDisplayCheckDescription(check)}\n`);
    stdout.write(`  Detalle: ${check.message}\n`);
  }
};

const runStatus = async ({
  args,
  env,
  stdout,
  issueKeyFromArg,
  baseDir,
  statusFn,
}) => {
  const parsed = parseStatusArgs(
    [issueKeyFromArg, ...args].filter((arg) => arg !== undefined && arg !== null),
  );
  if (parsed.help) {
    stdout.write(`${opsxjStatusHelp}\n`);
    return { exitCode: 0 };
  }

  const status = await statusFn({ baseDir, input: parsed.input, env });

  if (parsed.output === "json") {
    stdout.write(`${JSON.stringify(status, null, 2)}\n`);
  } else {
    printOpsxjStatusText({ stdout, result: status });
  }

  return { exitCode: 0 };
};

const parseKeyValueArgs = (rawArgs) => {
  const positional = [];
  const options = {};
  for (let index = 0; index < rawArgs.length; index += 1) {
    const value = String(rawArgs[index]);
    if (value.startsWith("--")) {
      options[value.slice(2)] = rawArgs[index + 1] ?? "";
      index += 1;
    } else {
      positional.push(value);
    }
  }
  return { positional, options };
};

const currentGitSha = async (baseDir) => {
  const result = await execFile("git", ["rev-parse", "HEAD"], { cwd: baseDir });
  return String(result.stdout).trim();
};

const resolveActiveChangeName = async ({ baseDir, input }) => {
  const status = await getOpsxjStatus({ baseDir, input, env: {} });
  if (!status.changeName || status.lifecycle !== "active") {
    throw new Error("opsxj:validate requiere un cambio OpenSpec activo.");
  }
  return status.changeName;
};

const runValidate = async ({ args, issueKeyFromArg, baseDir, env, stdout, validateFn, shaFn }) => {
  const parsed = parseKeyValueArgs([issueKeyFromArg, ...args].filter(Boolean));
  const input = parsed.positional[0];
  if (!input) throw new Error(`Falta SCRUM key o change-name para opsxj:validate.\n${usage}`);
  const changeName = await resolveActiveChangeName({ baseDir, input });
  const result = await validateFn({
    baseDir,
    changeName,
    env,
    currentSha: await shaFn(baseDir),
  });
  if (Object.hasOwn(parsed.options, "json")) {
    stdout.write(`${JSON.stringify({ changeName, ...result }, null, 2)}\n`);
  } else {
    stdout.write(`OPSXJ Validation: ${changeName}\nStatus: ${result.status}\n${result.message}\n`);
    for (const check of result.checks) stdout.write(`[${check.status}] ${check.name}\n`);
  }
  return { exitCode: result.status === "PASS" ? 0 : 1 };
};

const runValidationEvidence = async ({ args, issueKeyFromArg, baseDir, stdout, evidenceFn, shaFn }) => {
  const parsed = parseKeyValueArgs([issueKeyFromArg, ...args].filter(Boolean));
  const issueKey = parsed.positional[0];
  if (!issueKey) throw new Error(`Falta issueKey para opsxj:validation:evidence.\n${usage}`);
  const result = await evidenceFn({
    baseDir,
    issueKey,
    type: parsed.options.type,
    status: parsed.options.status || "pass",
    reference: parsed.options.reference,
    sha: await shaFn(baseDir),
  });
  stdout.write(`[opsxj:validation:evidence] Evidencia ${result.item.type} registrada en ${path.relative(baseDir, result.filePath)}\n`);
  return { exitCode: 0 };
};

const runClose = async ({
  args,
  env,
  stdout,
  issueKeyFromArg,
  closeFn,
}) => {
  const issueKey = issueKeyFromArg ?? args[0] ?? env.JIRA_ISSUE_KEY ?? "";
  if (!issueKey) {
    throw new Error(`Falta issueKey para opsxj:close.\n${usage}`);
  }

  const branchName = buildFeatureBranchName(issueKey);
  const result = await closeFn({
    issueKey,
    branchName,
    jira: {
      baseUrl: env.JIRA_BASE_URL,
      email: env.JIRA_EMAIL,
      apiToken: env.JIRA_API_TOKEN,
    },
    github: {
      token: env.GITHUB_TOKEN,
      repo: env.GITHUB_REPO,
      owner: env.GITHUB_OWNER,
      repoName: env.GITHUB_REPO_NAME,
      baseBranch: env.GITHUB_BASE_BRANCH || "main",
    },
  });

  stdout.write(`[opsxj:close] Ticket: ${String(issueKey).toUpperCase()}\n`);
  stdout.write(
    `[opsxj:close] PR mergeado validado: ${result.pullRequest?.html_url ?? "(sin URL)"}\n`,
  );
  stdout.write(
    `[opsxj:close] Jira actualizado a: ${result.transition?.to?.name ?? "Done"}\n`,
  );
  printCodexAgentHint({ stdout, command: "close" });
};

const commandRegistry = new Map([
  ["opsxj:new", runNew],
  ["new", runNew],
  ["opsxj:orchestrate:new", runNew],
  ["orchestrate:new", runNew],
  ["opsxj:prompt-review", runPromptReview],
  ["prompt-review", runPromptReview],
  ["opsxj:technical-review", runPromptReview],
  ["technical-review", runPromptReview],
  ["opsxj:status", runStatus],
  ["status", runStatus],
  ["opsxj:orchestrate:status", runStatus],
  ["orchestrate:status", runStatus],
  ["opsxj:validate", runValidate],
  ["validate", runValidate],
  ["opsxj:validation:evidence", runValidationEvidence],
  ["validation:evidence", runValidationEvidence],
  ["opsxj:archive", runArchive],
  ["archive", runArchive],
  ["opsxj:close", runClose],
  ["close", runClose],
  ["opesxj:close", runClose],
]);

export const runOpsxjCommand = async ({
  argv,
  env = process.env,
  stdin = process.stdin,
  stdout = process.stdout,
  stderr = process.stderr,
  baseDir = process.cwd(),
  createProposalFn = createProposalFromJira,
  setupProposalFn = setupProposalBranchAndCommit,
  archiveFn = archiveWithPullRequest,
  closeFn = closeIssueFromMergedPr,
  promptReviewFn = reviewTechnicalPrompt,
  promptCorrectionFn = applyTechnicalReviewCorrection,
  statusFn = getOpsxjStatus,
  validateFn = validateLegacyGovernance,
  evidenceFn = writeValidationEvidence,
  shaFn = currentGitSha,
  assertGitCleanFn,
  assertGitCleanAndSyncedFn,
  transitionJiraIssueFn = transitionJiraIssue,
}) => {
  const [command, issueKeyFromArg, ...rest] = argv;
  const selectedCommand = command || "opsxj:new";
  const handler = commandRegistry.get(selectedCommand);

  if (!handler) {
    stderr.write(`[opsxj:error] Comando no soportado: ${selectedCommand}\n`);
    stderr.write(`${usage}\n`);
    return 1;
  }

  try {
    const handlerResult = await handler({
      args: rest,
      env,
      stdin,
      stdout,
      issueKeyFromArg,
      createProposalFn,
      setupProposalFn,
      archiveFn,
      closeFn,
      promptReviewFn,
      promptCorrectionFn,
      statusFn,
      validateFn,
      evidenceFn,
      shaFn,
      baseDir,
      assertGitCleanFn,
      assertGitCleanAndSyncedFn,
      transitionJiraIssueFn,
    });
    if (Number.isInteger(handlerResult?.exitCode)) {
      return handlerResult.exitCode;
    }
    const prefix =
      selectedCommand === "opsxj:archive" || selectedCommand === "archive"
        ? "[opsxj:archive]"
        : selectedCommand === "opsxj:close" ||
            selectedCommand === "close" ||
            selectedCommand === "opesxj:close"
          ? "[opsxj:close]"
          : "[opsxj:new]";
    stdout.write(`${prefix} Proceso finalizado correctamente.\n`);
    return 0;
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    stderr.write(`[opsxj:error] ${message}\n`);
    return 1;
  }
};
