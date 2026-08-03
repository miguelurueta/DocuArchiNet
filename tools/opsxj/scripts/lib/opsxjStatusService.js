import { execFile as execFileCb } from "node:child_process";
import { access, readdir, readFile, stat } from "node:fs/promises";
import path from "node:path";
import { promisify } from "node:util";
import { getPullRequestStatusByBranch } from "./githubClient.js";
import { fetchJiraIssue } from "./jiraClient.js";

const execFile = promisify(execFileCb);

const REQUIRED_ARTIFACTS = [
  { id: "proposal", label: "proposal.md", relativePath: "proposal.md" },
  { id: "design", label: "design.md", relativePath: "design.md" },
  { id: "tasks", label: "tasks.md", relativePath: "tasks.md" },
];

const ISSUE_KEY_PATTERN = /^[A-Za-z]+-\d+$/;

const normalizeSlash = (value) => String(value).replace(/\\/g, "/");

const normalizeIssueKey = (value) => String(value ?? "").trim().toUpperCase();

const toChangePrefix = (issueKey) => normalizeIssueKey(issueKey).toLowerCase();

const buildFeatureBranchName = (issueKey) =>
  `feature/${normalizeIssueKey(issueKey)}`;

const pathExists = async (targetPath) => {
  await access(targetPath);
  return true;
};

const safePathExists = async (targetPath) => pathExists(targetPath).catch(() => false);

const readDirectoryNames = async (dir) => {
  const entries = await readdir(dir, { withFileTypes: true }).catch(() => []);
  return entries.filter((entry) => entry.isDirectory()).map((entry) => entry.name);
};

const findSpecFiles = async (dir) => {
  const entries = await readdir(dir, { withFileTypes: true }).catch(() => []);
  const files = [];

  for (const entry of entries) {
    const fullPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      files.push(...(await findSpecFiles(fullPath)));
      continue;
    }
    if (entry.isFile() && entry.name === "spec.md") {
      files.push(fullPath);
    }
  }

  return files;
};

const resolveByIssueKey = async ({ baseDir, issueKey }) => {
  const changesDir = path.join(baseDir, "openspec", "changes");
  const archiveDir = path.join(changesDir, "archive");
  const prefix = toChangePrefix(issueKey);

  const activeChangeNames = (await readDirectoryNames(changesDir))
    .filter((name) => name !== "archive")
    .filter((name) => name.toLowerCase().startsWith(prefix));

  if (activeChangeNames.length > 0) {
    const changeName = activeChangeNames.sort()[0];
    return {
      changeName,
      lifecycle: "active",
      changePath: path.join(changesDir, changeName),
      archivePath: null,
    };
  }

  const archivedChangeNames = (await readDirectoryNames(archiveDir)).filter((name) =>
    name.toLowerCase().includes(prefix),
  );

  if (archivedChangeNames.length > 0) {
    const archivedName = archivedChangeNames.sort().at(-1);
    return {
      changeName: archivedName.replace(/^\d{4}-\d{2}-\d{2}-/, ""),
      lifecycle: "archived",
      changePath: path.join(archiveDir, archivedName),
      archivePath: path.join(archiveDir, archivedName),
    };
  }

  return {
    changeName: null,
    lifecycle: "not_started",
    changePath: null,
    archivePath: null,
  };
};

const resolveByChangeName = async ({ baseDir, changeName }) => {
  const changesDir = path.join(baseDir, "openspec", "changes");
  const archiveDir = path.join(changesDir, "archive");
  const activePath = path.join(changesDir, changeName);

  if ((await stat(activePath).catch(() => null))?.isDirectory()) {
    return {
      changeName,
      lifecycle: "active",
      changePath: activePath,
      archivePath: null,
    };
  }

  const archivedChangeNames = (await readDirectoryNames(archiveDir)).filter(
    (name) =>
      name === changeName ||
      name.endsWith(`-${changeName}`) ||
      name.toLowerCase().includes(changeName.toLowerCase()),
  );

  if (archivedChangeNames.length > 0) {
    const archivedName = archivedChangeNames.sort().at(-1);
    return {
      changeName,
      lifecycle: "archived",
      changePath: path.join(archiveDir, archivedName),
      archivePath: path.join(archiveDir, archivedName),
    };
  }

  return {
    changeName,
    lifecycle: "not_started",
    changePath: null,
    archivePath: null,
  };
};

const resolveChange = async ({ baseDir, input }) => {
  const trimmed = String(input ?? "").trim();
  if (!trimmed) {
    throw new Error("Falta SCRUM key o change-name para opsxj:status.");
  }

  if (ISSUE_KEY_PATTERN.test(trimmed)) {
    return {
      issueKey: normalizeIssueKey(trimmed),
      ...(await resolveByIssueKey({ baseDir, issueKey: trimmed })),
    };
  }

  const issueMatch = trimmed.match(/[A-Za-z]+-\d+/);
  return {
    issueKey: issueMatch ? normalizeIssueKey(issueMatch[0]) : null,
    ...(await resolveByChangeName({ baseDir, changeName: trimmed })),
  };
};

const countPendingTasks = async (tasksPath) => {
  const content = await readFile(tasksPath, "utf8").catch(() => "");
  const pending = content.match(/^\s*-\s+\[\s\]/gm) ?? [];
  const complete = content.match(/^\s*-\s+\[[xX]\]/gm) ?? [];
  return {
    pending: pending.length,
    complete: complete.length,
    total: pending.length + complete.length,
  };
};

const getArtifactChecks = async (changePath) => {
  const missing = [];
  for (const artifact of REQUIRED_ARTIFACTS) {
    const fullPath = path.join(changePath, artifact.relativePath);
    if (!(await safePathExists(fullPath))) {
      missing.push(artifact.label);
    }
  }

  const specFiles = await findSpecFiles(path.join(changePath, "specs"));
  if (specFiles.length === 0) {
    missing.push("specs/**/spec.md");
  }

  return {
    missing,
    specFiles,
  };
};

const getGitWorkspaceCheck = async ({ baseDir }) => {
  try {
    const result = await execFile("git", ["status", "--porcelain"], { cwd: baseDir });
    const lines = String(result.stdout ?? "")
      .split(/\r?\n/)
      .map((line) => line.trim())
      .filter(Boolean);
    return {
      name: "git_workspace",
      status: lines.length > 0 ? "WARN" : "PASS",
      message:
        lines.length > 0
          ? `Workspace has ${lines.length} uncommitted change(s).`
          : "Workspace has no uncommitted changes.",
      details: { dirtyFiles: lines },
    };
  } catch (error) {
    return {
      name: "git_workspace",
      status: "WARN",
      message: "Could not inspect git workspace.",
      details: { error: error instanceof Error ? error.message : String(error) },
    };
  }
};

const getPullRequestCheck = async ({ issueKey, env, fetchImpl }) => {
  if (!issueKey) {
    return {
      name: "pull_request",
      status: "WARN",
      message: "Cannot inspect pull request without issue key.",
    };
  }
  if (!env.GITHUB_TOKEN || !(env.GITHUB_REPO || (env.GITHUB_OWNER && env.GITHUB_REPO_NAME))) {
    return {
      name: "pull_request",
      status: "WARN",
      message: "GitHub credentials are missing; PR status was not inspected.",
    };
  }

  try {
    const result = await getPullRequestStatusByBranch({
      repo: env.GITHUB_REPO,
      owner: env.GITHUB_OWNER,
      repoName: env.GITHUB_REPO_NAME,
      token: env.GITHUB_TOKEN,
      branchName: buildFeatureBranchName(issueKey),
      baseBranch: env.GITHUB_BASE_BRANCH || "main",
      fetchImpl,
    });
    const url = result.pullRequest?.html_url ?? null;
    if (result.merged) {
      return {
        name: "pull_request",
        status: "PASS",
        message: `Pull request is merged${url ? `: ${url}` : "."}`,
        details: { state: result.state, merged: result.merged, url },
      };
    }
    if (result.state === "open") {
      return {
        name: "pull_request",
        status: "WARN",
        message: `Pull request is open and not merged${url ? `: ${url}` : "."}`,
        details: { state: result.state, merged: result.merged, url },
      };
    }
    return {
      name: "pull_request",
      status: "WARN",
      message: "No pull request was found for the feature branch.",
      details: { state: result.state, merged: result.merged, url },
    };
  } catch (error) {
    return {
      name: "pull_request",
      status: "WARN",
      message: "Could not inspect pull request status.",
      details: { error: error instanceof Error ? error.message : String(error) },
    };
  }
};

const getJiraStatusCheck = async ({ issueKey, env, fetchImpl }) => {
  if (!issueKey) {
    return {
      name: "jira_status",
      status: "WARN",
      message: "Cannot inspect Jira without issue key.",
    };
  }
  if (!env.JIRA_BASE_URL || !env.JIRA_EMAIL || !env.JIRA_API_TOKEN) {
    return {
      name: "jira_status",
      status: "WARN",
      message: "Jira credentials are missing; Jira status was not inspected.",
    };
  }

  try {
    const issue = await fetchJiraIssue({
      issueKey,
      baseUrl: env.JIRA_BASE_URL,
      email: env.JIRA_EMAIL,
      apiToken: env.JIRA_API_TOKEN,
      commandName: "opsxj.js opsxj:status",
      fetchImpl,
    });
    const status = issue.metadata?.status ?? "";
    const statusCategory = issue.metadata?.statusCategory ?? "";
    return {
      name: "jira_status",
      status: statusCategory === "done" ? "PASS" : "WARN",
      message:
        statusCategory === "done"
          ? `Jira is done (${status}).`
          : `Jira is not done (${status || "unknown"}).`,
      details: { status, statusCategory },
    };
  } catch (error) {
    return {
      name: "jira_status",
      status: "WARN",
      message: "Could not inspect Jira status.",
      details: { error: error instanceof Error ? error.message : String(error) },
    };
  }
};

const toObservableCheckState = (check) => {
  if (check.name === "openspec_change") {
    return check.status === "PASS" ? "FOUND" : "MISSING";
  }
  if (check.name === "lifecycle") {
    return "OBSERVED";
  }
  if (check.name === "openspec_artifacts") {
    return check.status === "PASS" ? "PRESENT" : "MISSING";
  }
  if (check.name === "tasks") {
    return check.status === "PASS" ? "COMPLETE" : "PENDING";
  }
  if (check.name === "openspec_review") {
    return check.status === "PASS" ? "CONFIRMED" : "UNKNOWN";
  }
  if (check.name === "pull_request") {
    if (check.details?.merged === true) return "MERGED";
    if (check.details?.state === "open") return "OPEN";
    if (check.message?.startsWith("No pull request")) return "MISSING";
    return "UNKNOWN";
  }
  if (check.name === "jira_status") {
    if (check.details?.statusCategory === "done") return "DONE";
    if (check.details?.statusCategory) return "NOT_DONE";
    return "UNKNOWN";
  }
  if (check.name === "git_workspace") {
    if (check.status === "PASS") return "CLEAN";
    return check.details?.dirtyFiles?.length > 0 ? "DIRTY" : "UNKNOWN";
  }
  if (check.status === "PASS") return "OBSERVED";
  if (check.status === "FAIL") return "MISSING";
  return "ATTENTION";
};

const toObservableCheckDescription = (check, state) => {
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

const withObservableCheckStates = (checks) =>
  checks.map((check) => {
    const state = check.state ?? toObservableCheckState(check);
    return {
      ...check,
      state,
      description: check.description ?? toObservableCheckDescription(check, state),
    };
  });

const buildNotStartedStatus = async ({ baseDir, input, issueKey }) => ({
  issueKey,
  changeName: null,
  lifecycle: "not_started",
  archivePath: null,
  status: "NOT_STARTED",
  nextAction: "Ejecutar opsxj:new para iniciar el cambio.",
  checks: withObservableCheckStates([
    {
      name: "openspec_change",
      status: "FAIL",
      message: `No active or archived OpenSpec change was found for ${input}.`,
    },
    await getGitWorkspaceCheck({ baseDir }),
  ]),
});

const buildStatusFromChange = async ({ baseDir, resolved, env, fetchImpl }) => {
  const checks = [
    {
      name: "openspec_change",
      status: "PASS",
      message: `OpenSpec change found in ${resolved.lifecycle}.`,
    },
    {
      name: "lifecycle",
      status: "PASS",
      message: `Lifecycle is ${resolved.lifecycle}.`,
    },
  ];

  const artifactCheck = await getArtifactChecks(resolved.changePath);
  checks.push({
    name: "openspec_artifacts",
    status: artifactCheck.missing.length > 0 ? "FAIL" : "PASS",
    message:
      artifactCheck.missing.length > 0
        ? `Missing required artifact(s): ${artifactCheck.missing.join(", ")}.`
        : "Required OpenSpec artifacts exist.",
    details: {
      missing: artifactCheck.missing,
      specs: artifactCheck.specFiles.map((filePath) =>
        normalizeSlash(path.relative(baseDir, filePath)),
      ),
    },
  });

  const taskCounts = await countPendingTasks(path.join(resolved.changePath, "tasks.md"));
  checks.push({
    name: "tasks",
    status: taskCounts.pending > 0 ? "FAIL" : "PASS",
    message:
      taskCounts.pending > 0
        ? `tasks.md has ${taskCounts.pending} pending task(s).`
        : "tasks.md has no pending tasks.",
    details: taskCounts,
  });

  checks.push({
    name: "openspec_review",
    status: env.OPSXJ_OPENSPEC_REVIEW_CONFIRMED ? "PASS" : "WARN",
    message: env.OPSXJ_OPENSPEC_REVIEW_CONFIRMED
      ? "OpenSpec review confirmation is present."
      : "OpenSpec review confirmation is missing.",
  });
  if (resolved.lifecycle === "archived") {
    checks.push(
      await getPullRequestCheck({
        issueKey: resolved.issueKey,
        env,
        fetchImpl,
      }),
    );
    checks.push(
      await getJiraStatusCheck({
        issueKey: resolved.issueKey,
        env,
        fetchImpl,
      }),
    );
  }
  checks.push(await getGitWorkspaceCheck({ baseDir }));

  const hasFailures = checks.some((check) => check.status === "FAIL");
  const hasWarnings = checks.some(
    (check) => check.status === "WARN" && check.name !== "git_workspace",
  );
  const status =
    resolved.lifecycle === "archived"
      ? "ARCHIVED"
      : artifactCheck.missing.length > 0
        ? "BLOCKED"
        : taskCounts.pending > 0
          ? "IN_PROGRESS"
          : hasWarnings
            ? "WARN"
            : "READY";

  const observedChecks = withObservableCheckStates(checks);
  const pullRequestCheck = observedChecks.find((check) => check.name === "pull_request");
  const jiraStatusCheck = observedChecks.find((check) => check.name === "jira_status");
  const pullRequestMerged = pullRequestCheck?.state === "MERGED";
  const jiraDone = jiraStatusCheck?.state === "DONE";

  const nextAction =
    status === "ARCHIVED"
      ? pullRequestCheck?.state === "UNKNOWN" || jiraStatusCheck?.state === "UNKNOWN"
        ? "Revisar estado remoto de PR/Jira y continuar con merge o close segun corresponda."
        : pullRequestCheck?.state === "MISSING"
          ? "Revisar o crear PR asociado antes de cerrar Jira."
          : pullRequestMerged && !jiraDone
        ? "Ejecutar opsxj:close para mover Jira despues del merge."
        : pullRequestCheck?.state === "OPEN"
          ? "Mergear el PR y luego ejecutar opsxj:close."
          : "No local action required; change is archived and Jira is done."
      : status === "BLOCKED"
        ? "Crear los artefactos OpenSpec faltantes."
        : status === "IN_PROGRESS"
          ? "Completar tasks.md."
          : status === "WARN"
            ? "Revisar advertencias no bloqueantes."
            : "Cambio listo para la siguiente fase del flujo o para archive.";

  return {
    issueKey: resolved.issueKey,
    changeName: resolved.changeName,
    lifecycle: resolved.lifecycle,
    archivePath: resolved.archivePath ? normalizeSlash(resolved.archivePath) : null,
    status,
    nextAction,
    checks: observedChecks,
  };
};

export const getOpsxjStatus = async ({
  baseDir,
  input,
  env = process.env,
  fetchImpl = fetch,
}) => {
  const resolved = await resolveChange({ baseDir, input });
  if (resolved.lifecycle === "not_started") {
    return buildNotStartedStatus({
      baseDir,
      input,
      issueKey: resolved.issueKey,
    });
  }

  return buildStatusFromChange({ baseDir, resolved, env, fetchImpl });
};
