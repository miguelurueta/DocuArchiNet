#!/usr/bin/env node
import { fetchJiraIssue } from "./lib/jiraClient.js";

const issueKey = process.argv[2];

const run = async () => {
  try {
    const result = await fetchJiraIssue({
      issueKey,
      baseUrl: process.env.JIRA_BASE_URL,
      email: process.env.JIRA_EMAIL,
      apiToken: process.env.JIRA_API_TOKEN,
    });

    process.stdout.write(`${JSON.stringify(result, null, 2)}\n`);
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    process.stderr.write(`${message}\n`);
    process.exitCode = 1;
  }
};

run();
