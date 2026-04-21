#!/usr/bin/env node
import path from "node:path";
import { fileURLToPath } from "node:url";
import { runOpsxjCommand } from "./lib/opsxjCommandRunner.js";
import { loadEnvFile } from "./lib/loadEnvFile.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(__dirname, "..");

// Allow running opsxj from a plain shell by loading .env.jira (gitignored).
loadEnvFile({ baseDir: repoRoot, filename: ".env.jira" });

const exitCode = await runOpsxjCommand({
  argv: process.argv.slice(2),
  baseDir: repoRoot,
});

process.exitCode = exitCode;
