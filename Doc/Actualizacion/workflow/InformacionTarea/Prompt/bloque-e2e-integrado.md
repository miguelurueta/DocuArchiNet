# Requisito obligatorio — E2E integrada al cambio funcional

Incorporar estas reglas en cada etapa que introduzca o modifique un recorrido verificable. La E2E no es una tarea o entrega independiente.

- Leer `AGENTS.md`, `tools/e2e/AGENT-RUNBOOK.md`, `tools/e2e/package.json`, sesión, configuración, validadores, utilidades y evidencia existente.
- Reutilizar exclusivamente `tools/e2e`; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos.
- Ejecutar solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados.
- Usar secretos efímeros; no exponer, imprimir ni persistir credenciales, cookies, tokens, cadenas de conexión, campos sensibles o respuestas completas.
- Limitar controles de base de datos a `SELECT` y guardar únicamente evidencia saneada.
- Confirmar que abrir, consultar, buscar y cerrar no cambian tarea, estado, auditoría ni datos.
- Cubrir autorización, tarea propia/ajena/inactiva, aislamiento, catálogo permitido, exclusión sensible, estados UI, accesibilidad y regresión.
- Respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente.
- Ejecutar también pruebas unitarias/focales y MSBuild; registrar comandos, resultados y limitaciones.
- Ante ausencia de autorización, ambiente o datos, registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

Código, pruebas, E2E autorizada, compilación, documentación y evidencia saneada son una única unidad del mismo cambio. Una corrida anterior a modificaciones posteriores no valida la versión final.
