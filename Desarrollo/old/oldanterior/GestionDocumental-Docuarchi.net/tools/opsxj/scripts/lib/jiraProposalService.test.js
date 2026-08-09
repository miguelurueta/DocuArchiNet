import { mkdtemp, readFile, rm } from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { describe, expect, it, vi } from "vitest";
import { createProposalFromJira } from "./jiraProposalService.js";

describe("jiraProposalService", () => {
  it("creates proposal using summary-based OpenSpec folder strategy", async () => {
    const tempDir = await mkdtemp(path.join(os.tmpdir(), "jira-proposal-service-"));
    const fetchImpl = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => ({
        fields: {
          summary: "Auto complete asunto",
          description: "Detalle",
        },
      }),
    });

    try {
      const result = await createProposalFromJira({
        issueKey: "SCRUM-8",
        baseUrl: "https://example.atlassian.net",
        email: "user@example.com",
        apiToken: "token",
        baseDir: tempDir,
        folderStrategy: "summary",
        commandName: "opsxj.js opsxj:new",
        fetchImpl,
      });

      expect(result.changeName).toBe("scrum-8-auto-complete-asunto");
      expect(result.proposalPath).toContain(
        path.join(
          "openspec",
          "changes",
          "scrum-8-auto-complete-asunto",
          "proposal.md",
        ),
      );
      const content = await readFile(result.proposalPath, "utf8");
      expect(content).toContain("`auto-complete-asunto`");
      expect(content).not.toContain("`jira-proposal-generator`");
      expect(result.refinementArtifacts?.designPath).toContain(
        path.join("openspec", "changes", "scrum-8-auto-complete-asunto", "design.md"),
      );
      expect(result.refinementArtifacts?.tasksPath).toContain(
        path.join("openspec", "changes", "scrum-8-auto-complete-asunto", "tasks.md"),
      );
      expect(result.refinementArtifacts?.jiraContextPath).toContain(
        path.join(
          "openspec",
          "changes",
          "scrum-8-auto-complete-asunto",
          "specs",
          "auto-complete-asunto",
          "jira-context.md",
        ),
      );
    } finally {
      await rm(tempDir, { recursive: true, force: true });
    }
  });
});
