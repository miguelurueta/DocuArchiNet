import { access, cp, mkdir, readdir, rename, rm } from "node:fs/promises";
import path from "node:path";
import { promisify } from "node:util";
import { execFile as execFileCb } from "node:child_process";
import { slugifyForOpenSpec } from "./proposalGenerator.js";
import { addJiraComment, fetchJiraIssue } from "./jiraClient.js";
import { createOrGetPullRequest } from "./githubClient.js";
import { validateLegacyGovernance } from "./legacyGovernanceService.js";

const execFile = promisify(execFileCb);

const runGit = async ({ baseDir, args }) => execFile("git", args, { cwd: baseDir });

const runOpenSpecArchive = async ({ baseDir, changeName, skipSpecs = false }) => {
  const args = ["archive", "-y"];
  if (skipSpecs) {
    args.push("--skip-specs");
  }
  args.push(changeName);
  try {
    return await execFile("openspec", args, { cwd: baseDir });
  } catch (firstError) {
    return execFile("openspec.cmd", args, { cwd: baseDir, shell: true });
  }
};

const readActiveChanges = async (baseDir) => {
  const changesDir = path.join(baseDir, "openspec", "changes");
  const entries = await readdir(changesDir, { withFileTypes: true });
  return entries
    .filter((entry) => entry.isDirectory() && entry.name !== "archive")
    .map((entry) => entry.name);
};

const ensureArchivePath = async ({ baseDir, changeName }) => {
  const archiveDir = path.join(baseDir, "openspec", "changes", "archive");
  await mkdir(archiveDir, { recursive: true });
  const datePrefix = new Date().toISOString().slice(0, 10);
  const baseName = `${datePrefix}-${changeName}`;
  let targetPath = path.join(archiveDir, baseName);
  let suffix = 1;
  // Prevent collisions if archive is retried the same day.
  while (true) {
    try {
      await access(targetPath);
      targetPath = path.join(archiveDir, `${baseName}-${suffix}`);
      suffix += 1;
    } catch {
      return targetPath;
    }
  }
};

export const moveChangeToArchiveDir = async ({ baseDir, changeName }) => {
  const source = path.join(baseDir, "openspec", "changes", changeName);
  try {
    await access(source);
  } catch {
    return null;
  }

  const target = await ensureArchivePath({ baseDir, changeName });
  try {
    await rename(source, target);
  } catch (error) {
    const code = error && typeof error === "object" ? error.code : undefined;
    if (!["EPERM", "EXDEV", "ENOTEMPTY"].includes(code)) {
      throw error;
    }
    await cp(source, target, { recursive: true });
    await rm(source, { recursive: true, force: true });
  }
  return target;
};

export const resolveChangeNameFromIssueKey = async ({ baseDir, issueKey }) => {
  const changes = await readActiveChanges(baseDir);
  const issueSlug = slugifyForOpenSpec(issueKey);
  const candidates = changes.filter(
    (name) => name === issueSlug || name.startsWith(`${issueSlug}-`),
  );
  if (candidates.length === 0) {
    throw new Error(
      `No se encontro cambio activo para ${issueKey}. Cree el cambio con opsxj:new antes de archivar.`,
    );
  }
  if (candidates.length > 1) {
    throw new Error(
      `Se encontraron multiples cambios para ${issueKey}: ${candidates.join(", ")}.`,
    );
  }
  return candidates[0];
};

const archiveChangeWithFallback = async ({ baseDir, changeName }) => {
  try {
    await runOpenSpecArchive({ baseDir, changeName, skipSpecs: false });
    return { archivedWithSkipSpecs: false };
  } catch (_error) {
    await runOpenSpecArchive({ baseDir, changeName, skipSpecs: true });
    return { archivedWithSkipSpecs: true };
  }
};

const commitArchiveMovesIfNeeded = async ({
  baseDir,
  issueKey,
  remoteName = "origin",
}) => {
  await runGit({ baseDir, args: ["add", "-A", "openspec/changes"] });
  const staged = await runGit({
    baseDir,
    args: ["diff", "--cached", "--name-only"],
  });
  const hasStagedChanges = String(staged.stdout ?? "")
    .split(/\r?\n/)
    .map((line) => line.trim())
    .filter(Boolean).length > 0;

  if (!hasStagedChanges) {
    return { committed: false, pushed: false };
  }

  await runGit({
    baseDir,
    args: [
      "commit",
      "-m",
      `chore(${String(issueKey).toUpperCase()}): archive openspec change`,
    ],
  });

  await runGit({ baseDir, args: ["push", remoteName, "HEAD"] });
  return { committed: true, pushed: true };
};

export const archiveWithPullRequest = async ({
  issueKey,
  alsoIssueKeys = [],
  baseDir,
  jira,
  github,
  branchName,
  env = process.env,
  validateGovernanceFn = validateLegacyGovernance,
  fetchImpl = fetch,
}) => {
  const changeName = await resolveChangeNameFromIssueKey({ baseDir, issueKey });
  const shaResult = await runGit({ baseDir, args: ["rev-parse", "HEAD"] });
  const governance = await validateGovernanceFn({
    baseDir,
    changeName,
    env,
    currentSha: String(shaResult.stdout ?? "").trim(),
  });
  if (governance.status !== "PASS") {
    const failed = governance.checks
      .filter((check) => check.status === "FAIL")
      .map((check) => check.name)
      .join(", ");
    throw new Error(
      `El gobierno de ${changeName} no permite archivar. Pendiente: ${failed || governance.message}`,
    );
  }
  const archiveResult = await archiveChangeWithFallback({
    baseDir,
    changeName,
  });
  const archivedDirectoryPath = await moveChangeToArchiveDir({
    baseDir,
    changeName,
  });

  const alsoArchivedDirectoryPaths = [];
  for (const extraIssueKey of alsoIssueKeys) {
    const extraChangeName = await resolveChangeNameFromIssueKey({
      baseDir,
      issueKey: extraIssueKey,
    }).catch(() => null);
    if (!extraChangeName) continue;
    const moved = await moveChangeToArchiveDir({
      baseDir,
      changeName: extraChangeName,
    });
    if (moved) {
      alsoArchivedDirectoryPaths.push({ issueKey: extraIssueKey, path: moved });
    }
  }

  const gitArchiveCommit = await commitArchiveMovesIfNeeded({ baseDir, issueKey });

  const issue = await fetchJiraIssue({
    issueKey,
    baseUrl: jira.baseUrl,
    email: jira.email,
    apiToken: jira.apiToken,
    commandName: "opsxj.js opsxj:archive",
    fetchImpl,
  });

  const prResult = await createOrGetPullRequest({
    repo: github.repo,
    owner: github.owner,
    repoName: github.repoName,
    token: github.token,
    issueKey,
    summary: issue.summary,
    branchName,
    baseBranch: github.baseBranch ?? "main",
    fetchImpl,
  });

  await addJiraComment({
    issueKey,
    baseUrl: jira.baseUrl,
    email: jira.email,
    apiToken: jira.apiToken,
    message: `PR ${prResult.created ? "creado" : "existente"}: ${prResult.pullRequest.html_url}`,
    fetchImpl,
  });

  return {
    changeName,
    issue,
    pullRequest: prResult.pullRequest,
    pullRequestCreated: prResult.created,
    archivedWithSkipSpecs: archiveResult.archivedWithSkipSpecs,
    archivedDirectoryPath,
    alsoArchivedDirectoryPaths,
    gitArchiveCommit,
    governance,
  };
};
