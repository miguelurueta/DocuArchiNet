## ADDED Requirements

### Requirement: Opciones de trámite desde plantilla
El formulario de radicación SHALL poblar el campo `ra_tipo_tramite` usando las opciones provistas por `useCamposPlantilla` para el campo con `name_campo = "Descripcion_Documento"`.

#### Scenario: Plantilla contiene opciones de trámite
- **WHEN** `useCamposPlantilla` entrega un campo `Descripcion_Documento` con `ilist_row_drowlist`
- **THEN** el `Select` de `ra_tipo_tramite` muestra las opciones mapeadas desde `ilist_row_drowlist`

### Requirement: Opciones solo desde plantilla
Si no existe el campo `ra_tipo_tramite` o la lista `ilist_row_drowlist` está vacía, el formulario SHALL renderizar el `Select` sin opciones.

#### Scenario: Plantilla sin opciones de trámite
- **WHEN** `useCamposPlantilla` no trae `ra_tipo_tramite` o su lista está vacía
- **THEN** el `Select` de `ra_tipo_tramite` no muestra opciones
