# implementacion-visual-appmodal-gestion-correspondencia-14-fe Specification

## Purpose
TBD - created by archiving change scrumcore-140-implementacion-visual-appmodal-gestion-correspondencia-14-fe. Update Purpose after archive.
## Requirements
### Requirement: Apertura de modal desde opcion Reasignar Tramite
El sistema SHALL abrir `ReasignarRespuestaModal` cuando se dispare la accion `Reasignar Tramite` desde el dropdown de opciones de la tabla en Gestion Correspondencia.

#### Scenario: Apertura desde action id de reasignacion
- **WHEN** `onActionTriggered` recibe `actionId` igual a `reasignar_tramite` o `reasignar_tramite_menu`
- **THEN** el sistema SHALL abrir el modal de reasignacion

### Requirement: Integracion sin modificar la tabla base
El sistema SHALL resolver la conexion del modal exclusivamente desde el contenedor de pagina, manteniendo intacta la implementacion base de AppTable.

#### Scenario: Tabla estable sin cambios estructurales
- **WHEN** se implemente la conexion del modal
- **THEN** el sistema SHALL no modificar columnas, render, paginacion ni query de la tabla base

### Requirement: Contexto de fila para datos del modal
El sistema SHALL derivar `radicado` y `nota` del contexto de fila con estrategia de fallback cuando falte informacion.

#### Scenario: Radicado resuelto desde fila
- **WHEN** el usuario abre el modal desde una fila con identificadores de radicado disponibles
- **THEN** el sistema SHALL mostrar el radicado correspondiente en el encabezado del modal

#### Scenario: Fallback seguro sin datos completos
- **WHEN** la fila no contiene todas las claves esperadas
- **THEN** el sistema SHALL usar valores fallback para evitar errores de render y mantener el modal utilizable

### Requirement: Convivencia con flujo de navegacion existente
El sistema SHALL conservar el comportamiento de navegacion actual para acciones de gestion de tramite y separar ese flujo del trigger de reasignacion.

#### Scenario: Gestionar tramite mantiene navegacion
- **WHEN** `onActionTriggered` recibe `gestionar_tramite` o `gestionar_tramite_menu`
- **THEN** el sistema SHALL navegar al detalle de la fila como antes

#### Scenario: Reasignar tramite no navega
- **WHEN** `onActionTriggered` recibe una accion de reasignacion
- **THEN** el sistema SHALL abrir el modal y no disparar navegacion de detalle

### Requirement: Cierre de modal por controles de UI
El sistema SHALL permitir cerrar el modal desde controles de interfaz, sin llamadas backend.

#### Scenario: Cierre por cancelar o cerrar
- **WHEN** el usuario pulsa `Cancelar` o el boton de cierre del modal
- **THEN** el sistema SHALL cerrar `ReasignarRespuestaModal`

