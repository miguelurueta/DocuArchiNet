# DOC-119 — PRODUCCION-IMPORTACION-CARGA-CARPETA-SEGUIMIENTO

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Estado de esta entrega: refinamiento propuesto; sin implementación ni aceptación operativa.

## Evidencia requerida

Corrida local de este refinamiento, 2026-11-11:

- Base: d7dbf201338bbfef3f82e3521359f30b1824bf95.
- Comando: node --test tests/doc111-importacion-carpetas-client.test.cjs.
- Resultado: 16/16 PASS, sin fallos ni omitidas.
- Alcance: cliente/Chromium local, servicios sintéticos; incluye recarga con sessionStorage y actualización de rama con helper real. No hay código productivo nuevo.

DOC-111 y DOC-116–118 son antecedentes con sus propios SHA/ambientes. El cierre administrativo DOC-118 conservado (29/38) y su aceptación técnica posterior (38/38 en doc118-integracion-completa-20261110.json y reporte acumulado) son cortes distintos. No se reetiquetan como pruebas DOC-119.

## QA/E2E WebForms

No ejecutados en este refinamiento: autenticación real, HTTP de aplicación, MySQL/SELECT reales, carga, fallos inyectados, cambios de permisos, limpieza y despliegue. SEG-01/02 requieren plan y autorización específica antes de aceptación integrada. SEG-03 sigue condicionado/no ejecutado hasta resolución expresa.

Próximas verificaciones: filtros/páginas/CSV/denegación sin efectos; recarga/reselección/árbol; después UI-05 sobre navegación definitiva y menús compartidos/carga individual pertinente. Gobierno no debe aprobar por generación de archivos: refinement.md sigue draft; tasks.md ya contiene 24 tareas concretas sin ejecutar.

## Verificación del borrador

- `openspec.cmd validate doc-119-produccion-importacion-carga-carpeta-seguimiento --strict`: PASS.
- `git diff --check`: PASS.
- Auditoría actual `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-119 --sync`: FAIL únicamente en `refinement:approved`. Decisiones y orígenes en tasks pasan; se resolvieron los dos bloqueos anteriores de trazabilidad aplicando el mapa. No se fuerza aprobación; con estado draft no se habilita implementación.
- Plan aplicado en tasks.md: 24 tareas, 10 S y 14 M; cobertura D-01–D-10/RQ-01–RQ-10, 0/24 completadas. Validación estricta no certifica ejecución ni aprobación humana.

Revisión documental posterior: refinement.md desarrolla cada D-01–D-10 con situación comprobada, regla concreta, aceptación, compatibilidad y tareas del mapa. Incluye doce referencias de evidencia con rutas/símbolos, contratos de consulta/CSV, recuperación/árbol, permisos separados, destino pulsado/seleccionado, equivalencia de accesos, viewport/teclado y 22 consumidores del menú compartido encontrados por inspección. Esta ampliación no añade una corrida autenticada, no modifica producto y conserva el estado draft.
