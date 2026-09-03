# 04 — Verificación transversal y evidencia

## Rol esperado

Arquitecto de calidad y revisor técnico de Workflow ASP.NET Web Forms.

## Objetivo

Verificar la capacidad completa sobre la versión final sin crear una implementación alternativa.

## Contexto obligatorio

- Requiere 03 aprobado.
- Leer `00-contexto-obligatorio.md`, decisiones, documentación, inventario y evidencia de 01–03.
- Ubicar documentación existente y actualizarla o registrar formalmente la ruta faltante.

## Verificaciones obligatorias

- Trazar requisitos y decisiones a código y pruebas.
- Revisar request/response, DTOs, códigos, catálogo, sensibilidad, formato y estados UI.
- Confirmar tarea explícita, autorización fail-closed, tabla/columnas permitidas, parámetros y no mutación.
- Confirmar exclusión de campos técnicos, enmascaramiento y ausencia de HTML interpretado.
- Confirmar ruta moderna única y referencias legacy cero o excepciones justificadas.
- Confirmar accesibilidad, responsive, respuesta obsoleta, foco y regresión del Centro de Trabajo.

## Secuencia de verificación

1. Resolver la versión candidata y recopilar evidencia de 01–03.
2. Comparar decisiones, request/response, catálogo y privacidad con código y pruebas.
3. Ejecutar pruebas focales y MSBuild sobre la versión final.
4. Ejecutar la E2E autorizada con controles antes/después exclusivamente `SELECT`.
5. Revisar referencias legacy, regresión visual y evidencia saneada.
6. Emitir decisión apta, requiere corrección o bloqueada sin modificar producción.

## Restricciones críticas

- No corregir código productivo silenciosamente, cambiar configuración o alterar datos para obtener evidencia.
- No aprobar solo con inspección visual o compilación.
- No ejecutar pruebas reales sin autorización literal vigente.

## Reglas de antirregresión

Preservar otras opciones de Detalle, operaciones de tarea, tabla, colores, iconos, índice, visor y scroll. No llamar handlers retirados, no reactivar postbacks y no usar sesión o campos ocultos como workaround.

## Criterios de aceptación

- La matriz requisito–código–prueba no tiene vacíos críticos.
- La versión final posee evidencia reproducible o bloqueo explícito.
- La decisión es inequívoca: apto para 05, requiere corrección o bloqueado.

## Pruebas obligatorias

Ejecutar pruebas unitarias/focales VB.NET y CJS, MSBuild y suites E2E aplicables, registrando comandos, resultados y cobertura. La E2E es parte integral del mismo cambio y de su cierre; código, validación autorizada y evidencia saneada forman una única unidad.

Reutilizar exclusivamente `tools/e2e`, su sesión, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usar secretos efímeros, verificaciones solo `SELECT` y evidencia saneada; no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión.

Cubrir autorización/control de acceso, lectura sin mutación, tarea cruzada, catálogo y privacidad, vacío/error, ruta moderna única, accesibilidad y regresión. Escrituras autorizadas y concurrencia mutante no aplican. Respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente; no cerrar sin validación autorizada. Registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

## Documentación técnica

Actualizar índice, arquitectura, contratos, seguridad, `04-pruebas-y-evidencia.md`, inventario, riesgos y rollback del DOC.

## Entregable final

Entregar matriz, comandos, resultados, evidencia saneada, riesgos y recomendación para 05. No liberar ni desplegar.
