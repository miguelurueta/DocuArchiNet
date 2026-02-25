import { readdir } from "node:fs/promises";
import path from "node:path";
import { promisify } from "node:util";
import { execFile as execFileCb } from "node:child_process";
import { slugifyForOpenSpec } from "./proposalGenerator.js";
import { addJiraComment, fetchJiraIssue } from "./jiraClient.js";
import { createOrGetPullRequest } from "./githubClient.js";

const execFile = promisify(execFileCb);

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

export const archiveWithPullRequest = async ({
  issueKey,
  baseDir,
  jira,
  github,
  branchName,
  fetchImpl = fetch,
}) => {
  const changeName = await resolveChangeNameFromIssueKey({ baseDir, issueKey });
  const archiveResult = await archiveChangeWithFallback({
    baseDir,
    changeName,
  });

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
  };
};
