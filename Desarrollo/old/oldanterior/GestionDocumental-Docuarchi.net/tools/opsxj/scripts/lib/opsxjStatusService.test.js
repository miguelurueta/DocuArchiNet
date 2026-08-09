import { mkdir, mkdtemp, readFile, rm, writeFile } from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { describe, expect, it } from "vitest";
import { getOpsxjStatus } from "./opsxjStatusService.js";

const createTempRepo = async () => {
  const tempDir = await mkdtemp(path.join(os.tmpdir(), "opsxj-status-"));
  await mkdir(path.join(tempDir, "openspec", "changes"), { recursive: true });
  return tempDir;
};

const writeChange = async ({
  baseDir,
  changeName,
  archivedName = null,
  files = {},
}) => {
  const changeDir = archivedName
    ? path.join(baseDir, "openspec", "changes", "archive", archivedName)
    : path.join(baseDir, "openspec", "changes", changeName);
  await mkdir(path.join(changeDir, "specs", "demo"), { recursive: true });

  for (const [relativePath, content] of Object.entries(files)) {
    const filePath = path.join(changeDir, relativePath);
    await mkdir(path.dirname(filePath), { recursive: true });
    await writeFile(filePath, content, "utf8");
  }

  return changeDir;
};

const completeFiles = {
  "proposal.md": "## Why\nDemo\n",
  "design.md": "## Context\nDemo\n",
  "tasks.md": "- [x] Done\n",
  "specs/demo/spec.md": "## ADDED Requirements\n",
};

describe("opsxjStatusService", () => {
  it("returns NOT_STARTED when no active or archived change exists", async () => {
    const baseDir = await createTempRepo();
    try {
      const result = await getOpsxjStatus({ baseDir, input: "SCRUMCORE-999", env: {} });

      expect(result.status).toBe("NOT_STARTED");
      expect(result.lifecycle).toBe("not_started");
      expect(result.checks).toEqual(
        expect.arrayContaining([
          expect.objectContaining({
            name: "openspec_change",
            status: "FAIL",
            state: "MISSING",
          }),
        ]),
      );
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("reports active change with complete artifacts", async () => {
    const baseDir = await createTempRepo();
    try {
      await writeChange({
        baseDir,
        changeName: "scrumcore-346-implementacion-status",
        files: completeFiles,
      });

      const result = await getOpsxjStatus({
        baseDir,
        input: "SCRUMCORE-346",
        env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" },
      });

      expect(result.lifecycle).toBe("active");
      expect(result.status).toBe("READY");
      expect(result.checks).toEqual(
        expect.arrayContaining([
          expect.objectContaining({ name: "openspec_artifacts", status: "PASS" }),
          expect.objectContaining({
            name: "tasks",
            status: "PASS",
            state: "COMPLETE",
            description: "No hay tareas pendientes marcadas como - [ ] en tasks.md.",
          }),
        ]),
      );
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("returns BLOCKED when required artifacts are missing", async () => {
    const baseDir = await createTempRepo();
    try {
      await writeChange({
        baseDir,
        changeName: "scrumcore-346-implementacion-status",
        files: {
          "proposal.md": "## Why\nDemo\n",
          "tasks.md": "- [x] Done\n",
        },
      });

      const result = await getOpsxjStatus({ baseDir, input: "SCRUMCORE-346", env: {} });

      expect(result.status).toBe("BLOCKED");
      expect(result.checks).toEqual(
        expect.arrayContaining([
          expect.objectContaining({
            name: "openspec_artifacts",
            status: "FAIL",
            message: expect.stringContaining("design.md"),
          }),
        ]),
      );
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("returns IN_PROGRESS when tasks are pending", async () => {
    const baseDir = await createTempRepo();
    try {
      await writeChange({
        baseDir,
        changeName: "scrumcore-346-implementacion-status",
        files: {
          ...completeFiles,
          "tasks.md": "- [ ] Pending\n- [x] Done\n",
        },
      });

      const result = await getOpsxjStatus({
        baseDir,
        input: "scrumcore-346-implementacion-status",
        env: {},
      });

      expect(result.status).toBe("IN_PROGRESS");
      expect(result.nextAction).toBe("Completar tasks.md.");
      expect(result.checks).toEqual(
        expect.arrayContaining([
          expect.objectContaining({
            name: "tasks",
            status: "FAIL",
            state: "PENDING",
            description: "Hay tareas pendientes marcadas como - [ ] en tasks.md.",
            details: expect.objectContaining({ pending: 1 }),
          }),
        ]),
      );
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("returns ARCHIVED when the change is under archive", async () => {
    const baseDir = await createTempRepo();
    try {
      await writeChange({
        baseDir,
        changeName: "scrumcore-346-implementacion-status",
        archivedName: "2026-07-30-scrumcore-346-implementacion-status",
        files: completeFiles,
      });

      const result = await getOpsxjStatus({
        baseDir,
        input: "SCRUMCORE-346",
        env: { OPSXJ_OPENSPEC_REVIEW_CONFIRMED: "1" },
      });

      expect(result.status).toBe("ARCHIVED");
      expect(result.lifecycle).toBe("archived");
      expect(result.archivePath).toContain("2026-07-30-scrumcore-346-implementacion-status");
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("reports archived change with open pull request as pending close flow", async () => {
    const baseDir = await createTempRepo();
    try {
      await writeChange({
        baseDir,
        changeName: "scrumcore-346-implementacion-status",
        archivedName: "2026-07-30-scrumcore-346-implementacion-status",
        files: completeFiles,
      });
      const fetchImpl = async (url) => {
        const target = String(url);
        if (target.includes("/pulls?state=open")) {
          return Response.json([
            {
              state: "open",
              merged_at: null,
              html_url: "https://github.com/acme/repo/pull/369",
            },
          ]);
        }
        if (target.includes("/rest/api/3/issue/")) {
          return Response.json({
            fields: {
              summary: "IMPLEMENTACION-STATUS",
              status: {
                name: "En curso",
                statusCategory: { key: "indeterminate" },
              },
            },
          });
        }
        return Response.json([]);
      };

      const result = await getOpsxjStatus({
        baseDir,
        input: "SCRUMCORE-346",
        env: {
          GITHUB_TOKEN: "ghs_token",
          GITHUB_REPO: "acme/repo",
          JIRA_BASE_URL: "https://example.atlassian.net",
          JIRA_EMAIL: "user@example.com",
          JIRA_API_TOKEN: "token",
        },
        fetchImpl,
      });

      expect(result.status).toBe("ARCHIVED");
      expect(result.nextAction).toBe("Mergear el PR y luego ejecutar opsxj:close.");
      expect(result.checks).toEqual(
        expect.arrayContaining([
          expect.objectContaining({
            name: "pull_request",
            status: "WARN",
            message: expect.stringContaining("open and not merged"),
          }),
          expect.objectContaining({
            name: "jira_status",
            status: "WARN",
            message: expect.stringContaining("not done"),
          }),
        ]),
      );
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("reports archived merged PR with Jira not done as requiring opsxj:close", async () => {
    const baseDir = await createTempRepo();
    try {
      await writeChange({
        baseDir,
        changeName: "scrumcore-346-implementacion-status",
        archivedName: "2026-07-30-scrumcore-346-implementacion-status",
        files: completeFiles,
      });
      const fetchImpl = async (url) => {
        const target = String(url);
        if (target.includes("/pulls?state=open")) {
          return Response.json([]);
        }
        if (target.includes("/pulls?state=closed")) {
          return Response.json([
            {
              state: "closed",
              merged_at: "2026-07-31T01:00:00Z",
              html_url: "https://github.com/acme/repo/pull/369",
            },
          ]);
        }
        if (target.includes("/rest/api/3/issue/")) {
          return Response.json({
            fields: {
              summary: "IMPLEMENTACION-STATUS",
              status: {
                name: "En revision",
                statusCategory: { key: "indeterminate" },
              },
            },
          });
        }
        return Response.json([]);
      };

      const result = await getOpsxjStatus({
        baseDir,
        input: "SCRUMCORE-346",
        env: {
          GITHUB_TOKEN: "ghs_token",
          GITHUB_REPO: "acme/repo",
          JIRA_BASE_URL: "https://example.atlassian.net",
          JIRA_EMAIL: "user@example.com",
          JIRA_API_TOKEN: "token",
        },
        fetchImpl,
      });

      expect(result.status).toBe("ARCHIVED");
      expect(result.nextAction).toBe("Ejecutar opsxj:close para mover Jira despues del merge.");
      expect(result.checks).toEqual(
        expect.arrayContaining([
          expect.objectContaining({
            name: "pull_request",
            status: "PASS",
          }),
          expect.objectContaining({
            name: "jira_status",
            status: "WARN",
          }),
        ]),
      );
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });

  it("does not modify files while calculating status", async () => {
    const baseDir = await createTempRepo();
    try {
      const tasksPath = path.join(
        await writeChange({
          baseDir,
          changeName: "scrumcore-346-implementacion-status",
          files: completeFiles,
        }),
        "tasks.md",
      );
      const before = await readFile(tasksPath, "utf8");

      await getOpsxjStatus({ baseDir, input: "SCRUMCORE-346", env: {} });

      await expect(readFile(tasksPath, "utf8")).resolves.toBe(before);
    } finally {
      await rm(baseDir, { recursive: true, force: true });
    }
  });
});
