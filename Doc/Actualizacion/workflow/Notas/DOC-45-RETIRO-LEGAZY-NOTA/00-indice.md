# DOC-45 — Retiro controlado de legacy de Notas

## Resultado

DOC-45 retira la rutina duplicada `Class_anotacion_tarea.Eliminar_nota_tarea_workflow` y el consumidor visual legacy exclusivo del Centro de Trabajo. La única presentación de Notas en `Webworkflow` es el acceso moderno con contador y diálogo superpuesto. El cambio conserva los contratos legacy utilizados por Radicación y Correspondencia, agrega gestión exclusiva por propiedad, lectura completa segura, creación descubrible en estado vacío y compatibilidad con actualizaciones parciales de ASP.NET Web Forms.

## Documentos

- [Arquitectura](01-arquitectura.md)
- [Inventario y trazabilidad](02-inventario-y-trazabilidad.md)
- [Pruebas y evidencia](03-pruebas-y-evidencia.md)
- [Seguridad y operación](04-seguridad-y-operacion.md)
- [Rollback y deuda](05-rollback-y-deuda.md)
- [Liberación controlada](06-liberacion-controlada.md)
- [Diagramas](Diagramas/README.md)

## Trazabilidad

| Decisión | Requisito | Implementación | Verificación principal |
| --- | --- | --- | --- |
| D-01 | RQ-01 | Retiro de `Eliminar_nota_tarea_workflow` | Política DOC-45, inventario y compilación |
| D-02 | RQ-02 | Conservación de contratos con consumidores externos | Inventario ruta–consumidor y revisión de diff |
| D-03 | RQ-03 | Evidencia integrada y saneada en `tools/e2e` | Políticas, E2E autorizadas y runbook |
| D-04 | RQ-04 | Rollback integral y gate seguro | Configuración final y procedimiento de reversión |
| D-05 | RQ-05 | Retiro de UI legacy; acceso y modales modernos | Política DOC-45, MSBuild y E2E CRUD |
| D-06 | RQ-06 | E2E real como unidad de cierre | Runner y suite DOC-44 reutilizados |
| D-07 | RQ-07 | `PuedeGestionar`, `NotOwner` y visor de solo lectura | Políticas y E2E con nota ajena de la misma tarea |
| D-08 | RQ-08 | Acción `Nueva nota 0` y editor inmediato | E2E `test:doc45:empty-notes`, PASS 1/1 |
| D-09 | RQ-09 | Contraste de acciones e índice y caché versionada | Política focal y comprobación visual/E2E |
| D-10 | RQ-10 | Listener delegado y sincronización `endRequest` | E2E sin `page.reload`, PASS 1/1 |
