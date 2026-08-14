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
  it("runs opsxj:refine against an active change and forwards --sync", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const statusFn = vi.fn().mockResolvedValue({
      changeName: "scrum-8-modernizar",
      lifecycle: "active",
    });
    const refineFn = vi.fn().mockResolvedValue({
      status: "PASS",
      message: "Refinement aprobado y trazable con design, spec y tasks.",
      refinementPath: "openspec/changes/scrum-8-modernizar/refinement.md",
      synced: true,
      checks: [{ name: "refinement:approved", status: "PASS" }],
    });

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:refine", "SCRUM-8", "--sync"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      statusFn,
      refineFn,
    });

    expect(exitCode).toBe(0);
    expect(refineFn).toHaveBeenCalledWith({
      baseDir: "D:/repo",
      changeName: "scrum-8-modernizar",
      bootstrap: false,
      sync: true,
    });
    expect(stdout.read()).toContain("OPSXJ Refinement: scrum-8-modernizar");
    expect(stdout.read()).toContain("Traceability headers synchronized");
    expect(stderr.read()).toBe("");
  });

  it("runs opsxj:new and prints confirmation messages", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const createProposalFn = vi.fn().mockResolvedValue({
      issue: {
        issueKey: "SCRUM-8",
        summary: "Auto complete asunto",
        description: "Desc",
        metadata: {
          status: "Por hacer",
          statusCategory: "new",
        },
      },
      changeName: "scrum-8-auto-complete-asunto",
      proposalPath: "D:/repo/openspec/changes/scrum-8-auto-complete-asunto/proposal.md",
      refinementArtifacts: {
        designPath: "D:/repo/openspec/changes/scrum-8-auto-complete-asunto/design.md",
        specPath:
          "D:/repo/openspec/changes/scrum-8-auto-complete-asunto/specs/auto-complete-asunto/spec.md",
        tasksPath: "D:/repo/openspec/changes/scrum-8-auto-complete-asunto/tasks.md",
        jiraContextPath:
          "D:/repo/openspec/changes/scrum-8-auto-complete-asunto/specs/auto-complete-asunto/jira-context.md",
      },
    });
    const setupProposalFn = vi.fn().mockResolvedValue({
      branchName: "feature/SCRUM-8",
      committed: true,
      pushed: true,
      proposalRelativePath:
        "openspec/changes/scrum-8-auto-complete-asunto/proposal.md",
    });
    const assertGitCleanFn = vi.fn().mockResolvedValue(undefined);
    const transitionJiraIssueFn = vi.fn().mockResolvedValue({
      name: "Iniciar progreso",
      to: { name: "En curso" },
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
      setupProposalFn,
      assertGitCleanFn,
      transitionJiraIssueFn,
    });

    expect(exitCode).toBe(0);
    expect(createProposalFn).toHaveBeenCalledWith(
      expect.objectContaining({
        issueKey: "SCRUM-8",
        folderStrategy: "summary",
      }),
    );
    expect(stdout.read()).toContain("Carpeta OpenSpec: openspec");
    expect(stdout.read()).toMatch(
      /Jira context creado: openspec[\\/]changes[\\/]scrum-8-auto-complete-asunto[\\/]specs[\\/]auto-complete-asunto[\\/]jira-context\.md/,
    );
    expect(stdout.read()).toContain("Rama Git: feature/SCRUM-8");
    expect(transitionJiraIssueFn).toHaveBeenCalledWith({
      issueKey: "SCRUM-8",
      baseUrl: "https://example.atlassian.net",
      email: "user@example.com",
      apiToken: "token",
      target: "in_progress",
    });
    expect(stdout.read()).toContain("Jira actualizado a: En curso");
    expect(stdout.read()).toContain("Proceso finalizado correctamente");
    expect(stderr.read()).toBe("");
  });

  it("does not transition Jira during opsxj:new when issue is already in progress", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const createProposalFn = vi.fn().mockResolvedValue({
      issue: {
        issueKey: "SCRUM-8",
        summary: "Auto complete asunto",
        description: "Desc",
        metadata: {
          status: "En curso",
          statusCategory: "indeterminate",
        },
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
    const transitionJiraIssueFn = vi.fn();

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
      transitionJiraIssueFn,
    });

    expect(exitCode).toBe(0);
    expect(transitionJiraIssueFn).not.toHaveBeenCalled();
    expect(stdout.read()).toContain("Jira ya esta en curso: En curso");
    expect(stderr.read()).toBe("");
  });

  it("keeps opsxj:orchestrate:new equivalent and propagates both profiles", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const createProposalFn = vi.fn().mockResolvedValue({
      issue: { issueKey: "SCRUM-8", summary: "Modernizar", metadata: { statusCategory: "indeterminate" } },
      changeName: "scrum-8-modernizar",
      proposalPath: "D:/repo/openspec/changes/scrum-8-modernizar/proposal.md",
    });
    const setupProposalFn = vi.fn().mockResolvedValue({
      branchName: "feature/SCRUM-8",
      committed: false,
      pushed: false,
      proposalRelativePath: "openspec/changes/scrum-8-modernizar/proposal.md",
    });
    const assertGitCleanFn = vi.fn().mockResolvedValue(undefined);

    const exitCode = await runOpsxjCommand({
      argv: [
        "opsxj:orchestrate:new",
        "SCRUM-8",
        "--impact",
        "webforms_ui",
        "--profile",
        "enterprise-legacy-modernization",
        "--tech-profile",
        "legacy-webforms-vb",
      ],
      env: {},
      stdout,
      stderr,
      baseDir: "D:/repo",
      createProposalFn,
      setupProposalFn,
      assertGitCleanFn,
    });

    expect(exitCode).toBe(0);
    expect(createProposalFn).toHaveBeenCalledWith(expect.objectContaining({
      issueKey: "SCRUM-8",
      impact: "webforms_ui",
      architectureProfile: "enterprise-legacy-modernization",
      technologyProfile: "legacy-webforms-vb",
    }));
    expect(stdout.read()).toContain("Perfil de arquitectura: enterprise-legacy-modernization");
    expect(stdout.read()).toContain("Perfil tecnologico: legacy-webforms-vb");
    expect(stderr.read()).toBe("");
  });

  it("rejects an invalid profile before touching Git, Jira or branches", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const assertGitCleanFn = vi.fn();
    const createProposalFn = vi.fn();
    const setupProposalFn = vi.fn();

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:orchestrate:new", "SCRUM-8", "--profile", "unknown-profile"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      assertGitCleanFn,
      createProposalFn,
      setupProposalFn,
    });

    expect(exitCode).toBe(1);
    expect(assertGitCleanFn).not.toHaveBeenCalled();
    expect(createProposalFn).not.toHaveBeenCalled();
    expect(setupProposalFn).not.toHaveBeenCalled();
    expect(stderr.read()).toContain("Perfil de arquitectura no soportado");
  });

  it("rejects an invalid technology profile before touching Git or Jira", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const assertGitCleanFn = vi.fn();
    const createProposalFn = vi.fn();

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:orchestrate:new", "SCRUM-8", "--tech-profile", "unknown-stack"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      assertGitCleanFn,
      createProposalFn,
    });

    expect(exitCode).toBe(1);
    expect(assertGitCleanFn).not.toHaveBeenCalled();
    expect(createProposalFn).not.toHaveBeenCalled();
    expect(stderr.read()).toContain("Perfil tecnologico no soportado");
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

  it("runs opsxj:prompt-review and returns the review exit code", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const promptReviewFn = vi.fn().mockResolvedValue({
      status: "fail",
      promptPath: "D:/repo/docs/Architecture/PROMPT.md",
      reportPath: "D:/repo/.opsxj/reports/prompt-review-report.json",
      summary: {
        blockers: 1,
        major: 0,
        minor: 0,
        info: 0,
      },
      findings: [
        {
          severity: "BLOCKER",
          code: "ENTERPRISE_SECTION_REQUIRED",
          message: "Falta seccion obligatoria: OBJETIVO.",
        },
      ],
      exitCode: 1,
    });

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:prompt-review", "docs/Architecture/PROMPT.md"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      promptReviewFn,
    });

    expect(exitCode).toBe(1);
    expect(promptReviewFn).toHaveBeenCalledWith({
      baseDir: "D:/repo",
      promptInput: "docs/Architecture/PROMPT.md",
    });
    expect(stdout.read()).toContain("FAIL prompt-review");
    expect(stdout.read()).toContain("ENTERPRISE_SECTION_REQUIRED");
    expect(stdout.read()).not.toContain("Proceso finalizado correctamente");
    expect(stderr.read()).toBe("");
  });

  it("applies prompt-review corrections when --fix is provided", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const promptReviewFn = vi
      .fn()
      .mockResolvedValueOnce({
        status: "fail",
        promptPath: "D:/repo/docs/Architecture/PROMPT.md",
        reportPath: "D:/repo/.opsxj/reports/prompt-review-report.json",
        summary: {
          blockers: 1,
          major: 0,
          minor: 0,
          info: 0,
        },
        findings: [
          {
            severity: "BLOCKER",
            code: "DOCUMENTATION_PACKAGE_REQUIRED",
            message: "Falta paquete documental.",
          },
        ],
        exitCode: 1,
      })
      .mockResolvedValueOnce({
        status: "pass",
        promptPath: "D:/repo/docs/Architecture/PROMPT.md",
        reportPath: "D:/repo/.opsxj/reports/prompt-review-report.json",
        summary: {
          blockers: 0,
          major: 0,
          minor: 0,
          info: 1,
        },
        findings: [
          {
            severity: "INFO",
            code: "MANUAL_REVIEW_RECOMMENDED",
            message: "Validacion automatica sin bloqueantes.",
          },
        ],
        exitCode: 0,
      });
    const promptCorrectionFn = vi.fn().mockResolvedValue({ applied: true });

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:prompt-review", "--fix", "docs/Architecture/PROMPT.md"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      promptReviewFn,
      promptCorrectionFn,
    });

    expect(exitCode).toBe(0);
    expect(promptCorrectionFn).toHaveBeenCalledWith({
      promptPath: "D:/repo/docs/Architecture/PROMPT.md",
      findings: expect.arrayContaining([
        expect.objectContaining({ code: "DOCUMENTATION_PACKAGE_REQUIRED" }),
      ]),
      baseDir: "D:/repo",
    });
    expect(promptReviewFn).toHaveBeenLastCalledWith({
      baseDir: "D:/repo",
      promptInput: "D:/repo/docs/Architecture/PROMPT.md",
    });
    expect(stdout.read()).toContain("Correcciones aplicadas");
    expect(stdout.read()).toContain("PASS prompt-review");
    expect(stderr.read()).toBe("");
  });

  it("retries prompt-review corrections until the review passes", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const promptReviewFn = vi
      .fn()
      .mockResolvedValueOnce({
        status: "fail",
        promptPath: "D:/repo/docs/Architecture/PROMPT.md",
        reportPath: "D:/repo/.opsxj/reports/prompt-review-report.json",
        summary: {
          blockers: 1,
          major: 0,
          minor: 0,
          info: 0,
        },
        findings: [
          {
            severity: "BLOCKER",
            code: "ENTERPRISE_SECTION_REQUIRED",
            message: "Falta seccion.",
          },
        ],
        exitCode: 1,
      })
      .mockResolvedValueOnce({
        status: "fail",
        promptPath: "D:/repo/docs/Architecture/PROMPT.md",
        reportPath: "D:/repo/.opsxj/reports/prompt-review-report.json",
        summary: {
          blockers: 1,
          major: 0,
          minor: 0,
          info: 0,
        },
        findings: [
          {
            severity: "BLOCKER",
            code: "DOCUMENTATION_PATH_REQUIRED",
            message: "Falta ruta documental.",
          },
        ],
        exitCode: 1,
      })
      .mockResolvedValueOnce({
        status: "pass",
        promptPath: "D:/repo/docs/Architecture/PROMPT.md",
        reportPath: "D:/repo/.opsxj/reports/prompt-review-report.json",
        summary: {
          blockers: 0,
          major: 0,
          minor: 0,
          info: 1,
        },
        findings: [
          {
            severity: "INFO",
            code: "MANUAL_REVIEW_RECOMMENDED",
            message: "Validacion automatica sin bloqueantes.",
          },
        ],
        exitCode: 0,
      });
    const promptCorrectionFn = vi.fn().mockResolvedValue({ applied: true });

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:prompt-review", "--fix", "docs/Architecture/PROMPT.md"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      promptReviewFn,
      promptCorrectionFn,
    });

    expect(exitCode).toBe(0);
    expect(promptCorrectionFn).toHaveBeenCalledTimes(2);
    expect(stdout.read()).toContain("pasada 1");
    expect(stdout.read()).toContain("pasada 2");
    expect(stdout.read()).toContain("PASS prompt-review");
    expect(stderr.read()).toBe("");
  });

  it("runs prompt-review alias", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const promptReviewFn = vi.fn().mockResolvedValue({
      status: "pass",
      promptPath: "D:/repo/docs/Architecture/PROMPT.md",
      reportPath: "D:/repo/.opsxj/reports/prompt-review-report.json",
      summary: {
        blockers: 0,
        major: 0,
        minor: 0,
        info: 1,
      },
      findings: [
        {
          severity: "INFO",
          code: "MANUAL_REVIEW_RECOMMENDED",
          message: "Validacion automatica sin bloqueantes.",
        },
      ],
      exitCode: 0,
    });

    const exitCode = await runOpsxjCommand({
      argv: ["prompt-review", "docs/Architecture/PROMPT.md"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      promptReviewFn,
    });

    expect(exitCode).toBe(0);
    expect(stdout.read()).toContain("PASS prompt-review");
    expect(stderr.read()).toBe("");
  });

  it("runs the neutral technical-review alias while preserving prompt-review compatibility", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const promptReviewFn = vi.fn().mockResolvedValue({
      status: "pass",
      promptPath: "D:/repo/Doc/Tecnica/PROMPT.md",
      reportPath: "D:/repo/.opsxj/reports/technical-review-report.json",
      summary: { blockers: 0, major: 0, minor: 0, info: 0 },
      findings: [],
      exitCode: 0,
    });

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:technical-review", "Doc/Tecnica/PROMPT.md"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      promptReviewFn,
    });

    expect(exitCode).toBe(0);
    expect(promptReviewFn).toHaveBeenCalledWith({
      baseDir: "D:/repo",
      promptInput: "Doc/Tecnica/PROMPT.md",
    });
    expect(stdout.read()).toContain("PASS prompt-review");
    expect(stderr.read()).toBe("");
  });

  it("runs opsxj:status and prints local OpenSpec status", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const statusFn = vi.fn().mockResolvedValue({
      issueKey: "SCRUMCORE-346",
      changeName: "scrumcore-346-implementacion-status",
      lifecycle: "active",
      archivePath: null,
      status: "IN_PROGRESS",
      nextAction: "Completar tasks.md.",
      checks: [
        {
          name: "openspec_artifacts",
          status: "PASS",
          message: "Required OpenSpec artifacts exist.",
        },
        {
          name: "tasks",
          status: "FAIL",
          message: "tasks.md has 3 pending task(s).",
        },
      ],
    });

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:status", "SCRUMCORE-346"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      statusFn,
    });

    expect(exitCode).toBe(0);
    expect(statusFn).toHaveBeenCalledWith({
      baseDir: "D:/repo",
      input: "SCRUMCORE-346",
      env: process.env,
    });
    expect(stdout.read()).toContain("OPSXJ Status: SCRUMCORE-346");
    expect(stdout.read()).toContain("[PENDING] tasks");
    expect(stdout.read()).toContain(
      "Significado: Hay tareas pendientes marcadas como - [ ] en tasks.md.",
    );
    expect(stdout.read()).toContain("Detalle: tasks.md has 3 pending task(s).");
    expect(stdout.read()).not.toContain("Proceso finalizado correctamente");
    expect(stderr.read()).toBe("");
  });

  it("prints opsxj:status user help without resolving status", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const statusFn = vi.fn();

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:status", "--help"],
      stdout,
      stderr,
      baseDir: "D:/repo",
      statusFn,
    });

    expect(exitCode).toBe(0);
    expect(statusFn).not.toHaveBeenCalled();
    expect(stdout.read()).toContain("npm run opsxj:status");
    expect(stdout.read()).toContain("Estados:");
    expect(stdout.read()).toContain("Indicadores observables:");
    expect(stdout.read()).toContain("Estados observables:");
    expect(stdout.read()).toContain("JSON parseable:");
    expect(stderr.read()).toBe("");
  });

  it("runs opsxj:status with JSON output", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const statusFn = vi.fn().mockResolvedValue({
      issueKey: "SCRUMCORE-346",
      changeName: "scrumcore-346-implementacion-status",
      lifecycle: "active",
      archivePath: null,
      status: "READY",
      nextAction: "Cambio listo para validar o archivar.",
      checks: [],
    });

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:status", "SCRUMCORE-346", "--json"],
      env: {},
      stdout,
      stderr,
      baseDir: "D:/repo",
      statusFn,
    });

    expect(exitCode).toBe(0);
    expect(JSON.parse(stdout.read())).toEqual(
      expect.objectContaining({
        issueKey: "SCRUMCORE-346",
        status: "READY",
      }),
    );
    expect(stderr.read()).toBe("");
  });

  it("runs opsxj:orchestrate:status as monorepo status alias", async () => {
    const stdout = buildBufferWriter();
    const stderr = buildBufferWriter();
    const statusFn = vi.fn().mockResolvedValue({
      issueKey: "SCRUMCORE-346",
      changeName: "scrumcore-346-implementacion-status",
      lifecycle: "active",
      archivePath: null,
      status: "READY",
      nextAction: "Cambio listo para validar o archivar.",
      checks: [],
    });

    const exitCode = await runOpsxjCommand({
      argv: ["opsxj:orchestrate:status", "SCRUMCORE-346"],
      env: {},
      stdout,
      stderr,
      baseDir: "D:/repo",
      statusFn,
    });

    expect(exitCode).toBe(0);
    expect(statusFn).toHaveBeenCalledWith({
      baseDir: "D:/repo",
      input: "SCRUMCORE-346",
      env: {},
    });
    expect(stdout.read()).toContain("OPSXJ Status: SCRUMCORE-346");
    expect(stderr.read()).toBe("");
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
