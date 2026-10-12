<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-97-doble-factor-seguridad-regresion

## Fuente y alcance

- Ticket: `DOC-97` — DOBLE-FACTOR-SEGURIDAD-REGRESION.
- Cambio OpenSpec: `doc-97-doble-factor-seguridad-regresion`.
- Perfil: ASP.NET Web Forms/VB.NET 4.6.1, ASMX, MySQL/ODBC, JavaScript y pruebas CJS/.NET.
- Alcance: auditar el incremento Login 2FA entregado por DOC-91 a DOC-96, completar su matriz transversal, corregir solo defectos demostrados y conectar verificaciones estables al CI existente.
- Fuera de alcance: DDL, nuevas tablas/columnas/endpoints, refactor de capas, cambios en `ClassCorreo`, recuperación de contraseña o módulos ajenos al login 2FA.

## Contexto inspeccionado

- Contratos/modelos: `Modelo/Login/SegundoFactor/*.vb` y `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb`.
- Servicios: `Services/Login/SegundoFactor/*.vb`.
- Persistencia/configuración/principales: `Infrastructure/Repositories/Login/SegundoFactor/*.vb`.
- Seguridad, SMTP y diagnóstico: `Infrastructure/Login/SegundoFactor/**/*.vb`.
- Frontera y composición: `webservice/Login/SegundoFactor/*.vb`, `webservice/WebServiceLoginSegundoFactor.asmx(.vb)`.
- Integración UI: `gestor.aspx`, `gestor.aspx.vb`, `js/login/segundo-factor.js`, `Styles/login/segundo-factor.css`.
- Evidencia previa: OpenSpec/documentación DOC-91..96, suites `tests/login-second-factor-*`, harnesses `tests/LoginSecondFactor*Tests.cs` y políticas E2E DOC-95/DOC-96.
- CI: `.github/workflows/opsxj-validation.yml` ejecuta DOC-91..95, pero no registra rutas ni pasos DOC-96; es un defecto de automatización comprobado.
- Línea base local 2026-10-11: 35 pruebas de contratos/comportamiento, 7 de política DOC-95 y 21 de política DOC-96 aprobadas sin infraestructura real.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Requirement |
| --- | --- | --- | --- |
| D-01 | Auditar el flujo real `gestor.aspx` → preautenticación → challenge/SMTP → ASMX/UI → finalizador → cookie/ruta; no crear arquitectura paralela. | `gestor.aspx.vb`, `SecondFactorPreAuthenticationService.vb`, `SecondFactorAuthenticationService.vb`, `WebServiceLoginSegundoFactor.asmx.vb` | RQ-01 |
| D-02 | Construir una matriz determinista común para DocuArchi, Gestor, Radicación y Workflow, conservando que solo Gestor/Workflow aplican la regla legacy de estado. | repositorios `MySql*SecondFactorPrincipalRepository.vb`, fixture DOC-94 | RQ-02, RQ-03 |
| D-03 | Probar en el nivel estable: harness .NET para comportamiento, CJS/Roslyn para estructura y `tools/e2e` solo para integración real autorizada. | `tests/`, `tools/validation/`, `tools/e2e/` | RQ-04, RQ-05 |
| D-04 | Un fallo autoriza una corrección mínima solo en el propietario canónico y exige repetir prueba y regresión; sin fallo la producción permanece intacta. | rutas canónicas de Jira y `.vbproj` | RQ-06 |
| D-05 | Ampliar el CI existente, sin pipeline alterno, con verificaciones estables DOC-96 y DOC-97. | `.github/workflows/opsxj-validation.yml` | RQ-07 |
| D-06 | La E2E no cambia flags para fabricar escenarios; usa autorización actual, configuración existente, controles `SELECT` y estados passed/failed/blocked. | `AGENTS.md`, `tools/e2e/AGENT-RUNBOOK.md` | RQ-08 |
| D-07 | Documentar matriz, hallazgos, correcciones, comandos, resultados y riesgos sin secretos ni cobertura ficticia. | `Doc/Actualizacion/Login/Implementacion/DOC-91..96/` | RQ-09 |

## Requisitos verificables

| ID | Resultado observable | Criterio | Compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Trazabilidad completa del recorrido. | Cada etapa referencia clase y operación real, junto con su prueba propietaria. | Sin rutas ni composición duplicadas. |
| RQ-02 | Cuatro módulos con evidencia de credenciales, OFF/NULL, activo, identidad/correo y estado. | Matriz informa módulo/caso al fallar. | Preserva asimetría Gestor/Workflow. |
| RQ-03 | Ciclo OTP, límites, replay, concurrencia y aislamiento validados con dobles. | Se observan efectos y ganador único. | No equivale a E2E productiva. |
| RQ-04 | Secretos e identidad no cruzan superficies indebidas. | Pruebas fallan con ruta/símbolo. | Mantiene contratos DOC-91..96. |
| RQ-05 | Orden seguro, atomicidad y `AuthPayloadJson=NULL`. | Cookie/finalizador solo tras OTP válido. | Riesgo legacy no transaccional explícito. |
| RQ-06 | Solo defectos reproducidos cambian producto. | Corrección cita rojo previo y verde posterior. | Sin DDL/refactor cosmético. |
| RQ-07 | CI ejecuta verificaciones 2FA estables DOC-96/DOC-97. | Workflow falla diagnósticamente. | E2E interactiva fuera de CI. |
| RQ-08 | E2E usa infraestructura única, autorización vigente y flags inmutables. | Cada caso termina passed/failed/blocked. | Precondiciones pueden bloquear. |
| RQ-09 | Documentación coincide con código y resultados. | No declara cobertura completa con casos sin validar. | Evidencia saneada. |

## Resultado del refinamiento

- Estado: `approved`.
- Todas las decisiones funcionales necesarias para iniciar la auditoría están definidas.
- La autorización histórica DOC-95/DOC-96 no autoriza E2E DOC-97.
