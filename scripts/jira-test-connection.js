#!/usr/bin/env node

import path from "node:path";
import { fileURLToPath } from "node:url";
import { buildJiraAuthHeader } from "./lib/jiraClient.js";
import { loadEnvFile } from "./lib/loadEnvFile.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(__dirname, "..");

// Allow running from a plain shell by loading .env.jira (gitignored).
loadEnvFile({ baseDir: repoRoot, filename: ".env.jira" });

const requiredEnv = ["JIRA_BASE_URL", "JIRA_EMAIL", "JIRA_API_TOKEN"];
const missing = requiredEnv.filter(
  (key) => !String(process.env[key] ?? "").trim(),
);

if (missing.length > 0) {
  console.error(
    `[jira:test] Faltan variables requeridas: ${missing.join(", ")}`,
  );
  process.exitCode = 1;
  process.exit();
}

const baseUrl = String(process.env.JIRA_BASE_URL).trim().replace(/\/+$/, "");
const email = String(process.env.JIRA_EMAIL).trim();
const apiToken = String(process.env.JIRA_API_TOKEN).trim();

const url = `${baseUrl}/rest/api/3/myself`;

try {
  const response = await fetch(url, {
    headers: {
      Authorization: buildJiraAuthHeader(email, apiToken),
      Accept: "application/json",
    },
  });

  if (!response.ok) {
    const raw = await response.text().catch(() => "");
    const detail = raw ? ` Detalle: ${raw.slice(0, 500)}` : "";
    throw new Error(
      `No se pudo conectar a JIRA (${response.status} ${response.statusText}).${detail}`,
    );
  }

  const profile = await response.json();
  console.log("[jira:test] Conexion exitosa a JIRA");
  console.log(`[jira:test] Usuario: ${profile.displayName ?? "(sin displayName)"}`);
  console.log(`[jira:test] Account ID: ${profile.accountId ?? "(sin accountId)"}`);
} catch (error) {
  const message = error instanceof Error ? error.message : String(error);
  console.error(`[jira:test] ${message}`);
  process.exitCode = 1;
}
