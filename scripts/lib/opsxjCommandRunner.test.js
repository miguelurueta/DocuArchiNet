import { describe, expect, it, vi } from "vitest";
import { runOpsxjCommand } from "./opsxjCommandRunner.js";

const buildBufferWriter = () => {
  let buffer = "";
  return {
    write: (chunk) => {
      buffer += String(chunk);
    },
    read: () => buffer,
  };
};

describe("opsxjCommandRunner", () => {
  it("runs opsxj:new and prints confirmation messages", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const createProposalFn = vi.fn().mockResolvedValue({
      issue: {
        issueKey: "SCRUM-8",
        summary: "Auto complete asunto",
        description: "Desc",
      },
      changeName: "scrum-8-auto-complete-asunto",
      proposalPath: "D:/repo/openspec/changes/scrum-8-auto-complete-asunto/proposal.md",
    });
    const setupProposalFn = vi.fn().mockResolvedValue({
      branchName: "feature/SCRUM-8",
      committed: true,
      pushed: true,
      proposalRelativePath:
        "openspec/changes/scrum-8-auto-complete-asunto/proposal.md",
    });
    const assertGitCleanFn = vi.fn().mockResolvedValue(undefined);

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:new", "SCRUM-8"],
      env: {
        JIRA_BASE_URL: "https://example.atlassian.net",
        JIRA_EMAIL: "user@example.com",
        JIRA_API_TOKEN: "token",
      },
      stdout,
      stderr,
      baseDir: "D:/repo",
      createProposalFn,
      setupProposalFn,
      assertGitCleanFn,
    });

    expect(exitCode).toBe(0);
    expect(createProposalFn).toHaveBeenCalledWith(
      expect.objectContaining({
        issueKey: "SCRUM-8",
        folderStrategy: "summary",
      }),
    );
    expect(stdout.read()).toContain("Carpeta OpenSpec: openspec");
    expect(stdout.read()).toContain("Rama Git: feature/SCRUM-8");
    expect(stdout.read()).toContain("Proceso finalizado correctamente");
    expect(stderr.read()).toBe("");
  });

  it("blocks opsxj:new when jira lookup fails and does not continue to git", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const createProposalFn = vi.fn().mockRejectedValue(new Error("fetch failed"));
    const setupProposalFn = vi.fn();
    const assertGitCleanFn = vi.fn().mockResolvedValue(undefined);

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:new", "SCRUM-8"],
      env: {
        JIRA_BASE_URL: "https://example.atlassian.net",
        JIRA_EMAIL: "user@example.com",
        JIRA_API_TOKEN: "token",
      },
      stdout,
      stderr,
      baseDir: "D:/repo",
      createProposalFn,
      setupProposalFn,
      assertGitCleanFn,
    });

    expect(exitCode).toBe(1);
    expect(createProposalFn).toHaveBeenCalledTimes(1);
    expect(setupProposalFn).not.toHaveBeenCalled();
    expect(stdout.read()).toBe("");
    expect(stderr.read()).toContain("[opsxj:error] fetch failed");
  });

  it("blocks opsxj:new when git has pending changes before consulting Jira", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const createProposalFn = vi.fn();
    const setupProposalFn = vi.fn();

    const assertGitCleanFn = vi.fn().mockRejectedValue(new Error("Git tiene cambios sin commit"));

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:new", "SCRUM-8"],
      env: {
        JIRA_BASE_URL: "https://example.atlassian.net",
        JIRA_EMAIL: "user@example.com",
        JIRA_API_TOKEN: "token",
      },
      stdout,
      stderr,
      baseDir: "D:/repo",
      createProposalFn,
      setupProposalFn,
      assertGitCleanFn,
    });

    expect(exitCode).toBe(1);
    expect(assertGitCleanFn).toHaveBeenCalledTimes(1);
    expect(createProposalFn).not.toHaveBeenCalled();
    expect(setupProposalFn).not.toHaveBeenCalled();
    expect(stderr.read()).toContain("Git tiene cambios sin commit");
  });

  it("returns clear error when command is unsupported", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:unknown"],
      stdout,
      stderr,
    });

    expect(exitCode).toBe(1);
    expect(stderr.read()).toContain("Comando no soportado");
  });

  it("runs opsxj:archive and prints PR context", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const archiveFn = vi.fn().mockResolvedValue({
      changeName: "scrum-10-demo",
      pullRequestCreated: true,
      archivedWithSkipSpecs: false,
      pullRequest: { html_url: "https://github.com/acme/repo/pull/10" },
    });
    const assertGitCleanAndSyncedFn = vi.fn().mockResolvedValue(undefined);

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:archive", "SCRUM-10"],
      env: {
        JIRA_BASE_URL: "https://example.atlassian.net",
        JIRA_EMAIL: "user@example.com",
        JIRA_API_TOKEN: "token",
        GITHUB_TOKEN: "ghs_token",
        GITHUB_REPO: "acme/repo",
      },
      stdout,
      stderr,
      baseDir: "D:/repo",
      archiveFn,
      assertGitCleanAndSyncedFn,
    });

    expect(exitCode).toBe(0);
    expect(stdout.read()).toContain("PR creado");
    expect(stderr.read()).toBe("");
  });

  it("runs opsxj:close and closes Jira when PR is merged", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const closeFn = vi.fn().mockResolvedValue({
      pullRequest: { html_url: "https://github.com/acme/repo/pull/24" },
      transition: { to: { name: "Finalizado" } },
    });

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:close", "SCRUM-12"],
      env: {
        JIRA_BASE_URL: "https://example.atlassian.net",
        JIRA_EMAIL: "user@example.com",
        JIRA_API_TOKEN: "token",
        GITHUB_TOKEN: "ghs_token",
        GITHUB_REPO: "acme/repo",
      },
      stdout,
      stderr,
      closeFn,
    });

    expect(exitCode).toBe(0);
    expect(closeFn).toHaveBeenCalledWith(
      expect.objectContaining({
        issueKey: "SCRUM-12",
        branchName: "feature/SCRUM-12",
      }),
    );
    expect(stdout.read()).toContain("PR mergeado validado");
    expect(stdout.read()).toContain("Jira actualizado a: Finalizado");
    expect(stderr.read()).toBe("");
  });
});
