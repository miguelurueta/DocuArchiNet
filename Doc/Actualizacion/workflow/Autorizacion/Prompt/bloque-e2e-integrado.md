# Requisito obligatorio — E2E integrada al cambio funcional

Incorporar este bloque en toda etapa que introduzca o modifique un recorrido E2E. La E2E no es una tarea, historia ni entrega independiente.

## Objetivo

Integrar cobertura E2E real en el mismo cambio, con evidencia saneada y sin ampliar el alcance autorizado.

## Controles obligatorios

- Leer `AGENTS.md`, `tools/e2e/AGENT-RUNBOOK.md`, `tools/e2e/package.json`, configuración, sesión autenticada, validadores, utilidades y evidencias existentes antes de diseñar o ejecutar.
- Reutilizar exclusivamente la infraestructura de `tools/e2e`; no crear login, arnés, proyecto Playwright, configuración o `.env` paralelo.
- Ejecutar únicamente con ambiente, cuentas, tareas y datos expresamente autorizados. Una autorización de lectura no autoriza descarga de datos sensibles ni mutaciones.
- Capturar secretos de forma efímera y oculta. No imprimir ni persistir credenciales, cookies, tokens, cadenas de conexión, información personal, XML real ni respuestas sin sanear.
- Las verificaciones de base de datos son solo `SELECT`. Listado, detalle y descarga no deben cambiar tarea, autorización, estado, auditoría ni datos de negocio.
- No habilitar arbitrariamente feature flags, gates, usuarios, grupos o controles de seguridad.
- Ubicar pruebas, ejecutores, validadores y evidencia únicamente bajo rutas existentes de `tools/e2e/`.
- Cubrir, cuando aplique: rechazo anónimo, control de acceso, tarea propia/ajena/inactiva, lectura sin mutación, aislamiento entre tareas, filtros/paginación, autorización cruzada, descarga individual y consolidada, archivo adjunto sin navegación, estados UI, accesibilidad y regresión de consumidores.
- Ejecutar además pruebas focales y MSBuild, `dotnet` u otra compilación correspondiente. Registrar comandos, resultados y limitaciones.
- Si faltan autorización, ambiente, configuración o datos, registrar el bloqueo explícito. No usar mocks, simulaciones, resultados inventados ni evidencia ficticia como sustituto.

## Cierre

Código, pruebas focales, E2E real autorizada, compilación, documentación y evidencia saneada constituyen una única unidad. Una corrida anterior a cambios posteriores no valida la versión final.

