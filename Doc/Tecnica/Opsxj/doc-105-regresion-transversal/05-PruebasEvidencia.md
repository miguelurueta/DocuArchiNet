# DOC-105 — Pruebas y evidencia

- Ticket: DOC-105
- Cambio OpenSpec: doc-105-regresion-transversal
- Clasificacion: cross_cutting

Cambio: `doc-105-regresion-transversal`. Impacto: `cross_cutting`.

## Evidencia requerida

Preparación de PR: se repitieron visor 73/73, política DOC-105 70/70, servidor/grafo 34/34 y streaming compilado 2/2. `opsxj:validate DOC-105` sigue FAIL por seis controles: refinement:no_placeholders, refinement:tasks:origins, openspec_tasks, openspec_review, evidence:unit y evidence:manual_qa. Las ejecuciones locales no equivalen al registro de evidencia por SHA en OPSXJ. Publicación solicitada como borrador, no merge ni cierre Jira; las salvedades no se convierten en controles aprobados.

Consolidación al archivo: visor 73/73, DOC-105 70/70, DOM Chrome 1/1 y MSBuild Debug código 0 con advertencias históricas. Evidencia y límites detallados en `Doc/Actualizacion/VisorPdf/Implementacion/DOC-105/03-PruebasLocales.md`. Los resultados históricos inferiores no representan el estado final.

## QA/E2E WebForms

Workflow focal final PASSED: tres aperturas 2.266/350/347 ms, 34 páginas, preview de 390.850 bytes al primer canvas frente a principal de 9.646.444 bytes. Transporte RANGE, feedback, scroll y zoom aprobados. Producción y otros consumidores tienen aceptaciones de alcance definido en la matriz canónica; no se proclama cobertura total. Migración original pasa; convertido queda BLOCKED por destino no disponible al runner, sin demostrar ausencia del documento. Migración no usa express en las rutas revisadas.

Archivo autorizado con cinco tareas pendientes: 7.5/7.5.5, 7.6 (guardado instrumentado), 8.6 (rollback) y 9.6 (comparación remota pendiente). No se marcan como realizadas ni se fabrican evidencias. No se sincronizan especificaciones, no se autoriza despliegue ni merge por esta documentación.

## Local, 2026-10-14

- Política DOC-105: 27/27 `PASSED`.
- Política integral de rendimiento: 38/38 `PASSED`.
- Streaming DOC-102: 11/11 `PASSED`.
- Grafo DOC-103: 18/18 `PASSED`.
- Servidor DOC-104: 29/29 `PASSED`.
- MSBuild WebForms: `PASSED`, código 0; advertencias históricas sin errores nuevos.

## Ejecución real

Workflow pasó las ocho celdas de 10/20/50/100 Mbps, fría y caliente. Todas usaron HTTP 200 `STREAM`; no se declara descarga parcial. La cobertura transversal permanece `BLOCKED` porque cuatro clases de consumidor siguen sin perfil autorizado.

## Reproducción

Copiar el perfil de ejemplo con sufijo `.runtime.json`, completar únicamente datos autorizados y ejecutar:

```powershell
npm.cmd --prefix tools/e2e run test:doc105:pdf-cross-module -- --profile profiles/<perfil-runtime.json>
```

También puede reutilizarse en memoria el perfil Workflow ya autorizado con `--workflow-profile profiles/<perfil-rendimiento-runtime.json>` para conservar el mismo PDF sin copiar sus identificadores a otro archivo. Ese modo ejecuta solo Workflow y deja los otros consumidores `BLOCKED` hasta proporcionar su contexto autorizado.

La consola exige confirmaciones de ambiente, cuenta/consumidores y registros normales de visualización antes de solicitar secretos. Si se habilita el grafo, exige autorización adicional de cuenta/tarea, PDF descartable y cuenta ODBC de solo lectura.

Los artefactos runtime permanecen ignorados y la evidencia persistida contiene únicamente métricas y categorías saneadas.
