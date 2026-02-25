import path from "node:path";
import { createProposalFromJira } from "./jiraProposalService.js";

const usage = [
  "Uso:",
  "  node scripts/opsxj.js opsxj:new <ISSUE-KEY>",
  "  npm run opsxj:new -- <ISSUE-KEY>",
].join("\n");

const buildContext = ({
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

const runNew = async ({
  args,
  env,
  stdout,
  issueKeyFromArg,
  createProposalFn,
  baseDir,
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

  buildContext({
    env,
    stdout,
    issueKey,
    issue: result.issue,
    changeName: result.changeName,
    proposalPath: result.proposalPath,
    baseDir,
  });
};

const commandRegistry = new Map([
  ["opsxj:new", runNew],
  ["new", runNew],
]);

export const runOpsxjCommand = async ({
  argv,
  env = process.env,
  stdout = process.stdout,
  stderr = process.stderr,
  baseDir = process.cwd(),
  createProposalFn = createProposalFromJira,
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
      baseDir,
    });
    stdout.write("[opsxj:new] Proceso finalizado correctamente.\n");
    return 0;
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    stderr.write(`[opsxj:error] ${message}\n`);
    return 1;
  }
};
