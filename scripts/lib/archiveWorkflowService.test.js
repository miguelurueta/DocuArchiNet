import { mkdtemp, mkdir, rm } from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { describe, expect, it } from "vitest";
import { resolveChangeNameFromIssueKey } from "./archiveWorkflowService.js";

describe("archiveWorkflowService", () => {
  it("resolves active change by issue key prefix", async () => {
    const tempDir = await mkdtemp(path.join(os.tmpdir(), "archive-workflow-"));
    const changesDir = path.join(tempDir, "openspec", "changes");
    await mkdir(path.join(changesDir, "archive"), { recursive: true });
    await mkdir(path.join(changesDir, "scrum-10-demo-change"), { recursive: true });

    try {
      const resolved = await resolveChangeNameFromIssueKey({
        baseDir: tempDir,
        issueKey: "SCRUM-10",
      });
      expect(resolved).toBe("scrum-10-demo-change");
    } finally {
      await rm(tempDir, { recursive: true, force: true });
    }
  });
});

