## Context

`GestionRespuesta` ya tiene cambios funcionales previos en el contexto transversal y el árbol documental. El riesgo actual no está en funcionalidad nueva, sino en **consistencia operativa post-refactor**: estados de gabinete, recargas, re-renders, fallback y coordinación entre visor/adjuntos/árbol.

Este ticket actúa como **consolidación de calidad**. El objetivo es demostrar que el sistema funciona como un conjunto estable sin cambios de contrato.

## Goals

### Objetivo principal
- Asegurar estabilidad funcional y ausencia de regresiones en el flujo documental completo de `GestionRespuesta` tras los tickets 219, 220 y 221.

### Alcance de validación
- Contexto transversal (`GestionRespuestaDocumentosContext`, `useGestionRespuestaDocumentos`).
- Normalización y tipado de estructura por tarea (impacto en consumidores de `idRespuestaRadicado`).
- Hook documental (`useListaDocumentosRadicadosTreeTable`) y sus acciones de consulta.
- Workbench (`DocumentosWorkbench`) con integración `AppTreeTable` + `AppVisorEmbedPdf`.

### Límites (No Goals)
- No cambiar endpoints ni contratos backend.
- No introducir features nuevas.
- No alterar la UI más allá de estabilización técnica ya existente.
- No reemplazar `AppTreeTable` o `AppVisorEmbedPdf`.

## Design Decisions

1. **Hardening por capas, no por parche puntual**
   - Se validan capas con checklist y pruebas: contexto → hook → Workbench/visor → integración E2E.
   - Motivo: reduce riesgo de regresión cruzada.

2. **Una sola fuente de verdad de gabinete**
   - El hook documental consume `nombreGabinete`, `gabineteLoading` y `gabineteError` desde contexto.
   - Motivo: evita duplicación y estados inconsistentes.

3. **Documentación enterprise obligatoria y trazable**
   - Cada capa debe tener evidencia explícita en artefactos de arquitectura, implementación, integración y pruebas.
   - Motivo: continuidad de mantenimiento y QA verificable.

## Risk/Trade-offs

- Riesgo de regresión en rendimiento por re-renders en tabla/visor.
  - Mitigación: pruebas de interacción (focus, selección, expand/collapse) y clasificación de pendientes.
- Riesgo de estado obsoleto por cambios rápidos de tarea.
  - Mitigación: validar cancelación/idempotencia y fallback seguro en test y manuales.
- Riesgo de cobertura insuficiente en regresión real.
  - Mitigación: separar pruebas ejecutadas/péndientes con evidencia textual y CI.
- Riesgo de “god context”.
  - Mitigación: restricción contractual del contexto únicamente a estado documental transversal.

## Migration Plan

No hay migración de datos ni de esquema.

Plan de hardening:
1. Levantar estado base actual.
2. Ejecutar regresión por capas (unitarias + integración + interacción + E2E).
3. Corregir únicamente fallos de estabilidad.
4. Documentar resultados, riesgos y deuda residual.

## Open Questions

- ¿Qué escenarios de E2E en móvil/tablet quedan como cobertura mínima obligatoria antes de cierre?
- ¿Se requiere un ciclo de QA en ambiente integrador o basta validación local reproducible + evidencia de consola?
