# Design - SCRUMCORE-227

## Contexto

Ticket: `SCRUMCORE-227` — **IMPLEMENTACION-VER-DOCUMENTO-GESTION-CORRESPONDENCIA**

Objetivo: integrar el core reusable `AppDocumentViewerOrchestrator` dentro de `DocumentosWorkbench` para unificar el flujo **ver documento** desde:

- `row_click` (click en fila)
- `menu_action` (acción “ver_documento”)

sin duplicar lógica, manteniendo estabilidad del visor, y sin afectar selección múltiple ni documento activo.

## Objetivos / No-objetivos

**Objetivos**
- Converger `row_click` y `menu_action` en un único flujo orquestador local.
- Usar `action/ver_documento` para obtener `DocumentResolveRequest` (contrato canónico de integración).
- Invocar `AppDocumentViewerOrchestrator.visualizarDocumento()` con `{ documentId, nombreGabinete, context }`.
- Mantener `AppVisorEmbedPdf` como render del PDF (sin tocar su lógica interna de permisos/policy).
- UX estable: loading visible, errores visibles, sin flicker, foco/scroll estables.

**No-objetivos**
- No cambiar backend ni endpoints.
- No duplicar lógica de resolve/firma en `DocumentosWorkbench` (eso vive en el orquestador).
- No alterar permisos internos del visor ni su policy.
- No romper Dynamic UI ni `AppTreeTable`.
- No romper selección múltiple ni documento activo.

## Ubicaciones esperadas

- Workbench: `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- Hooks: `src/modules/gestionCorrespondencia/hooks/*`
- Adapters: `src/modules/gestionCorrespondencia/adapters/*`
- Tests: `src/modules/gestionCorrespondencia/tests/*`

## Diseño propuesto (alto nivel)

### 1) Función orquestadora única en DocumentosWorkbench

Crear una función local (memoizada) responsable de:

1. Ejecutar `action/ver_documento` (ListaDocumentosRadicados/action).
2. Validar `success` y extraer `DocumentResolveRequest`.
3. Llamar `visualizarDocumento({ documentId, nombreGabinete, context })`.
4. Consolidar el “documento activo” del workbench:
   - `activeRowId`
   - `activeFileUrl` (derivado del estado consolidado del orquestador)
   - estados `loading/error` para UI

Regla: si falla `action/ver_documento`, NO llamar al orquestador.

### 2) Source of truth de integración (del prompt)

El contrato canónico para invocar al orquestador proviene de:

`POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`

que retorna:

```json
{
  "success": true,
  "data": {
    "DocumentResolveRequest": {
      "NombreGabinete": "string",
      "IdDocumento": 123
    }
  }
}
```

`DocumentosWorkbench` NO debe reconstruir payloads alternos; debe usar esa respuesta como fuente.

### 3) Concurrencia y estabilidad (workbench + orquestador)

- Clicks rápidos / acciones consecutivas deben converger sin estados stale.
- La cancelación/anti-race se delega al orquestador; el workbench evita sobrescrituras accidentales de `activeRowId` con respuestas tardías.
- Si falla resolve o firma, el documento previamente visible debe mantenerse.

### 4) Selección múltiple vs documento activo

- La selección múltiple (state de `AppTreeTable`) debe permanecer intacta.
- El documento activo es un state independiente, no debe mezclarse con “selected rows”.

## Decisiones

1. `DocumentosWorkbench` actúa solo como consumidor del orquestador y puente hacia `AppVisorEmbedPdf`.
2. `row_click` y `menu_action` convergen a una única función orquestadora.
3. `DocumentResolveRequest` es contrato canónico y obligatorio.

## Riesgos / trade-offs

- Riesgo de romper selección múltiple si se reutiliza state existente: mitigar separando estados.
- Riesgo de stale updates entre handler de menu y click: mitigar con una única función + ids estables.
- Riesgo de re-render completo del workbench: mitigar memoización y updates focales.

## Plan de migración

1. Añadir integración en `DocumentosWorkbench` sin tocar permisos del visor.
2. Añadir tests unitarios del flujo convergente.
3. Añadir pruebas de integración UI/e2e según el repo lo permita.

## Preguntas abiertas

- ¿Dónde está centralizado el “actionId” de `ver_documento` dentro del workbench hoy?
- ¿Cómo se expone `menu_action` en `AppTreeTable` (callback/adapter) para reutilizar el mismo flujo?
