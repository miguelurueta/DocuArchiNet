import { describe, expect, it, vi } from "vitest";
import { closeIssueFromMergedPr } from "./closeWorkflowService.js";

vi.mock("./githubClient.js", () => ({
  getMergedPullRequestByBranch: vi.fn(),
}));

vi.mock("./jiraClient.js", () => ({
  transitionJiraIssue: vi.fn(),
  addJiraComment: vi.fn(),
}));

import { getMergedPullRequestByBranch } from "./githubClient.js";
import { transitionJiraIssue, addJiraComment } from "./jiraClient.js";

describe("closeWorkflowService", () => {
  it("cierra Jira cuando existe PR mergeado", async () => {
    vi.mocked(getMergedPullRequestByBranch).mockResolvedValue({
      pullRequest: {
        number: 24,
        html_url: "https://github.com/acme/repo/pull/24",
        merged_at: "2026-02-25T20:53:45Z",
      },
      repository: { owner: "acme", repo: "repo" },
    });
    vi.mocked(transitionJiraIssue).mockResolvedValue({
      id: "41",
      to: { name: "Finalizado" },
    });
    vi.mocked(addJiraComment).mockResolvedValue(undefined);

    const result = await closeIssueFromMergedPr({
      issueKey: "SCRUM-12",
      branchName: "feature/SCRUM-12",
      jira: {
        baseUrl: "https://example.atlassian.net",
        email: "user@example.com",
        apiToken: "token",
      },
      github: {
        repo: "acme/repo",
        token: "ghs_token",
        baseBranch: "main",
      },
    });

    expect(result.pullRequest.number).toBe(24);
    expect(transitionJiraIssue).toHaveBeenCalledWith(
      expect.objectContaining({
        issueKey: "SCRUM-12",
        target: "done",
      }),
    );
    expect(addJiraComment).toHaveBeenCalledWith(
      expect.objectContaining({
        issueKey: "SCRUM-12",
      }),
    );
  });

  it("falla cuando no hay PR mergeado", async () => {
    vi.mocked(getMergedPullRequestByBranch).mockResolvedValue({
      pullRequest: null,
      repository: { owner: "acme", repo: "repo" },
    });

    await expect(
      closeIssueFromMergedPr({
        issueKey: "SCRUM-99",
        branchName: "feature/SCRUM-99",
        jira: {
          baseUrl: "https://example.atlassian.net",
          email: "user@example.com",
          apiToken: "token",
        },
        github: {
          repo: "acme/repo",
          token: "ghs_token",
          baseBranch: "main",
        },
      }),
    ).rejects.toThrow(/no existe un pr mergeado/i);
  });
});
