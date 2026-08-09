#!/usr/bin/env node
import path from "node:path";
import { fileURLToPath } from "node:url";
import { transitionJiraIssue, addJiraComment } from "./lib/jiraClient.js";
import { loadEnvFile } from "./lib/loadEnvFile.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(__dirname, "../../..");

// Allow running from a plain shell by loading .env.jira (gitignored).
loadEnvFile({ baseDir: repoRoot, filename: ".env.jira" });

const usage = () => {
  // eslint-disable-next-line no-console
  console.log("Uso: node scripts/jira-transition.js <ISSUE-KEY> <done|in_progress> [comment]");
};

const main = async () => {
  const [issueKey, target, ...commentParts] = process.argv.slice(2);
  if (!issueKey || !target) {
    usage();
    process.exitCode = 2;
    return;
  }

  const baseUrl = process.env.JIRA_BASE_URL;
  const email = process.env.JIRA_EMAIL;
  const apiToken = process.env.JIRA_API_TOKEN;
  const comment = commentParts.join(" ").trim();

  const transition = await transitionJiraIssue({
    issueKey,
    baseUrl,
    email,
    apiToken,
    target,
  });

  if (comment) {
    await addJiraComment({ issueKey, baseUrl, email, apiToken, message: comment });
  }

  // eslint-disable-next-line no-console
  console.log(
    JSON.stringify(
      {
        issueKey,
        target,
        transition: { id: transition.id, name: transition.name, to: transition.to?.name },
        commented: Boolean(comment),
      },
      null,
      2,
    ),
  );
};

main().catch((error) => {
  const message = error instanceof Error ? error.message : String(error);
  // eslint-disable-next-line no-console
  console.error(`[jira-transition] ${message}`);
  process.exitCode = 1;
});

