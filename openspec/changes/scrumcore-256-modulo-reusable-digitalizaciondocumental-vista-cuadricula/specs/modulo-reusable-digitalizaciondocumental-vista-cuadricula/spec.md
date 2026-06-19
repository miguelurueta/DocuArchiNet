## ADDED Requirements
### Requirement: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- VISTA-CUADRICULA
El sistema SHALL permitir multiples modos de visualizacion para las miniaturas capturadas.

#### Scenario: Cambiar modo de vista
- **GIVEN** existe un lote capturado con paginas
- **WHEN** el usuario activa el boton `Vista`
- **THEN** el panel de miniaturas permite seleccionar `1x1`, `2x2`, `3x3`, `4x4`, `5x5` y `6x6`
- **AND** el modo activo se refleja en el layout del panel

#### Scenario: Mantener seleccion y reordenamiento
- **GIVEN** el panel de miniaturas esta en modo cuadricula
- **WHEN** el usuario selecciona o arrastra una miniatura
- **THEN** la seleccion conserva el `page.id`
- **AND** el reordenamiento invoca la misma operacion existente sobre `scanner.pages`

#### Scenario: Lotes grandes
- **GIVEN** el lote tiene mas de 100 paginas
- **WHEN** se renderiza el panel de miniaturas
- **THEN** el panel activa una estrategia de virtualizacion CSS para reducir trabajo fuera de viewport

#### Scenario: Organizador como overlay sobre preview
- **GIVEN** existe un lote capturado con paginas
- **WHEN** el usuario activa `Organizar paginas`
- **THEN** el sistema muestra un overlay absoluto sobre el preview existente
- **AND** las miniaturas laterales, preview, scanner y configuracion siguen montados
- **AND** el overlay usa directamente `scanner.pages`

#### Scenario: Cerrar organizador sin recargar
- **GIVEN** el organizador esta abierto
- **WHEN** el usuario activa `Cerrar organizacion`
- **THEN** el overlay se oculta
- **AND** el preview sigue en el mismo estado sin regenerar miniaturas ni solicitar imagenes al scanner

#### Scenario: Acciones del organizador
- **GIVEN** el organizador muestra paginas desde `scanner.pages`
- **WHEN** el usuario selecciona paginas, rota, elimina o reordena con drag/drop
- **THEN** el sistema ejecuta las mismas operaciones existentes sobre los `page.id`

#### Scenario: No-regresion
- **WHEN** se valida el modulo afectado
- **THEN** no se rompen flujos existentes
### Requirement: Detalle funcional Jira
El sistema SHALL considerar las reglas detalladas del ticket.

#### Scenario: Reglas del ticket
- VISTA AVANZADA DE MINIATURAS
- OBJETIVO
- Permitir múltiples modos de visualización.
- ==================================================MODOS
- Lista
- 2x2
- 4x4
- 6x6
- ==================================================COMPORTAMIENTO
- Mantener:
- Drag & Drop.
- 
- Selección.
- 
- Checkboxes.
- 
- Reordenamiento.
- 
- ==================================================UI
- Botón:
- ⊞ Vista
- ==================================================DOCUMENTAR
- docs/Architecture/DigitalizacionDocumental/SCRUMCORE-267-thumbnail-grid.md
- ==================================================RENDIMIENTO
- Virtualización si supera 100 páginas.
- IMPLEMENTAR.
