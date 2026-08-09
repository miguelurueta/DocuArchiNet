import { describe, expect, it, vi } from "vitest";
import {
  createOrGetPullRequest,
  getMergedPullRequestByBranch,
} from "./githubClient.js";

describe("githubClient", () => {
  it("creates pull request when GitHub accepts request", async () => {
    const fetchImpl = vi.fn().mockResolvedValue({
      ok: true,
      status: 201,
      json: async () => ({
        number: 10,
        html_url: "https://github.com/acme/repo/pull/10",
      }),
    });

    const result = await createOrGetPullRequest({
      repo: "acme/repo",
      token: "ghs_token",
      issueKey: "SCRUM-10",
      summary: "Demo",
      branchName: "feature/SCRUM-10",
      fetchImpl,
    });

    expect(result.created).toBe(true);
    expect(result.pullRequest.number).toBe(10);
  });

  it("returns merged pull request for a branch when it exists", async () => {
    const fetchImpl = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [
        { number: 10, merged_at: null, html_url: "https://github.com/acme/repo/pull/10" },
        { number: 11, merged_at: "2026-02-25T20:53:45Z", html_url: "https://github.com/acme/repo/pull/11" },
      ],
    });

    const result = await getMergedPullRequestByBranch({
      repo: "acme/repo",
      token: "ghs_token",
      branchName: "feature/SCRUM-12",
      baseBranch: "main",
      fetchImpl,
    });

    expect(result.pullRequest?.number).toBe(11);
  });
});
