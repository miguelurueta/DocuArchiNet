import { execFile as execFileCb } from "node:child_process";
import { access, readdir, readFile, stat } from "node:fs/promises";
import path from "node:path";
import { promisify } from "node:util";
import { getPullRequestStatusByBranch } from "./githubClient.js";
import { fetchJiraIssue } from "./jiraClient.js";
import { readRunChecklist, resolveRunChecklistStage } from "./runChecklistService.js";

const execFile = promisify(execFileCb);

const REQUIRED_ARTIFACTS = [
  { id: "proposal", label: "proposal.md", relativePath: "proposal.md" },
  { id: "design", label: "design.md", relativePath: "design.md" },
  { id: "tasks", label: "tasks.md", relativePath: "tasks.md" },
];

const ISSUE_KEY_PATTERN = /^[A-Za-z]+-\d+$/;
const CHECKLIST_STAGE_ORDER = Object.freeze(["new", "refine", "review", "validate", "archive", "pull_request", "close"]);
const CHECKLIST_LABELS = Object.freeze({
  new: "Inicio",
  refine: "Refinement",
  review: "Revision OpenSpec",
  validate: "Validacion",
  archive: "Archivo",
  pull_request: "PR fusionado",
  close: "Cierre Jira",
});

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

const getCurrentGitSha = async ({ baseDir }) => {
  try {
    const result = await execFile("git", ["rev-parse", "HEAD"], { cwd: baseDir });
    return String(result.stdout ?? "").trim() || null;
  } catch {
    return null;
  }
};

const toChecklistItem = ({ id, state, recordedAtUtc = null, sha = null, reference = null, detail = null, nextAction = null }) => ({
  id,
  label: CHECKLIST_LABELS[id],
  state,
  recordedAtUtc,
  sha,
  reference,
  detail,
  nextAction,
});

const checklistNextAction = (id, state) => {
  if (state === "COMPLETE" || state === "NOT_APPLICABLE") return null;
  if (id === "new") return "Ejecutar opsxj:new para iniciar el cambio.";
  if (id === "refine") return "Completar y verificar refinement con opsxj:refine.";
  if (id === "review") return state === "STALE"
    ? "Confirmar nuevamente la revision OpenSpec para el SHA actual."
    : "Confirmar la revision OpenSpec para el SHA actual y ejecutar opsxj:validate.";
  if (id === "validate") return "Ejecutar opsxj:validate para el SHA actual.";
  if (id === "archive") return "Completar las compuertas y ejecutar opsxj:archive.";
  if (id === "pull_request") return "Crear o mergear el pull request asociado.";
  if (id === "close") return "Ejecutar opsxj:close despues del merge del PR.";
  return null;
};

const getSafeRunChecklist = async ({ baseDir, issueKey }) => {
  if (!issueKey) return { state: "absent", run: null };
  try {
    return await readRunChecklist({ baseDir, issueKey });
  } catch (error) {
    return { state: "invalid", run: null, error: error instanceof Error ? error.message : String(error) };
  }
};

const getRecordedChecklistItem = ({ id, runChecklist, currentSha, lifecycle }) => {
  const resolved = resolveRunChecklistStage({
    readResult: runChecklist,
    stage: id,
    currentSha,
  });
  const state = resolved.state === "UNAVAILABLE" && runChecklist.state === "absent" && lifecycle === "active"
    ? "PENDING"
    : resolved.state;
  return toChecklistItem({
    id,
    state,
    recordedAtUtc: resolved.recordedAtUtc,
    sha: resolved.sha,
    reference: resolved.reference,
    detail: resolved.detail,
    nextAction: checklistNextAction(id, state),
  });
};

const buildChecklist = ({ resolved, runChecklist, currentSha, env, checks }) => {
  const newEvent = resolveRunChecklistStage({ readResult: runChecklist, stage: "new", shaSensitive: false });
  const newItem = toChecklistItem({
    id: "new",
    state: resolved.lifecycle === "not_started" ? "PENDING" : "COMPLETE",
    recordedAtUtc: newEvent.recordedAtUtc,
    sha: newEvent.sha,
    reference: newEvent.reference ?? resolved.changeName,
    detail: resolved.lifecycle === "not_started" ? "No se encontro un cambio OpenSpec." : "Cambio OpenSpec localizado.",
    nextAction: checklistNextAction("new", resolved.lifecycle === "not_started" ? "PENDING" : "COMPLETE"),
  });
  if (resolved.lifecycle === "not_started") {
    return CHECKLIST_STAGE_ORDER.map((id) => id === "new"
      ? newItem
      : toChecklistItem({ id, state: "NOT_APPLICABLE", detail: "El ciclo aun no ha iniciado." }));
  }

  const refineItem = getRecordedChecklistItem({ id: "refine", runChecklist, currentSha, lifecycle: resolved.lifecycle });
  const reviewItem = env.OPSXJ_OPENSPEC_REVIEW_CONFIRMED
    ? toChecklistItem({
      id: "review",
      state: "COMPLETE",
      sha: currentSha,
      reference: "OPSXJ_OPENSPEC_REVIEW_CONFIRMED",
      detail: "Confirmacion temporal observada; persistira al ejecutar opsxj:validate.",
      nextAction: "Ejecutar opsxj:validate para persistir la revision.",
    })
    : getRecordedChecklistItem({ id: "review", runChecklist, currentSha, lifecycle: resolved.lifecycle });
  const validateItem = getRecordedChecklistItem({ id: "validate", runChecklist, currentSha, lifecycle: resolved.lifecycle });
  const archiveEvent = getRecordedChecklistItem({ id: "archive", runChecklist, currentSha, lifecycle: resolved.lifecycle });
  const archiveItem = resolved.lifecycle === "archived"
    ? toChecklistItem({
      ...archiveEvent,
      id: "archive",
      state: "COMPLETE",
      detail: "Cambio OpenSpec archivado.",
      nextAction: null,
    })
    : archiveEvent;
  const pullRequest = checks.find((check) => check.name === "pull_request");
  const jira = checks.find((check) => check.name === "jira_status");
  const pullRequestItem = resolved.lifecycle !== "archived"
    ? toChecklistItem({ id: "pull_request", state: "NOT_APPLICABLE", detail: "Se habilita despues de archivar el cambio." })
    : pullRequest?.details?.merged
      ? toChecklistItem({ id: "pull_request", state: "COMPLETE", reference: pullRequest.details.url, detail: "Pull request fusionado." })
      : pullRequest?.details?.state === "open"
        ? toChecklistItem({ id: "pull_request", state: "PENDING", reference: pullRequest.details.url, detail: "Pull request abierto.", nextAction: checklistNextAction("pull_request", "PENDING") })
        : pullRequest?.message?.startsWith("No pull request")
          ? toChecklistItem({ id: "pull_request", state: "BLOCKED", detail: "No se encontro pull request para la rama esperada.", nextAction: checklistNextAction("pull_request", "BLOCKED") })
          : toChecklistItem({ id: "pull_request", state: "UNAVAILABLE", detail: pullRequest?.message ?? "No fue posible consultar GitHub.", nextAction: checklistNextAction("pull_request", "UNAVAILABLE") });
  const closeItem = resolved.lifecycle !== "archived" || pullRequestItem.state !== "COMPLETE"
    ? toChecklistItem({ id: "close", state: "NOT_APPLICABLE", detail: "Requiere cambio archivado y PR fusionado." })
    : jira?.details?.statusCategory === "done"
      ? toChecklistItem({ id: "close", state: "COMPLETE", reference: jira.details.status, detail: "Jira finalizado." })
      : jira?.details?.statusCategory
        ? toChecklistItem({ id: "close", state: "PENDING", detail: "Jira aun no esta finalizado.", nextAction: checklistNextAction("close", "PENDING") })
        : toChecklistItem({ id: "close", state: "UNAVAILABLE", detail: jira?.message ?? "No fue posible consultar Jira.", nextAction: checklistNextAction("close", "UNAVAILABLE") });
  return [newItem, refineItem, reviewItem, validateItem, archiveItem, pullRequestItem, closeItem];
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

const buildNotStartedStatus = async ({ baseDir, input, issueKey }) => {
  const checks = withObservableCheckStates([
    {
      name: "openspec_change",
      status: "FAIL",
      message: `No active or archived OpenSpec change was found for ${input}.`,
    },
    await getGitWorkspaceCheck({ baseDir }),
  ]);
  return {
    issueKey,
    changeName: null,
    lifecycle: "not_started",
    archivePath: null,
    status: "NOT_STARTED",
    nextAction: "Ejecutar opsxj:new para iniciar el cambio.",
    checks,
    checklist: buildChecklist({
      resolved: { issueKey, changeName: null, lifecycle: "not_started" },
      runChecklist: { state: "absent", run: null },
      currentSha: null,
      env: {},
      checks,
    }),
  };
};

const buildStatusFromChange = async ({ baseDir, resolved, env, fetchImpl, currentSha }) => {
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

  const runChecklist = await getSafeRunChecklist({ baseDir, issueKey: resolved.issueKey });
  const checklist = buildChecklist({
    resolved,
    runChecklist,
    currentSha,
    env,
    checks,
  });
  const reviewItem = checklist.find((item) => item.id === "review");
  const reviewCheck = checks.find((check) => check.name === "openspec_review");
  if (reviewCheck && reviewItem) {
    reviewCheck.status = reviewItem.state === "COMPLETE" ? "PASS" : "WARN";
    reviewCheck.message = reviewItem.state === "COMPLETE"
      ? "OpenSpec review is confirmed for the current SHA."
      : reviewItem.detail ?? reviewCheck.message;
    reviewCheck.details = {
      state: reviewItem.state,
      recordedAtUtc: reviewItem.recordedAtUtc,
      sha: reviewItem.sha,
      reference: reviewItem.reference,
    };
  }

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

  const observedNextAction =
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
  const blockingChecklistItem = checklist.find((item) => ["STALE", "BLOCKED"].includes(item.state));
  const nextAction = blockingChecklistItem?.nextAction ?? observedNextAction;

  return {
    issueKey: resolved.issueKey,
    changeName: resolved.changeName,
    lifecycle: resolved.lifecycle,
    archivePath: resolved.archivePath ? normalizeSlash(resolved.archivePath) : null,
    status,
    nextAction,
    checks: observedChecks,
    checklist,
  };
};

export const getOpsxjStatus = async ({
  baseDir,
  input,
  env = process.env,
  fetchImpl = fetch,
  currentSha = null,
}) => {
  const resolved = await resolveChange({ baseDir, input });
  if (resolved.lifecycle === "not_started") {
    return buildNotStartedStatus({
      baseDir,
      input,
      issueKey: resolved.issueKey,
    });
  }

  return buildStatusFromChange({
    baseDir,
    resolved,
    env,
    fetchImpl,
    currentSha: currentSha ?? (await getCurrentGitSha({ baseDir })),
  });
};
