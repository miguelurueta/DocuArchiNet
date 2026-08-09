import fs from "node:fs";
import path from "node:path";

const stripQuotes = (value) => {
  const trimmed = String(value ?? "").trim();
  if (trimmed.length >= 2) {
    const first = trimmed[0];
    const last = trimmed[trimmed.length - 1];
    if ((first === `"` && last === `"`) || (first === `'` && last === `'`)) {
      return trimmed.slice(1, -1);
    }
  }
  return trimmed;
};

/**
 * Minimal .env loader (no external deps).
 * - Ignores empty lines and comments (# ...)
 * - Parses KEY=VALUE pairs
 * - Does not override existing process.env values unless override=true
 */
export const loadEnvFile = ({ baseDir, filename, override = false } = {}) => {
  const resolvedBaseDir = baseDir ?? process.cwd();
  const resolvedFilename = filename ?? ".env.jira";
  const filePath = path.resolve(resolvedBaseDir, resolvedFilename);

  if (!fs.existsSync(filePath)) {
    return { loaded: false, filePath };
  }

  const content = fs.readFileSync(filePath, "utf8");
  const lines = content.split(/\r?\n/g);
  for (const line of lines) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith("#")) continue;

    const eqIndex = trimmed.indexOf("=");
    if (eqIndex <= 0) continue;

    const key = trimmed.slice(0, eqIndex).trim();
    const rawValue = trimmed.slice(eqIndex + 1);
    const value = stripQuotes(rawValue);

    if (!key) continue;
    if (!override && process.env[key] !== undefined && String(process.env[key]).trim() !== "") {
      continue;
    }

    process.env[key] = value;
  }

  return { loaded: true, filePath };
};

