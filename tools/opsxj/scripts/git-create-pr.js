#!/usr/bin/env node
import { execFileSync } from "node:child_process";

const readEnv = (key) => String(process.env[key] ?? "").trim();

const getGit = (args) =>
  execFileSync("git", args, { encoding: "utf8" }).trim();

const githubToken = readEnv("GITHUB_TOKEN");
const githubRepo = readEnv("GITHUB_REPO"); // owner/repo
const baseBranch = readEnv("GITHUB_BASE_BRANCH") || "main";

if (!githubToken) {
  throw new Error(
    "Falta GITHUB_TOKEN en el entorno. Definelo para poder crear PR automaticamente.",
  );
}
if (!githubRepo || !githubRepo.includes("/")) {
  throw new Error(
    "Falta GITHUB_REPO (formato owner/repo) en el entorno. Definelo para poder crear PR automaticamente.",
  );
}

const [owner, repo] = githubRepo.split("/", 2);
const headBranch = getGit(["rev-parse", "--abbrev-ref", "HEAD"]);
if (!headBranch || headBranch === "HEAD") {
  throw new Error("No se puede crear PR con HEAD detached.");
}
if (headBranch === baseBranch) {
  throw new Error(
    `Estas en '${baseBranch}'. Cree una rama antes de crear un PR automatico.`,
  );
}

const title = getGit(["log", "-1", "--pretty=%s"]);
const body = `PR auto-creado por \`git:update\`.\n\n- Rama: \`${headBranch}\`\n- Base: \`${baseBranch}\`\n`;

const apiBase = "https://api.github.com";
const headers = {
  Authorization: `token ${githubToken}`,
  Accept: "application/vnd.github+json",
  "User-Agent": "DocuArchiCore.git-update",
};

const fetchJson = async (url, options = {}) => {
  const response = await fetch(url, { ...options, headers });
  const raw = await response.text().catch(() => "");
  const payload = raw ? JSON.parse(raw) : null;
  if (!response.ok) {
    const message = payload?.message ?? raw ?? `${response.status} ${response.statusText}`;
    throw new Error(`GitHub API error: ${message}`);
  }
  return payload;
};

// 1) Reuse open PR if present
const existing = await fetchJson(
  `${apiBase}/repos/${owner}/${repo}/pulls?state=open&head=${encodeURIComponent(`${owner}:${headBranch}`)}&base=${encodeURIComponent(baseBranch)}`,
);
if (Array.isArray(existing) && existing.length > 0) {
  process.stdout.write(`${existing[0].html_url}\n`);
  process.exit(0);
}

// 2) Create PR
const created = await fetchJson(`${apiBase}/repos/${owner}/${repo}/pulls`, {
  method: "POST",
  body: JSON.stringify({
    title,
    head: headBranch,
    base: baseBranch,
    body,
  }),
});

process.stdout.write(`${created.html_url}\n`);

