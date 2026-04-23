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

const normalizeLines = (value) =>
  String(value ?? "")
    .split(/\r?\n/)
    .map((line) => line.trim())
    .filter(Boolean);

export const assertGitClean = async ({ baseDir, commandLabel = "opsxj:new" }) => {
  if (!baseDir) {
    throw new Error("No se pudo validar Git: baseDir es obligatorio.");
  }

  const status = await runGit({
    args: ["status", "--porcelain"],
    cwd: baseDir,
  });
  const statusLines = normalizeLines(status.stdout);
  if (statusLines.length > 0) {
    const preview = statusLines.slice(0, 10).join("\n");
    throw new Error(
      `Git tiene cambios sin commit. ${commandLabel} se bloquea para evitar mezclar tickets.\n\nCambios detectados (preview):\n${preview}\n\nSolucion:\n- Revise: git status\n- Luego: git add -A && git commit -m \"...\" (o git stash) y reintente.`,
    );
  }

  const staged = await runGit({
    args: ["diff", "--cached", "--name-only"],
    cwd: baseDir,
  });
  const stagedLines = normalizeLines(staged.stdout);
  if (stagedLines.length > 0) {
    const preview = stagedLines.slice(0, 10).join("\n");
    throw new Error(
      `Git tiene cambios staged sin commit. ${commandLabel} se bloquea.\n\nStaged (preview):\n${preview}\n\nSolucion:\n- Commit: git commit -m \"...\" (o reset/stash) y reintente.`,
    );
  }
};

export const assertGitCleanAndSynced = async ({
  baseDir,
  expectedBranchName,
  remoteName = "origin",
}) => {
  if (!baseDir) {
    throw new Error("No se pudo validar Git: baseDir es obligatorio.");
  }

  const branchResult = await runGit({
    args: ["rev-parse", "--abbrev-ref", "HEAD"],
    cwd: baseDir,
  });
  const currentBranch = branchResult.stdout.trim();

  if (expectedBranchName && currentBranch !== expectedBranchName) {
    throw new Error(
      `Git no esta en la rama esperada.\n- Esperada: ${expectedBranchName}\n- Actual: ${currentBranch}\nSolucion: cambie a la rama correcta antes de archivar.`,
    );
  }

  const status = await runGit({
    args: ["status", "--porcelain"],
    cwd: baseDir,
  });
  const statusLines = normalizeLines(status.stdout);
  if (statusLines.length > 0) {
    const preview = statusLines.slice(0, 10).join("\n");
    throw new Error(
      `Git tiene cambios sin commit. opsxj:archive se bloquea para evitar PR incompleto.\n\nCambios detectados (preview):\n${preview}\n\nSolucion:\n- Revise: git status\n- Luego: git add -A && git commit -m \"...\" && git push`,
    );
  }

  const staged = await runGit({
    args: ["diff", "--cached", "--name-only"],
    cwd: baseDir,
  });
  const stagedLines = normalizeLines(staged.stdout);
  if (stagedLines.length > 0) {
    const preview = stagedLines.slice(0, 10).join("\n");
    throw new Error(
      `Git tiene cambios staged sin commit. opsxj:archive se bloquea.\n\nStaged (preview):\n${preview}\n\nSolucion:\n- Commit: git commit -m \"...\" && git push`,
    );
  }

  const upstream = await runGit({
    args: ["rev-parse", "--abbrev-ref", "--symbolic-full-name", "@{upstream}"],
    cwd: baseDir,
    allowFailure: true,
  });
  if (!upstream.ok) {
    throw new Error(
      `La rama no tiene upstream configurado.\nSolucion: git push -u ${remoteName} ${currentBranch}`,
    );
  }

  const aheadBehind = await runGit({
    args: ["rev-list", "--left-right", "--count", "@{upstream}...HEAD"],
    cwd: baseDir,
  });
  const [behindRaw, aheadRaw] = aheadBehind.stdout.trim().split(/\s+/);
  const behind = Number(behindRaw ?? "0");
  const ahead = Number(aheadRaw ?? "0");

  if (Number.isNaN(behind) || Number.isNaN(ahead)) {
    throw new Error(
      `No se pudo validar el estado contra upstream (salida inesperada): ${aheadBehind.stdout}`,
    );
  }

  if (behind !== 0 || ahead !== 0) {
    throw new Error(
      `La rama no esta sincronizada con upstream.\n- Behind: ${behind}\n- Ahead: ${ahead}\n\nSolucion:\n- Si ahead>0: git push\n- Si behind>0: git pull --rebase`,
    );
  }
};

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

