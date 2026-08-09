import { mkdir, mkdtemp, readFile, rm, writeFile } from "node:fs/promises";
import os from "node:os";
import path from "node:path";
import { describe, expect, it } from "vitest";
import {
  buildPromptReviewCorrection,
  resolvePromptReviewInput,
  reviewFrontendPrompt,
  testFrontendPromptReview,
} from "./frontendPromptReviewService.js";

const validPrompt = [
  "## Rol esperado",
  "Actua como Arquitecto Frontend Senior especialista en React 19 y tooling Node.",
  "",
  "## Contexto obligatorio",
  "Leer scripts/lib/opsxjCommandRunner.js y scripts/lib/frontendPromptReviewService.js.",
  "Para tooling usar scripts/lib/ siguiendo el patron existente.",
  "Si el cambio fuera app reusable, usar src/app/Components/<NombreComponente>/.",
  "Si el cambio fuera modulo funcional, usar src/modules/<modulo>/components/, hooks/, services/, adapters/ o types/ segun corresponda.",
  "Adaptarse a la estructura existente del repo antes de crear carpetas nuevas.",
  "",
  "## Objetivo",
  "Implementar un comando local en scripts/lib para validar prompts enterprise.",
  "",
  "## Restricciones criticas",
  "- No consultar Jira.",
  "- No modificar el prompt evaluado.",
  "- Preservar el comportamiento existente del runner.",
  "- No romper comandos opsxj:new, opsxj:archive ni opsxj:close.",
  "",
  "## Contrato tecnico",
  "Documentar contrato de entrada prompt path/SCRUM key, salida JSON, exit codes y payload de findings.",
  "",
  "## Documentacion tecnica",
  "Actualizar docs/Architecture/OpsxjNewArchitecture/SCRUMCORE-000-prompt-review-before-jira/.",
  "Generar como minimo:",
  "00-Indice.md",
  "01-Arquitectura.md",
  "02-FlujoIntegracion.md",
  "03-ContratoUploadYMapping.md",
  "04-EstadosErroresYAntiregresion.md",
  "05-PruebasEvidencia.md",
  "06-Diagramas.md",
  "07-Metadata.md",
  "Crear carpeta Diagramas/ para diagramas individuales.",
  "Documentar funciones creadas en una tabla con columnas Funcion, Ruta, Ubicacion, Parametros y Responsabilidad.",
  "00-Indice.md debe incluir objetivo, alcance, componentes, hooks/adapters/servicios, modulos, dependencias y listado documental.",
  "01-Arquitectura.md debe explicar decisiones arquitectonicas, reutilizacion, responsabilidades, desacople, alternativas descartadas, componentes de presentacion, contenedores, servicios, adapters, mappers, hooks e infraestructura.",
  "02-FlujoIntegracion.md debe cubrir usuario, renderizado, carga de datos, requests, backend, responses, estado, interfaz UI y batch/lote si aplica.",
  "03-ContratoUploadYMapping.md debe documentar props, contexto, DTOs, request, response, modelos, transformacion/mapping, deduplicacion, metadata y frontera frontend/backend.",
  "04-EstadosErroresYAntiregresion.md debe cubrir estado inicial, carga/loading, exito, errores, datos incompletos, estados parciales, respuestas invalidas, antirregresion, remount, refresh, recargas silenciosas, duplicacion, logica heredada y soluciones temporales.",
  "05-PruebasEvidencia.md debe listar pruebas unitarias, integracion, manuales, comandos, resultados, limitaciones, riesgos y evidencia.",
  "06-Diagramas.md debe incluir componentes, secuencia, flujo principal, flujo alterno, casos de uso, estados y Mermaid o formato estructurado legible.",
  "07-Metadata.md debe consolidar SCRUMCORE, branch/rama, fecha, estado, archivos modificados, prompts, dependencias, riesgos y deuda tecnica.",
  "",
  "## Pruebas obligatorias",
  "Ejecutar npm run build y npm test -- scripts/lib/frontendPromptReviewService.test.js.",
  "Ejecutar E2E con Playwright si el cambio afecta flujo completo de usuario; si no aplica, dejar justificacion formal y evidencia manual.",
  "Registrar comandos ejecutados, resultado y evidencia.",
  "",
  "## Reglas React transversales",
  "Mantener ownership del estado con fuente unica de verdad y no duplicar estado derivado salvo justificacion.",
  "Usar keys estables de dominio en listas dinamicas; no usar indices como key salvo listas estaticas justificadas.",
  "Controlar re-renders en componentes pesados; usar props estables, useMemo o useCallback solo cuando tenga impacto real.",
  "No agregar dependencias nuevas si el repo ya cubre la necesidad; justificar cualquier libreria nueva con alternativa evaluada e impacto.",
  "No loguear tokens, credenciales, payloads sensibles, documentos ni datos personales.",
  "",
  "## Criterios de aceptacion",
  "El comando retorna exit codes deterministas.",
  "",
  "## Entregable final",
  "Servicio, runner y pruebas.",
  "",
].join("\n");

describe("frontendPromptReviewService", () => {
  it("passes a valid enterprise frontend tooling prompt", () => {
    const findings = testFrontendPromptReview({ promptText: validPrompt });

    expect(findings.some((finding) => finding.severity === "BLOCKER")).toBe(false);
    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "INFO",
          code: "MANUAL_REVIEW_RECOMMENDED",
        }),
      ]),
    );
  });

  it("builds correction guidance for structural prompt findings", () => {
    const correction = buildPromptReviewCorrection({
      findings: [
        {
          severity: "BLOCKER",
          code: "DOCUMENTATION_PACKAGE_REQUIRED",
        },
        {
          severity: "BLOCKER",
          code: "CODE_LOCATION_CONTEXT_REQUIRED",
        },
        {
          severity: "INFO",
          code: "MANUAL_REVIEW_RECOMMENDED",
        },
      ],
    });

    expect(correction).toContain("## Correcciones opsxj:prompt-review");
    expect(correction).toContain("00-Indice.md");
    expect(correction).toContain("src/app/Components");
    expect(correction).not.toContain("MANUAL_REVIEW_RECOMMENDED");
  });

  it("reports blockers when required enterprise sections are missing", () => {
    const findings = testFrontendPromptReview({
      promptText: "## Contexto\nPrompt incompleto.",
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "BLOCKER",
          code: "ENTERPRISE_SECTION_REQUIRED",
          message: expect.stringContaining("ROL ESPERADO"),
        }),
        expect.objectContaining({
          severity: "BLOCKER",
          code: "ENTERPRISE_SECTION_REQUIRED",
          message: expect.stringContaining("OBJETIVO"),
        }),
      ]),
    );
  });

  it("blocks API prompts without a service or hook boundary", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        "## Rol esperado",
        "Arquitecto React.",
        "## Objetivo",
        "Crear consumo API con endpoint remoto.",
        "## Restricciones criticas",
        "Mantener comportamiento.",
        "## Contexto",
        "src/modules/radicacion/demo.ts",
        "## Documentacion tecnica",
        "docs/Architecture/radicacion/",
        "## Pruebas obligatorias",
        "unit.",
        "## Criterios de aceptacion",
        "Validado.",
        "## Entregable final",
        "Diff.",
      ].join("\n"),
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "BLOCKER",
          code: "API_SERVICE_BOUNDARY_REQUIRED",
        }),
      ]),
    );
  });

  it("blocks AppUploadDocumental prompts without deduplication", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        "## Rol esperado",
        "Arquitecto React.",
        "## Objetivo",
        "Implementar AppUploadDocumental con onStored y onBatchComplete.",
        "## Restricciones criticas",
        "Mantener comportamiento.",
        "## Contexto",
        "src/modules/radicacion/components/demo.tsx",
        "## Documentacion tecnica",
        "docs/Architecture/radicacion/",
        "## Pruebas obligatorias",
        "unit.",
        "## Criterios de aceptacion",
        "Validado.",
        "## Entregable final",
        "Diff.",
      ].join("\n"),
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "BLOCKER",
          code: "BATCH_IDENTITY_REQUIRED",
        }),
      ]),
    );
  });

  it("blocks documentation requirements without a concrete docs path", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        "## Rol esperado",
        "Arquitecto React.",
        "## Objetivo",
        "Implementar componente reusable.",
        "## Restricciones criticas",
        "No duplicar componentes.",
        "## Contexto",
        "src/modules/radicacion/demo.tsx",
        "## Documentacion tecnica",
        "Dejar documentacion del cambio.",
        "## Pruebas obligatorias",
        "Ejecutar build, test focal y registrar comandos con resultado.",
        "## Criterios de aceptacion",
        "Validado.",
        "## Entregable final",
        "Diff y evidencia.",
      ].join("\n"),
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "BLOCKER",
          code: "DOCUMENTATION_PATH_REQUIRED",
        }),
      ]),
    );
  });

  it("accepts module documentation paths under docs/modulos with SCRUMCORE folder", () => {
    const modulePrompt = validPrompt.replace(
      "Actualizar docs/Architecture/OpsxjNewArchitecture/SCRUMCORE-000-prompt-review-before-jira/.",
      "Actualizar docs/modulos/radicacion/ConsultaPreviaRadicado/SCRUMCORE-345-workbench-contextual/.",
    );

    const findings = testFrontendPromptReview({ promptText: modulePrompt });

    expect(findings).not.toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          code: "DOCUMENTATION_PATH_REQUIRED",
        }),
      ]),
    );
  });

  it("blocks documentation requirements without the enterprise documentation package", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        "## Rol esperado",
        "Arquitecto React.",
        "## Objetivo",
        "Implementar hook reusable.",
        "## Restricciones criticas",
        "No duplicar logica.",
        "## Contexto",
        "src/modules/radicacion/hooks/demo.ts",
        "## Documentacion tecnica",
        "Actualizar docs/Architecture/radicacion/demo/.",
        "## Pruebas obligatorias",
        "Ejecutar test focal.",
        "## Criterios de aceptacion",
        "Validado.",
        "## Entregable final",
        "Diff.",
      ].join("\n"),
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "BLOCKER",
          code: "DOCUMENTATION_PACKAGE_REQUIRED",
        }),
        expect.objectContaining({
          severity: "MAJOR",
          code: "BUILD_EVIDENCE_RECOMMENDED",
        }),
        expect.objectContaining({
          severity: "MAJOR",
          code: "COMMAND_EVIDENCE_REQUIRED",
        }),
      ]),
    );
  });

  it("requires SCRUMCORE canonical docs path, diagrams folder and function table", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        "## Rol esperado",
        "Arquitecto React.",
        "## Objetivo",
        "Implementar componente reusable.",
        "## Restricciones criticas",
        "No duplicar componentes.",
        "## Contexto",
        "src/modules/radicacion/demo.tsx",
        "## Documentacion tecnica",
        "Actualizar docs/Architecture/Radicacion/ConsultaPrevia/Core/SCRUM-000-demo/.",
        "00-Indice.md",
        "01-Arquitectura.md",
        "02-FlujoIntegracion.md",
        "03-ContratoUploadYMapping.md",
        "04-EstadosErroresYAntiregresion.md",
        "05-PruebasEvidencia.md",
        "06-Diagramas.md",
        "07-Metadata.md",
        "## Pruebas obligatorias",
        "Ejecutar build, test focal y registrar comandos con resultado.",
        "## Criterios de aceptacion",
        "Validado.",
        "## Entregable final",
        "Diff y evidencia.",
      ].join("\n"),
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "BLOCKER",
          code: "DOCUMENTATION_PATH_REQUIRED",
        }),
        expect.objectContaining({
          severity: "BLOCKER",
          code: "DOCUMENTATION_DIAGRAM_FOLDER_REQUIRED",
        }),
        expect.objectContaining({
          severity: "BLOCKER",
          code: "DOCUMENTATION_FUNCTION_TABLE_REQUIRED",
        }),
      ]),
    );
  });

  it("blocks documentation package without detailed content rules per artifact", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        "## Rol esperado",
        "Arquitecto React.",
        "## Objetivo",
        "Implementar componente reusable.",
        "## Restricciones criticas",
        "No duplicar componentes.",
        "## Contexto",
        "Si el cambio fuera app reusable, usar src/app/Components/<NombreComponente>/.",
        "Si el cambio fuera modulo funcional, usar src/modules/<modulo>/components/.",
        "Adaptarse a la estructura existente del repo.",
        "## Documentacion tecnica",
        "Actualizar docs/Architecture/Radicacion/ConsultaPrevia/SCRUMCORE-000-demo/.",
        "00-Indice.md",
        "01-Arquitectura.md",
        "02-FlujoIntegracion.md",
        "03-ContratoUploadYMapping.md",
        "04-EstadosErroresYAntiregresion.md",
        "05-PruebasEvidencia.md",
        "06-Diagramas.md",
        "07-Metadata.md",
        "Crear carpeta Diagramas/.",
        "Documentar funciones creadas en una tabla con columnas Funcion, Ruta, Ubicacion, Parametros y Responsabilidad.",
        "## Pruebas obligatorias",
        "Ejecutar build, test focal y registrar comandos con resultado.",
        "## Criterios de aceptacion",
        "Validado.",
        "## Entregable final",
        "Diff y evidencia.",
      ].join("\n"),
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "BLOCKER",
          code: "DOCUMENTATION_ARCHITECTURE_DETAIL_REQUIRED",
        }),
        expect.objectContaining({
          severity: "BLOCKER",
          code: "DOCUMENTATION_FLOW_DETAIL_REQUIRED",
        }),
      ]),
    );
  });

  it("blocks code work without contextual code location guidance", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        "## Rol esperado",
        "Arquitecto React.",
        "## Objetivo",
        "Implementar componente y hook para una experiencia UI.",
        "## Restricciones criticas",
        "No duplicar componentes.",
        "## Contexto",
        "Leer documentacion funcional.",
        "## Documentacion tecnica",
        "Actualizar docs/Architecture/Radicacion/ConsultaPrevia/SCRUMCORE-000-demo/.",
        "00-Indice.md",
        "01-Arquitectura.md",
        "02-FlujoIntegracion.md",
        "03-ContratoUploadYMapping.md",
        "04-EstadosErroresYAntiregresion.md",
        "05-PruebasEvidencia.md",
        "06-Diagramas.md",
        "07-Metadata.md",
        "Crear carpeta Diagramas/.",
        "Documentar funciones creadas en una tabla con columnas Funcion, Ruta, Ubicacion, Parametros y Responsabilidad.",
        "## Pruebas obligatorias",
        "Ejecutar build, test focal y registrar comandos con resultado.",
        "## Criterios de aceptacion",
        "Validado.",
        "## Entregable final",
        "Diff y evidencia.",
      ].join("\n"),
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "BLOCKER",
          code: "CODE_LOCATION_CONTEXT_REQUIRED",
        }),
      ]),
    );
  });

  it("blocks critical user flows without E2E evidence or formal justification", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        "## Rol esperado",
        "Arquitecto React.",
        "## Objetivo",
        "Implementar flujo completo de usuario para abrir, editar, guardar y cerrar un panel.",
        "## Restricciones criticas",
        "No romper estado existente.",
        "## Contexto",
        "Si el cambio fuera app reusable, usar src/app/Components/<NombreComponente>/.",
        "Si el cambio fuera modulo funcional, usar src/modules/<modulo>/components/.",
        "Adaptarse a la estructura existente del repo.",
        "## Documentacion tecnica",
        "Actualizar docs/Architecture/Radicacion/ConsultaPrevia/SCRUMCORE-000-demo/.",
        "00-Indice.md",
        "01-Arquitectura.md",
        "02-FlujoIntegracion.md",
        "03-ContratoUploadYMapping.md",
        "04-EstadosErroresYAntiregresion.md",
        "05-PruebasEvidencia.md",
        "06-Diagramas.md",
        "07-Metadata.md",
        "Crear carpeta Diagramas/.",
        "Documentar funciones creadas en una tabla con columnas Funcion, Ruta, Ubicacion, Parametros y Responsabilidad.",
        "00-Indice.md debe incluir objetivo, alcance, componentes, hooks/adapters/servicios, modulos, dependencias y listado documental.",
        "01-Arquitectura.md debe explicar decisiones arquitectonicas, reutilizacion, responsabilidades, desacople, alternativas descartadas, componentes de presentacion, contenedores, servicios, adapters, mappers, hooks e infraestructura.",
        "02-FlujoIntegracion.md debe cubrir usuario, renderizado, carga de datos, requests, backend, responses, estado, interfaz UI y batch/lote si aplica.",
        "03-ContratoUploadYMapping.md debe documentar props, contexto, DTOs, request, response, modelos, transformacion/mapping, deduplicacion, metadata y frontera frontend/backend.",
        "04-EstadosErroresYAntiregresion.md debe cubrir estado inicial, carga/loading, exito, errores, datos incompletos, estados parciales, respuestas invalidas, antirregresion, remount, refresh, recargas silenciosas, duplicacion, logica heredada y soluciones temporales.",
        "05-PruebasEvidencia.md debe listar pruebas unitarias, integracion, manuales, comandos, resultados, limitaciones, riesgos y evidencia.",
        "06-Diagramas.md debe incluir componentes, secuencia, flujo principal, flujo alterno, casos de uso, estados y Mermaid o formato estructurado legible.",
        "07-Metadata.md debe consolidar SCRUMCORE, branch/rama, fecha, estado, archivos modificados, prompts, dependencias, riesgos y deuda tecnica.",
        "## Pruebas obligatorias",
        "Ejecutar build, test focal y registrar comandos con resultado.",
        "## Criterios de aceptacion",
        "Validado.",
        "## Entregable final",
        "Diff y evidencia.",
      ].join("\n"),
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          severity: "BLOCKER",
          code: "E2E_EVIDENCE_REQUIRED",
        }),
      ]),
    );
  });

  it("blocks frontend implementation prompts without quality guardrails", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        "## Rol esperado",
        "Arquitecto React.",
        "## Objetivo",
        "Implementar componente y hook para una experiencia UI.",
        "## Restricciones criticas",
        "No duplicar componentes.",
        "## Contexto",
        "Si el cambio fuera app reusable, usar src/app/Components/<NombreComponente>/.",
        "Si el cambio fuera modulo funcional, usar src/modules/<modulo>/components/.",
        "Adaptarse a la estructura existente del repo.",
        "## Documentacion tecnica",
        "Actualizar docs/Architecture/Radicacion/ConsultaPrevia/SCRUMCORE-000-demo/.",
        "00-Indice.md",
        "01-Arquitectura.md",
        "02-FlujoIntegracion.md",
        "03-ContratoUploadYMapping.md",
        "04-EstadosErroresYAntiregresion.md",
        "05-PruebasEvidencia.md",
        "06-Diagramas.md",
        "07-Metadata.md",
        "Crear carpeta Diagramas/.",
        "Documentar funciones creadas en una tabla con columnas Funcion, Ruta, Ubicacion, Parametros y Responsabilidad.",
        "00-Indice.md debe incluir objetivo, alcance, componentes, hooks/adapters/servicios, modulos, dependencias y listado documental.",
        "01-Arquitectura.md debe explicar decisiones arquitectonicas, reutilizacion, responsabilidades, desacople, alternativas descartadas, componentes de presentacion, contenedores, servicios, adapters, mappers, hooks e infraestructura.",
        "02-FlujoIntegracion.md debe cubrir usuario, renderizado, carga de datos, requests, backend, responses, estado, interfaz UI y batch/lote si aplica.",
        "03-ContratoUploadYMapping.md debe documentar props, contexto, DTOs, request, response, modelos, transformacion/mapping, deduplicacion, metadata y frontera frontend/backend.",
        "04-EstadosErroresYAntiregresion.md debe cubrir estado inicial, carga/loading, exito, errores, datos incompletos, estados parciales, respuestas invalidas, antirregresion, remount, refresh, recargas silenciosas, duplicacion, logica heredada y soluciones temporales.",
        "05-PruebasEvidencia.md debe listar pruebas unitarias, integracion, manuales, comandos, resultados, limitaciones, riesgos y evidencia.",
        "06-Diagramas.md debe incluir componentes, secuencia, flujo principal, flujo alterno, casos de uso y estados.",
        "07-Metadata.md debe consolidar SCRUMCORE, branch/rama, fecha, estado, archivos modificados, prompts, dependencias, riesgos y deuda tecnica.",
        "## Pruebas obligatorias",
        "Ejecutar build, test focal y registrar comandos con resultado.",
        "Ejecutar E2E con Playwright si aplica; si no aplica, dejar justificacion formal y evidencia manual.",
        "## Criterios de aceptacion",
        "Validado.",
        "## Entregable final",
        "Diff y evidencia.",
      ].join("\n"),
    });

    expect(findings).toEqual(
      expect.arrayContaining([
        expect.objectContaining({ severity: "BLOCKER", code: "CLEAN_ARCHITECTURE_REQUIRED" }),
        expect.objectContaining({ severity: "BLOCKER", code: "SOLID_REQUIRED" }),
        expect.objectContaining({ severity: "BLOCKER", code: "STRICT_TYPESCRIPT_REQUIRED" }),
        expect.objectContaining({ severity: "BLOCKER", code: "REACT_STATE_OWNERSHIP_REQUIRED" }),
        expect.objectContaining({ severity: "BLOCKER", code: "REACT_LIST_KEYS_REQUIRED" }),
        expect.objectContaining({ severity: "BLOCKER", code: "RENDER_PERFORMANCE_REQUIRED" }),
        expect.objectContaining({ severity: "BLOCKER", code: "ACCESSIBILITY_REQUIRED" }),
        expect.objectContaining({ severity: "BLOCKER", code: "DEPENDENCY_GOVERNANCE_REQUIRED" }),
        expect.objectContaining({ severity: "BLOCKER", code: "SECURITY_LOGGING_REQUIRED" }),
        expect.objectContaining({ severity: "BLOCKER", code: "MERMAID_DIAGRAMS_REQUIRED" }),
      ]),
    );
  });

  it("does not apply UI contextual rules to prompt-review tooling prompts", () => {
    const findings = testFrontendPromptReview({
      promptText: [
        validPrompt,
        "Debe implementar opsxj:prompt-review en scripts/lib/frontendPromptReviewService.js.",
        "El validador debe saber detectar axios, AppTreeTable, DocumentalWorkbench y scanner en prompts futuros.",
      ].join("\n"),
    });

    expect(findings).not.toEqual(
      expect.arrayContaining([
        expect.objectContaining({ code: "INCREMENTAL_UI_ANTI_REFRESH_REQUIRED" }),
        expect.objectContaining({ code: "SCANNER_BOUNDARY_REQUIRED" }),
        expect.objectContaining({ code: "AXIOS_DIRECT_COMPONENT_RISK" }),
      ]),
    );
  });

  it("resolves a prompt by explicit path and issue key", async () => {
    const tempDir = await mkdtemp(path.join(os.tmpdir(), "prompt-review-"));
    const promptDir = path.join(tempDir, "docs", "Architecture", "OpsxjNewArchitecture");
    const promptPath = path.join(promptDir, "PROMPT-SCRUM-9000-demo.md");
    try {
      await mkdir(promptDir, { recursive: true });
      await writeFile(promptPath, "SCRUM-9000\n", "utf8");

      await expect(
        resolvePromptReviewInput({
          baseDir: tempDir,
          promptInput: "docs/Architecture/OpsxjNewArchitecture/PROMPT-SCRUM-9000-demo.md",
        }),
      ).resolves.toBe(promptPath);
      await expect(
        resolvePromptReviewInput({
          baseDir: tempDir,
          promptInput: "SCRUM-9000",
        }),
      ).resolves.toBe(promptPath);
    } finally {
      await rm(tempDir, { recursive: true, force: true });
    }
  });

  it("returns exit code 2 and writes report for operational errors", async () => {
    const tempDir = await mkdtemp(path.join(os.tmpdir(), "prompt-review-error-"));
    try {
      const result = await reviewFrontendPrompt({
        baseDir: tempDir,
        promptInput: "docs/Architecture/NO-EXISTE.md",
      });

      expect(result.exitCode).toBe(2);
      expect(result.status).toBe("error");
      expect(result.error).toContain("Prompt file not found");

      const report = JSON.parse(await readFile(result.reportPath, "utf8"));
      expect(report.status).toBe("error");
      expect(report.error).toContain("Prompt file not found");
    } finally {
      await rm(tempDir, { recursive: true, force: true });
    }
  });

  it("writes pass/fail JSON reports with summary counts", async () => {
    const tempDir = await mkdtemp(path.join(os.tmpdir(), "prompt-review-report-"));
    const promptPath = path.join(tempDir, "PROMPT.md");
    try {
      await writeFile(promptPath, validPrompt, "utf8");

      const result = await reviewFrontendPrompt({
        baseDir: tempDir,
        promptInput: promptPath,
      });

      expect(result.exitCode).toBe(0);
      expect(result.status).toBe("pass");
      const report = JSON.parse(await readFile(result.reportPath, "utf8"));
      expect(report.summary.blockers).toBe(0);
      expect(report.findings).toEqual(
        expect.arrayContaining([
          expect.objectContaining({ code: "MANUAL_REVIEW_RECOMMENDED" }),
        ]),
      );
    } finally {
      await rm(tempDir, { recursive: true, force: true });
    }
  });
});
