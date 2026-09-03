# 04 — Verificación transversal y evidencia

## Rol esperado

Arquitecto de calidad y revisor técnico independiente del flujo Workflow.

## Objetivo

Verificar que la solución completa coincide con las decisiones aprobadas y que código, contratos, UI, descargas, migración, pruebas y documentación forman una entrega coherente.

## Contexto obligatorio

- Requiere 03 aprobado.
- Leer `00-contexto-obligatorio.md`, artefactos Jira/OpenSpec si existen, documentación técnica, resultados y evidencia de 01–03.
- Esta etapa verifica; no corrige código productivo silenciosamente. Un defecto abre o devuelve el ticket correspondiente.

## Verificaciones obligatorias

- Trazar cada requisito y decisión a código y prueba.
- Confirmar tarea explícita, autorización fail-closed, pertenencia, SQL parametrizado, orden cerrado, paginación y no mutación.
- Confirmar descargas adjuntas, nombres/tipos saneados, conservación del modal y rechazo cruzado.
- Confirmar accesibilidad, responsive, foco, Escape, estados, respuesta obsoleta y no regresión del Centro de Trabajo.
- Confirmar migración del consumidor secundario, ruta moderna única y referencias legacy cero o excepciones justificadas.
- Ejecutar compilación, pruebas focales y suites E2E existentes aplicables sobre la versión final autorizada. Una evidencia anterior a un cambio posterior no cierra la entrega.
- Verificar que la evidencia no contiene credenciales, cookies, tokens, cadenas de conexión, datos personales ni cuerpos completos.

## Restricciones críticas

- No cambiar configuración, gates, datos, tareas o código productivo para obtener un resultado favorable.
- No ejecutar E2E real sin autorización literal vigente ni sustituirla por mocks.
- No aprobar solo con inspección visual o solo con compilación.

## Criterios de aceptación

- La matriz requisito–código–prueba no tiene huecos críticos.
- La implementación final tiene pruebas y E2E autorizada aprobadas, o el estado se declara bloqueado con causa reproducible.
- La recomendación es inequívoca: apto para 05, requiere corrección o bloqueado.

## Entregable final

Actualizar `04-pruebas-y-evidencia.md`, índice e inventario con comandos, resultados, cobertura, limitaciones, riesgos y decisión Jira. No liberar ni desplegar.

## Contratos y reglas de antirregresión

Comparar DTOs, request/response, códigos, eventos UI y transporte de descarga con la documentación aprobada. Preservar selección de tarea, tabla, colores, iconos, índice, scroll y operaciones vecinas; no llamar handlers retirados, no reactivar postbacks y no usar workarounds de sesión o campos ocultos.

## Pruebas obligatorias

Ejecutar pruebas unitarias/focales VB.NET y CJS, MSBuild y suites E2E aplicables; registrar comandos, resultados y cobertura. Código, E2E, validación autorizada y evidencia saneada forman una única unidad del cambio.

Reutilizar exclusivamente `tools/e2e`, su sesión, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas, tareas y datos expresamente autorizados. Usar secretos efímeros, verificaciones solo `SELECT` y evidencia saneada; no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión.

Cubrir autorización/control de acceso, lectura sin mutación, tarea/autorización cruzada, descargas, ruta moderna única, accesibilidad y regresión. Escrituras autorizadas y concurrencia mutante no aplican. Respetar gates y seguridad sin habilitarlos arbitrariamente; registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

La E2E es parte integral del mismo cambio funcional y de su criterio de cierre. Antes de autenticar se requieren ambiente, cuentas y datos o tareas descartables expresamente autorizados. No cerrar sin validación autorizada; respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente.

## Documentación técnica

Ubicar y actualizar la documentación existente del DOC; si falta una ruta o artefacto requerido, registrar la ausencia y no crear documentación en la raíz.
