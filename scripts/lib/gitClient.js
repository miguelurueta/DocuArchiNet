import { promisify } from "node:util";
import { execFile as execFileCb } from "node:child_process";
import path from "node:path";

const execFile = promisify(execFileCb);

const toGitPath = (value) => value.replace(/\\/g, "/");

const runGit = async ({ args, cwd, allowFailure = false }) => {
  try {
    const result = await execFile("git", args, { cwd });
    return {
      ok: true,
      stdout: result.stdout ?? "",
      stderr: result.stderr ?? "",
      code: 0,
    };
  } catch (error) {
    if (!allowFailure) {
      throw error;
    }
    return {
      ok: false,
      stdout: error.stdout ?? "",
      stderr: error.stderr ?? "",
      code: error.code ?? 1,
    };
  }
};

export const buildFeatureBranchName = (issueKey) =>
  `feature/${String(issueKey ?? "").trim().toUpperCase()}`;

export const setupProposalBranchAndCommit = async ({
  baseDir,
  issueKey,
  proposalPath,
  remoteName = "origin",
  autoPush = true,
}) => {
  if (!issueKey) {
    throw new Error("No se pudo crear rama Git: issueKey es obligatorio.");
  }
  if (!proposalPath) {
    throw new Error("No se pudo crear commit inicial: proposalPath es obligatorio.");
  }

  const branchName = buildFeatureBranchName(issueKey);
  await runGit({ args: ["checkout", "-B", branchName], cwd: baseDir });

  const proposalRelativePath = toGitPath(path.relative(baseDir, proposalPath));
  await runGit({ args: ["add", proposalRelativePath], cwd: baseDir });

  const staged = await runGit({
    args: ["diff", "--cached", "--name-only"],
    cwd: baseDir,
  });

  const hasStagedChanges = staged.stdout
    .split(/\r?\n/)
    .map((line) => line.trim())
    .filter(Boolean).length > 0;

  let committed = false;
  if (hasStagedChanges) {
    await runGit({
      args: [
        "commit",
        "-m",
        `feat(${String(issueKey).toUpperCase()}): proposal inicial OpenSpec`,
      ],
      cwd: baseDir,
    });
    committed = true;
  }

  let pushed = false;
  if (autoPush) {
    await runGit({
      args: ["push", "-u", remoteName, branchName],
      cwd: baseDir,
    });
    pushed = true;
  }

  return {
    branchName,
    committed,
    pushed,
    proposalRelativePath,
  };
};

