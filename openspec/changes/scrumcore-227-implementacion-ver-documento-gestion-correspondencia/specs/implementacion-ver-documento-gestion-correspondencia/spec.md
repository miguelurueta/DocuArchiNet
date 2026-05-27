# Spec - SCRUMCORE-227: Ver documento (Gestión Correspondencia)

## Alcance

Integrar `AppDocumentViewerOrchestrator` en `DocumentosWorkbench` para unificar el flujo “ver documento” desde `row_click` y `menu_action`, usando `DocumentResolveRequest` (obtenido vía `action/ver_documento`) como contrato canónico, y consolidando estado estable para `AppVisorEmbedPdf`.

## Guardrails (no negociables)

- NO cambiar backend ni endpoints.
- NO duplicar lógica resolve/firma en `DocumentosWorkbench`.
- NO usar `any`.
- NO tocar la lógica interna de `AppVisorEmbedPdf` (permisos/policy).
- NO romper Dynamic UI, `AppTreeTable`, selección múltiple ni documento activo.

## Contratos y source of truth

### Source of truth (integración)

`DocumentResolveRequest` proviene de:

`POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action` (acción `ver_documento`).

Response esperada:

```json
{
  "success": true,
  "data": {
    "DocumentResolveRequest": { "NombreGabinete": "string", "IdDocumento": 123 }
  }
}
```

### Input al orquestador

`DocumentosWorkbench` SHALL invocar:

```ts
visualizarDocumento({
  documentId,
  nombreGabinete,
  context: { idTareaWorkflow?, radicado?, grafo? }
})
```

### Output para el visor

`DocumentosWorkbench` SHALL mantener:

- `activeRowId`
- `activeFileUrl` (derivado del estado consolidado del orquestador)

y SHALL pasar al visor:

- `fileUrl={activeFileUrl}`

## Requirements

### R1: Convergencia de handlers

El sistema SHALL asegurar que `row_click` y `menu_action ver_documento` convergen en una única función orquestadora local.

#### Scenario R1.1: Click en fila
- **WHEN** el usuario hace click en una fila
- **THEN** se ejecuta la misma función orquestadora usada por `menu_action`

#### Scenario R1.2: Acción de menú ver_documento
- **WHEN** el usuario ejecuta `menu_action` con `ver_documento`
- **THEN** se ejecuta la misma función orquestadora usada por `row_click`

### R2: Uso obligatorio de DocumentResolveRequest

El sistema SHALL usar `DocumentResolveRequest` como único contrato canónico para invocar `visualizarDocumento()`.

#### Scenario R2.1: Action OK
- **GIVEN** `action/ver_documento` responde `success=true`
- **WHEN** existe `DocumentResolveRequest`
- **THEN** `visualizarDocumento()` MUST ser llamado con `{ documentId, nombreGabinete }` derivados de ese contrato

#### Scenario R2.2: Action falla
- **WHEN** `action/ver_documento` falla (error o `success=false`)
- **THEN** NO se debe llamar `visualizarDocumento()`
- **AND** el documento previamente visible MUST mantenerse

### R3: Integración con visor estable

El sistema SHALL actualizar el visor con `activeFileUrl` sin romper estabilidad.

#### Scenario R3.1: Resolve/firma ok
- **WHEN** el orquestador resuelve `fileUrl`
- **THEN** `activeFileUrl` se actualiza y el visor carga el documento

#### Scenario R3.2: Resolve/firma falla
- **WHEN** falla resolve o firma dentro del orquestador
- **THEN** el documento previamente visible MUST mantenerse (sin flicker / sin pérdida)

### R4: Selección múltiple intacta

El sistema SHALL preservar selección múltiple de `AppTreeTable`.

#### Scenario R4.1: Visualización no altera selección
- **WHEN** se visualiza un documento por click o menú
- **THEN** la selección múltiple MUST permanecer intacta y no mezclarse con documento activo

## Calidad y pruebas (mínimo)

- Unit tests: convergencia de handlers y payload correcto hacia `visualizarDocumento`.
- Integration UI: click/menu -> action -> orquestador -> visor (sin romper selección múltiple).
