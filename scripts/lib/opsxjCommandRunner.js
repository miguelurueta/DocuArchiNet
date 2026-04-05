import path from "node:path";
import { createProposalFromJira } from "./jiraProposalService.js";
import {
  buildFeatureBranchName,
  setupProposalBranchAndCommit,
} from "./gitClient.js";
import { archiveWithPullRequest } from "./archiveWorkflowService.js";
import { closeIssueFromMergedPr } from "./closeWorkflowService.js";

const usage = [
  "Uso:",
  "  node scripts/opsxj.js opsxj:new <ISSUE-KEY>",
  "  node scripts/opsxj.js opsxj:archive <ISSUE-KEY>",
  "  node scripts/opsxj.js opsxj:close <ISSUE-KEY>",
  "  npm run opsxj:new -- <ISSUE-KEY>",
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
  baseDir,
}) => {
  const relativeProposalPath = path.relative(baseDir, proposalPath);
  const changeDir = path.join("openspec", "changes", changeName);

  stdout.write(`[opsxj:new] Ticket: ${issueKey}\n`);
  stdout.write(`[opsxj:new] Resumen Jira: ${issue.summary || "(sin resumen)"}\n`);
  stdout.write(`[opsxj:new] Carpeta OpenSpec: ${changeDir}\n`);
  stdout.write(`[opsxj:new] Proposal creado: ${relativeProposalPath}\n`);
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

const printCodexAgentHint = ({ stdout, command }) => {
  if (command === "new") {
    stdout.write(
      "[opsxj:new] Sugerencia Codex: use subagente mini para design/spec/tasks y agente principal para implementacion e integracion final.\n",
    );
    return;
  }

  if (command === "archive") {
    stdout.write(
      "[opsxj:archive] Sugerencia Codex: use agente principal para verify, archive, revision final de diff y coordinacion del PR.\n",
    );
    return;
  }

  if (command === "close") {
    stdout.write(
      "[opsxj:close] Sugerencia Codex: use agente principal para validar merge, cierre Jira y sincronizacion final del flujo.\n",
    );
  }
};

const runNew = async ({
  args,
  env,
  stdout,
  issueKeyFromArg,
  createProposalFn,
  baseDir,
  setupProposalFn,
}) => {
  const issueKey = issueKeyFromArg ?? args[0] ?? env.JIRA_ISSUE_KEY ?? "";
  if (!issueKey) {
    throw new Error(`Falta issueKey para opsxj:new.\n${usage}`);
  }

  const result = await createProposalFn({
    issueKey,
    baseUrl: env.JIRA_BASE_URL,
    email: env.JIRA_EMAIL,
    apiToken: env.JIRA_API_TOKEN,
    baseDir,
    commandName: "opsxj.js opsxj:new",
    folderStrategy: "summary",
  });

  buildNewContext({
    env,
    stdout,
    issueKey,
    issue: result.issue,
    changeName: result.changeName,
    proposalPath: result.proposalPath,
    baseDir,
  });

  const gitResult = await setupProposalFn({
    baseDir,
    issueKey,
    proposalPath: result.proposalPath,
    autoPush: parseBoolean(env.GIT_AUTO_PUSH, true),
  });
  printGitSummary({ stdout, gitResult });
  printCodexAgentHint({ stdout, command: "new" });
};

const runArchive = async ({
  args,
  env,
  stdout,
  issueKeyFromArg,
  baseDir,
  archiveFn,
}) => {
  const issueKey = issueKeyFromArg ?? args[0] ?? env.JIRA_ISSUE_KEY ?? "";
  if (!issueKey) {
    throw new Error(`Falta issueKey para opsxj:archive.\n${usage}`);
  }

  const branchName = buildFeatureBranchName(issueKey);
  const result = await archiveFn({
    issueKey,
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
  printCodexAgentHint({ stdout, command: "archive" });
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
  ["opsxj:archive", runArchive],
  ["archive", runArchive],
  ["opsxj:close", runClose],
  ["close", runClose],
  ["opesxj:close", runClose],
]);

export const runOpsxjCommand = async ({
  argv,
  env = process.env,
  stdout = process.stdout,
  stderr = process.stderr,
  baseDir = process.cwd(),
  createProposalFn = createProposalFromJira,
  setupProposalFn = setupProposalBranchAndCommit,
  archiveFn = archiveWithPullRequest,
  closeFn = closeIssueFromMergedPr,
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
    await handler({
      args: rest,
      env,
      stdout,
      issueKeyFromArg,
      createProposalFn,
      setupProposalFn,
      archiveFn,
      closeFn,
      baseDir,
    });
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
