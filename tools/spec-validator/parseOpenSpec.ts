import { readFile } from "node:fs/promises";
import path from "node:path";
import { load } from "js-yaml";
import { SPEC_MODULES } from "./config.ts";

export type SpecScenario = {
  id: string;
  title?: string;
  module: string;
  source: string;
};

export type SpecParseResult = {
  specIds: Set<string>;
  scenarios: SpecScenario[];
  byModule: Record<string, string[]>;
  sources: Record<string, string[]>;
};

type ScenarioYaml = {
  id?: unknown;
  title?: unknown;
};

const SPEC_ID_REGEX = /^[A-Z]+-\d+$/;

const isRecord = (value: unknown): value is Record<string, unknown> => {
  return typeof value === "object" && value !== null;
};

const toScenario = (
  value: unknown,
  module: string,
  source: string
): SpecScenario | null => {
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
  return { id, title, module, source };
};

const extractScenarios = (
  rawYaml: unknown,
  module: string,
  source: string
): SpecScenario[] => {
  if (!isRecord(rawYaml)) {
    return [];
  }

  const scenariosValue = rawYaml.scenarios;
  if (!Array.isArray(scenariosValue)) {
    return [];
  }

  return scenariosValue
    .map(scenario => toScenario(scenario, module, source))
    .filter((scenario): scenario is SpecScenario => scenario !== null);
};

const parseSpecFile = async (
  absolutePath: string,
  module: string,
  source: string
): Promise<SpecScenario[]> => {
  const raw = await readFile(absolutePath, "utf-8");
  const parsed = load(raw) as unknown;
  return extractScenarios(parsed, module, source);
};

export const parseOpenSpec = async (projectRoot: string): Promise<SpecParseResult> => {
  const sources: Record<string, string[]> = {};
  const byModule: Record<string, string[]> = {};
  const scenarios: SpecScenario[] = [];

  for (const moduleConfig of SPEC_MODULES) {
    const sourcePaths = [...moduleConfig.behaviorSpecs, ...moduleConfig.contractSpecs];

    for (const sourcePath of sourcePaths) {
      const absolutePath = path.resolve(projectRoot, sourcePath);
      const parsedScenarios = await parseSpecFile(
        absolutePath,
        moduleConfig.module,
        sourcePath
      );

      sources[sourcePath] = parsedScenarios.map(({ id }) => id);
      scenarios.push(...parsedScenarios);
    }
  }

  const specIds = new Set<string>();
  for (const scenario of scenarios) {
    specIds.add(scenario.id);
    if (!byModule[scenario.module]) {
      byModule[scenario.module] = [];
    }
    byModule[scenario.module].push(scenario.id);
  }

  for (const moduleName of Object.keys(byModule)) {
    byModule[moduleName] = Array.from(new Set(byModule[moduleName])).sort();
  }

  return {
    specIds,
    scenarios,
    byModule,
    sources,
  };
};
