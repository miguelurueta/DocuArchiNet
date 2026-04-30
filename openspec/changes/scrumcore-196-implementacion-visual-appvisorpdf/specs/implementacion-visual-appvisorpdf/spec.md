# [SPEC:SCRUMCORE-196] Integración funcional AppVisorPdf en Tab Documentos

## Contexto

El Tab **Documentos** en `GestionRespuesta` debe permitir consultar y visualizar los documentos/anexos asociados a una respuesta (`idTareaWf`). El visor base del PDF será `AppVisorPdf` y el listado/selección se presentará en el sidebar plegable (`AppCollapseRail`).

## Alcance

- Listado real de documentos asociados a `idTareaWf`.
- Selección de un documento para cargar y renderizar el PDF en `AppVisorPdf`.
- Manejo de estados (`loading`, `empty`, `error`, `unauthorized/forbidden` si aplica).
- Responsive estable en desktop/tablet/mobile, incluyendo comportamiento overlay del sidebar.

Fuera de alcance:
- Edición de PDFs.
- Persistencia/almacenamiento nuevo.
- Definir contratos backend nuevos (solo consumir los existentes).

## Reglas

1) `DocumentosWorkbench` debe recibir `idTareaWf` (prop) desde `GestionRespuesta`.
2) Al cargar el tab Documentos, si `idTareaWf` es válido:
   - se consulta el listado de documentos/anexos asociados.
3) El listado debe permitir seleccionar un documento PDF y disparar su carga para visualizarlo.
4) El visor debe mostrar:
   - estado de carga mientras descarga/resuelve el PDF
   - estado vacío si no hay documentos
   - estado de error si falla la consulta o la descarga
5) Responsive/overlay:
   - en mobile/tablet el sidebar opera como overlay con backdrop
   - al cambiar de tab, el overlay debe colapsarse automáticamente
   - no se debe ocultar el overlay usando `aria-hidden` si contiene un elemento con foco; usar el patrón actual (`inert`/blur).

## Escenarios (Behavior)

### Escenario 1 — Tab Documentos carga listado real

**Given** el usuario está en `GestionRespuesta` con `idTareaWf` válido  
**When** abre el tab **Documentos**  
**Then** se muestra un estado `loading` mientras se consulta el listado  
**And** al resolver, se renderiza el listado de documentos asociado a `idTareaWf`  

### Escenario 2 — Sin documentos

**Given** el usuario abre el tab **Documentos** con `idTareaWf` válido  
**And** el servicio retorna lista vacía  
**Then** se muestra un estado `empty` indicando que no hay documentos disponibles  
**And** no se muestra visor activo

### Escenario 3 — Selección de documento PDF

**Given** el usuario ve el listado de documentos  
**When** selecciona un documento PDF  
**Then** el visor muestra `loading` mientras se obtiene el contenido/URL del PDF  
**And** al completar, `AppVisorPdf` renderiza el documento seleccionado

### Escenario 4 — Error al listar o descargar

**Given** el usuario abre el tab **Documentos**  
**When** falla la consulta del listado o la descarga del PDF  
**Then** se muestra un estado de error accionable (reintentar)  
**And** no se rompe el layout del tab

### Escenario 5 — Responsive overlay estable

**Given** el usuario está en tablet/mobile (incl. 1024×1366)  
**When** abre el sidebar de documentos (overlay) y selecciona un documento  
**Then** el visor no queda en blanco  
**And** el overlay puede cerrarse sin colapsar la UI  
**And** al cambiar a otro tab, el overlay se oculta

