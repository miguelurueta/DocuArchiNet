import { mkdir, readFile, readdir, stat, writeFile } from "node:fs/promises";
import path from "node:path";

const SUPPORTED_EXTENSIONS = new Set([".md", ".txt"]);
const REPORT_RELATIVE_PATH = path.join(".opsxj", "reports", "prompt-review-report.json");

const SECTION_RULES = [
  {
    name: "ROL ESPERADO",
    pattern: /\brol\s+esperado\b/i,
    severity: "BLOCKER",
    code: "ENTERPRISE_SECTION_REQUIRED",
  },
  {
    name: "OBJETIVO",
    pattern: /\bobjetivo\b/i,
    severity: "BLOCKER",
    code: "ENTERPRISE_SECTION_REQUIRED",
  },
  {
    name: "RESTRICCIONES CRITICAS",
    pattern: /\brestricciones?\s+cr[ií]ticas?\b|\brestricciones?\s+criticas?\b/i,
    severity: "BLOCKER",
    code: "ENTERPRISE_SECTION_REQUIRED",
  },
  {
    name: "CRITERIOS DE ACEPTACION",
    pattern: /\bcriterios?\s+de\s+aceptaci[oó]n\b|\bcriterios?\s+de\s+aceptacion\b/i,
    severity: "BLOCKER",
    code: "ENTERPRISE_SECTION_REQUIRED",
  },
  {
    name: "CONTEXTO",
    pattern: /\bcontexto\b/i,
    severity: "MAJOR",
    code: "ENTERPRISE_SECTION_RECOMMENDED",
  },
  {
    name: "PRUEBAS OBLIGATORIAS",
    pattern: /\bpruebas?\s+obligatorias?\b/i,
    severity: "MAJOR",
    code: "ENTERPRISE_SECTION_RECOMMENDED",
  },
  {
    name: "DOCUMENTACION TECNICA",
    pattern: /\bdocumentaci[oó]n\s+t[eé]cnica\b|\bdocumentacion\s+tecnica\b/i,
    severity: "MAJOR",
    code: "ENTERPRISE_SECTION_RECOMMENDED",
  },
  {
    name: "ENTREGABLE FINAL",
    pattern: /\bentregable\s+final\b/i,
    severity: "MAJOR",
    code: "ENTERPRISE_SECTION_RECOMMENDED",
  },
];

const normalizeText = (value) =>
  String(value ?? "")
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .toLowerCase();

export const newPromptReviewFinding = ({
  severity,
  code,
  message,
  expected,
  evidence = null,
}) => ({
  severity,
  code,
  message,
  expected,
  evidence,
});

const hasAny = (text, patterns) => patterns.some((pattern) => pattern.test(text));

const hasDocumentationSection = (normalized) =>
  hasAny(normalized, [
    /\bdocumentacion\s+tecnica\b/,
    /\bdocumentacion\s+enterprise\b/,
    /\bdocumentar\b/,
    /\bdocs[\\/]/,
  ]);

const hasDocsPath = (text) =>
  hasAny(text, [
    /docs[\\/]+modulos[\\/]+[^\\/\s`]+[\\/]+[^\\/\s`]+(?:[\\/]+[^\\/\s`]+)*[\\/]+SCRUMCORE-\d+-[^\\/\s`]+[\\/]?/i,
    /docs[\\/]+Architecture[\\/]+[^\\/\s`]+(?:[\\/]+[^\\/\s`]+)*[\\/]+SCRUMCORE-\d+-[^\\/\s`]+[\\/]?/i,
    /docs[\\/]+Components[\\/]+[^\\/\s`]+(?:[\\/]+[^\\/\s`]+)*[\\/]+SCRUMCORE-\d+-[^\\/\s`]+[\\/]?/i,
    /docs[\\/]+GestorDocumental[\\/]+[^\\/\s`]+(?:[\\/]+[^\\/\s`]+)*[\\/]+SCRUMCORE-\d+-[^\\/\s`]+[\\/]?/i,
  ]);

const ENTERPRISE_DOCUMENTATION_PACKAGE = [
  "00-Indice.md",
  "01-Arquitectura.md",
  "02-FlujoIntegracion.md",
  "03-ContratoUploadYMapping.md",
  "04-EstadosErroresYAntiregresion.md",
  "05-PruebasEvidencia.md",
  "06-Diagramas.md",
  "07-Metadata.md",
];

const getMissingEnterpriseDocumentationArtifacts = (text) =>
  ENTERPRISE_DOCUMENTATION_PACKAGE.filter(
    (artifactName) => !text.includes(artifactName),
  );

const ENTERPRISE_DOCUMENTATION_DETAIL_RULES = [
  {
    artifact: "00-Indice.md",
    code: "DOCUMENTATION_INDEX_DETAIL_REQUIRED",
    expected:
      "objetivo, alcance, componentes, hooks/adapters/servicios, modulos, dependencias y listado documental.",
    patterns: [
      /\bobjetivo\b/,
      /\balcance\b/,
      /\bcomponentes?\b/,
      /\b(hooks?|adapters?|servicios?)\b/,
      /\bmodulos?\b/,
      /\bdependencias?\b/,
      /\blistado\b|\blista\b/,
    ],
  },
  {
    artifact: "01-Arquitectura.md",
    code: "DOCUMENTATION_ARCHITECTURE_DETAIL_REQUIRED",
    expected:
      "decisiones arquitectonicas, reutilizacion, responsabilidades, desacople y alternativas descartadas.",
    patterns: [
      /\bdecisiones?\s+arquitectonicas?\b/,
      /\breutiliz/,
      /\bresponsabilidad(?:es)?\b/,
      /\bdesacopl/,
      /\balternativas?\b.*\bdescartad/,
      /\b(componentes?\s+de\s+presentacion|presentacion)\b/,
      /\bcontenedores?\b/,
      /\b(servicios?|adapters?|mappers?|hooks?|infraestructura)\b/,
    ],
  },
  {
    artifact: "02-FlujoIntegracion.md",
    code: "DOCUMENTATION_FLOW_DETAIL_REQUIRED",
    expected:
      "usuario, renderizado, carga de datos, requests, backend, responses, estado, UI y batch si aplica.",
    patterns: [
      /\busuario\b/,
      /\brenderiz/,
      /\bcarga\s+de\s+datos\b|\bcargar\s+datos\b/,
      /\brequests?\b/,
      /\bbackend\b/,
      /\bresponses?\b|\brespuestas?\b/,
      /\bestado\b/,
      /\binterfaz\b|\bui\b/,
      /\bbatch\b|\blote\b|\bsi\s+aplica\b/,
    ],
  },
  {
    artifact: "03-ContratoUploadYMapping.md",
    code: "DOCUMENTATION_CONTRACT_MAPPING_DETAIL_REQUIRED",
    expected:
      "props, contexto, DTOs, request/response, modelos, transformacion, deduplicacion, metadata y frontera frontend/backend.",
    patterns: [
      /\bprops?\b/,
      /\bcontexto\b/,
      /\bdtos?\b/,
      /\brequests?\b/,
      /\bresponses?\b|\brespuestas?\b/,
      /\bmodelos?\b/,
      /\btransformacion\b|\bmapping\b|\bmapeo\b/,
      /\bdeduplic/,
      /\bmetadata\b/,
      /\bfrontend\b.*\bbackend\b|\bbackend\b.*\bfrontend\b/,
    ],
  },
  {
    artifact: "04-EstadosErroresYAntiregresion.md",
    code: "DOCUMENTATION_STATES_ERRORS_REGRESSION_DETAIL_REQUIRED",
    expected:
      "estado inicial, carga, exito, error, datos incompletos, parciales, respuestas invalidas y reglas de antirregresion.",
    patterns: [
      /\bestado\s+inicial\b/,
      /\bcarga\b|\bloading\b/,
      /\bexito\b/,
      /\berror(?:es)?\b/,
      /\bdatos\s+incompletos\b/,
      /\bestados?\s+parciales?\b/,
      /\brespuestas?\s+invalidas?\b/,
      /\bantirregresion\b|\banti-regresion\b/,
      /\b(remount|refresh|recargas?\s+silenciosas?|duplicacion|logica\s+heredada|soluciones?\s+temporales?)\b/,
    ],
  },
  {
    artifact: "05-PruebasEvidencia.md",
    code: "DOCUMENTATION_TEST_EVIDENCE_DETAIL_REQUIRED",
    expected:
      "unitarias, integracion, manuales, comandos, resultados, limitaciones, riesgos y evidencia.",
    patterns: [
      /\bunitarias?\b|\bunit\b/,
      /\bintegracion\b/,
      /\bmanuales?\b/,
      /\bcomandos?\b/,
      /\bresultados?\b/,
      /\blimitaciones?\b/,
      /\briesgos?\b/,
      /\bevidencia\b/,
    ],
  },
  {
    artifact: "06-Diagramas.md",
    code: "DOCUMENTATION_DIAGRAMS_DETAIL_REQUIRED",
    expected:
      "componentes, secuencia, flujo principal, flujo alterno, casos de uso, estados y formato Mermaid/estructurado.",
    patterns: [
      /\bcomponentes?\b/,
      /\bsecuencia\b/,
      /\bflujo\s+principal\b/,
      /\bflujo\s+alterno\b/,
      /\bcasos?\s+de\s+uso\b/,
      /\bestados?\b/,
      /\bmermaid\b|\bformato\s+estructurado\b|\bestructurado\s+legible\b/,
    ],
  },
  {
    artifact: "07-Metadata.md",
    code: "DOCUMENTATION_METADATA_DETAIL_REQUIRED",
    expected:
      "SCRUMCORE, branch, fecha, estado, archivos modificados, prompts, dependencias, riesgos y deuda tecnica.",
    patterns: [
      /\bscrumcore\b/,
      /\bbranch\b|\brama\b/,
      /\bfecha\b/,
      /\bestado\b/,
      /\barchivos?\s+modificados?\b|\bpaths?\s+tocados?\b/,
      /\bprompts?\b/,
      /\bdependencias?\b/,
      /\briesgos?\b/,
      /\bdeuda\s+tecnica\b/,
    ],
  },
];

const getMissingEnterpriseDocumentationDetails = (normalized) =>
  ENTERPRISE_DOCUMENTATION_DETAIL_RULES
    .map((rule) => ({
      ...rule,
      missing: rule.patterns.filter((pattern) => !pattern.test(normalized)).length,
    }))
    .filter((rule) => rule.missing > 0);

const hasDiagramFolderRequirement = (text) =>
  hasAny(text, [
    /(?:docs[\\/][^ \n\r`]+[\\/])?Diagramas[\\/]/i,
    /(?:docs[\\/][^ \n\r`]+[\\/])?diagrams[\\/]/i,
    /\bcarpeta\s+de\s+diagramas\b/i,
  ]);

const hasFunctionTableRequirement = (normalized) =>
  hasAny(normalized, [
    /\btabla\b[\s\S]*\bfunciones?\b[\s\S]*\bruta\b[\s\S]*\bubicacion\b[\s\S]*\bparametros?\b/,
    /\bfunciones?\b[\s\S]*\btabla\b[\s\S]*\bruta\b[\s\S]*\bubicacion\b[\s\S]*\bparametros?\b/,
    /\|\s*funcion\s*\|[\s\S]*\|\s*ruta\s*\|[\s\S]*\|\s*ubicacion\s*\|[\s\S]*\|\s*parametros?\s*\|/,
  ]);

const hasCodeLocationGuidance = (text, normalized) => {
  const hasReusableRoute = hasAny(text, [
    /src[\\/]+app[\\/]+Components[\\/]+<[^\\/\s`]+>/i,
    /src[\\/]+app[\\/]+Components[\\/]+[A-Z][A-Za-z0-9]+/i,
    /src[\\/]+shared[\\/]+/i,
    /src[\\/]+components[\\/]+/i,
  ]);
  const hasModuleRoute = hasAny(text, [
    /src[\\/]+modules[\\/]+<modulo>[\\/]+/i,
    /src[\\/]+modules[\\/]+[a-zA-Z0-9_-]+[\\/]+(components|hooks|services|adapters|types)[\\/]+/i,
  ]);
  const explainsContextRule = hasAny(normalized, [
    /\bapp\s+reusable\b/,
    /\bcomponente\s+compartido\b/,
    /\bmodulo\s+funcional\b/,
    /\bpatron\s+existente\b/,
    /\bestructura\s+existente\b/,
  ]);

  return (hasReusableRoute || hasModuleRoute) && explainsContextRule;
};

const requiresE2EEvidence = (normalized) =>
  hasAny(normalized, [
    /\bflujo\s+completo\b/,
    /\bend\s+to\s+end\b/,
    /\be2e\b/,
    /\bnavegacion\b|\brouting\b|\bruta\b.*\bnueva\b/,
    /\bintegracion\b.*\b(componentes?|paneles?|vistas?|modulos?)\b/,
    /\b(componentes?|paneles?|vistas?|modulos?)\b.*\bintegracion\b/,
    /\bpreservar\b.*\bestado\b|\bestado\b.*\bpreservar\b/,
    /\babrir\b.*\bcerrar\b|\bcerrar\b.*\babrir\b/,
    /\b(transaccional|guardar|eliminar|upload|batch|lote|adjuntar|almacenar)\b/,
    /\bregresion\s+critica\b|\bantirregresion\b|\banti-regresion\b/,
  ]);

const hasE2EEvidenceRequirement = (normalized) =>
  hasAny(normalized, [
    /\be2e\b/,
    /\bend\s+to\s+end\b/,
    /\bplaywright\b/,
    /\bpruebas?\s+e2e\b/,
    /\bvalidacion\s+manual\b.*\bevidencia\b/,
    /\bmanual\b.*\bresponsive\b.*\bevidencia\b/,
    /\bjustificacion\b.*\b(no\s+aplica|infraestructura|no\s+existe|no\s+disponible)\b/,
    /\b(no\s+aplica|infraestructura|no\s+existe|no\s+disponible)\b.*\bjustificacion\b/,
  ]);

const getMissingFrontendQualityRules = (normalized) => {
  const rules = [
    {
      code: "CLEAN_ARCHITECTURE_REQUIRED",
      message: "El prompt pide trabajo frontend pero no exige Clean Architecture.",
      expected:
        "Declarar separacion por capas/responsabilidades, dependencias hacia adentro y fronteras entre UI, hooks, services, adapters y types.",
      patterns: [
        /\bclean\s+architecture\b/,
        /\barquitectura\s+limpia\b/,
        /\bseparacion\s+de\s+responsabilidades\b/,
        /\bcapas\b.*\b(ui|hooks?|services?|adapters?|types?)\b/,
      ],
    },
    {
      code: "SOLID_REQUIRED",
      message: "El prompt pide trabajo frontend pero no exige SOLID.",
      expected:
        "Declarar principios SOLID o responsabilidades unicas, inversion de dependencias y extensibilidad sin modificar contratos existentes.",
      patterns: [
        /\bsolid\b/,
        /\bresponsabilidad\s+unica\b/,
        /\binversion\s+de\s+dependencias\b/,
        /\babierto\s*\/?\s*cerrado\b/,
      ],
    },
    {
      code: "STRICT_TYPESCRIPT_REQUIRED",
      message: "El prompt pide trabajo frontend pero no exige TypeScript estricto.",
      expected:
        "Exigir TypeScript estricto, contratos tipados, no any, sin casts amplios y modelos/props/eventos tipados.",
      patterns: [
        /\btypescript\s+estricto\b/,
        /\bstrict\s+typescript\b/,
        /\bno\s+any\b/,
        /\bsin\s+any\b/,
        /\bcontratos?\s+tipados?\b/,
      ],
    },
    {
      code: "REACT_PROJECT_CONVENTIONS_REQUIRED",
      message:
        "El prompt pide trabajo frontend pero no exige convenciones React del proyecto.",
      expected:
        "Exigir seguir patrones existentes del repo para componentes, hooks, services, adapters, tests, imports y estilos.",
      patterns: [
        /\bconvenciones?\s+react\b/,
        /\bpatrones?\s+(existentes?|del\s+repo|del\s+proyecto)\b/,
        /\bestructura\s+existente\b/,
        /\bcomponentes?,\s*hooks?,\s*services?/,
      ],
    },
    {
      code: "REACT_STATE_OWNERSHIP_REQUIRED",
      message:
        "El prompt pide trabajo frontend pero no exige fuente unica de estado ni evitar estado derivado duplicado.",
      expected:
        "Exigir ownership claro del estado, fuente unica de verdad y no duplicar estado derivado salvo justificacion.",
      patterns: [
        /\bfuente\s+unica\b/,
        /\bsingle\s+source\s+of\s+truth\b/,
        /\bownership\s+del\s+estado\b/,
        /\bno\s+duplicar\s+estado\b/,
        /\bestado\s+derivado\b/,
      ],
    },
    {
      code: "REACT_LIST_KEYS_REQUIRED",
      message:
        "El prompt pide trabajo frontend pero no prohibe indices como key en listas dinamicas.",
      expected:
        "Exigir keys estables de dominio en listas dinamicas y prohibir indices como key salvo listas realmente estaticas.",
      patterns: [
        /\bkeys?\s+estables?\b/,
        /\bkey\s+estable\b/,
        /\bno\s+usar\s+indices?\s+como\s+key\b/,
        /\bindices?\s+como\s+key\b/,
        /\bindex\s+as\s+key\b/,
      ],
    },
    {
      code: "RENDER_PERFORMANCE_REQUIRED",
      message:
        "El prompt pide trabajo frontend pero no exige controlar re-renders innecesarios en componentes pesados.",
      expected:
        "Exigir revisar re-renders, callbacks/objetos inline, memoizacion justificada y estabilidad de props en tablas, visores, grids o workbenches.",
      patterns: [
        /\bre-?renders?\b/,
        /\brerenders?\b/,
        /\busememo\b/,
        /\busecallback\b/,
        /\bprops?\s+estables?\b/,
        /\bobjetos?\s+inline\b/,
      ],
    },
    {
      code: "VALIDATION_RULES_REQUIRED",
      message:
        "El prompt pide trabajo frontend pero no exige reglas de validacion de entradas/estado.",
      expected:
        "Exigir validaciones de formulario/props/contexto/datos requeridos, errores controlados y estados invalidos.",
      patterns: [
        /\breglas?\s+de\s+validacion\b/,
        /\bvalidacion(?:es)?\s+de\s+(formulario|props?|contexto|datos|estado)\b/,
        /\bdato(?:s)?\s+requerido(?:s)?\b/,
        /\bestados?\s+invalidos?\b/,
      ],
    },
    {
      code: "ACCESSIBILITY_REQUIRED",
      message: "El prompt pide trabajo frontend pero no exige accesibilidad.",
      expected:
        "Exigir accesibilidad: keyboard navigation, focus management, labels/aria, contraste y estados perceptibles.",
      patterns: [
        /\baccesibilidad\b/,
        /\ba11y\b/,
        /\baria\b/,
        /\bkeyboard\b|\bteclado\b/,
        /\bfocus\b|\bfoco\b/,
      ],
    },
    {
      code: "TESTING_RULES_REQUIRED",
      message:
        "El prompt pide trabajo frontend pero no define reglas de testing completas.",
      expected:
        "Exigir piramide de pruebas segun impacto: unit/focal, integration, E2E o justificacion, build y evidencia.",
      patterns: [
        /\breglas?\s+de\s+testing\b/,
        /\bpiramide\s+de\s+pruebas\b/,
        /\bunitarias?\b.*\bintegracion\b/,
        /\btesting\s+library\b|\bvitest\b|\bplaywright\b/,
      ],
    },
    {
      code: "DEPENDENCY_GOVERNANCE_REQUIRED",
      message:
        "El prompt pide trabajo frontend pero no exige justificar dependencias nuevas.",
      expected:
        "Exigir no agregar librerias nuevas si el repo ya cubre la necesidad; cualquier dependencia nueva debe tener justificacion, alternativa evaluada e impacto.",
      patterns: [
        /\bdependencias?\s+nuevas?\b/,
        /\blibrerias?\s+nuevas?\b/,
        /\bno\s+agregar\s+(?:dependencias?|librerias?)\b/,
        /\bjustificar\s+(?:dependencias?|librerias?)\b/,
      ],
    },
    {
      code: "SECURITY_LOGGING_REQUIRED",
      message:
        "El prompt pide trabajo frontend pero no exige evitar logs sensibles.",
      expected:
        "Exigir no loguear tokens, credenciales, payloads sensibles, documentos o datos personales; usar logging controlado si aplica.",
      patterns: [
        /\blogs?\s+sensibles?\b/,
        /\bno\s+loguear\b/,
        /\bconsole\.log\b.*\b(prohib|no)\b/,
        /\btokens?\b.*\blogs?\b/,
        /\bpayloads?\s+sensibles?\b/,
        /\bdatos?\s+personales\b/,
      ],
    },
    {
      code: "MERMAID_DIAGRAMS_REQUIRED",
      message:
        "El prompt pide documentacion tecnica pero no exige diagramas Mermaid obligatorios.",
      expected:
        "Exigir diagramas Mermaid para componentes, secuencia, estados y casos de uso cuando aplique.",
      patterns: [
        /\bmermaid\b/,
        /```\s*mermaid\b/,
        /\bdiagramas?\s+mermaid\b/,
      ],
    },
  ];

  return rules.filter((rule) => !hasAny(normalized, rule.patterns));
};

const findFiles = async ({ dir, extensions, ignoreDirs = new Set() }) => {
  const entries = await readdir(dir, { withFileTypes: true });
  const results = [];
  for (const entry of entries) {
    if (entry.name.startsWith(".") || ignoreDirs.has(entry.name)) continue;
    const fullPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      results.push(...(await findFiles({ dir: fullPath, extensions, ignoreDirs })));
      continue;
    }
    if (entry.isFile() && extensions.has(path.extname(entry.name).toLowerCase())) {
      results.push(fullPath);
    }
  }
  return results;
};

export const resolvePromptReviewInput = async ({ baseDir, promptInput }) => {
  const trimmed = String(promptInput ?? "").trim();
  if (!trimmed) {
    throw new Error("Prompt path or SCRUM key is required.");
  }

  if (/^[A-Za-z]+-\d+$/.test(trimmed)) {
    const docsRoot = path.join(baseDir, "docs");
    const files = await findFiles({
      dir: docsRoot,
      extensions: SUPPORTED_EXTENSIONS,
      ignoreDirs: new Set(["node_modules", "dist", "playwright-report", "test-results"]),
    });
    const matches = [];
    for (const filePath of files) {
      const fileName = path.basename(filePath);
      if (fileName.includes(trimmed)) {
        matches.push(filePath);
        continue;
      }
      const content = await readFile(filePath, "utf8");
      if (content.includes(trimmed)) {
        matches.push(filePath);
      }
    }

    if (matches.length === 0) {
      throw new Error(`No prompt found for '${trimmed}'. Provide a prompt file path.`);
    }
    if (matches.length > 1) {
      throw new Error(
        `Multiple prompts found for '${trimmed}'. Provide an explicit path. Candidates: ${matches.join("; ")}`,
      );
    }
    return path.resolve(matches[0]);
  }

  const candidate = path.isAbsolute(trimmed) ? trimmed : path.join(baseDir, trimmed);
  const resolved = path.resolve(candidate);
  const info = await stat(resolved).catch(() => null);
  if (!info) {
    throw new Error(`Prompt file not found: ${trimmed}`);
  }
  if (info.isDirectory()) {
    throw new Error(`Prompt path points to a directory: ${trimmed}`);
  }
  const extension = path.extname(resolved).toLowerCase();
  if (!SUPPORTED_EXTENSIONS.has(extension)) {
    throw new Error(`Unsupported prompt extension '${extension}'. Supported extensions: .md, .txt.`);
  }
  return resolved;
};

export const readPromptReviewText = async ({ promptPath }) => {
  const text = await readFile(promptPath, "utf8");
  if (!text.trim()) {
    throw new Error(`Prompt file is empty: ${promptPath}`);
  }
  return text;
};

const addSectionFindings = ({ findings, text }) => {
  for (const rule of SECTION_RULES) {
    if (rule.pattern.test(text)) continue;
    findings.push(
      newPromptReviewFinding({
        severity: rule.severity,
        code: rule.code,
        message:
          rule.severity === "BLOCKER"
            ? `Falta seccion obligatoria: ${rule.name}.`
            : `Falta seccion recomendada: ${rule.name}.`,
        expected: rule.name,
      }),
    );
  }
};

const addStructuralFindings = ({ findings, text }) => {
  const normalized = normalizeText(text);
  const isPromptReviewToolingPrompt = hasAny(normalized, [
    /opsxj:prompt-review/,
    /frontendpromptreviewservice/,
    /validar\s+prompts?\s+enterprise/,
  ]);

  const mentionsImplementation = hasAny(normalized, [
    /\b(implementar|crear|modificar|ajustar|refactor|corregir|actualizar)\b/,
  ]);

  if (
    mentionsImplementation &&
    !hasAny(normalized, [
      /\b(no\s+debe|prohibid|restriccion|restricciones|fuera\s+de\s+alcance)\b/,
    ])
  ) {
    findings.push(
      newPromptReviewFinding({
        severity: "BLOCKER",
        code: "NEGATIVE_CONSTRAINTS_REQUIRED",
        message: "El prompt pide implementacion pero no define restricciones negativas o fuera de alcance.",
        expected: "Bloque de NO debe, prohibiciones, restricciones criticas o fuera de alcance.",
      }),
    );
  }

  if (
    mentionsImplementation &&
    !hasAny(normalized, [
      /\b(si\s+debe|debe\s+incluir|comportamiento\s+esperado|entregable\s+final|criterios?\s+de\s+aceptacion)\b/,
    ])
  ) {
    findings.push(
      newPromptReviewFinding({
        severity: "MAJOR",
        code: "POSITIVE_REQUIREMENTS_REQUIRED",
        message: "El prompt pide implementacion pero no define requisitos positivos verificables.",
        expected: "Bloque SI debe, comportamiento esperado, entregable final o criterios de aceptacion.",
      }),
    );
  }

  if (hasDocumentationSection(normalized)) {
    if (!hasDocsPath(text)) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: "DOCUMENTATION_PATH_REQUIRED",
          message:
            "El prompt exige documentacion pero no define una ruta documental canonica segun el contexto del repo.",
          expected:
            "Modulo: docs/modulos/<modulo>/<feature>/SCRUMCORE-000-resumen/. Reusable/core: docs/Architecture/<area>/<feature>/SCRUMCORE-000-resumen/ o docs/Components/<componente>/SCRUMCORE-000-resumen/.",
        }),
      );
    }

    const missingArtifacts = getMissingEnterpriseDocumentationArtifacts(text);
    if (missingArtifacts.length > 0) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: "DOCUMENTATION_PACKAGE_REQUIRED",
          message:
            "El prompt exige documentacion pero no lista el paquete documental enterprise minimo.",
          expected: ENTERPRISE_DOCUMENTATION_PACKAGE.join(", "),
          evidence: `Faltan: ${missingArtifacts.join(", ")}`,
        }),
      );
    }

    const missingDetails = getMissingEnterpriseDocumentationDetails(normalized);
    for (const detailRule of missingDetails) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: detailRule.code,
          message: `El prompt no detalla el contenido obligatorio de ${detailRule.artifact}.`,
          expected: detailRule.expected,
          evidence: `${detailRule.missing} criterio(s) estructural(es) faltante(s).`,
        }),
      );
    }

    if (!hasDiagramFolderRequirement(text)) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: "DOCUMENTATION_DIAGRAM_FOLDER_REQUIRED",
          message:
            "El prompt exige documentacion pero no define carpeta para diagramas individuales.",
          expected:
            "Carpeta Diagramas/ con archivos individuales para los diagramas requeridos.",
        }),
      );
    }

    if (!hasFunctionTableRequirement(normalized)) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: "DOCUMENTATION_FUNCTION_TABLE_REQUIRED",
          message:
            "El prompt exige documentacion pero no pide tabla de funciones creadas/modificadas.",
          expected:
            "Tabla de funciones con columnas Funcion, Ruta, Ubicacion, Parametros y responsabilidad/retorno segun aplique.",
        }),
      );
    }
  }

  if (text.match(/(?<!blocker si )\b(?:permitir|permite|sugiere)\s+(?:documentos?|archivos?)\s+en\s+ra[ií]z\b/i)) {
    findings.push(
      newPromptReviewFinding({
        severity: "BLOCKER",
        code: "ROOT_DOCS_FORBIDDEN",
        message: "El prompt permite o sugiere documentos en raiz.",
        expected: "Ubicar documentacion bajo docs/modulos/..., docs/Architecture/..., docs/Components/... o ruta tecnica equivalente existente.",
        evidence: text.match(/(?<!blocker si )\b(?:permitir|permite|sugiere)\s+(?:documentos?|archivos?)\s+en\s+ra[ií]z\b/i)?.[0],
      }),
    );
  }

  const mentionsCodeWork = hasAny(normalized, [
    /\b(react|tsx|ui|hook|service|servicio|adapter|mapper|componente|modal|formulario|tooling|script)\b/,
    /\b(crear|modificar|implementar|ajustar)\b.*\b(componente|hook|service|servicio|adapter|mapper|ui|script)\b/,
  ]);
  if (
    mentionsCodeWork &&
    !isPromptReviewToolingPrompt &&
    !hasCodeLocationGuidance(text, normalized)
  ) {
    findings.push(
      newPromptReviewFinding({
        severity: "BLOCKER",
        code: "CODE_LOCATION_CONTEXT_REQUIRED",
        message:
          "El prompt pide trabajo de codigo pero no define regla de ubicacion segun contexto del repo.",
        expected:
          "Distinguir app reusable/componente compartido y modulo funcional con rutas src/app/Components/... o src/modules/<modulo>/{components,hooks,services,adapters,types}/...",
      }),
    );
  }

  if (mentionsCodeWork && !isPromptReviewToolingPrompt) {
    for (const rule of getMissingFrontendQualityRules(normalized)) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: rule.code,
          message: rule.message,
          expected: rule.expected,
        }),
      );
    }
  }

  const mentionsContractSurface = hasAny(normalized, [
    /\b(props?|callbacks?|eventos?|interfaces?|types?|tipos?|request|response|contrato|payload)\b/,
  ]);
  if (
    mentionsContractSurface &&
    !hasAny(normalized, [
      /\bcontrato\b/,
      /\bshape\b/,
      /\bprops?\s+usad/,
      /\brequest\/response\b/,
      /\btipos?\s+typescript\b/,
      /\btype\s+[a-z0-9_]+\b/,
      /\binterface\s+[a-z0-9_]+\b/,
    ])
  ) {
    findings.push(
      newPromptReviewFinding({
        severity: "MAJOR",
        code: "CONTRACT_DETAIL_REQUIRED",
        message: "El prompt menciona superficie contractual pero no exige detalle de contratos.",
        expected: "Props, callbacks, eventos, request/response, payloads o tipos documentados.",
      }),
    );
  }

  const mentionsFlow = hasAny(normalized, [
    /\b(flujo|secuencia|paso\s+a\s+paso|proceso|workflow|batch|callback|abrir|cerrar)\b/,
  ]);
  if (
    mentionsFlow &&
    !hasAny(normalized, [
      /\bflujo\b/,
      /\bsecuencia\b/,
      /\bpaso\s+a\s+paso\b/,
      /\bcomportamiento\s+esperado\b/,
      /\bdiagrama\s+de\s+secuencia\b/,
    ])
  ) {
    findings.push(
      newPromptReviewFinding({
        severity: "MAJOR",
        code: "FLOW_DETAIL_REQUIRED",
        message: "El prompt describe comportamiento de flujo pero no exige secuencia funcional.",
        expected: "Flujo paso a paso, secuencia o comportamiento esperado.",
      }),
    );
  }

  const likelyStatefulWork = hasAny(normalized, [
    /\b(api|upload|formulario|modal|hook|query|mutation|estado|loading|error|batch|callback)\b/,
  ]);
  if (
    likelyStatefulWork &&
    !hasAny(normalized, [
      /\b(error|errores|loading|estado|estados|fallo|fallar|vacio|vacio|exitoso|controlad[oa])\b/,
    ])
  ) {
    findings.push(
      newPromptReviewFinding({
        severity: "MAJOR",
        code: "STATE_ERROR_DETAIL_REQUIRED",
        message: "El prompt toca trabajo con estado pero no exige estados y errores controlados.",
        expected: "Estados loading/error/exito/vacio o manejo controlado de errores segun aplique.",
      }),
    );
  }

  const isFixOrUpdate = hasAny(normalized, [
    /\b(fix|correccion|correcion|refactor)\b/,
  ]);
  if (
    isFixOrUpdate &&
    !hasAny(normalized, [
      /\b(ubicar|actualizar|conservar|mantener)\s+documentacion\b/,
      /\bdocumentacion\s+existente\b/,
      /\bsolicitar\s+ruta\b/,
    ])
  ) {
    findings.push(
      newPromptReviewFinding({
        severity: "MAJOR",
        code: "EXISTING_DOC_UPDATE_REQUIRED",
        message: "Actualizaciones/fixes deben exigir ubicar y actualizar documentacion existente o registrar la ruta faltante.",
        expected: "Ubicar documentacion existente, actualizarla o documentar la ausencia/ruta requerida.",
      }),
    );
  }

  const mentionsRegressionRisk = hasAny(normalized, [
    /\b(regresion|antiregresion|existente|base\s+ya\s+implementada|preservar|no\s+romper|sin\s+recarga|no\s+recarg)\b/,
  ]);
  if (
    mentionsRegressionRisk &&
    !hasAny(normalized, [
      /\b(antiregresion|anti-regresion|no\s+romper|preservar|mantener|no\s+debe|no\s+usar|no\s+llamar)\b/,
    ])
  ) {
    findings.push(
      newPromptReviewFinding({
        severity: "MAJOR",
        code: "ANTI_REGRESSION_DETAIL_REQUIRED",
        message: "El prompt alude a comportamiento existente pero no define reglas de antirregresion.",
        expected: "Reglas explicitas de no romper, preservar, no llamar o no usar workarounds.",
      }),
    );
  }

  if (!isPromptReviewToolingPrompt) {
    for (const requirement of [
      { pattern: /\b(build|npm\s+run\s+build|tsc)\b/, code: "BUILD_EVIDENCE_RECOMMENDED", expected: "build/tsc segun impacto." },
      { pattern: /\b(unit|unitarias?|test|vitest|testing library|focal)\b/, code: "UNIT_TEST_EVIDENCE_REQUIRED", expected: "unit/focal test segun impacto." },
      { pattern: /\b(comando|comandos|evidencia|resultado)\b/, code: "COMMAND_EVIDENCE_REQUIRED", expected: "comandos ejecutados y resultado." },
    ]) {
      if (requirement.pattern.test(normalized)) continue;
      findings.push(
        newPromptReviewFinding({
          severity: "MAJOR",
          code: requirement.code,
          message: `Falta exigir evidencia estructural de ${requirement.expected}`,
          expected: requirement.expected,
        }),
      );
    }
  }

  if (
    !isPromptReviewToolingPrompt &&
    requiresE2EEvidence(normalized) &&
    !hasE2EEvidenceRequirement(normalized)
  ) {
    findings.push(
      newPromptReviewFinding({
        severity: "BLOCKER",
        code: "E2E_EVIDENCE_REQUIRED",
        message:
          "El prompt describe un flujo que requiere validacion E2E o justificacion formal.",
        expected:
          "Exigir E2E real con Playwright/end-to-end, o justificar explicitamente por que no aplica y dejar evidencia manual.",
      }),
    );
  }
};

const addFrontendFindings = ({ findings, text }) => {
  const normalized = normalizeText(text);
  const isPromptReviewToolingPrompt = hasAny(normalized, [
    /opsxj:prompt-review/,
    /frontendpromptreviewservice/,
    /validar\s+prompts?\s+enterprise/,
  ]);

  if (isPromptReviewToolingPrompt) {
    return;
  }

  const mentionsFrontendWork = hasAny(normalized, [
    /\b(react|tsx|ui|hook|service|servicio|adapter|mapper|componente|modal|formulario)\b/,
    /\b(crear|modificar|implementar|ajustar)\b.*\b(componente|hook|service|servicio|adapter|mapper|ui)\b/,
  ]);
  const hasExpectedRoute = hasAny(text, [
    /src[\\/]+modules[\\/]+/i,
    /src[\\/]+app[\\/]+Components[\\/]+/i,
    /scripts[\\/]+/i,
  ]);
  if (mentionsFrontendWork && !hasExpectedRoute) {
    findings.push(
      newPromptReviewFinding({
        severity: "BLOCKER",
        code: "FRONTEND_ROUTE_REQUIRED",
        message: "El prompt pide trabajo frontend/tooling pero no define rutas locales esperadas.",
        expected: "Rutas src/modules, src/app/Components o scripts segun el tipo de cambio.",
      }),
    );
  }

  const touchesApi = hasAny(normalized, [/\b(api|endpoint|axios|query|mutation|react query)\b/]);
  if (touchesApi) {
    if (!hasAny(normalized, [/\b(service|servicio|hook)\b/])) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: "API_SERVICE_BOUNDARY_REQUIRED",
          message: "El prompt menciona consumo API pero no exige service/hook dedicado.",
          expected: "Service/hook dedicado para consumo API.",
        }),
      );
    }
    if (!hasAny(normalized, [/\b(request|response|contrato|contract)\b/])) {
      findings.push(
        newPromptReviewFinding({
          severity: "MAJOR",
          code: "API_CONTRACT_REQUIRED",
          message: "El prompt menciona API pero no exige contrato request/response.",
          expected: "Contrato request/response documentado.",
        }),
      );
    }
    if (!hasAny(normalized, [/\berror(es)?\b/, /\bmanejo\s+centralizado\b/, /\bcontrolad[oa]s?\b/])) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: "API_ERROR_HANDLING_REQUIRED",
          message: "El prompt menciona API pero no exige manejo controlado de errores.",
          expected: "Manejo centralizado/controlado de errores.",
        }),
      );
    }
  }

  if (/\baxios\b/.test(normalized) && !/axios\s+directo/.test(normalized)) {
    findings.push(
      newPromptReviewFinding({
        severity: "MAJOR",
        code: "AXIOS_DIRECT_COMPONENT_RISK",
        message: "El prompt menciona axios pero no prohibe su uso directo en componentes.",
        expected: "Prohibir axios directo en componentes o exigir service dedicado.",
      }),
    );
  }

  const touchesBatch = hasAny(normalized, [
    /\b(batch|upload|onstored|onbatchcomplete|appuploaddocumental)\b/,
  ]);
  if (touchesBatch) {
    if (!hasAny(normalized, [/\b(deduplic|duplicad|identidad|idalmacen|id\s+estable)\b/])) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: "BATCH_IDENTITY_REQUIRED",
          message: "El prompt menciona batch/upload pero no define identidad o deduplicacion.",
          expected: "Identidad estable y regla de deduplicacion.",
        }),
      );
    }
    if (!hasAny(normalized, [/\bonstored\b.*\bonbatchcomplete\b|\bonbatchcomplete\b.*\bonstored\b/])) {
      findings.push(
        newPromptReviewFinding({
          severity: "MAJOR",
          code: "BATCH_CALLBACK_CONTRACT_REQUIRED",
          message: "El prompt menciona batch/upload pero no define claramente efectos por callback.",
          expected: "Contrato de efectos para onStored y onBatchComplete.",
        }),
      );
    }
  }

  if (/appuploaddocumental/.test(normalized)) {
    for (const requirement of [
      { pattern: /\b(config|configuracion|tipologia|tipologias)\b/, code: "UPLOAD_CONFIG_REQUIRED", expected: "Config/tipologias." },
      { pattern: /\b(mapper|adapter|request builder|buildstorerequest|mapping)\b/, code: "UPLOAD_MAPPING_REQUIRED", expected: "Mapper/adapter/request builder." },
      { pattern: /\b(prueba|test|vitest|testing library|focal)\b/, code: "UPLOAD_TEST_REQUIRED", expected: "Pruebas focales del upload." },
    ]) {
      if (requirement.pattern.test(normalized)) continue;
      findings.push(
        newPromptReviewFinding({
          severity: "MAJOR",
          code: requirement.code,
          message: `El prompt menciona AppUploadDocumental pero no exige ${requirement.expected}`,
          expected: requirement.expected,
        }),
      );
    }
  }

  if (hasAny(normalized, [/\b(apptreetable|documentalworkbench|insercion incremental)\b/])) {
    if (!hasAny(normalized, [/\b(no\s+recarg|sin\s+recarg|no\s+refresh|no\s+remount|no\s+cambiar\s+key)\b/])) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: "INCREMENTAL_UI_ANTI_REFRESH_REQUIRED",
          message: "El prompt menciona tabla/workbench incremental pero no prohibe recarga/remount.",
          expected: "Reglas anti-recarga, anti-remount o justificacion.",
        }),
      );
    }
  }

  if (hasAny(normalized, [/\b(scanner|dynamsoft|digitalizaciondocumentalworkspace)\b/])) {
    if (!hasAny(normalized, [/\b(no\s+inicializ|no\s+initialize|no\s+listdevices|no\s+scanner)\b/])) {
      findings.push(
        newPromptReviewFinding({
          severity: "BLOCKER",
          code: "SCANNER_BOUNDARY_REQUIRED",
          message: "El prompt menciona scanner/digitalizacion sin definir frontera de no reinicializacion.",
          expected: "Regla explicita para no reinicializar scanner o justificacion.",
        }),
      );
    }
  }
};

export const testFrontendPromptReview = ({ promptText }) => {
  const findings = [];
  const text = String(promptText ?? "");
  addSectionFindings({ findings, text });
  addStructuralFindings({ findings, text });
  addFrontendFindings({ findings, text });

  if (findings.length === 0) {
    findings.push(
      newPromptReviewFinding({
        severity: "INFO",
        code: "MANUAL_REVIEW_RECOMMENDED",
        message: "Validacion automatica sin bloqueantes; revisar coherencia semantica antes de Jira.",
        expected: "Revision humana del prompt.",
      }),
    );
  }

  return findings;
};

const CORRECTION_SECTION_TITLE = "## Correcciones opsxj:prompt-review";

const correctionSnippets = new Map([
  [
    "ENTERPRISE_SECTION_REQUIRED",
    [
      "## Rol esperado",
      "Definir el rol tecnico esperado para ejecutar el ticket.",
      "",
      "## Objetivo",
      "Describir el objetivo funcional y tecnico verificable.",
      "",
      "## Restricciones criticas",
      "- No introducir cambios fuera del alcance declarado.",
      "- No romper comportamiento existente ni contratos publicos.",
      "",
      "## Criterios de aceptacion",
      "- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.",
    ].join("\n"),
  ],
  [
    "ENTERPRISE_SECTION_RECOMMENDED",
    [
      "## Contexto obligatorio",
      "Listar archivos, modulos, servicios, hooks, adapters y documentacion que deben leerse antes de implementar.",
      "",
      "## Pruebas obligatorias",
      "Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.",
      "",
      "## Documentacion tecnica",
      "Actualizar el paquete documental canonico del ticket.",
      "",
      "## Entregable final",
      "Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.",
    ].join("\n"),
  ],
  [
    "NEGATIVE_CONSTRAINTS_REQUIRED",
    [
      "## Restricciones criticas",
      "- No usar soluciones temporales para ocultar inconsistencias.",
      "- No duplicar logica existente si hay servicios, hooks o adapters reutilizables.",
      "- No modificar capas no relacionadas con el alcance del ticket.",
    ].join("\n"),
  ],
  [
    "POSITIVE_REQUIREMENTS_REQUIRED",
    [
      "## Requisitos positivos",
      "- Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.",
      "- Mantener la integracion sobre los puntos de extension existentes del repo.",
      "- Dejar evidencia de pruebas y documentacion tecnica actualizada.",
    ].join("\n"),
  ],
  [
    "DOCUMENTATION_PATH_REQUIRED",
    [
      "## Ruta documental obligatoria",
      "La documentacion debe quedar en una ruta canonica segun el contexto:",
      "",
      "```txt",
      "Modulo funcional:",
      "docs/modulos/<modulo>/<feature>/SCRUMCORE-000-resumen-del-asunto/",
      "",
      "App reusable / nucleo compartido:",
      "docs/Architecture/<area>/<feature>/SCRUMCORE-000-resumen-del-asunto/",
      "",
      "Componente compartido documentado historicamente:",
      "docs/Components/<componente>/SCRUMCORE-000-resumen-del-asunto/",
      "```",
      "",
      "Usar siempre identificador SCRUMCORE para el paquete documental del frontend.",
    ].join("\n"),
  ],
  [
    "DOCUMENTATION_PACKAGE_REQUIRED",
    [
      "## Paquete documental minimo",
      "Generar como minimo:",
      "",
      "```txt",
      "00-Indice.md",
      "01-Arquitectura.md",
      "02-FlujoIntegracion.md",
      "03-ContratoUploadYMapping.md",
      "04-EstadosErroresYAntiregresion.md",
      "05-PruebasEvidencia.md",
      "06-Diagramas.md",
      "07-Metadata.md",
      "```",
    ].join("\n"),
  ],
  [
    "DOCUMENTATION_DIAGRAM_FOLDER_REQUIRED",
    "Crear carpeta `Diagramas/` dentro del paquete documental para diagramas individuales.",
  ],
  [
    "DOCUMENTATION_FUNCTION_TABLE_REQUIRED",
    [
      "## Tabla de funciones creadas o modificadas",
      "| Funcion | Ruta | Ubicacion | Parametros | Responsabilidad |",
      "| --- | --- | --- | --- | --- |",
      "| `<nombre>` | `<path>` | `<componente/hook/service/adapter>` | `<params>` | `<responsabilidad>` |",
    ].join("\n"),
  ],
  [
    "DOCUMENTATION_INDEX_DETAIL_REQUIRED",
    "00-Indice.md debe incluir objetivo, alcance, componentes, hooks/adapters/servicios, modulos, dependencias y listado documental.",
  ],
  [
    "DOCUMENTATION_ARCHITECTURE_DETAIL_REQUIRED",
    "01-Arquitectura.md debe explicar decisiones arquitectonicas, reutilizacion, responsabilidades, desacople, alternativas descartadas, componentes de presentacion, contenedores, servicios, adapters, mappers, hooks e infraestructura.",
  ],
  [
    "DOCUMENTATION_FLOW_DETAIL_REQUIRED",
    "02-FlujoIntegracion.md debe cubrir usuario, renderizado, carga de datos, requests, backend, responses, estado, interfaz UI y batch/lote si aplica.",
  ],
  [
    "DOCUMENTATION_CONTRACT_MAPPING_DETAIL_REQUIRED",
    "03-ContratoUploadYMapping.md debe documentar props, contexto, DTOs, request, response, modelos, transformacion/mapping, deduplicacion, metadata y frontera frontend/backend.",
  ],
  [
    "DOCUMENTATION_STATES_ERRORS_REGRESSION_DETAIL_REQUIRED",
    "04-EstadosErroresYAntiregresion.md debe cubrir estado inicial, carga/loading, exito, errores, datos incompletos, estados parciales, respuestas invalidas, antirregresion, remount, refresh, recargas silenciosas, duplicacion, logica heredada y soluciones temporales.",
  ],
  [
    "DOCUMENTATION_TEST_EVIDENCE_DETAIL_REQUIRED",
    "05-PruebasEvidencia.md debe listar pruebas unitarias, integracion, manuales, comandos, resultados, limitaciones, riesgos y evidencia.",
  ],
  [
    "DOCUMENTATION_DIAGRAMS_DETAIL_REQUIRED",
    "06-Diagramas.md debe incluir componentes, secuencia, flujo principal, flujo alterno, casos de uso, estados y Mermaid o formato estructurado legible.",
  ],
  [
    "DOCUMENTATION_METADATA_DETAIL_REQUIRED",
    "07-Metadata.md debe consolidar SCRUMCORE, branch/rama, fecha, estado, archivos modificados, prompts, dependencias, riesgos y deuda tecnica.",
  ],
  [
    "CODE_LOCATION_CONTEXT_REQUIRED",
    [
      "## Reglas de ubicacion de codigo",
      "- Si se construye una app reusable o componente compartido, ubicarlo bajo `src/app/Components/<NombreComponente>/` o la ruta compartida equivalente existente.",
      "- Si se implementa comportamiento de modulo funcional, ubicarlo bajo `src/modules/<modulo>/components/`, `hooks/`, `services/`, `adapters/` o `types/` segun responsabilidad.",
      "- Adaptarse a la estructura existente del repo antes de crear carpetas nuevas.",
    ].join("\n"),
  ],
  [
    "CLEAN_ARCHITECTURE_REQUIRED",
    "Exigir Clean Architecture: separacion de responsabilidades por capas UI, hooks, services, adapters y types; dependencias hacia contratos estables.",
  ],
  [
    "SOLID_REQUIRED",
    "Exigir SOLID: responsabilidad unica, inversion de dependencias y extensibilidad sin modificar contratos existentes innecesariamente.",
  ],
  [
    "STRICT_TYPESCRIPT_REQUIRED",
    "Exigir TypeScript estricto: contratos tipados, props/eventos/modelos tipados, no `any` y sin casts amplios.",
  ],
  [
    "REACT_PROJECT_CONVENTIONS_REQUIRED",
    "Exigir convenciones React del proyecto: seguir patrones existentes de componentes, hooks, services, adapters, imports, estilos y pruebas.",
  ],
  [
    "REACT_STATE_OWNERSHIP_REQUIRED",
    "Exigir ownership claro del estado: fuente unica de verdad, no duplicar estado derivado salvo justificacion y sincronizacion explicita entre padre/hijo.",
  ],
  [
    "REACT_LIST_KEYS_REQUIRED",
    "Exigir keys estables de dominio en listas dinamicas; prohibir indices como `key` salvo listas estaticas justificadas.",
  ],
  [
    "RENDER_PERFORMANCE_REQUIRED",
    "Exigir revision de performance React: evitar re-renders innecesarios en tablas, visores, grids o workbenches; estabilizar props/callbacks/objetos cuando tenga impacto real.",
  ],
  [
    "VALIDATION_RULES_REQUIRED",
    "Exigir reglas de validacion de formulario, props, contexto y datos requeridos; manejar estados invalidos con errores controlados.",
  ],
  [
    "ACCESSIBILITY_REQUIRED",
    "Exigir accesibilidad: navegacion por teclado, manejo de foco, labels/aria, contraste y estados perceptibles.",
  ],
  [
    "TESTING_RULES_REQUIRED",
    "Exigir reglas de testing completas: unitarias/focales, integracion, E2E o justificacion formal, build y evidencia.",
  ],
  [
    "DEPENDENCY_GOVERNANCE_REQUIRED",
    "Exigir gobierno de dependencias: no agregar librerias nuevas si el repo ya cubre la necesidad; justificar dependencia, alternativa evaluada e impacto.",
  ],
  [
    "SECURITY_LOGGING_REQUIRED",
    "Exigir seguridad de logs: no loguear tokens, credenciales, payloads sensibles, documentos ni datos personales; usar logging controlado si aplica.",
  ],
  [
    "MERMAID_DIAGRAMS_REQUIRED",
    "Exigir diagramas Mermaid obligatorios para componentes, secuencia, estados y casos de uso cuando aplique.",
  ],
  [
    "E2E_EVIDENCE_REQUIRED",
    "Cuando el ticket afecte un flujo completo de usuario, navegacion, integracion entre vistas, persistencia de estado u operacion transaccional, exigir E2E real con Playwright; si no aplica, documentar justificacion formal y evidencia manual.",
  ],
  [
    "BUILD_EVIDENCE_RECOMMENDED",
    "Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.",
  ],
  [
    "UNIT_TEST_EVIDENCE_REQUIRED",
    "Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.",
  ],
  [
    "COMMAND_EVIDENCE_REQUIRED",
    "Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.",
  ],
]);

const getCorrectionSnippet = (finding) =>
  correctionSnippets.get(finding.code) ??
  (finding.expected
    ? `Agregar regla para [${finding.code}]: ${finding.expected}`
    : null);

export const buildPromptReviewCorrection = ({ findings }) => {
  const snippets = [];
  const seen = new Set();

  for (const finding of findings ?? []) {
    if (finding.severity === "INFO" || seen.has(finding.code)) continue;
    const snippet = getCorrectionSnippet(finding);
    if (!snippet) continue;
    seen.add(finding.code);
    snippets.push(snippet);
  }

  if (snippets.length === 0) {
    return "";
  }

  return [
    "",
    "",
    CORRECTION_SECTION_TITLE,
    "",
    "Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.",
    "",
    ...snippets.flatMap((snippet) => [snippet, ""]),
  ].join("\n").trimEnd();
};

export const applyPromptReviewCorrection = async ({ promptPath, findings }) => {
  const promptText = await readPromptReviewText({ promptPath });
  const correction = buildPromptReviewCorrection({ findings });
  if (!correction) {
    return { applied: false, promptText };
  }

  const nextPromptText = `${promptText.trimEnd()}${correction}\n`;
  await writeFile(promptPath, nextPromptText, "utf8");
  return { applied: true, promptText: nextPromptText };
};

const summarizeFindings = (findings) => ({
  blockers: findings.filter((item) => item.severity === "BLOCKER").length,
  major: findings.filter((item) => item.severity === "MAJOR").length,
  minor: findings.filter((item) => item.severity === "MINOR").length,
  info: findings.filter((item) => item.severity === "INFO").length,
});

const writePromptReviewReport = async ({ baseDir, promptPath, findings, error = null }) => {
  const summary = summarizeFindings(findings);
  const status = error ? "error" : summary.blockers > 0 ? "fail" : "pass";
  const report = {
    status,
    promptPath,
    reviewedAtUtc: new Date().toISOString(),
    summary,
    findings,
    ...(error ? { error } : {}),
  };
  const reportPath = path.join(baseDir, REPORT_RELATIVE_PATH);
  await mkdir(path.dirname(reportPath), { recursive: true });
  await writeFile(reportPath, `${JSON.stringify(report, null, 2)}\n`, "utf8");
  return { report, reportPath };
};

export const reviewFrontendPrompt = async ({ baseDir, promptInput }) => {
  try {
    const promptPath = await resolvePromptReviewInput({ baseDir, promptInput });
    const promptText = await readPromptReviewText({ promptPath });
    const findings = testFrontendPromptReview({ promptText });
    const { report, reportPath } = await writePromptReviewReport({
      baseDir,
      promptPath,
      findings,
    });
    return {
      ...report,
      reportPath,
      exitCode: report.summary.blockers > 0 ? 1 : 0,
    };
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    const findings = [];
    const { report, reportPath } = await writePromptReviewReport({
      baseDir,
      promptPath: String(promptInput ?? ""),
      findings,
      error: message,
    });
    return {
      ...report,
      reportPath,
      exitCode: 2,
    };
  }
};
