## 1. Preparación / Contexto

- [x] 1.1 Identificar `tableId` real usado por Workbench (ListaDocumentosRadicados) y documentarlo
- [x] 1.2 Identificar set real de columnas backend-driven para Workbench (flatDocuments vs hierarchical) y documentarlo

## 2. Selector de 2 columnas (backend-driven)

- [x] 2.1 Implementar selector determinístico de columnas (primaria `TIPODOCUMENTO` + secundaria válida) con fallback seguro
- [x] 2.2 Implementar lista anti-legacy para `flatDocuments` (SCRUM-209) para evitar columnas no garantizadas
- [x] 2.3 Asegurar scoping estricto por `tableId` (no afectar otros módulos)

## 3. Sizing preset enterprise (2 columnas visibles)

- [x] 3.1 Aplicar preset de sizing a las 2 columnas seleccionadas (`flex` + `minWidth` + truncado)
- [x] 3.2 Validar que no cambie comportamiento de sorting/filtering/actions respecto a config existente

## 4. Pruebas unitarias (Vitest)

- [x] 4.1 Agregar unit tests del selector (prioridad/fallback/anti-legacy) con etiqueta `[SPEC:APPTREETABLE-225-001]`
- [x] 4.2 Agregar unit tests del scoping por `tableId` (solo Workbench)
- [x] 4.3 Agregar unit tests del preset de sizing (flex/minWidth aplicados)

## 5. Pruebas de integración UI

- [x] 5.1 Agregar pruebas de `DocumentosWorkbench` validando 2 headers visibles (mock de config/rows)
- [x] 5.2 Validar estados `loading/error/empty` sin regresión
- [x] 5.3 Validar que `onSelectRow` y `onActionTriggered` siguen funcionando (sin cambios funcionales)

## 6. Playwright (regresión visual/funcional)

- [x] 6.1 Crear test Playwright: Workbench renderiza exactamente 2 columnas visibles (headers) con etiqueta `[SPEC:APPTREETABLE-225-001]`
- [x] 6.2 Crear test Playwright: click primario actualiza visor sin romper layout (pendiente de ejecutar en entorno real)
- [x] 6.3 Crear test Playwright: acción secundaria no rompe selección/visor (pendiente de ejecutar en entorno real)

## 7. Documentación enterprise (ruta obligatoria)

- [x] 7.1 Crear carpeta `docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnas/`
- [x] 7.2 Documentar `SCRUMCORE-225-Arquitectura.md` (incluye Mermaid + riesgos/mitigaciones + trazabilidad)
- [x] 7.3 Documentar `SCRUMCORE-225-Implementacion-Detallada.md` (archivos tocados, qué/cómo/por qué)
- [x] 7.4 Documentar `SCRUMCORE-225-Pruebas.md` (unit/integración/Playwright: ejecutadas vs pendientes + evidencias)
- [x] 7.5 Documentar `SCRUMCORE-225-Metadata.md` (ticket, autor, fecha, versión, historial, refs cruzadas)

## 8. Flujo opsxj

- [x] 8.1 Dejar cambio listo para implementación (sin deuda de refinement)
- [ ] 8.2 Preparar cambio para `opsxj:archive SCRUMCORE-225` después de merge (cuando aplique)
