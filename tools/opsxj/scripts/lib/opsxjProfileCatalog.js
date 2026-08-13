const freezeProfile = (profile) => Object.freeze({ ...profile });

export const ARCHITECTURE_PROFILE_CATALOG = Object.freeze({
  "enterprise-legacy-modernization": freezeProfile({
    name: "enterprise-legacy-modernization",
    version: 1,
    description:
      "Modernizacion gradual de capacidades legacy con fronteras tipadas y compatibilidad controlada.",
    artifactMarkers: Object.freeze({
      proposal: "## Politica de modernizacion enterprise legacy",
      design: "## Arquitectura de modernizacion enterprise legacy",
      spec: "### Requirement: Frontera de capacidad legacy",
      tasks: "## Gobierno de modernizacion enterprise legacy",
    }),
    requiredTaskMarkers: Object.freeze([
      "Gateway/Adapter tipado por capacidad",
      "pruebas de equivalencia",
      "piloto y rollback",
    ]),
  }),
});

export const TECHNOLOGY_PROFILE_CATALOG = Object.freeze({
  "legacy-webforms-vb": freezeProfile({
    name: "legacy-webforms-vb",
    version: 1,
    description: "ASP.NET Web Forms, code-behind VB.NET/C# y JavaScript gradual.",
  }),
  "tooling-node": freezeProfile({
    name: "tooling-node",
    version: 1,
    description: "Herramientas internas Node.js/ESM y sus pruebas automatizadas.",
  }),
  "frontend-react-ts": freezeProfile({
    name: "frontend-react-ts",
    version: 1,
    description: "Frontend React con TypeScript y convenciones de componentes/hook.",
  }),
  generic: freezeProfile({
    name: "generic",
    version: 1,
    description: "Revisión estructural sin imponer reglas de un framework concreto.",
  }),
});

const normalizeCatalogValue = ({ value, catalog, kind, allowEmpty = true }) => {
  const normalized = String(value ?? "").trim().toLowerCase();
  if (!normalized && allowEmpty) return null;
  if (!catalog[normalized]) {
    throw new Error(
      `${kind} no soportado: ${value}. Use: ${Object.keys(catalog).join(", ")}.`,
    );
  }
  return normalized;
};

export const normalizeArchitectureProfile = (value) =>
  normalizeCatalogValue({
    value,
    catalog: ARCHITECTURE_PROFILE_CATALOG,
    kind: "Perfil de arquitectura",
  });

export const normalizeTechnologyProfile = (value) =>
  normalizeCatalogValue({
    value,
    catalog: TECHNOLOGY_PROFILE_CATALOG,
    kind: "Perfil tecnologico",
  });

const hasAny = (text, patterns) => patterns.some((pattern) => pattern.test(text));

export const detectTechnologyProfile = ({ promptText = "" }) => {
  const text = String(promptText);

  if (
    hasAny(text, [
      /\b(?:ASP\.NET\s+)?Web\s*Forms?\b/i,
      /\b(?:VB\.NET|code-behind|UpdatePanel|ViewState|GridView|ModalPopupExtender)\b/i,
      /\.aspx(?:\.vb)?\b/i,
    ])
  ) {
    return "legacy-webforms-vb";
  }

  if (
    hasAny(text, [
      /\b(?:React|TSX|useState|useEffect|useMemo|useCallback|TypeScript)\b/i,
      /\bsrc[\\/]modules[\\/].*\b(?:components|hooks|adapters|types)[\\/]/i,
    ])
  ) {
    return "frontend-react-ts";
  }

  if (
    hasAny(text, [
      /\b(?:Node\.js|Node|ESM|Vitest|npm(?:\.cmd)?\s+run)\b/i,
      /\btools[\\/]opsxj[\\/]scripts[\\/]/i,
    ])
  ) {
    return "tooling-node";
  }

  return "generic";
};

export const resolveTechnologyProfile = ({ technologyProfile, promptText }) =>
  normalizeTechnologyProfile(technologyProfile) ?? detectTechnologyProfile({ promptText });

export const getArchitectureProfile = (profileName) =>
  profileName ? ARCHITECTURE_PROFILE_CATALOG[normalizeArchitectureProfile(profileName)] : null;

export const getTechnologyProfile = (profileName) =>
  TECHNOLOGY_PROFILE_CATALOG[normalizeTechnologyProfile(profileName) ?? "generic"];
