#!/usr/bin/env node
import path from "node:path";
import { fileURLToPath } from "node:url";
import { createProposalFromJira } from "./lib/jiraProposalService.js";

const issueKey = process.argv[2];
const __dirname = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(__dirname, "../../..");

const run = async () => {
  try {
    const { proposalPath } = await createProposalFromJira({
      issueKey,
      baseUrl: process.env.JIRA_BASE_URL,
      email: process.env.JIRA_EMAIL,
      apiToken: process.env.JIRA_API_TOKEN,
      commandName: "generate-proposal-from-jira.js",
      folderStrategy: "issueKey",
      baseDir: repoRoot,
    });

    process.stdout.write(
      `Propuesta generada en ${proposalPath}\n`
    );
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    process.stderr.write(`${message}\n`);
    process.exitCode = 1;
  }
};

run();
