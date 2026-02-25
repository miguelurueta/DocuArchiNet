import { describe, expect, it, vi } from "vitest";
import { buildJiraAuthHeader, fetchJiraIssue, normalizeDescription } from "./jiraClient.js";

describe("jiraClient", () => {
  it("buildJiraAuthHeader throws when credentials are missing", () => {
    expect(() => buildJiraAuthHeader("", "token")).toThrow(/credenciales/i);
    expect(() => buildJiraAuthHeader("user", "")).toThrow(/credenciales/i);
  });

  it("normalizeDescription handles ADF description", () => {
    const adf = {
      type: "doc",
      version: 1,
      content: [
        { type: "paragraph", content: [{ type: "text", text: "Linea 1" }] },
        { type: "paragraph", content: [{ type: "text", text: "Linea 2" }] },
      ],
    };

    expect(normalizeDescription(adf)).toBe("Linea 1\nLinea 2");
  });

  it("fetchJiraIssue returns summary and description", async () => {
    const fetchImpl = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => ({
        fields: { summary: "Titulo", description: "Descripcion" },
      }),
    });

    const result = await fetchJiraIssue({
      issueKey: "ABC-1",
      baseUrl: "https://example.atlassian.net",
      email: "user@example.com",
      apiToken: "token",
      fetchImpl,
    });

    expect(result).toEqual({
      issueKey: "ABC-1",
      summary: "Titulo",
      description: "Descripcion",
    });
  });

  it("fetchJiraIssue throws with clear error on failed response", async () => {
    const fetchImpl = vi.fn().mockResolvedValue({
      ok: false,
      status: 401,
      statusText: "Unauthorized",
      text: async () => "Invalid credentials",
    });

    await expect(
      fetchJiraIssue({
        issueKey: "ABC-2",
        baseUrl: "https://example.atlassian.net",
        email: "user@example.com",
        apiToken: "bad",
        fetchImpl,
      })
    ).rejects.toThrow(/401/i);
  });

  it("fetchJiraIssue includes command name in usage when issueKey is missing", async () => {
    await expect(
      fetchJiraIssue({
        issueKey: "",
        baseUrl: "https://example.atlassian.net",
        email: "user@example.com",
        apiToken: "token",
        commandName: "generate-proposal-from-jira.js",
      })
    ).rejects.toThrow(/generate-proposal-from-jira\.js/i);
  });
});
