import { describe, expect, it, vi } from "vitest";
import { createOrGetPullRequest } from "./githubClient.js";

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
});

