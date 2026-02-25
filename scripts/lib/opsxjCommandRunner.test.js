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
    });

    expect(exitCode).toBe(0);
    expect(createProposalFn).toHaveBeenCalledWith(
      expect.objectContaining({
        issueKey: "SCRUM-8",
        folderStrategy: "summary",
      }),
    );
    expect(stdout.read()).toContain("Carpeta OpenSpec: openspec");
    expect(stdout.read()).toContain("Proceso finalizado correctamente");
    expect(stderr.read()).toBe("");
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
});
