import { access, mkdir, readFile, rename, rm, writeFile } from "node:fs/promises";
import path from "node:path";

export const RUN_CHECKLIST_VERSION = 1;
export const RUN_CHECKLIST_STAGES = Object.freeze(["new", "refine", "review", "validate", "archive", "close"]);
export const RUN_CHECKLIST_EVENT_STATUSES = Object.freeze(["pass", "fail"]);

const SHA_SENSITIVE_STAGES = new Set(["review", "validate"]);
const SENSITIVE_VALUE_PATTERN = /\b(?:api[_-]?key|access[_-]?token|refresh[_-]?token|token|password|passwd|secret|authorization|bearer)\b\s*(?:=|:|\s)/i;
const ISSUE_KEY_PATTERN = /^[A-Z][A-Z0-9]*-\d+$/;

const fileExists = async (targetPath) => access(targetPath).then(() => true).catch(() => false);

export const normalizeRunChecklistIssueKey = (value) => {
  const issueKey = String(value ?? "").trim().toUpperCase();
  if (!ISSUE_KEY_PATTERN.test(issueKey)) {
    throw new Error("Ticket OPSXJ invalido. Use el formato PROYECTO-123.");
  }
  return issueKey;
};

const normalizeStage = (value) => {
  const stage = String(value ?? "").trim().toLowerCase();
  if (!RUN_CHECKLIST_STAGES.includes(stage)) {
    throw new Error(`Etapa OPSXJ no soportada: ${value}. Use: ${RUN_CHECKLIST_STAGES.join(", ")}.`);
  }
  return stage;
};

const normalizeEventStatus = (value) => {
  const status = String(value ?? "").trim().toLowerCase();
  if (!RUN_CHECKLIST_EVENT_STATUSES.includes(status)) {
    throw new Error(`Resultado OPSXJ no soportado: ${value}. Use: ${RUN_CHECKLIST_EVENT_STATUSES.join(", ")}.`);
  }
  return status;
};

const normalizeSafeText = (value, fieldName, maximumLength) => {
  if (value === undefined || value === null || value === "") return undefined;
  const normalized = String(value).trim();
  if (!normalized) return undefined;
  if (normalized.length > maximumLength) {
    throw new Error(`${fieldName} supera el limite de ${maximumLength} caracteres.`);
  }
  if (SENSITIVE_VALUE_PATTERN.test(normalized)) {
    throw new Error(`${fieldName} no puede contener secretos ni credenciales.`);
  }
  return normalized;
};

const normalizeSha = (value) => {
  const sha = String(value ?? "").trim();
  if (!sha) throw new Error("Falta el SHA evaluado para registrar la etapa OPSXJ.");
  if (sha.length > 128) throw new Error("El SHA evaluado supera el limite permitido.");
  return sha;
};

const normalizeRecordedAtUtc = (value) => {
  const date = value ? new Date(value) : new Date();
  if (Number.isNaN(date.getTime())) throw new Error("La fecha del evento OPSXJ no es valida.");
  return date.toISOString();
};

const isValidEvent = (event) => {
  if (!event || typeof event !== "object") return false;
  if (!RUN_CHECKLIST_STAGES.includes(event.stage) || !RUN_CHECKLIST_EVENT_STATUSES.includes(event.status)) return false;
  if (typeof event.sha !== "string" || !event.sha.trim()) return false;
  if (typeof event.recordedAtUtc !== "string" || Number.isNaN(new Date(event.recordedAtUtc).getTime())) return false;
  return true;
};

const isValidRunChecklist = (run, expectedIssueKey) =>
  Boolean(run)
  && typeof run === "object"
  && run.version === RUN_CHECKLIST_VERSION
  && run.issueKey === expectedIssueKey
  && Array.isArray(run.events)
  && run.events.every(isValidEvent);

export const getRunChecklistPath = ({ baseDir, issueKey }) =>
  path.join(baseDir, ".opsxj", "runs", `${normalizeRunChecklistIssueKey(issueKey)}.json`);

export const readRunChecklist = async ({ baseDir, issueKey }) => {
  const normalizedIssueKey = normalizeRunChecklistIssueKey(issueKey);
  const filePath = getRunChecklistPath({ baseDir, issueKey: normalizedIssueKey });
  if (!(await fileExists(filePath))) {
    return { state: "absent", filePath, issueKey: normalizedIssueKey, run: null };
  }

  try {
    const run = JSON.parse(await readFile(filePath, "utf8"));
    if (!isValidRunChecklist(run, normalizedIssueKey)) {
      return {
        state: "invalid",
        filePath,
        issueKey: normalizedIssueKey,
        run: null,
        error: "El registro OPSXJ no cumple el esquema v1.",
      };
    }
    return { state: "present", filePath, issueKey: normalizedIssueKey, run };
  } catch (error) {
    return {
      state: "invalid",
      filePath,
      issueKey: normalizedIssueKey,
      run: null,
      error: `No se puede leer el registro OPSXJ: ${error.message}`,
    };
  }
};

export const appendRunChecklistEvent = async ({
  baseDir,
  issueKey,
  stage,
  status,
  sha,
  actor,
  source,
  reference,
  detail,
  recordedAtUtc,
}) => {
  const normalizedIssueKey = normalizeRunChecklistIssueKey(issueKey);
  const safeActor = normalizeSafeText(actor, "actor", 128);
  const safeSource = normalizeSafeText(source, "source", 128);
  const safeReference = normalizeSafeText(reference, "reference", 512);
  const safeDetail = normalizeSafeText(detail, "detail", 2000);
  const event = {
    stage: normalizeStage(stage),
    status: normalizeEventStatus(status),
    sha: normalizeSha(sha),
    recordedAtUtc: normalizeRecordedAtUtc(recordedAtUtc),
    ...(safeActor ? { actor: safeActor } : {}),
    ...(safeSource ? { source: safeSource } : {}),
    ...(safeReference ? { reference: safeReference } : {}),
    ...(safeDetail ? { detail: safeDetail } : {}),
  };
  const current = await readRunChecklist({ baseDir, issueKey: normalizedIssueKey });
  if (current.state === "invalid") {
    throw new Error(`No se puede registrar OPSXJ: ${current.error}`);
  }
  const next = current.run ?? { version: RUN_CHECKLIST_VERSION, issueKey: normalizedIssueKey, events: [] };
  next.events.push(event);

  await mkdir(path.dirname(current.filePath), { recursive: true });
  const temporaryPath = `${current.filePath}.${process.pid}.${Date.now()}.tmp`;
  try {
    await writeFile(temporaryPath, `${JSON.stringify(next, null, 2)}\n`, "utf8");
    await rename(temporaryPath, current.filePath);
  } catch (error) {
    await rm(temporaryPath, { force: true }).catch(() => undefined);
    throw error;
  }
  return { filePath: current.filePath, run: next, event };
};

export const resolveRunChecklistStage = ({ readResult, stage, currentSha = null, shaSensitive = undefined }) => {
  const normalizedStage = normalizeStage(stage);
  const requiresCurrentSha = shaSensitive ?? SHA_SENSITIVE_STAGES.has(normalizedStage);
  if (!readResult || readResult.state === "absent") return { state: "UNAVAILABLE", detail: "Sin registro OPSXJ local." };
  if (readResult.state === "invalid") return { state: "UNAVAILABLE", detail: readResult.error ?? "Registro OPSXJ invalido." };
  if (!readResult.run) return { state: "UNAVAILABLE", detail: "Sin registro OPSXJ local." };

  const events = readResult.run.events.filter((event) => event.stage === normalizedStage);
  if (events.length === 0) return { state: "PENDING", detail: "La etapa no tiene ejecuciones registradas." };

  if (!requiresCurrentSha) {
    const event = events.at(-1);
    return {
      state: event.status === "pass" ? "COMPLETE" : "BLOCKED",
      event,
      recordedAtUtc: event.recordedAtUtc,
      sha: event.sha,
      reference: event.reference,
      detail: event.detail,
    };
  }

  const normalizedCurrentSha = String(currentSha ?? "").trim();
  if (!normalizedCurrentSha) return { state: "UNAVAILABLE", detail: "No fue posible determinar el SHA actual." };
  const matchingEvents = events.filter((event) => event.sha === normalizedCurrentSha);
  if (matchingEvents.length > 0) {
    const event = matchingEvents.at(-1);
    return {
      state: event.status === "pass" ? "COMPLETE" : "BLOCKED",
      event,
      recordedAtUtc: event.recordedAtUtc,
      sha: event.sha,
      reference: event.reference,
      detail: event.detail,
    };
  }

  const latestPassingEvent = events.filter((event) => event.status === "pass").at(-1);
  if (latestPassingEvent) {
    return {
      state: "STALE",
      event: latestPassingEvent,
      recordedAtUtc: latestPassingEvent.recordedAtUtc,
      sha: latestPassingEvent.sha,
      reference: latestPassingEvent.reference,
      detail: `La evidencia corresponde a ${latestPassingEvent.sha}, no al SHA actual ${normalizedCurrentSha}.`,
    };
  }
  return { state: "PENDING", detail: "La etapa no tiene una ejecucion aplicable al SHA actual." };
};
