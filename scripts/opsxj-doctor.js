#!/usr/bin/env node

import path from "node:path";
import { fileURLToPath } from "node:url";
import { loadEnvFile } from "./lib/loadEnvFile.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(__dirname, "..");

// Allow running doctor from a plain shell by loading .env.jira (gitignored).
loadEnvFile({ baseDir: repoRoot, filename: ".env.jira" });

const required = {
  jira: ["JIRA_BASE_URL", "JIRA_EMAIL", "JIRA_API_TOKEN"],
  github: ["GITHUB_TOKEN"],
  githubRepoAny: ["GITHUB_REPO", ["GITHUB_OWNER", "GITHUB_REPO_NAME"]],
};

const read = (key) => String(process.env[key] ?? "").trim();

const checkGroup = (name, keys) => {
  const missing = keys.filter((key) => !read(key));
  return {
    name,
    ok: missing.length === 0,
    missing,
  };
};

const jira = checkGroup("jira", required.jira);
const github = checkGroup("github", required.github);
const hasRepoBySingle = Boolean(read(required.githubRepoAny[0]));
const hasRepoBySplit = required.githubRepoAny[1].every((key) => Boolean(read(key)));
const githubRepoOk = hasRepoBySingle || hasRepoBySplit;

if (jira.ok) {
  console.log("[opsxj:doctor] Jira: OK");
} else {
  console.log(
    `[opsxj:doctor] Jira: faltan ${jira.missing.join(", ")}`,
  );
}

if (github.ok) {
  console.log("[opsxj:doctor] GitHub token: OK");
} else {
  console.log(
    `[opsxj:doctor] GitHub token: falta ${github.missing.join(", ")}`,
  );
}

if (githubRepoOk) {
  console.log("[opsxj:doctor] GitHub repo: OK");
} else {
  console.log(
    "[opsxj:doctor] GitHub repo: defina GITHUB_REPO o GITHUB_OWNER + GITHUB_REPO_NAME",
  );
}

if (jira.ok && github.ok && githubRepoOk) {
console.log(
  "[opsxj:doctor] Configuracion lista para opsxj:new, opsxj:archive y opsxj:close.",
);
  process.exitCode = 0;
} else {
  process.exitCode = 1;
}
