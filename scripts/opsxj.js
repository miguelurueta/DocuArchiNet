#!/usr/bin/env node
import path from "node:path";
import { fileURLToPath } from "node:url";
import { runOpsxjCommand } from "./lib/opsxjCommandRunner.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(__dirname, "..");

const exitCode = await runOpsxjCommand({
  argv: process.argv.slice(2),
  baseDir: repoRoot,
});

process.exitCode = exitCode;
