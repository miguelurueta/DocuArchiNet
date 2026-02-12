import { readFile } from "node:fs/promises";
import path from "node:path";
import { load } from "js-yaml";

export type SpecScenario = {
  id: string;
  title?: string;
};

export type SpecParseResult = {
  specIds: Set<string>;
  scenarios: SpecScenario[];
  sources: Record<string, string[]>;
};

type ScenarioYaml = {
  id?: unknown;
  title?: unknown;
};

type OpenSpecYaml = {
  scenarios?: unknown;
};

const SPEC_ID_REGEX = /^[A-Z]+-\d+$/;

const isRecord = (value: unknown): value is Record<string, unknown> => {
  return typeof value === "object" && value !== null;
};

const toScenario = (value: unknown): SpecScenario | null => {
  if (!isRecord(value)) {
    return null;
  }

  const scenario = value as ScenarioYaml;
  if (typeof scenario.id !== "string") {
    return null;
  }

  const id = scenario.id.trim();
  if (!SPEC_ID_REGEX.test(id)) {
    return null;
  }

  const title = typeof scenario.title === "string" ? scenario.title : undefined;
  return { id, title };
};

const parseSpecFile = async (filePath: string): Promise<SpecScenario[]> => {
  const raw = await readFile(filePath, "utf-8");
  const parsed = load(raw) as unknown;

  if (!isRecord(parsed)) {
    return [];
  }

  const yaml = parsed as OpenSpecYaml;
  if (!Array.isArray(yaml.scenarios)) {
    return [];
  }

  return yaml.scenarios
    .map(toScenario)
    .filter((scenario): scenario is SpecScenario => scenario !== null);
};

export const parseOpenSpec = async (projectRoot: string): Promise<SpecParseResult> => {
  const behaviorPath = path.resolve(projectRoot, "openspec/dashboard.behavior.yaml");
  const contractPath = path.resolve(projectRoot, "openspec/dashboard.contract.yaml");

  const sources: Record<string, string[]> = {
    "openspec/dashboard.behavior.yaml": [],
    "openspec/dashboard.contract.yaml": [],
  };

  const behaviorScenarios = await parseSpecFile(behaviorPath);
  sources["openspec/dashboard.behavior.yaml"] = behaviorScenarios.map(({ id }) => id);

  let contractScenarios: SpecScenario[] = [];
  try {
    contractScenarios = await parseSpecFile(contractPath);
    sources["openspec/dashboard.contract.yaml"] = contractScenarios.map(({ id }) => id);
  } catch {
    contractScenarios = [];
  }

  const scenarios = [...behaviorScenarios, ...contractScenarios];
  const specIds = new Set(scenarios.map(({ id }) => id));

  return {
    specIds,
    scenarios,
    sources,
  };
};
