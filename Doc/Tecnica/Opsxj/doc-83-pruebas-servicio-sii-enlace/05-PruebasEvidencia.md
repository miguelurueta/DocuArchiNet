# DOC-83 — Pruebas y evidencia

- Ticket: DOC-83
- Cambio OpenSpec: doc-83-pruebas-servicio-sii-enlace
- Clasificacion: cross_cutting

## Evidencia requerida

Fecha de ejecución determinística y E2E DOC-83: 2026-09-27.

| Tipo | Comando | Resultado |
|---|---|---|
| unit/policy | `npm.cmd --prefix tools/e2e run test:doc83:closure` | PASS 4/4 |
| regression | `npm.cmd --prefix tools/e2e run test:doc83:regression` | PASS 149/149 |
| build | MSBuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m | PASS, 0 errores; 1 advertencia preexistente |

## QA/E2E WebForms

La corrida inicial de regresión falló 26 casos porque `npm --prefix` ubicó el proceso en `tools/e2e`; el lanzador se corrigió para usar la raíz y la repetición fue totalmente satisfactoria.

E2E real reutilizada: DOC-80 lectura sin cambios; DOC-81 importación múltiple 2/2 sin asignación; DOC-82 UI con nueve anexos y siete controles invariantes. Las referencias exactas están en la documentación canónica DOC-83.

Acceso negativo real: PASS, sin sesión, cero elementos y código opaco `FEATURE_DISABLED`. La asignación explícita confirmó ambos caminos con validación autoritativa: `BLOCKED` dejó `workflow-assignment-state` intacto cuando faltaba `Recibo De Caja`, y `ASSIGNED` produjo la transición después de una importación real y autorizada del tipo 186. Importación y asignación conservaron autorizaciones y reservas E2E independientes; la misma tarea descartable se reutilizó por autorización expresa. El artefacto canónico conserva el último resultado (`ASSIGNED`) y la salida saneada de consola conserva el `BLOCKED` previo. El gate quedó restaurado. No se guardaron secretos, textos de alertas ni contenido documental.
