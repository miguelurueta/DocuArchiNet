#!/usr/bin/env node
import { readFile } from "node:fs/promises";
import { transitionJiraIssue, addJiraComment } from "./lib/jiraClient.js";

const issuePattern = /([A-Z]+-\d+)/;

const main = async () => {
  const eventPath = process.env.GITHUB_EVENT_PATH;
  if (!eventPath) {
    throw new Error("Falta GITHUB_EVENT_PATH.");
  }

  const raw = await readFile(eventPath, "utf8");
  const event = JSON.parse(raw);
  const pullRequest = event.pull_request;
  if (!pullRequest) {
    throw new Error("Evento sin pull_request.");
  }

  const sourceText = [
    pullRequest.head?.ref ?? "",
    pullRequest.title ?? "",
    pullRequest.body ?? "",
  ].join(" ");
  const match = sourceText.match(issuePattern);
  if (!match) {
    console.log("No se encontro ISSUE_KEY en PR; se omite sincronizacion Jira.");
    return;
  }

  const issueKey = match[1];
  const merged = Boolean(pullRequest.merged);
  const target = merged ? "done" : "in_progress";
  const baseUrl = process.env.JIRA_BASE_URL;
  const email = process.env.JIRA_EMAIL;
  const apiToken = process.env.JIRA_API_TOKEN;

  await transitionJiraIssue({
    issueKey,
    baseUrl,
    email,
    apiToken,
    target,
  });

  const prUrl = pullRequest.html_url;
  const message = merged
    ? `PR mergeado: ${prUrl}. Jira movido a Done.`
    : `PR cerrado sin merge: ${prUrl}. Jira revertido a In Progress.`;

  await addJiraComment({
    issueKey,
    baseUrl,
    email,
    apiToken,
    message,
  });

  console.log(`Jira sincronizado para ${issueKey}: ${target}`);
};

main().catch((error) => {
  const message = error instanceof Error ? error.message : String(error);
  console.error(`[opsxj:sync-pr-status] ${message}`);
  process.exitCode = 1;
});

