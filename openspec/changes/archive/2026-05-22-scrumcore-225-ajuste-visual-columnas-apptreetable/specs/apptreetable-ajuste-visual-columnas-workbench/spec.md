# Spec: apptreetable-ajuste-visual-columnas-workbench

**SPEC ID:** `APPTREETABLE-225-001`

## ADDED Requirements

### Requirement: Two Columns Only (Workbench)
El frontend **SHALL** renderizar exactamente **2 columnas** en el listado de documentos del Workbench (DocumentosWorkbench → AppTreeTable), cuando el contexto corresponda al `tableId` del listado documental (ej. `InboxListaDocumentosRadicado`).

- La columna primaria **SHALL** corresponder a la columna documental principal (preferentemente `TIPODOCUMENTO` cuando exista en la configuración backend-driven).
- La columna secundaria **SHALL** seleccionarse de forma determinística a partir de la configuración backend-driven (Dynamic UI), sin hardcodear una key inexistente.
- El resto de columnas **SHALL NOT** renderizarse en el Workbench bajo este modo.

### Requirement: Column Selection Rules (Backend-driven)
El selector de columnas **SHALL** aplicar las siguientes reglas:

1. **Primaria**:
   - Si existe una columna con key `TIPODOCUMENTO`, **SHALL** usarse como columna primaria.
   - En caso contrario, **SHALL** usarse la primera columna visible definida por backend-driven config.
2. **Secundaria**:
   - **SHALL** escogerse la primera columna distinta a la primaria que sea considerada “válida”.
3. Columnas “no válidas”:
   - En `ViewMode=flatDocuments`, el selector **SHALL NOT** elegir columnas legacy no garantizadas (ej. `PAG`, `ESTADO_FIRMA_DIGITAL` y otras equivalentes definidas por la guía SCRUM-209).
4. Fallback:
   - Si no existe una columna secundaria válida, el sistema **SHALL** renderizar únicamente la primaria (sin romper la UI), y **SHALL** mantener el comportamiento funcional intacto.

### Requirement: Enterprise Sizing Preset
Para las 2 columnas seleccionadas, el frontend **SHALL** aplicar un preset de sizing enterprise para maximizar visibilidad:

- Columna primaria: `flex` mayor que la secundaria y `minWidth` definido (valores exactos documentados en `design.md` y verificables en tests).
- Columna secundaria: `flex` menor y `minWidth` definido.
- En viewports estrechos, el contenido **SHALL** truncarse (ellipsis) antes de colapsar visualmente una columna.

### Requirement: Isolation / No Regressions
El ajuste de “2 columnas + sizing preset”:

- **SHALL** estar aislado al contexto Workbench (scoping por `tableId` o equivalente).
- **SHALL NOT** afectar otros consumidores de `AppTable`/`AppTreeTable`.
- **SHALL NOT** cambiar contratos backend, actions, selection, ni eventos.

### Requirement: Responsive Definition of “Always Visible”
La condición “2 columnas siempre visibles” **SHALL** interpretarse así:
- En viewport objetivo de desktop/tablet (definido en tests Playwright), los 2 headers **SHALL** ser visibles sin degradar el layout.
- En viewports estrechos, se permite truncado y/o scroll horizontal siempre que el layout del Workbench (visor + rail) no se rompa.

### Requirement: Accessibility
El listado del Workbench con 2 columnas:

- **SHALL** mantener navegación keyboard-friendly.
- **SHALL** mantener focus visible para celdas y acciones.
- **SHALL** mantener headers y acciones accesibles (aria/role existentes sin degradación).

## Scenarios

### Scenario: Renderiza 2 columnas visibles (flatDocuments)
**Given** el Workbench recibe configuración backend-driven con múltiples columnas incluyendo `TIPODOCUMENTO`  
**When** se renderiza el listado en `ViewMode=flatDocuments` y `tableId` corresponde al Workbench  
**Then** el listado muestra exactamente 2 columnas visibles  
**And** la primera columna corresponde a `TIPODOCUMENTO`  
**And** la segunda columna corresponde a la primera columna válida distinta a `TIPODOCUMENTO` según reglas  

### Scenario: Evita columnas legacy en flatDocuments
**Given** el backend envía columnas legacy (ej. `PAG`, `ESTADO_FIRMA_DIGITAL`) junto con otras  
**When** el selector construye las 2 columnas del Workbench en `flatDocuments`  
**Then** ninguna columna legacy prohibida es seleccionada como primaria o secundaria  

### Scenario: No afecta acciones / selección
**Given** el usuario selecciona filas y dispara una acción secundaria (`onActionTriggered`)  
**When** el listado opera con 2 columnas  
**Then** la selección y acciones se comportan igual que antes del cambio  
