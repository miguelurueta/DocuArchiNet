import { mkdir, mkdtemp, readFile, rm, writeFile } from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { describe, expect, it } from "vitest";
import {
  appendRunChecklistEvent,
  getRunChecklistPath,
  readRunChecklist,
  resolveRunChecklistStage,
} from "./runChecklistService.js";

describe("runChecklistService", () => {
  it("writes a versioned append-only event without sensitive input", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-runs-"));
    try {
      const first = await appendRunChecklistEvent({
        baseDir,
        issueKey: "doc-9",
        stage: "review",
        status: "pass",
        sha: "abc",
        actor: "Codex",
        source: "opsxj:validate",
        reference: "formal-review",
      });
      await appendRunChecklistEvent({ baseDir, issueKey: "DOC-9", stage: "validate", status: "fail", sha: "abc", detail: "Falta evidencia manual." });
      const saved = JSON.parse(await readFile(first.filePath, "utf8"));
      expect(saved).toMatchObject({ version: 1, issueKey: "DOC-9" });
      expect(saved.events).toHaveLength(2);
      expect(saved.events[0]).toMatchObject({ stage: "review", status: "pass", sha: "abc", actor: "Codex" });
      await expect(appendRunChecklistEvent({ baseDir, issueKey: "DOC-9", stage: "validate", status: "fail", sha: "abc", detail: "token=super-secreto" })).rejects.toThrow("secretos");
      expect(JSON.stringify(saved)).not.toContain("super-secreto");
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("resolves the latest event for the current SHA and reports stale history", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-runs-"));
    try {
      await appendRunChecklistEvent({ baseDir, issueKey: "DOC-10", stage: "review", status: "pass", sha: "first" });
      const readResult = await readRunChecklist({ baseDir, issueKey: "DOC-10" });
      expect(resolveRunChecklistStage({ readResult, stage: "review", currentSha: "first" }).state).toBe("COMPLETE");
      expect(resolveRunChecklistStage({ readResult, stage: "review", currentSha: "second" })).toMatchObject({ state: "STALE", sha: "first" });
      await appendRunChecklistEvent({ baseDir, issueKey: "DOC-10", stage: "review", status: "fail", sha: "second", detail: "Revision rechazada." });
      const failed = await readRunChecklist({ baseDir, issueKey: "DOC-10" });
      expect(resolveRunChecklistStage({ readResult: failed, stage: "review", currentSha: "second" })).toMatchObject({ state: "BLOCKED", detail: "Revision rechazada." });
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("tolerates an absent or corrupt local file without allowing it to be overwritten", async () => {
    const baseDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-runs-"));
    try {
      const absent = await readRunChecklist({ baseDir, issueKey: "DOC-11" });
      expect(absent.state).toBe("absent");
      expect(resolveRunChecklistStage({ readResult: absent, stage: "validate", currentSha: "abc" }).state).toBe("UNAVAILABLE");
      const targetPath = getRunChecklistPath({ baseDir, issueKey: "DOC-11" });
      await mkdir(path.dirname(targetPath), { recursive: true });
      await writeFile(targetPath, "{invalid", "utf8");
      const corrupt = await readRunChecklist({ baseDir, issueKey: "DOC-11" });
      expect(corrupt.state).toBe("invalid");
      await expect(appendRunChecklistEvent({ baseDir, issueKey: "DOC-11", stage: "new", status: "pass", sha: "abc" })).rejects.toThrow("No se puede registrar OPSXJ");
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });
});
