import { describe, expect, it, vi } from "vitest";
import {
  addJiraComment,
  buildJiraAuthHeader,
  fetchJiraIssue,
  normalizeDescription,
  transitionJiraIssue,
} from "./jiraClient.js";

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
        fields: {
          summary: "Titulo",
          description: "Descripcion",
          issuetype: { name: "Story" },
          priority: { name: "High" },
          status: { name: "En curso", statusCategory: { key: "indeterminate" } },
          labels: ["frontend"],
          components: [{ name: "gestion" }],
          subtasks: [{ key: "ABC-2", fields: { summary: "Subtask demo" } }],
          comment: {
            comments: [{ id: "1001", body: "Comentario demo" }],
          },
        },
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
      metadata: {
        issueType: "Story",
        priority: "High",
        status: "En curso",
        statusCategory: "indeterminate",
        labels: ["frontend"],
        components: ["gestion"],
        subtasks: [{ key: "ABC-2", summary: "Subtask demo" }],
        comments: [{ id: "1001", body: "Comentario demo" }],
      },
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

  it("transitionJiraIssue picks done transition by status category", async () => {
    const fetchImpl = vi
      .fn()
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => ({
          transitions: [
            { id: "21", name: "Finalizado", to: { statusCategory: { key: "done" } } },
          ],
        }),
      })
      .mockResolvedValueOnce({
        ok: true,
        status: 204,
        text: async () => "",
      });

    const transition = await transitionJiraIssue({
      issueKey: "ABC-1",
      baseUrl: "https://example.atlassian.net",
      email: "user@example.com",
      apiToken: "token",
      target: "done",
      fetchImpl,
    });

    expect(transition.id).toBe("21");
  });

  it("transitionJiraIssue picks in progress transition by status category", async () => {
    const fetchImpl = vi
      .fn()
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => ({
          transitions: [
            { id: "11", name: "Abrir", to: { statusCategory: { key: "new" } } },
            {
              id: "31",
              name: "Iniciar progreso",
              to: { name: "En curso", statusCategory: { key: "indeterminate" } },
            },
          ],
        }),
      })
      .mockResolvedValueOnce({
        ok: true,
        status: 204,
        text: async () => "",
      });

    const transition = await transitionJiraIssue({
      issueKey: "ABC-1",
      baseUrl: "https://example.atlassian.net",
      email: "user@example.com",
      apiToken: "token",
      target: "in_progress",
      fetchImpl,
    });

    expect(transition.id).toBe("31");
    expect(fetchImpl).toHaveBeenLastCalledWith(
      "https://example.atlassian.net/rest/api/3/issue/ABC-1/transitions",
      expect.objectContaining({
        method: "POST",
        body: JSON.stringify({ transition: { id: "31" } }),
      }),
    );
  });

  it("addJiraComment sends ADF payload", async () => {
    const fetchImpl = vi.fn().mockResolvedValue({
      ok: true,
      status: 201,
      json: async () => ({ id: "10001" }),
    });

    await addJiraComment({
      issueKey: "ABC-1",
      baseUrl: "https://example.atlassian.net",
      email: "user@example.com",
      apiToken: "token",
      message: "PR: https://github.com/acme/repo/pull/1",
      fetchImpl,
    });

    const secondArg = fetchImpl.mock.calls[0][1];
    expect(secondArg.body).toMatch(/\"type\":\"doc\"/);
  });
});
