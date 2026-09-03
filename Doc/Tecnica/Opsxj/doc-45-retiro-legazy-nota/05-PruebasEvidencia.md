# RETIRO-LEGAZY-NOTA — Pruebas y evidencia

- Ticket: DOC-45
- Cambio OpenSpec: doc-45-retiro-legazy-nota
- Clasificacion: cross_cutting

## Evidencia requerida

- Política DOC-45 final: PASS 4/4.
- MSBuild Debug registrado: PASS, 0 errores; 309 advertencias preexistentes.
- E2E definitiva de propiedad, visor y CRUD sobre el ejecutor estabilizado: PASS 1/1, 19.6 segundos totales.
- E2E de estado vacío: PASS 1/1, 17.1 segundos totales; la nota temporal fue eliminada.
- E2E no mutante de tarea no asignada: PASS 1/1, 15.7 segundos totales; fondo verde y glifo blanco comprobados mediante estilos computados.
- OpenSpec estricto y `git diff --check`: PASS.
- Gate final: `WorkflowCentroTrabajoModernActive=false`, usuarios y grupos vacíos.

## QA/E2E WebForms

Se reutilizaron la sesión autenticada, configuración, runner y suite existentes bajo `tools/e2e`. No se creó autenticación, proyecto Playwright ni `.env` paralelo. Las corridas reales usaron autorización explícita, tareas descartables y evidencia saneada sin credenciales, cookies, contenido de notas ni cuerpos HTTP.
