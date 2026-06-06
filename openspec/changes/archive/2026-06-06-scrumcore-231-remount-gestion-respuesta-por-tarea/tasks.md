# Tasks: SCRUMCORE-231 — Remount garantizado de GestionRespuesta por cambio de tarea

Objetivo del refinamiento:
- Garantizar remount completo del árbol de detalle al cambiar `:id` en la ruta.
- Mantener aislamiento de estado entre tareas sin tocar lógica funcional.
- Documentar y trazar riesgos/validación en formato enterprise.

## A. Capa 1 — Riesgo y alcance
- [x] Revisar flujo actual de navegación y confirmar que la identidad del subárbol de detalle se pudiera reutilizar.
- [x] Confirmar impacto transversal potencial sobre `context + hooks + documentos + visor + tree table + SCRUM-205 + adjuntos`.
- [x] Definir estrategia de remount basada en `parsedId` (clave determinística).
- [x] Aceptar limitantes: sin endpoints nuevos ni cambios de contratos funcionales.

## B. Capa 2 — Implementación controlada
- [x] Aplicar `key={\"gestion-respuesta-${parsedId}\"}` (o fallback estable cuando no hay id válido) al contenedor de detalle de ruta.
- [x] Mantener guardas existentes (`hasDetail`, `hasValidId`, `loading`, `blocked`) sin alterar UX.
- [x] Asegurar que el key cubre contenedor que incluye providers + `GestionRespuesta` + `DocumentosWorkbench` + visor.
- [x] Mantener compatibilidad con `AppTable`, `AppTreeTable`, tabs y navegación interna.

## C. Capa 3 — Verificación funcional del remount
- [x] Añadir probe de remount en `GestionCorrespondenciaRoute.spec.test.tsx` para cambio `/respuesta/924 -> /respuesta/925`.
- [x] Añadir probe de estado local de detalle para demostrar que no persiste entre tareas.
- [x] Ejecutar test específico de ruta (vitest) y validar pass.
- [x] Ejecutar lint de archivos tocados y validar pass.

## D. Capa 4 — Hardening anti-stale y lifecycle
- [x] Levantar/validar pruebas de navegación rápida entre rutas para confirmar ausencia de contaminación por async pending.
- [x] Validar teardown de effects/cancel de requests en cambios de id de tarea con pruebas de interacción (via remount lifecycle + navegación consecutiva en pruebas de ruta).
- [x] Documentar riesgos residuales y observables de stale en la pestaña de pruebas.

## E. Capa 5 — Cobertura transversal y regresión
- [ ] Ejecutar validación UI completa: cambio entre tareas desde bandeja, tabs, visor sincronizado, árbol estable. *(pendiente: requiere entorno manual/QA)*
- [ ] Ejecutar validación de integridad de adjuntos + reload + responsive. *(pendiente: requiere entorno manual/QA)*
- [ ] Validar que no hay regresión en `AppTable`, `AppTreeTable`, `AppTable`, `AppVisorEmbedPdf`, `SCRUM-205`. *(pendiente: requiere suite e2e/regresión completa)*
- [ ] Ejecutar/confirmar pruebas e2e de remount y estado reseteado. *(pendiente: requiere entorno e2e disponible)*

## F. Capa 6 — Trazabilidad y cierre enterprise
- [x] Redactar documentación enterprise completa:
  - [x] `SCRUMCORE-231-Arquitectura.md`
  - [x] `SCRUMCORE-231-Implementacion-Detallada.md`
  - [x] `SCRUM-231-Integracion-BackEnd.md`
  - [x] `SCRUMCORE-231-Pruebas.md`
  - [x] `SCRUMCORE-231-Metadata.md`
- [x] Marcar explícitamente pruebas ejecutadas vs pendientes.
- [x] Referenciar dependencia técnica con SCRUMCORE-219 / 220 / 221.
- [ ] Actualizar estado de ticket en JIRA y archivar change cuando toda la evidencia esté completa.
